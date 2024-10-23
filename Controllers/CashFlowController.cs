using Microsoft.AspNetCore.Mvc;
using FinanceManagement.Models;

using System;
using FinanceManagement;
using System.Security.Claims;
using FinanceManagement.ViewModels;
using FinanceManagement.IRepository;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

public class CashFlowController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IExpensesRepository expensesRepository;
    private readonly ICashFlowRepository cashFlowRepository;
    private readonly IPaymentRepository paymentRepository;
    private readonly IUpadRepository upadRepository;
    private readonly IOtherRepository otherRepository;

    public CashFlowController(ApplicationDbContext context, IExpensesRepository expensesRepository, ICashFlowRepository cashFlowRepository, IPaymentRepository paymentRepository, IUpadRepository upadRepository, IOtherRepository otherRepository)
    {
        _context = context;
        this.expensesRepository = expensesRepository;
        this.cashFlowRepository = cashFlowRepository;
        this.paymentRepository = paymentRepository;
        this.upadRepository = upadRepository;
        this.otherRepository = otherRepository;
    }


    #region CashFlowIndex
    public IActionResult CashFlowIndex(int? month, int? year, string dateFilter, DateTime? startDate, DateTime? endDate)
    {
        var cashFlows = _context.CashFlows.AsQueryable();

        if (!string.IsNullOrEmpty(dateFilter))
        {
            DateTime filterStartDate;
            DateTime filterEndDate;

            switch (dateFilter)
            {
                case "1": // Yesterday
                    filterStartDate = DateTime.Now.AddDays(-1);
                    filterEndDate = filterStartDate;
                    break;

                case "2": // Last 7 days
                    filterStartDate = DateTime.Now.AddDays(-7);
                    filterEndDate = DateTime.Now;
                    break;

                case "3": // Last 15 days
                    filterStartDate = DateTime.Now.AddDays(-15);
                    filterEndDate = DateTime.Now;
                    break;

                case "4": // Last month
                    filterStartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-1);
                    filterEndDate = filterStartDate.AddMonths(1).AddDays(-1);
                    break;

                case "5": // Custom Date
                    if (startDate.HasValue && endDate.HasValue)
                    {
                        filterStartDate = startDate.Value;
                        filterEndDate = endDate.Value;
                    }
                    else
                    {
                        // Handle the case where custom dates are not provided
                        return View("Error"); // Or any other appropriate error handling
                    }
                    break;

                default:
                    // Handle unexpected or empty dateFilter values
                    return View("Error"); // Or any other appropriate error handling
            }

            cashFlows = cashFlows.Where(cf => cf.Date >= filterStartDate.Date && cf.Date <= filterEndDate.Date);
        }
        else if (month.HasValue && year.HasValue)
        {
            cashFlows = cashFlows.Where(cf => cf.Date.Month == month.Value && cf.Date.Year == year.Value);
        }
        else if (year.HasValue)
        {
            cashFlows = cashFlows.Where(cf => cf.Date.Year == year.Value);
        }
        

        return View(cashFlows.OrderBy(o => o.Date).ToList());
    }
    #endregion


    #region CashFlowCreate
    [HttpGet]
    public IActionResult CashFlowCreate()
    {
        return View();
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CashFlowCreate(CashFlowVM cashFlowViewModel)
    {
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Login", "Account"); // Adjust to your login route
        }

        // Retrieve the user ID
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Retrieve the company name associated with the user ID
        var user = _context.Users.FirstOrDefault(u => u.Id == userId);
        var companyName = user?.CompanyName;

        if (string.IsNullOrEmpty(companyName))
        {
            return View("Error");
        }

        // Conditional validation for PaymentCategory
        if (cashFlowViewModel.Category == "Payments" && string.IsNullOrEmpty(cashFlowViewModel.PaymentCategory))
        {
            ModelState.AddModelError("PaymentCategory", "Payment category is required when the category is Payments.");
        }

        // Exclude PaymentCategory from validation when the category is not "Payments"
        if (cashFlowViewModel.Category != "Payments")
        {
            ModelState.Remove("PaymentCategory");
        }

        if (ModelState.IsValid)
        {          
            // Create a new instance of CashFlow model
            var cashFlow = new CashFlow
            {
                UserId = userId,
                Date = cashFlowViewModel.Date,
                Description = cashFlowViewModel.Description,
                TransactionType = cashFlowViewModel.TransactionType,
                TotalAmount = cashFlowViewModel.Amount,
                Category = cashFlowViewModel.Category,
                Amount = cashFlowViewModel.PaidAmount,
                PaymentCategory = cashFlowViewModel.Category == "Payments" ? cashFlowViewModel.PaymentCategory : null,
                UpadCategory = cashFlowViewModel.UpadCategory
            };

            _context.CashFlows.Add(cashFlow);
            _context.SaveChanges();

            if (cashFlow.Category == "Expenses")
            {
                var expense = new Expenses
                {
                    UserId = userId,
                    CashFlowId = cashFlow.Id,
                    Amount = cashFlow.Amount,
                    Description = cashFlow.Description,
                    Date = cashFlow.Date,
                };

                expensesRepository.Add(expense);
            }
            else if (cashFlow.Category == "Others")
            {
                var others = new Other
                {
                    UserId = userId,
                    CashFlowId = cashFlow.Id,
                    Amount = (decimal)cashFlow.Amount,
                    Description = cashFlow.Description,
                    Date = cashFlow.Date,
                    
                };

                otherRepository.Add(others);
            }
            else if (cashFlow.Category == "Payments")
            {
                var payments = new Payment
                {
                    UserId = userId,
                    CashFlowId = cashFlow.Id,
                    PaidAmount = cashFlow.Amount,
                    TotalAmount = cashFlow.TotalAmount,
                    Description = cashFlow.Description,
                    Date = cashFlow.Date,
                    PaymentCategory = cashFlow.PaymentCategory
                };

                paymentRepository.Add(payments);
            }
            else if (cashFlow.Category == "Upad")
            {
                var upad = new Upad
                {
                    UserId = userId,
                    CashFlowId = cashFlow.Id,
                    Amount = cashFlow.Amount,
                    Description = cashFlow.Description,
                    Date = cashFlow.Date,
                    UpadOption = cashFlow.UpadCategory
                };

                upadRepository.Add(upad);
            }

            // Redirect to CashFlowIndex action
            return RedirectToAction("CashFlowIndex", "CashFlow");
        }

        // If ModelState is not valid, return the view with validation errors
        return View(cashFlowViewModel);
    }


    #endregion


    #region CashFlow Edit
    [HttpGet]
    public IActionResult CashFlowEdit(int id)
    {

        var cashflow = cashFlowRepository.GetById(id);
        return View(cashflow);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CashFlowEdit(int id, CashFlow cashFlow)
    {
        var existingCashFlow = cashFlowRepository.GetById(id);

        if (existingCashFlow == null)
        {
            return NotFound();
        }


        if (existingCashFlow.Category != cashFlow.Category)
        {
            // If the category is changed from "Expenses" to "Upad"
            if (existingCashFlow.Category == "Expenses" && cashFlow.Category == "Upad")
            {
                // Remove the existing expense entry
                var expense = _context.expenses.FirstOrDefault(e => e.CashFlowId == existingCashFlow.Id);
                if (expense != null)
                {
                    _context.expenses.Remove(expense);
                }

                // Add a new upad entry
                var upad = new Upad
                {
                    UserId = existingCashFlow.UserId,
                    CashFlowId = existingCashFlow.Id,
                    Amount = cashFlow.Amount,
                    Description = cashFlow.Description,
                    Date = cashFlow.Date,
                    UpadOption = cashFlow.UpadCategory
                };
                _context.Upads.Add(upad);
            }

            // If the category is changed from "Expenses" to "Payments"
            else if (existingCashFlow.Category == "Expenses" && cashFlow.Category == "Payments")
            {
                // Remove the existing expense entry
                var expense = _context.expenses.FirstOrDefault(e => e.CashFlowId == existingCashFlow.Id);
                if (expense != null)
                {
                    _context.expenses.Remove(expense);
                }

                // Add a new upad entry
                var payment = new Payment
                {
                    UserId = existingCashFlow.UserId,
                    CashFlowId = existingCashFlow.Id,
                    // Set PaidAmount based on the PaymentCategory and existing conditions
                    PaidAmount = cashFlow.PaymentCategory == "Full Payment"
            ? existingCashFlow.TotalAmount  // Set TotalAmount as PaidAmount if PaymentCategory is "Full Payment"
            : existingCashFlow.Amount,       // Use Amount otherwise

                    Description = cashFlow.Description,
                    Date = cashFlow.Date,
                };
                _context.Payments.Add(payment);
            }

            else if (existingCashFlow.Category == "Expenses" && cashFlow.Category == "Others")
            {
                // Remove the existing expense entry
                var expense = _context.expenses.FirstOrDefault(e => e.CashFlowId == existingCashFlow.Id);
                if (expense != null)
                {
                    _context.expenses.Remove(expense);
                }

                // Add a new upad entry
                var other = new Other
                {
                    UserId = existingCashFlow.UserId,
                    CashFlowId = existingCashFlow.Id,
                    Amount = cashFlow.Amount,
                    Description = cashFlow.Description,
                    Date = cashFlow.Date,
                };
                _context.Others.Add(other);
            }

            // If the category is changed from "Upad" to "Payments"
            else if (existingCashFlow.Category == "Upad" && cashFlow.Category == "Payments")
            {
                // Remove the existing expense entry
                var upad = _context.Upads.FirstOrDefault(e => e.CashFlowId == existingCashFlow.Id);
                if (upad != null)
                {
                    _context.Upads.Remove(upad);
                }


                // Add a new upad entry
                var payment = new Payment
                {
                    UserId = existingCashFlow.UserId,
                    CashFlowId = existingCashFlow.Id,
                    PaidAmount = cashFlow.PaymentCategory == "Full Payment"
            ? existingCashFlow.TotalAmount  // Set TotalAmount as PaidAmount if PaymentCategory is "Full Payment"
            : existingCashFlow.Amount,       // Use Amount otherwise
                    Description = cashFlow.Description,
                    Date = cashFlow.Date,
                };
                _context.Payments.Add(payment);
            }

            // If the category is changed from "Upad" to "Expenses"
            else if (existingCashFlow.Category == "Upad" && cashFlow.Category == "Expenses")
            {
                // Remove the existing expense entry
                var upad = _context.Upads.FirstOrDefault(e => e.CashFlowId == existingCashFlow.Id);
                if (upad != null)
                {
                    _context.Upads.Remove(upad);
                }


                // Add a new expense entry
                var expense = new Expenses
                {
                    UserId = existingCashFlow.UserId,
                    CashFlowId = existingCashFlow.Id,
                    Amount = cashFlow.Amount,
                    Description = cashFlow.Description,
                    Date = cashFlow.Date,
                };
                _context.expenses.Add(expense);
            }

            else if (existingCashFlow.Category == "Upad" && cashFlow.Category == "Others")
            {
                // Remove the existing expense entry
                var upad = _context.Upads.FirstOrDefault(e => e.CashFlowId == existingCashFlow.Id);
                if (upad != null)
                {
                    _context.Upads.Remove(upad);
                }


                // Add a new expense entry
                var others = new Other
                {
                    UserId = existingCashFlow.UserId,
                    CashFlowId = existingCashFlow.Id,
                    Amount = cashFlow.Amount,
                    Description = cashFlow.Description,
                    Date = cashFlow.Date,
                };
                _context.Others.Add(others);
            }


            // If the category is changed from "Payments" to "Expenses"
            else if (existingCashFlow.Category == "Payments" && cashFlow.Category == "Expenses")
            {
                // Remove the existing expense entry
                var payment = _context.Payments.FirstOrDefault(e => e.CashFlowId == existingCashFlow.Id);
                if (payment != null)
                {
                    _context.Payments.Remove(payment);
                }

                // Add a new expense entry
                var expense = new Expenses
                {
                    UserId = existingCashFlow.UserId,
                    CashFlowId = existingCashFlow.Id,
                    Amount = cashFlow.Amount,
                    Description = cashFlow.Description,
                    Date = cashFlow.Date,
                };
                _context.expenses.Add(expense);
            }

            // If the category is changed from "Payments" to "Upad"
            else if (existingCashFlow.Category == "Payments" && cashFlow.Category == "Upad")
            {
                // Remove the existing expense entry
                var payment = _context.Payments.FirstOrDefault(e => e.CashFlowId == existingCashFlow.Id);
                if (payment != null)
                {
                    _context.Payments.Remove(payment);
                }


                // Add a new expense entry
                var upad = new Upad
                {
                    UserId = existingCashFlow.UserId,
                    CashFlowId = existingCashFlow.Id,
                    Amount = cashFlow.Amount,
                    Description = cashFlow.Description,
                    Date = cashFlow.Date,
                    UpadOption = cashFlow.UpadCategory
                };
                _context.Upads.Add(upad);
            }

            else if (existingCashFlow.Category == "Payments" && cashFlow.Category == "Others")
            {
                // Remove the existing expense entry
                var payment = _context.Payments.FirstOrDefault(e => e.CashFlowId == existingCashFlow.Id);
                if (payment != null)
                {
                    _context.Payments.Remove(payment);
                }


                // Add a new expense entry
                var others = new Other
                {
                    UserId = existingCashFlow.UserId,
                    CashFlowId = existingCashFlow.Id,
                    Amount = cashFlow.Amount,
                    Description = cashFlow.Description,
                    Date = cashFlow.Date,
                };
                _context.Others.Add(others);
            }


            else if (existingCashFlow.Category == "Others" && cashFlow.Category == "Payments")
            {
                // Remove the existing expense entry
                var other = _context.Others.FirstOrDefault(e => e.CashFlowId == existingCashFlow.Id);
                if (other != null)
                {
                    _context.Others.Remove(other);
                }


                // Add a new expense entry
                var payments = new Payment
                {
                    UserId = existingCashFlow.UserId,
                    CashFlowId = existingCashFlow.Id,
                    PaidAmount = cashFlow.PaymentCategory == "Full Payment"
            ? existingCashFlow.TotalAmount  // Set TotalAmount as PaidAmount if PaymentCategory is "Full Payment"
            : existingCashFlow.Amount,       // Use Amount otherwise
                    Description = cashFlow.Description,
                    Date = cashFlow.Date,
                };
                _context.Payments.Add(payments);
            }

            else if (existingCashFlow.Category == "Others" && cashFlow.Category == "Expenses")
            {
                // Remove the existing expense entry
                var other = _context.Others.FirstOrDefault(e => e.CashFlowId == existingCashFlow.Id);
                if (other != null)
                {
                    _context.Others.Remove(other);
                }


                // Add a new expense entry
                var expense = new Expenses
                {
                    UserId = existingCashFlow.UserId,
                    CashFlowId = existingCashFlow.Id,
                    Amount = cashFlow.Amount,
                    Description = cashFlow.Description,
                    Date = cashFlow.Date,
                };
                _context.expenses.Add(expense);
            }

            else if (existingCashFlow.Category == "Others" && cashFlow.Category == "Others")
            {
                // Remove the existing expense entry
                var other = _context.Others.FirstOrDefault(e => e.CashFlowId == existingCashFlow.Id);
                if (other != null)
                {
                    _context.Others.Remove(other);
                }


                // Add a new expense entry
                var upad = new Upad
                {
                    UserId = existingCashFlow.UserId,
                    CashFlowId = existingCashFlow.Id,
                    Amount = cashFlow.Amount,
                    Description = cashFlow.Description,
                    Date = cashFlow.Date,
                    UpadOption = cashFlow.UpadCategory
                };
                _context.Upads.Add(upad);
            }
        }

        // Update core cash flow details
        existingCashFlow.Date = cashFlow.Date;
        existingCashFlow.Description = cashFlow.Description;
        existingCashFlow.TransactionType = cashFlow.TransactionType;
        existingCashFlow.Amount = cashFlow.Amount;
        existingCashFlow.Category = cashFlow.Category;
        existingCashFlow.TotalAmount = cashFlow.TransactionType switch
        {
            "Credit" => cashFlow.Amount,
            "Debit" => -cashFlow.Amount,
            _ => 0
        };

        // Update corresponding expense amount if necessary
        if (existingCashFlow.Category == "Expenses")
        {
            var expense = _context.expenses.FirstOrDefault(e => e.CashFlowId == existingCashFlow.Id);
            if (expense != null)
            {
                expense.Amount = cashFlow.Amount;
                expense.Description = cashFlow.Description;
                expense.Date = cashFlow.Date;
                _context.expenses.Update(expense);
            }
        }

        else if (existingCashFlow.Category == "Upad")
        {
            var upad = _context.Upads.FirstOrDefault(e => e.CashFlowId == existingCashFlow.Id);
            if (upad != null)
            {
                upad.Amount = cashFlow.Amount;
                upad.Description = cashFlow.Description;
                upad.Date = cashFlow.Date;
                
                _context.Upads.Update(upad);
            }
        }

        else if (existingCashFlow.Category == "Payments")
        {
            var payment = _context.Payments.FirstOrDefault(e => e.CashFlowId == existingCashFlow.Id);
            if (payment != null)
            {
                payment.PaidAmount = cashFlow.Amount;
                payment.Description = cashFlow.Description;
                payment.Date = cashFlow.Date;
                payment.PaymentCategory = existingCashFlow.PaymentCategory;

                _context.Payments.Update(payment);
            }
        }

        cashFlowRepository.Update(existingCashFlow);
        _context.SaveChanges();

        return RedirectToAction("CashFlowIndex", "CashFlow");
    }






    #endregion


    #region CashFlow Delete
    [HttpGet]
    public IActionResult CashFlowDelete(int id)
    {
        // Ensure that the product exists
        var cashflow = cashFlowRepository.GetById(id);
        if (cashflow == null)
        {
            return NotFound();
        }

        return View(cashflow); // Assuming you have a view to confirm the deletion
    }



    [HttpPost]

    public IActionResult ConfirmCashFlowDelete(int id)
    {
        // Ensure that the product exists
        var cashflow = cashFlowRepository.GetById(id);
        if (cashflow == null)
        {
            return NotFound();
        }

        // Delete the product
        cashFlowRepository.Delete(id);

        return RedirectToAction("CashFlowIndex");
    }
    #endregion


    #region CashFlow Excel
    public IActionResult ExportToExcel(DateTime? startDate, DateTime? endDate)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get current user's ID
        var user = _context.Users.FirstOrDefault(u => u.Id == userId);
        var companyName = user != null ? user.CompanyName : null;

        IEnumerable<CashFlow> cashflows = Enumerable.Empty<CashFlow>();

        if (!string.IsNullOrEmpty(companyName))
        {
            cashflows = cashFlowRepository.GetCashFlowFromCompanyName(companyName);

            if (startDate.HasValue && endDate.HasValue)
            {
                cashflows = cashflows.Where(p => p.Date >= startDate && p.Date <= endDate).ToList();
            }
            cashflows = cashflows.OrderBy(o => o.Date).ToList();
        }

        string startDateString = startDate?.ToString("dd-MM-yyyy") ?? "StartDate";
        string endDateString = endDate?.ToString("dd-MM-yyyy") ?? "EndDate";

        string fileName = $"Orders{startDateString}_to_{endDateString}.xlsx";

        // Create an instance of the ExcelPackage
        using (var package = new ExcelPackage())
        {
            // Create a worksheet
            var worksheet = package.Workbook.Worksheets.Add("CashFlow");

            // Set the title
            worksheet.Cells["A1"].Value = $"CashFlow Report ({startDateString} to {endDateString})"; // Title text
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


            worksheet.Cells["A3:E3"].Style.Font.Bold = true;
            worksheet.Cells["A3:E3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["A3:E3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells["A3:E3"].Style.Font.Size = 16; // Increase the font size
                                                           // Apply background color to the entire row
            worksheet.Cells["A3:E3"].Style.Fill.PatternType = ExcelFillStyle.Solid; // Solid fill
            worksheet.Cells["A3:E3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
            worksheet.Cells["A3:E3"].Style.Border.Top.Style = ExcelBorderStyle.Thin; // Top border
            worksheet.Cells["A3:E3"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin; // Bottom border
            worksheet.Cells["A3:E3"].Style.Border.Left.Style = ExcelBorderStyle.Thin; // Left border
            worksheet.Cells["A3:E3"].Style.Border.Right.Style = ExcelBorderStyle.Thin; // Right border



            worksheet.Cells["H3:J3"].Style.Font.Bold = true;
            
            worksheet.Cells["H3:J3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells["H3:J3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["H3:J3"].Style.Font.Size = 17; // Increase the font size
                                                           // Apply background color to the entire row
            worksheet.Cells["H3:J3"].Style.Fill.PatternType = ExcelFillStyle.Solid; // Solid fill
            worksheet.Cells["H3:J3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
            worksheet.Cells["H3:J3"].Style.Border.Top.Style = ExcelBorderStyle.Thin; // Top border
            worksheet.Cells["H3:J3"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin; // Bottom border
            worksheet.Cells["H3:J3"].Style.Border.Left.Style = ExcelBorderStyle.Thin; // Left border
            worksheet.Cells["H3:J3"].Style.Border.Right.Style = ExcelBorderStyle.Thin; // Right border


            worksheet.Cells["A3"].Value = "Date";
            worksheet.Cells["B3"].Value = "Description";
            worksheet.Cells["C3"].Value = "Credit";
            worksheet.Cells["D3"].Value = "Debit";
            worksheet.Cells["E3"].Value = "Cash Balance";
            worksheet.Cells["H3"].Value = "Total Credit Amount";
            worksheet.Cells["I3"].Value = "Total Debit Amount";
            worksheet.Cells["J3"].Value = "Total Amount";


            worksheet.Column(1).Width = 17; 
            worksheet.Column(2).Width = 30; 
            worksheet.Column(3).Width = 20; 
            worksheet.Column(4).Width = 20; 
            worksheet.Column(5).Width = 22;
            worksheet.Column(8).Width = 30;
            worksheet.Column(9).Width = 30;
            worksheet.Column(10).Width = 30;


            worksheet.Row(2).Height = 45;
            worksheet.Row(3).Height = 45;




            // Add data to the worksheet
            int row = 4;
            decimal accountBalance = 0;
            decimal totalCreditAmount = 0;
            decimal totalDebitAmount = 0;


            foreach (var cashflow in cashflows)
            {

                worksheet.Cells[$"A{row}:D{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"A{row}:E{row}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Row(row).Height = 28;
                worksheet.Cells[$"A{row}"].Value = cashflow.Date.ToString("dd-MM-yyyy");
                worksheet.Cells[$"A{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Center-align Price
                worksheet.Cells[$"B{row}"].Value = cashflow.Description;
                worksheet.Cells[$"B{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Center-align Date
                worksheet.Cells[$"C{row}"].Value = cashflow.Amount;
                worksheet.Cells[$"C{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Center-align ProductName
                worksheet.Cells[$"D{row}"].Value = cashflow.Category;
                worksheet.Cells[$"D{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Center-align SKU
                worksheet.Cells[$"H{row}:J{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                if (cashflow.TransactionType == "Credit")
                {
                    worksheet.Cells[$"C{row}"].Value = cashflow.Amount;
                    worksheet.Cells[$"D{row}"].Value = null;
                    worksheet.Cells[$"C{row}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[$"C{row}"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGreen);
                    worksheet.Cells[$"C{row}"].Style.Numberformat.Format = "₹ #,##0.00";
                    accountBalance += cashflow.Amount;
                    totalCreditAmount += cashflow.Amount;
                }
                else if (cashflow.TransactionType == "Debit")
                {
                    worksheet.Cells[$"C{row}"].Value = null;
                    worksheet.Cells[$"D{row}"].Value = cashflow.Amount;
                    worksheet.Cells[$"D{row}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[$"D{row}"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightPink);
                    worksheet.Cells[$"D{row}"].Style.Numberformat.Format = "₹ #,##0.00";
                    accountBalance -= cashflow.Amount;
                    totalDebitAmount += cashflow.Amount;
                }

                worksheet.Cells[$"E{row}"].Value = accountBalance;
                worksheet.Cells[$"E{row}"].Style.Numberformat.Format = "₹ #,##0.00";
                worksheet.Cells[$"E{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[$"H4"].Value = totalCreditAmount;
                worksheet.Cells[$"I4"].Value = totalDebitAmount;
                worksheet.Cells[$"J4"].Value = totalCreditAmount - totalDebitAmount;




                worksheet.Cells[$"B{row}"].Style.Font.Bold = true;
                worksheet.Cells[$"H{row}:J{row}"].Style.Font.Bold = true;
                worksheet.Cells[$"H{row}:J{row}"].Style.Font.Size = 22;

                worksheet.Cells[$"A{row}:F{row}"].Style.Font.Size = 14;
                
                
                worksheet.Cells[$"B{row}"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                row++;
            }

            // Fill background color for total amount cells
            worksheet.Cells["H4:J4"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells["H4:J4"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.AntiqueWhite);

            worksheet.Cells["H4"].Value = $"₹{totalCreditAmount}";
            worksheet.Cells["I4"].Value = $"₹{totalDebitAmount}";
            worksheet.Cells["J4"].Value = $"₹{totalCreditAmount - totalDebitAmount}";

            // Return the Excel file as a downloadable file
            byte[] excelData = package.GetAsByteArray();
            return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"CashFlow {startDateString}_to_{endDateString}.xlsx");
        }
    }
    #endregion

 
}
