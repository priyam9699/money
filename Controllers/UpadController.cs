using FinanceManagement.IRepository;
using FinanceManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Security.Claims;

namespace FinanceManagement.Controllers
{
    public class UpadController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IExpensesRepository expensesRepository;
        private readonly ICashFlowRepository cashFlowRepository;
        private readonly IPaymentRepository paymentRepository;
        private readonly IUpadRepository upadRepository;

        public UpadController(ApplicationDbContext context, IExpensesRepository expensesRepository, ICashFlowRepository cashFlowRepository, IPaymentRepository paymentRepository, IUpadRepository upadRepository)
        {
            _context = context;
            this.expensesRepository = expensesRepository;
            this.cashFlowRepository = cashFlowRepository;
            this.paymentRepository = paymentRepository;
            this.upadRepository = upadRepository;
        }

        #region Upad Index
        [HttpGet]
        public IActionResult UpadIndex(int? month, int? year)
        {
            var upads = _context.Upads.AsQueryable();

            if (month.HasValue && year.HasValue)
            {
                upads = upads.Where(cf => cf.Date.Month == month.Value && cf.Date.Year == year.Value);
            }
            else if (year.HasValue)
            {
                upads = upads.Where(cf => cf.Date.Year == year.Value);
            }

            return View(upads.ToList());
        }
        #endregion


        #region ExportToExcel
        public IActionResult ExportToExcel(DateTime? startDate, DateTime? endDate)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get current user's ID
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            var companyName = user != null ? user.CompanyName : null;

            IEnumerable<Upad> upads = Enumerable.Empty<Upad>();

            if (!string.IsNullOrEmpty(companyName))
            {
                upads = upadRepository.GetUpadsFromCompanyName(companyName);

                if (startDate.HasValue && endDate.HasValue)
                {
                    upads = upads.Where(p => p.Date >= startDate && p.Date <= endDate).ToList();
                }
                upads = upads.OrderBy(o => o.Date).ToList();
            }

            string startDateString = startDate?.ToString("dd-MM-yyyy") ?? "StartDate";
            string endDateString = endDate?.ToString("dd-MM-yyyy") ?? "EndDate";

            string fileName = $"Upad`{startDateString}_to_{endDateString}.xlsx";

