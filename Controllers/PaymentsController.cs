using FinanceManagement.IRepository;
using FinanceManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Security.Claims;
using System.Drawing;
using FinanceManagement.ViewModels;

namespace FinanceManagement.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IExpensesRepository expensesRepository;
        private readonly ICashFlowRepository cashFlowRepository;
        private readonly IPaymentRepository paymentRepository;
        private readonly IUpadRepository upadRepository;

        public PaymentsController(ApplicationDbContext context, IExpensesRepository expensesRepository, ICashFlowRepository cashFlowRepository, IPaymentRepository paymentRepository, IUpadRepository upadRepository)
        {
            _context = context;
            this.expensesRepository = expensesRepository;
            this.cashFlowRepository = cashFlowRepository;
            this.paymentRepository = paymentRepository;
            this.upadRepository = upadRepository;
        }

        #region Payment Index
        [HttpGet]
        public IActionResult PaymentsIndex(int? month, int? year)
        {
            var payments = _context.Payments.AsQueryable();

            if (month.HasValue && year.HasValue)
            {
                payments = payments.Where(cf => cf.Date.Month == month.Value && cf.Date.Year == year.Value);
            }
            else if (year.HasValue)
            {
                payments = payments.Where(cf => cf.Date.Year == year.Value);
            }
            return View(payments.OrderBy(o => o.Date).ToList());

        }
        #endregion


        #region Payment Excel
        public IActionResult ExportToExcel(DateTime? startDate, DateTime? endDate)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get current user's ID
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            var companyName = user != null ? user.CompanyName : null;

            if (user == null)
            {
                return BadRequest("User not found.");
            }

            var companies = _context.Companies.Where(c => c.UserId == userId).ToList();

            if (!companies.Any())
            {
                return BadRequest("No companies found for the user.");
            }

            string startDateString = startDate?.ToString("dd-MM-yyyy") ?? "StartDate";
            string endDateString = endDate?.ToString("dd-MM-yyyy") ?? "EndDate";
            string fileName = $"Payments{startDateString}_to_{endDateString}.xlsx";

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Payments");

                // Header for overall table
                worksheet.Cells["A1"].Value = $"Payments Report ({startDateString} to {endDateString})";
                worksheet.Cells["A1"].Style.Font.Size = 25;
                worksheet.Cells["A1"].Style.Font.Bold = true;
                worksheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A1:K1"].Merge = true;
                worksheet.Cells["A1:K1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A1:K1"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);

                // Set the height of the header row
                worksheet.Row(1).Height = 30;

                int startRow = 4;
                int startCol = 1;

                foreach (var company in companies)
                {
                    // Fetch payments for the current company
                    var payments = paymentRepository.GetPaymentFromCompanyName(company.FirmName);

                    // Filter payments based on the provided start and end dates
                    if (startDate.HasValue && endDate.HasValue)
                    {
                        payments = payments.Where(p => p.Date >= startDate && p.Date <= endDate).ToList();
                    }

                    // Calculate summary totals
                    decimal dueTotal = payments.Where(p => p.PaymentCategory == "Due Payment").Sum(p => p.TotalAmount);
                    decimal paidTotal = payments.Sum(p => p.PaidAmount);
                    decimal pendingTotal = dueTotal - paidTotal;

                    // Summary headers
                    worksheet.Cells[startRow, startCol].Value = "Due";
                    worksheet.Cells[startRow, startCol + 1].Value = "Pending";
                    worksheet.Cells[startRow, startCol + 2].Value = "Paid";

                    worksheet.Cells[startRow, startCol, startRow, startCol + 2].Style.Font.Bold = true;
                    worksheet.Cells[startRow, startCol, startRow, startCol + 2].Style.Font.Size = 16;
                    worksheet.Cells[startRow, startCol, startRow, startCol + 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[startRow, startCol, startRow, startCol + 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    worksheet.Cells[startRow, startCol, startRow, startCol + 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[startRow, startCol, startRow, startCol + 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells[startRow, startCol, startRow, startCol + 2].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                    worksheet.Cells[startRow, startCol, startRow, startCol + 2].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                    worksheet.Cells[startRow, startCol, startRow, startCol + 2].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                    worksheet.Cells[startRow, startCol, startRow, startCol + 2].Style.Border.Right.Style = ExcelBorderStyle.Medium;

                    // Set the height of the summary header row
                    worksheet.Row(startRow).Height = 25;

                    // Add summary totals to the worksheet with rupee sign
                    worksheet.Cells[startRow + 1, startCol].Value = dueTotal;
                    worksheet.Cells[startRow + 1, startCol].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[startRow + 1, startCol].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.AntiqueWhite);
                    worksheet.Cells[startRow + 1, startCol].Style.Font.Bold = true;
                    worksheet.Cells[startRow + 1, startCol].Style.Numberformat.Format = "₹ #,##0.00";

                    worksheet.Cells[startRow + 1, startCol + 1].Value = pendingTotal;
                    worksheet.Cells[startRow + 1, startCol + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[startRow + 1, startCol + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.AntiqueWhite);
                    worksheet.Cells[startRow + 1, startCol + 1].Style.Font.Bold = true;
                    worksheet.Cells[startRow + 1, startCol + 1].Style.Numberformat.Format = "₹ #,##0.00";

                    worksheet.Cells[startRow + 1, startCol + 2].Value = paidTotal;
                    worksheet.Cells[startRow + 1, startCol + 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[startRow + 1, startCol + 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.AntiqueWhite);
                    worksheet.Cells[startRow + 1, startCol + 2].Style.Font.Bold = true;
                    worksheet.Cells[startRow + 1, startCol + 2].Style.Numberformat.Format = "₹ #,##0.00";

                    worksheet.Cells[startRow + 1, startCol, startRow + 1, startCol + 2].Style.Font.Size = 12;
                    worksheet.Cells[startRow + 1, startCol, startRow + 1, startCol + 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[startRow + 1, startCol, startRow + 1, startCol + 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    // Set the height of the summary totals row
                    worksheet.Row(startRow + 1).Height = 20;
                    startRow += 3;

                    // Section header for each company
                    worksheet.Cells[startRow, startCol].Value = company.FirmName;
                    worksheet.Cells[startRow, startCol].Style.Font.Size = 23;
                    worksheet.Cells[startRow, startCol].Style.Font.Bold = true;
                    worksheet.Cells[startRow, startCol, startRow, startCol + 3].Merge = true;
                    worksheet.Cells[startRow, startCol, startRow, startCol + 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[startRow, startCol, startRow, startCol + 3].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                    worksheet.Row(startRow).Height = 30; // Set the height of the company header row
                    startRow++;

                    // Headers for each company's section
                    worksheet.Cells[startRow, startCol].Value = "Date";
                    worksheet.Cells[startRow, startCol + 1].Value = "Description";
                    worksheet.Cells[startRow, startCol + 1].Style.Fill.PatternType = ExcelFillStyle.Solid; // Solid fill
                    worksheet.Cells[startRow, startCol + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Yellow);
                    worksheet.Cells[startRow, startCol + 2].Value = "Due";
                    worksheet.Cells[startRow, startCol + 2].Style.Fill.PatternType = ExcelFillStyle.Solid; // Solid fill
                    worksheet.Cells[startRow, startCol + 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.IndianRed);
                    worksheet.Cells[startRow, startCol + 3].Value = "Paid";
                    worksheet.Cells[startRow, startCol + 3].Style.Fill.PatternType = ExcelFillStyle.Solid; // Solid fill
                    worksheet.Cells[startRow, startCol + 3].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGreen);

                    worksheet.Cells[startRow, startCol, startRow, startCol + 3].Style.Font.Size = 14;
                    worksheet.Cells[startRow, startCol, startRow, startCol].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[startRow, startCol, startRow, startCol].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    worksheet.Cells[startRow, startCol, startRow, startCol + 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[startRow, startCol, startRow, startCol + 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells[startRow, startCol, startRow, startCol + 3].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                    worksheet.Cells[startRow, startCol, startRow, startCol + 3].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                    worksheet.Cells[startRow, startCol, startRow, startCol + 3].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                    worksheet.Cells[startRow, startCol, startRow, startCol + 3].Style.Border.Right.Style = ExcelBorderStyle.Medium;

                    // Set the height of the headers row for each company's section
                    worksheet.Row(startRow).Height = 25;
                    startRow++;

                    // Add payments data
                    foreach (var payment in payments)
                    {
                        worksheet.Cells[startRow, startCol].Value = payment.Date.ToString("dd-MM-yyyy");
                        worksheet.Cells[startRow, startCol + 1].Value = payment.Description;
                        if (payment.PaymentCategory == "Due Payment")
                        {
                            worksheet.Cells[startRow, startCol + 2].Value = payment.TotalAmount;
                            worksheet.Cells[startRow, startCol + 2].Style.Font.Bold = true;
                            worksheet.Cells[startRow, startCol + 2].Style.Fill.PatternType = ExcelFillStyle.Solid; // Solid fill
                            worksheet.Cells[startRow, startCol + 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightPink);
                            worksheet.Cells[startRow, startCol + 2].Style.Numberformat.Format = "₹ #,##0.00";
                        }
                        worksheet.Cells[startRow, startCol + 3].Value = payment.PaidAmount;
                        worksheet.Cells[startRow, startCol + 3].Style.Font.Bold = true;
                        worksheet.Cells[startRow, startCol + 3].Style.Numberformat.Format = "₹ #,##0.00";

                        worksheet.Cells[startRow, startCol, startRow, startCol + 3].Style.Font.Size = 12;
                        worksheet.Cells[startRow, startCol, startRow, startCol + 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[startRow, startCol, startRow, startCol + 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                        // Set the height of each payment row
                        worksheet.Row(startRow).Height = 20;

                        startRow++;
                    }

                    // Reset the row and move to the next section
                    startRow = 4;
                    startCol += 5; // Move to the next section horizontally
                }

                // Auto-fit columns
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Manually set the column widths again after auto-fitting
                for (int i = 1; i <= startCol; i++)
                {
                    worksheet.Column(i).Width = 20;
                    worksheet.Column(i).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Column(i).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                }

                // Return the Excel file as a downloadable file
                byte[] excelData = package.GetAsByteArray();
                return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }
        #endregion





    }
}





