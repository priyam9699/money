using FinanceManagement.IRepository;
using FinanceManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Security.Claims;

namespace FinanceManagement.Controllers
{
    public class ExpensesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IExpensesRepository expensesRepository;
        private readonly ICashFlowRepository cashFlowRepository;
        private readonly IPaymentRepository paymentRepository;
        private readonly IUpadRepository upadRepository;

        public ExpensesController(ApplicationDbContext context, IExpensesRepository expensesRepository, ICashFlowRepository cashFlowRepository, IPaymentRepository paymentRepository, IUpadRepository upadRepository)
        {
            _context = context;
            this.expensesRepository = expensesRepository;
            this.cashFlowRepository = cashFlowRepository;
            this.paymentRepository = paymentRepository;
            this.upadRepository = upadRepository;
        }

        #region Expense Index
        [HttpGet]
        public IActionResult ExpensesIndex(int? month, int? year)
        {
            var expenses = _context.expenses.AsQueryable();

            if (month.HasValue && year.HasValue)
            {
                expenses = expenses.Where(cf => cf.Date.Month == month.Value && cf.Date.Year == year.Value);
            }
            else if (year.HasValue)
            {
                expenses = expenses.Where(cf => cf.Date.Year == year.Value);
            }

            return View(expenses.ToList());
        }
        #endregion


        #region Expense Excel
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
            var OverAllExpenseAmount = expensesRepository.GetOverAllExpenseAmount(companyName, startDate, endDate);

            if (!companies.Any())
            {
                return BadRequest("No companies found for the user.");
            }

            string startDateString = startDate?.ToString("dd-MM-yyyy") ?? "StartDate";
            string endDateString = endDate?.ToString("dd-MM-yyyy") ?? "EndDate";
            string fileName = $"Expenses {startDateString}_to_{endDateString}.xlsx";

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Expenses");

                // Header for overall table
                worksheet.Cells["A1"].Value = $"Expenses Report ({startDateString} to {endDateString})";
                worksheet.Cells["A1"].Style.Font.Size = 25;
                worksheet.Cells["A1"].Style.Font.Bold = true;
                worksheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A1:K1"].Merge = true;
                worksheet.Cells["A1:K1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A1:K1"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);

                // Set the height of the header row
                worksheet.Row(1).Height = 35;


