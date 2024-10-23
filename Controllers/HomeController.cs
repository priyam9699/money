using FinanceManagement.Models;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Diagnostics;
using System.Security.Claims;
using FinanceManagement.IRepository;
using FinanceManagement.ViewModels;

namespace FinanceManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext context;
        private readonly ICashFlowRepository cashFlowRepository;
        private readonly IAccountMasterRepository accountMasterRepository;
        private readonly IExpensesRepository expensesRepository;
        private readonly IPaymentRepository paymentRepository;
        private readonly IUpadRepository upadRepository;
        private readonly IDashboardRepository dashboardRepository;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, ICashFlowRepository cashFlowRepository, IAccountMasterRepository accountMasterRepository, IExpensesRepository expensesRepository, IPaymentRepository paymentRepository, IUpadRepository upadRepository, IDashboardRepository dashboardRepository)
        {
            _logger = logger;
            this.context = context;
            this.cashFlowRepository = cashFlowRepository;
            this.accountMasterRepository = accountMasterRepository;
            this.expensesRepository = expensesRepository;
            this.paymentRepository = paymentRepository;
            this.upadRepository = upadRepository;
            this.dashboardRepository = dashboardRepository;
        }

        public IActionResult Dashboard(DateTime? startDate, DateTime? endDate, int SelectedMonth, int month)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = context.Users.FirstOrDefault(u => u.Id == userId);
            var companyName = user?.CompanyName;

            // Retrieve total amounts for the specified month
            var totalAmount = dashboardRepository.GetTotalAmount(userId, companyName, SelectedMonth);
            var dashboardData = dashboardRepository.GetDashboardData(userId, companyName, startDate, endDate, SelectedMonth);
            var accountBalances = accountMasterRepository.GetAccountBalancesForCompanyName(companyName, startDate, endDate, month);
            var TotalUpad = upadRepository.GetUpadTotal(companyName, startDate, endDate);
            var TotalCashBalance = cashFlowRepository.GetCashFlowTotal(companyName, startDate, endDate);

            var viewModel = new DashboardIndexVM
            {
                TotalAmountMonthly = totalAmount,
                FirmAccountBalances = accountBalances
            };

            ViewBag.TotalUpadAmount = TotalUpad;
            ViewBag.TotalCashBalance = TotalCashBalance;

            return View(viewModel);
        }


        #region Dashboard Excel
        public IActionResult ExportToExcel(DateTime? startDate, DateTime? endDate, int SelectedMonth, int year, int month)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = context.Users.FirstOrDefault(u => u.Id == userId);
            var companyName = user?.CompanyName;

            DateTime start = startDate ?? DateTime.MinValue;
            DateTime end = endDate ?? DateTime.MaxValue;

            var dashboardData = dashboardRepository.GetDashboardData(userId,companyName, startDate, endDate, SelectedMonth);
            decimal TOTAL = accountMasterRepository.GetTotalAccountBalance(userId,companyName, month);
            
            decimal? advancePayment = context.Payments
                                    .Where(p => p.UserId == userId && p.PaymentCategory == "Advance Payment" && p.Date >= start && p.Date <= end)
                                    .Sum(p => (decimal?)p.PaidAmount);

            string startDateString = startDate?.ToString("dd-MM-yyyy") ?? "StartDate";
            string endDateString = endDate?.ToString("dd-MM-yyyy") ?? "EndDate";

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Dashboard");

                worksheet.Cells["A1"].Value = $"Dashboard Report ({startDateString} to {endDateString})";
                worksheet.Cells["A1:G1"].Merge = true;
                worksheet.Cells["A1:G1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A1:G1"].Style.Font.Size = 30;
                worksheet.Cells["A1:G1"].Style.Font.Bold = true;
                worksheet.Cells["A1:G1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A1:G1"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);
                worksheet.Cells["A1:G1"].Style.Border.BorderAround(ExcelBorderStyle.Medium);

                worksheet.Cells["A3:G3"].Style.Font.Bold = true;
                worksheet.Cells["A3:G3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A3:G3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["A3:G3"].Style.Font.Size = 18;
                worksheet.Cells["A3:G3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A3:G3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                worksheet.Cells["A3:G3"].Style.Border.Top.Style = ExcelBorderStyle.Medium; // Top border
                worksheet.Cells["A3:G3"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium; // Bottom border
                worksheet.Cells["A3:G3"].Style.Border.Left.Style = ExcelBorderStyle.Medium; // Left border
                worksheet.Cells["A3:G3"].Style.Border.Right.Style = ExcelBorderStyle.Medium;

                worksheet.Cells["A3"].Value = "Company";
                worksheet.Cells["B3"].Value = "Account Balance";
                worksheet.Cells["C3"].Value = "Flipkart Out Standing Payment";
                worksheet.Cells["D3"].Value = "Amazon Out Standing Payment";
                worksheet.Cells["E3"].Value = "Meesho Out Standing Payment";
                worksheet.Cells["F3"].Value = "Other Out Standing Payment";
                worksheet.Cells["G3"].Value = "Pending Payment";

                worksheet.Column(1).Width = 25;
                worksheet.Column(2).Width = 35;
                worksheet.Column(3).Width = 46;
                worksheet.Column(4).Width = 46;
                worksheet.Column(5).Width = 46;
                worksheet.Column(6).Width = 42;
                worksheet.Column(7).Width = 28;

                worksheet.Row(1).Height = 45;
                worksheet.Row(3).Height = 45;

                int row = 4;
                // Initialize totals
                decimal? totalAccountBalance = 0;
                decimal? totalFlipkartOutstandingPayment = 0;
                decimal? totalAmazonOutstandingPayment = 0;
                decimal? totalMeeshoOutstandingPayment = 0;
                decimal? totalOtherOutstandingPayment = 0;
                decimal? totalPendingPayment = 0;
                decimal? cashBalance = 0;
                decimal? totalUpad = 0;

                foreach (var data in dashboardData)
                {

                    //Company Name
                    worksheet.Cells[row, 1].Value = data.Company;
                    worksheet.Cells[row, 1].Style.Font.Size = 15;
                    worksheet.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[row, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells[row, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[row, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Yellow);
                    worksheet.Cells[row, 1].Style.Border.Top.Style = ExcelBorderStyle.Medium; // Left border
                    worksheet.Cells[row, 1].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                    worksheet.Cells[row, 1].Style.Border.Left.Style = ExcelBorderStyle.Medium; // Left border
                    worksheet.Cells[row, 1].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                    worksheet.Row(row).Height = 30;

                    //Account Balance
                    worksheet.Cells[row, 2].Value = data.AccountBalance;
                    worksheet.Cells[row, 2].Style.Numberformat.Format = "₹ #,##0.00";
                    worksheet.Cells[row, 2].Style.Font.Size = 15;
                    worksheet.Cells[row, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    // Read values from cells C4, C5, and C6

                    // Read the values from cells C4 to C11
                    decimal c4Value = worksheet.Cells["C4"].GetValue<decimal>();
                    decimal c5Value = worksheet.Cells["C5"].GetValue<decimal>();
                    decimal c6Value = worksheet.Cells["C6"].GetValue<decimal>();
                    decimal c7Value = worksheet.Cells["C7"].GetValue<decimal>();
                    decimal c8Value = worksheet.Cells["C8"].GetValue<decimal>();
                    decimal c9Value = worksheet.Cells["C9"].GetValue<decimal>();
                    decimal c10Value = worksheet.Cells["C10"].GetValue<decimal>();
                    decimal c11Value = worksheet.Cells["C11"].GetValue<decimal>();

                    // Sum the values
                    decimal total = c4Value + c5Value + c6Value + c7Value + c8Value + c9Value + c10Value + c11Value;

                    // Set the calculated total into a specific cell, e.g., C12
                    worksheet.Cells["C12"].Value = total;


                    worksheet.Cells["C12"].Style.Numberformat.Format = "₹ #,##0.00"; // Optional currency formatting


                    //Amazon Outstanding Payment
                    worksheet.Cells[row, 4].Value = data.AmazonOutstandingPayment;

                    //Meesho Outstanding Payment
                    worksheet.Cells[row, 5].Value = data.MeeshoOutstandingPayment;

                    //Other outstanding Payment
                    worksheet.Cells[row, 6].Value = data.OtherOutstandingPayment;


                    worksheet.Cells[row, 7].Value = data.PendingPayment;
                    worksheet.Cells[row, 7].Style.Numberformat.Format = "₹ #,##0.00";
                    worksheet.Cells[row, 7].Style.Font.Size = 15;
                    worksheet.Cells[row, 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;


                    // Accumulate totals
                    totalAccountBalance += data.AccountBalance;
                    totalFlipkartOutstandingPayment += data.FlipkartOutstandingPayment;
                    totalAmazonOutstandingPayment += data.AmazonOutstandingPayment;
                    totalMeeshoOutstandingPayment += data.MeeshoOutstandingPayment;
                    totalOtherOutstandingPayment += data.OtherOutstandingPayment;
                    totalPendingPayment += data.PendingPayment;
                    cashBalance = data.CashBalance;
                    totalUpad = data.TotalUpad;


                    row++;
                }

                var totalPlusAmount = totalAccountBalance + cashBalance + advancePayment;
                var totalMinusAmount = totalPendingPayment + totalUpad;
                var finalAmount = totalPlusAmount -  totalMinusAmount ;


                // Add a total row
                worksheet.Cells[row, 1].Value = "Total";
                worksheet.Cells[row, 1].Style.Font.Bold = true;
                worksheet.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[row, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[row, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[row, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Green);
                worksheet.Cells[row, 1].Style.Border.Top.Style = ExcelBorderStyle.Medium; // Left border
                worksheet.Cells[row, 1].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                worksheet.Cells[row, 1].Style.Border.Left.Style = ExcelBorderStyle.Medium; // Left border
                worksheet.Cells[row, 1].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                worksheet.Cells[row, 1].Style.Font.Size = 18;

                worksheet.Cells[row, 2].Value = totalAccountBalance;
                worksheet.Cells[row, 2].Style.Numberformat.Format = "₹ #,##0.00";
                worksheet.Cells[row, 2].Style.Font.Bold = true;
                worksheet.Cells[row, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[row, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[row, 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGreen);
                worksheet.Cells[row, 2].Style.Font.Size = 15;

                worksheet.Cells[row, 3].Value = totalFlipkartOutstandingPayment;
                worksheet.Cells[row, 3].Style.Numberformat.Format = "₹ #,##0.00";
                worksheet.Cells[row, 3].Style.Font.Bold = true;
                worksheet.Cells[row, 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[row, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[row, 3].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGreen);

                worksheet.Cells[row, 4].Value = totalAmazonOutstandingPayment;
                worksheet.Cells[row, 4].Style.Numberformat.Format = "₹ #,##0.00";
                worksheet.Cells[row, 4].Style.Font.Bold = true;
                worksheet.Cells[row, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[row, 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[row, 4].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGreen);

                worksheet.Cells[row, 5].Value = totalMeeshoOutstandingPayment;
                worksheet.Cells[row, 5].Style.Numberformat.Format = "₹ #,##0.00";
                worksheet.Cells[row, 5].Style.Font.Bold = true;
                worksheet.Cells[row, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[row, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[row, 5].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGreen);

                worksheet.Cells[row, 6].Value = totalOtherOutstandingPayment;
                worksheet.Cells[row, 6].Style.Numberformat.Format = "₹ #,##0.00";
                worksheet.Cells[row, 6].Style.Font.Bold = true;
                worksheet.Cells[row, 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[row, 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[row, 6].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGreen);

                worksheet.Cells[row, 7].Value = totalPendingPayment;
                worksheet.Cells[row, 7].Style.Numberformat.Format = "₹ #,##0.00";
                worksheet.Cells[row, 7].Style.Font.Bold = true;
                worksheet.Cells[row, 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[row, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[row, 7].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGreen);
                worksheet.Cells[row, 7].Style.Font.Size = 15;


                row += 4; 

                // Adding additional rows in column 2
                worksheet.Cells[row, 2].Value = "Total Amount";
                worksheet.Cells[row, 2].Style.Font.Size = 15;
                worksheet.Cells[row, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[row, 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);  
                
                worksheet.Cells[row + 1, 2].Value = "Cash Balance";
                worksheet.Cells[row + 1, 2, row + 5, 2].Style.Font.Size = 15;
                worksheet.Cells[row + 1, 2, row + 5, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[row + 1, 2, row + 5, 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightCyan);
                worksheet.Cells[row + 2, 2].Value = "Stock";
                worksheet.Cells[row + 3, 2].Value = "Warehouse Stock";
                worksheet.Cells[row + 4, 2].Value = "Provada";
                worksheet.Cells[row + 5, 2].Value = "Other (Packaging Material)";


                worksheet.Cells[row + 6, 2].Value = "Belt";
                worksheet.Cells[row + 7, 2].Value = "Upad";
                worksheet.Cells[row + 8, 2].Value = "Total Pending Amount";
                worksheet.Cells[row + 6, 2, row + 8, 2].Style.Font.Size = 15;
                worksheet.Cells[row + 6, 2, row + 8, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[row + 6, 2, row + 8, 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightPink);

                worksheet.Cells[row + 9, 2].Value = "Advance Paid";
                worksheet.Cells[row + 9, 2].Style.Font.Size = 15;
                worksheet.Cells[row + 9, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[row + 9, 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Lime);

                worksheet.Cells[row + 11, 2].Value = "Final Amount : ";
                worksheet.Cells[row + 11, 2].Style.Font.Size = 15;
                worksheet.Cells[row + 11, 2].Style.Font.Bold = true;


                // Example formatting for "Total Amount"
                worksheet.Cells[row, 2, row + 11, 2].Style.Font.Bold = true;
                worksheet.Cells[row, 2, row + 11, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[row, 2, row + 11, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[row, 2, row + 11, 2].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                worksheet.Cells[row, 2, row + 11, 2].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                worksheet.Cells[row, 2, row + 11, 2].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                worksheet.Cells[row, 2, row + 11, 2].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                for (int i = 14; i <= 25; i++)
                {
                    worksheet.Row(i).Height = 30;
                }


                // Adding values in column 3 (adjust column index as needed)
                worksheet.Cells[row, 3].Value = totalAccountBalance;
                worksheet.Cells[row + 1, 3].Value = cashBalance;
                worksheet.Cells[row + 2, 3].Value = 0;
                worksheet.Cells[row + 3, 3].Value = 0;
                worksheet.Cells[row + 4, 3].Value = 0;
                worksheet.Cells[row + 5, 3].Value = 0;
                worksheet.Cells[row + 6, 3].Value = 0;
                worksheet.Cells[row + 7, 3].Value = totalUpad;
                worksheet.Cells[row + 8, 3].Value = totalPendingPayment;
                worksheet.Cells[row + 9, 3].Value = advancePayment;
                worksheet.Cells[row + 11, 3].Value = finalAmount;
                worksheet.Cells[row + 11, 3].Style.Font.Bold = true;

                for (int i = row; i <= row + 11; i++)
                {
                    worksheet.Cells[i, 3].Style.Font.Size = 15;
                    worksheet.Cells[i, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[i, 3].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);
                    worksheet.Cells[i, 3].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                    worksheet.Cells[i, 3].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                    worksheet.Cells[i, 3].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                    worksheet.Cells[i, 3].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                    worksheet.Cells[i, 3].Style.Numberformat.Format = "₹ #,##0.00";
                    worksheet.Cells[i, 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;  
                }


                worksheet.Row(10).Height = 30;

                byte[] excelData = package.GetAsByteArray();
                return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Dashboard_{startDateString}_to_{endDateString}.xlsx");
            }
        }
        #endregion

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