            // Create an instance of the ExcelPackage
            using (var package = new ExcelPackage())
            {
                // Create a worksheet
                var worksheet = package.Workbook.Worksheets.Add("Upad");


                // Set the title
                worksheet.Cells["A1"].Value = $"Upad Report ({startDateString} to {endDateString})"; // Title text
                worksheet.Cells["A1:K1"].Merge = true; // Merge cells for the title
                worksheet.Cells["A1:K1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A1:K1"].Style.Font.Size = 25; // Increase the font size
                worksheet.Cells["A1:K1"].Style.Font.Bold = true; // Bold font
                worksheet.Cells["A1:K1"].Style.Fill.PatternType = ExcelFillStyle.Solid; // Solid fill
                worksheet.Cells["A1:K1"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);

                worksheet.Cells["A1:K1"].Style.Border.Top.Style = ExcelBorderStyle.Medium; // Top border
                worksheet.Cells["A1:K1"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium; // Bottom border
                worksheet.Cells["A1:K1"].Style.Border.Left.Style = ExcelBorderStyle.Medium; // Left border
                worksheet.Cells["A1:K1"].Style.Border.Right.Style = ExcelBorderStyle.Medium;

                worksheet.Cells["A3:D3"].Style.Font.Bold = true;
                worksheet.Cells["A3:D3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A3:D3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["A3:D3"].Style.Font.Size = 16; // Increase the font size
                                                               // Apply background color to the entire row
                worksheet.Cells["A3:D3"].Style.Fill.PatternType = ExcelFillStyle.Solid; // Solid fill
                worksheet.Cells["A3:D3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                worksheet.Cells["A3:D3"].Style.Border.Top.Style = ExcelBorderStyle.Thin; // Top border
                worksheet.Cells["A3:D3"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin; // Bottom border
                worksheet.Cells["A3:D3"].Style.Border.Left.Style = ExcelBorderStyle.Thin; // Left border
                worksheet.Cells["A3:D3"].Style.Border.Right.Style = ExcelBorderStyle.Thin; // Right border

                worksheet.Cells["H3:I3"].Style.Font.Bold = true;
                worksheet.Cells["H3:I3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["H3:I3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["H3:I3"].Style.Font.Size = 17; // Increase the font size
                                                               // Apply background color to the entire row
                worksheet.Cells["H3:I3"].Style.Fill.PatternType = ExcelFillStyle.Solid; // Solid fill
                worksheet.Cells["H3:I3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                worksheet.Cells["H3:I3"].Style.Border.Top.Style = ExcelBorderStyle.Thin; // Top border
                worksheet.Cells["H3:I3"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin; // Bottom border
                worksheet.Cells["H3:I3"].Style.Border.Left.Style = ExcelBorderStyle.Thin; // Left border
                worksheet.Cells["H3:I3"].Style.Border.Right.Style = ExcelBorderStyle.Thin; // Right border

                worksheet.Cells["A3"].Value = "Date";
                worksheet.Cells["B3"].Value = "Description";
                worksheet.Cells["C3"].Value = "Amount";
                worksheet.Cells["D3"].Value = "Name";
                worksheet.Cells["H3"].Value = "Name";
                worksheet.Cells["I3"].Value = "Total Upad Amount";
                


                worksheet.Cells["H4:I4"].Style.Border.Top.Style = ExcelBorderStyle.Thin; // Top border
                worksheet.Cells["H4:I4"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin; // Bottom border
                worksheet.Cells["H4:I4"].Style.Border.Left.Style = ExcelBorderStyle.Thin; // Left border
                worksheet.Cells["H4:I4"].Style.Border.Right.Style = ExcelBorderStyle.Thin; // Right border
                worksheet.Cells["H5:I5"].Style.Border.Top.Style = ExcelBorderStyle.Thin; // Top border
                worksheet.Cells["H5:I5"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin; // Bottom border
                worksheet.Cells["H5:I5"].Style.Border.Left.Style = ExcelBorderStyle.Thin; // Left border
                worksheet.Cells["H5:I5"].Style.Border.Right.Style = ExcelBorderStyle.Thin; // Right border
                worksheet.Cells["H6:I6"].Style.Border.Top.Style = ExcelBorderStyle.Thin; // Top border
                worksheet.Cells["H6:I6"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin; // Bottom border
                worksheet.Cells["H6:I6"].Style.Border.Left.Style = ExcelBorderStyle.Thin; // Left border
                worksheet.Cells["H6:I6"].Style.Border.Right.Style = ExcelBorderStyle.Thin; // Right border


                worksheet.Column(1).Width = 17;
                worksheet.Column(2).Width = 25;
                worksheet.Column(3).Width = 20;
                worksheet.Column(4).Width = 20;
                
                worksheet.Column(8).Width = 30;
                worksheet.Column(9).Width = 30;
                worksheet.Column(10).Width = 30;

                worksheet.Row(2).Height = 45;
                worksheet.Row(3).Height = 45;

                // Add data to the worksheet
                int row = 4;

                

                // Calculate totals for each UpadOption
                var upadTotals = upads
                    .GroupBy(u => u.UpadOption)
                    .Select(g => new
                    {
                        UpadOption = g.Key,
                        TotalAmount = g.Sum(u => u.Amount)
                    })
                    .ToList();

                foreach (var upad in upads)
                {
                    worksheet.Cells[$"A{row}:D{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[$"A{row}:E{row}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Row(row).Height = 28;
                    worksheet.Cells[$"A{row}"].Value = upad.Date.ToString("dd-MM-yyyy");
                    worksheet.Cells[$"A{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Center-align Price
                    worksheet.Cells[$"B{row}"].Value = upad.Description;
                    worksheet.Cells[$"B{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Center-align Date
                    worksheet.Cells[$"C{row}"].Value = upad.Amount;
                    worksheet.Cells[$"C{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Center-align ProductName
                    worksheet.Cells[$"C{row}"].Style.Numberformat.Format = "₹ #,##0.00";
                    worksheet.Cells[$"D{row}"].Value = upad.UpadOption;
                    worksheet.Cells[$"D{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Center-align ProductName
                    worksheet.Cells[$"H{row}:J{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    worksheet.Cells[$"B{row}"].Style.Font.Bold = true;
                    worksheet.Cells[$"H{row}:I{row}"].Style.Font.Bold = true;
                    worksheet.Cells[$"H{row}:I{row}"].Style.Font.Size = 22;

                    worksheet.Cells[$"A{row}:F{row}"].Style.Font.Size = 14;
                    worksheet.Cells[$"B{row}"].Style.Fill.PatternType = ExcelFillStyle.Solid; // Solid fill
                    worksheet.Cells[$"B{row}"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Yellow);

                    worksheet.Cells[$"B{row}"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    
                    row++;
                }

                int totalRow = 4;
                decimal grandTotal = 0;
                decimal totalHarsh = 0;
                decimal totalVijay = 0;

                foreach (var upadTotal in upadTotals)
                {
                    worksheet.Cells[$"H{totalRow}"].Value = upadTotal.UpadOption;
                    worksheet.Cells[$"I{totalRow}"].Value = upadTotal.TotalAmount;
                    worksheet.Cells[$"I{totalRow}"].Style.Numberformat.Format = "₹ #,##0.00";
                    worksheet.Cells[$"H{totalRow}:I{totalRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[$"H{totalRow}:I{totalRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells[$"H{totalRow}:I{totalRow}"].Style.Font.Size = 14;
                    grandTotal += upadTotal.TotalAmount;
                    if (upadTotal.UpadOption == "Harsh")
                    {
                        totalHarsh = upadTotal.TotalAmount;
                    }
                    else if (upadTotal.UpadOption == "Vijay")
                    {
                        totalVijay = upadTotal.TotalAmount;
                    }
                    worksheet.Row(totalRow).Height = 28;
                    totalRow++;
                }

                worksheet.Cells["J4"].Formula = $"=\"₹\" & I4 + I5";
                worksheet.Cells["J4:J5"].Merge = true;
                // Apply style to the merged cell
                worksheet.Cells["J4:J5"].Style.Font.Size = 20; // Set font size
                worksheet.Cells[$"I{totalRow}"].Formula = $"=\"₹\" & SUM(I4:I{totalRow - 1})";
                worksheet.Cells["J4:J5"].Style.HorizontalAlignment= ExcelHorizontalAlignment.Center;
                worksheet.Cells["J4:J5"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["J4:J5"].Style.Font.Bold = true;
                worksheet.Cells["J4:J5"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["J4:J5"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Yellow);
                worksheet.Cells[$"I{totalRow}"].Value = grandTotal;
                
                worksheet.Cells[$"H{totalRow}:I{totalRow}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"H{totalRow}:I{totalRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[$"H{totalRow}:I{totalRow}"].Style.Font.Size = 20;
                worksheet.Cells[$"H{totalRow}:I{totalRow}"].Style.Font.Bold = true;
                worksheet.Cells[$"H{totalRow}:I{totalRow}"].Style.Numberformat.Format = "₹ #,##0.00";
                worksheet.Row(totalRow).Height = 28;


                // Fill background color for total amount cells
                worksheet.Cells["H4:I4"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["H4:I4"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.AntiqueWhite);
                worksheet.Cells["H5:I5"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["H5:I5"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.AntiqueWhite);
                worksheet.Cells["H6:I6"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["H6:I6"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.AntiqueWhite);
                
                worksheet.Cells["I7"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["I7"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGreen);



                // Return the Excel file as a downloadable file
                byte[] excelData = package.GetAsByteArray();
                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }
        #endregion

    }
}