                // Insert the overall total expense amount at the top
                worksheet.Cells["A3:B3"].Merge = true;
                worksheet.Cells["A3:B3"].Value = "Overall Total Expense:";
                worksheet.Cells["A3:B3"].Style.Font.Size = 17;
                worksheet.Cells["A3:B3"].Style.Font.Bold = true;
                worksheet.Cells["A3:B3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells["C3"].Value = OverAllExpenseAmount;
                worksheet.Cells["C3"].Style.Font.Size = 17;
                worksheet.Cells["C3"].Style.Font.Bold = true;
                worksheet.Cells["C3"].Style.Numberformat.Format = "₹#,##0.00";
                worksheet.Cells["C3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["C3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.AntiqueWhite);



                int row = 7;
                // Add Office section header
                worksheet.Cells[row, 1].Value = "Cash Expenses";
                worksheet.Cells[row, 1].Style.Font.Size = 23;
                worksheet.Cells[row, 1].Style.Font.Bold = true;
                worksheet.Cells[row, 1, row, 3].Merge = true;
                worksheet.Cells[row, 1, row, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[row, 1, row, 3].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                worksheet.Row(row).Height = 30;
                row++;

                // Add headers for the "Office" section
                worksheet.Cells[row, 1].Value = "Date";
                worksheet.Cells[row, 2].Value = "Description";
                worksheet.Cells[row, 3].Value = "Amount";
                worksheet.Cells[row, 1, row, 3].Style.Font.Size = 14;
                
                worksheet.Cells[row, 1, row, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[row, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                worksheet.Cells[row, 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Yellow);
                worksheet.Cells[row, 3].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.IndianRed);
                worksheet.Cells[row, 1, row, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[row, 1, row, 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[row, 1, row, 3].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                worksheet.Cells[row, 1, row, 3].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                worksheet.Cells[row, 1, row, 3].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                worksheet.Cells[row, 1, row, 3].Style.Border.Right.Style = ExcelBorderStyle.Medium;

                // Set the height of the headers row for the "Office" section
                worksheet.Row(row).Height = 25;
                row++;

                // Add Office expense data
                var officeExpenses = expensesRepository.GetOfficeExpenses(startDate, endDate);
                foreach (var expense in officeExpenses)
                {
                    worksheet.Cells[row, 1].Value = expense.Date.ToString("dd-MM-yyyy"); ;
                    worksheet.Cells[row, 2].Value = expense.Description;
                    worksheet.Cells[row, 3].Value = expense.Amount;
                    worksheet.Cells[row, 3].Style.Font.Bold = true;
                    worksheet.Cells[row, 3].Style.Fill.PatternType = ExcelFillStyle.Solid; // Solid fill
                    worksheet.Cells[row, 3].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightPink);
                    worksheet.Cells[row, 3].Style.Numberformat.Format = "₹#,##0.00";
                    worksheet.Cells[row, 3].Style.Font.Size = 15;

                    worksheet.Cells[row, 1, row, 3].Style.Font.Size = 12;
                    worksheet.Cells[row, 1, row, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[row, 1, row, 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    // Set the height of each expense row
                    worksheet.Row(row).Height = 20;
                    row++;
                }


                int startRow = 4;
                int startCol = 5;

                foreach (var company in companies)
                {
                    // Fetch payments for the current company
                    var expense = expensesRepository.GetExpensesFromCompanyName(company.FirmName);

                    // Filter payments based on the provided start and end dates
                    if (startDate.HasValue && endDate.HasValue)
                    {
                        expense = expense.Where(p => p.Date >= startDate && p.Date <= endDate).ToList();
                    }

                    // Calculate summary totals
                    decimal TotalExpense = expensesRepository.GetTotalExpenseAmount(company.FirmName, startDate,  endDate);
                    

                    // Summary headers
                    worksheet.Cells[startRow, startCol].Value = "Total Expense";
                    

                    worksheet.Cells[startRow, startCol, startRow, startCol].Style.Font.Bold = true;
                    worksheet.Cells[startRow, startCol, startRow, startCol].Style.Font.Size = 14;
                    worksheet.Cells[startRow, startCol, startRow, startCol + 1].Merge = true;
                    worksheet.Cells[startRow, startCol, startRow, startCol + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[startRow, startCol, startRow, startCol + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    worksheet.Cells[startRow, startCol, startRow, startCol + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[startRow, startCol, startRow, startCol + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[startRow, startCol, startRow, startCol + 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells[startRow, startCol, startRow, startCol + 1].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                    worksheet.Cells[startRow, startCol, startRow, startCol + 1].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                    worksheet.Cells[startRow, startCol, startRow, startCol + 1].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                    worksheet.Cells[startRow, startCol, startRow, startCol + 1].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                    worksheet.Cells[startRow, startCol, startRow, startCol + 1].Style.Border.Right.Style = ExcelBorderStyle.Medium;

                    // Set the height of the summary header row
                    worksheet.Row(startRow).Height = 25;

                    worksheet.Cells[startRow + 1, startCol, startRow + 1, startCol + 1].Merge = true;
                    worksheet.Cells[startRow + 1, startCol].Value = TotalExpense;
                    worksheet.Cells[startRow + 1, startCol, startRow + 1, startCol + 1].Style.Font.Bold = true;
                    worksheet.Cells[startRow + 1, startCol, startRow + 1, startCol + 1].Style.Font.Size = 17;
                    worksheet.Cells[startRow + 1, startCol, startRow + 1, startCol + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[startRow + 1, startCol, startRow + 1, startCol + 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells[startRow + 1, startCol, startRow + 1, startCol + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[startRow + 1, startCol, startRow + 1, startCol + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.AntiqueWhite);
                    worksheet.Cells[startRow + 1, startCol].Style.Numberformat.Format = "₹#,##0.00";




                    // Set the height of the summary totals row
                    worksheet.Row(startRow + 1).Height = 25;
                    startRow += 3;

                    // Section header for each company
                    worksheet.Cells[startRow, startCol].Value = company.FirmName;
                    worksheet.Cells[startRow, startCol].Style.Font.Size = 23;
                    worksheet.Cells[startRow, startCol].Style.Font.Bold = true;
                    worksheet.Cells[startRow, startCol, startRow, startCol + 2].Merge = true;
                    worksheet.Cells[startRow, startCol, startRow, startCol + 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[startRow, startCol, startRow, startCol + 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                    worksheet.Row(startRow).Height = 30; // Set the height of the company header row
                    startRow++;

                    // Headers for each company's section
                    worksheet.Cells[startRow, startCol].Value = "Date";
                    worksheet.Cells[startRow, startCol + 1].Value = "Description";
                    worksheet.Cells[startRow, startCol + 1].Style.Fill.PatternType = ExcelFillStyle.Solid; // Solid fill
                    worksheet.Cells[startRow, startCol + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Yellow);
                    worksheet.Cells[startRow, startCol + 2].Value = "Amount";
                    worksheet.Cells[startRow, startCol + 2].Style.Fill.PatternType = ExcelFillStyle.Solid; // Solid fill
                    worksheet.Cells[startRow, startCol + 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.IndianRed);
                    

                    worksheet.Cells[startRow, startCol, startRow, startCol + 2].Style.Font.Size = 14;
                    worksheet.Cells[startRow, startCol, startRow, startCol].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[startRow, startCol, startRow, startCol].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    worksheet.Cells[startRow, startCol, startRow, startCol + 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[startRow, startCol, startRow, startCol + 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells[startRow, startCol, startRow, startCol + 2].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                    worksheet.Cells[startRow, startCol, startRow, startCol + 2].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                    worksheet.Cells[startRow, startCol, startRow, startCol + 2].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                    worksheet.Cells[startRow, startCol, startRow, startCol + 2].Style.Border.Right.Style = ExcelBorderStyle.Medium;

                    // Set the height of the headers row for each company's section
                    worksheet.Row(startRow).Height = 25;
                    startRow++;

                    // Add payments data
                    foreach (var payment in expense)
                    {
                        worksheet.Cells[startRow, startCol].Value = payment.Date.ToString("dd-MM-yyyy");
                        worksheet.Cells[startRow, startCol + 1].Value = payment.Description;
                        
                            worksheet.Cells[startRow, startCol + 2].Value = payment.Amount;
                            worksheet.Cells[startRow, startCol + 2].Style.Font.Bold = true;
                            worksheet.Cells[startRow, startCol + 2].Style.Fill.PatternType = ExcelFillStyle.Solid; // Solid fill
                            worksheet.Cells[startRow, startCol + 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightPink);
                            worksheet.Cells[startRow, startCol + 2].Style.Numberformat.Format = "₹ #,##0.00";
                        
                        

                        worksheet.Cells[startRow, startCol, startRow, startCol + 3].Style.Font.Size = 12;
                        worksheet.Cells[startRow, startCol, startRow, startCol + 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[startRow, startCol, startRow, startCol + 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                        // Set the height of each payment row
                        worksheet.Row(startRow).Height = 20;

                        startRow++;
                    }

                    // Reset the row and move to the next section
                    startRow = 4;
                    startCol += 4; // Move to the next section horizontally
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



        //#region Expense Excel
        //public IActionResult ExportToExcel(DateTime? startDate, DateTime? endDate)
        //{
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get current user's ID
        //    var user = _context.Users.FirstOrDefault(u => u.Id == userId);

        //    if (user == null)
        //    {
        //        return BadRequest("User not found.");
        //    }

        //    var companies = _context.Companies.Where(c => c.UserId == userId).ToList();

        //    if (!companies.Any())
        //    {
        //        return BadRequest("No companies found for the user.");
        //    }

        //    string startDateString = startDate?.ToString("dd-MM-yyyy") ?? "StartDate";
        //    string endDateString = endDate?.ToString("dd-MM-yyyy") ?? "EndDate";
        //    string fileName = $"Expenses_{startDateString}_to_{endDateString}.xlsx";

        //    using (var package = new ExcelPackage())
        //    {
        //        var worksheet = package.Workbook.Worksheets.Add("Expenses");

        //        // Header for overall table
        //        worksheet.Cells["A1"].Value = $"Expenses Report ({startDateString} to {endDateString})";
        //        worksheet.Cells["A1"].Style.Font.Size = 25;
        //        worksheet.Cells["A1"].Style.Font.Bold = true;
        //        worksheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //        worksheet.Cells["A1:K1"].Merge = true;
        //        worksheet.Cells["A1:K1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
        //        worksheet.Cells["A1:K1"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);

        //        int startRow = 3;
        //        int startCol = 1;

        //        // Define columns for date and each company's expenses
        //        worksheet.Cells[startRow, startCol].Value = "Date";
        //        worksheet.Cells[startRow, startCol].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //        worksheet.Cells[startRow, startCol].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        //        worksheet.Cells[startRow, startCol].Style.Font.Size = 30;

        //        int col = startCol + 1;
        //        foreach (var company in companies)
        //        {
        //            worksheet.Cells[startRow, col].Value = company.FirmName;
        //            worksheet.Cells[startRow, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
        //            worksheet.Cells[startRow, col].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Yellow); // Set background color to yellow
        //            worksheet.Cells[startRow, col + 1].Value = "Description";
        //            col += 2;
        //        }

        //        // Styling headers
        //        worksheet.Cells[startRow, startCol, startRow, col - 1].Style.Font.Bold = true;
        //        worksheet.Cells[startRow, startCol, startRow, col - 1].Style.Font.Size = 14;
        //        worksheet.Cells[startRow, startCol, startRow, col - 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
        //        worksheet.Cells[startRow, startCol, startRow, col - 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
        //        worksheet.Cells[startRow, startCol, startRow, col - 1].Style.Border.Top.Style = ExcelBorderStyle.Medium; // Top border
        //        worksheet.Cells[startRow, startCol, startRow, col - 1].Style.Border.Bottom.Style = ExcelBorderStyle.Medium; // Bottom border
        //        worksheet.Cells[startRow, startCol, startRow, col - 1].Style.Border.Left.Style = ExcelBorderStyle.Medium; // Left border
        //        worksheet.Cells[startRow, startCol, startRow, col - 1].Style.Border.Right.Style = ExcelBorderStyle.Medium; // Right border

        //        // Increase row height for the headers
        //        worksheet.Row(startRow).Height = 30;

        //        int row = startRow + 1;

        //        // Fetch expenses for each company
        //        var allExpenses = new List<(DateTime date, string firmName, string description, decimal amount)>();
        //        var companyTotals = new Dictionary<string, decimal>();
        //        foreach (var company in companies)
        //        {
        //            // Retrieve expenses for the current company
        //            var expenses = expensesRepository.GetExpensesFromCompanyName(company.FirmName);

        //            // Filter expenses based on the provided start and end dates
        //            if (startDate.HasValue && endDate.HasValue)
        //            {
        //                expenses = expenses.Where(p => p.Date >= startDate && p.Date <= endDate).ToList();
        //            }

        //            // Calculate total amount for the current company
        //            decimal totalAmount = expenses.Sum(e => e.Amount);
        //            companyTotals[company.FirmName] = totalAmount;

        //            // Add the filtered expenses to the list
        //            foreach (var expense in expenses)
        //            {
        //                allExpenses.Add((expense.Date, company.FirmName, expense.Description, expense.Amount));
        //            }
        //        }

        //        // Insert totals into the worksheet
        //        col = startCol + 1;
        //        foreach (var company in companies)
        //        {
        //            if (companyTotals.ContainsKey(company.FirmName))
        //            {
        //                var totalCell = worksheet.Cells[startRow - 1, col];
        //                totalCell.Value = companyTotals[company.FirmName];

        //                // Set background color to green
        //                totalCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
        //                totalCell.Style.Font.Bold = true;
        //                totalCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGreen);
        //                totalCell.Style.Font.Size = 16;
        //            }
        //            col += 2;
        //        }

        //        // Group expenses by date
        //        var groupedExpenses = allExpenses.GroupBy(e => e.date).OrderBy(g => g.Key);

        //        // Add data to the worksheet
        //        foreach (var group in groupedExpenses)
        //        {
        //            worksheet.Cells[row, startCol].Value = group.Key.ToString("dd-MM-yyyy");
        //            col = startCol + 1;
        //            foreach (var company in companies)
        //            {
        //                var companyExpenses = group.Where(e => e.firmName == company.FirmName).ToList();
        //                if (companyExpenses.Any())
        //                {
        //                    var expense = companyExpenses.First();
        //                    worksheet.Cells[row, col].Value = expense.amount;
        //                    worksheet.Cells[row, col].Style.Font.Size = 12;
        //                    worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
        //                    worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Yellow); // Set background color to yellow
        //                    worksheet.Cells[row, col].Style.Font.Bold = true;
        //                    worksheet.Cells[row, col + 1].Value = expense.description;
        //                    worksheet.Cells[row, col + 1].Style.Font.Size = 12;
        //                }
        //                col += 2;
        //            }
        //            row++;
        //        }

        //        // Increase row height for the data rows
        //        for (int r = startRow + 1; r < row; r++)
        //        {
        //            worksheet.Row(r).Height = 20;
        //        }

        //        // Auto-fit columns
        //        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

        //        // Manually set the column widths again after auto-fitting
        //        for (int i = 2; i <= col - 1; i += 2)
        //        {
        //            worksheet.Column(i).Width = 20; // Increase width for Amount columns
        //            worksheet.Column(i).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //            worksheet.Column(i).Style.VerticalAlignment = ExcelVerticalAlignment.Center;

        //            worksheet.Column(i + 1).Width = 30; // Increase width for Description columns
        //            worksheet.Column(i + 1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //            worksheet.Column(i + 1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        //        }

        //        // Return the Excel file as a downloadable file
        //        byte[] excelData = package.GetAsByteArray();
        //        return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        //    }
        //}
        //#endregion




    }
}
