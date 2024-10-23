using FinanceManagement.IRepository;
using FinanceManagement.Models;
using FinanceManagement.ViewModels;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Diagnostics;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.ComponentModel.Design;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FinanceManagement.Controllers
{
    public class AccountMasterController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IExpensesRepository expensesRepository;
        private readonly ICashFlowRepository cashFlowRepository;
        private readonly IPaymentRepository paymentRepository;
        private readonly IUpadRepository upadRepository;
        private readonly ICompanyRepository companyRepository;
        private readonly IAccountMasterRepository accountMasterRepository;
        private readonly IOtherRepository otherRepository;

        public AccountMasterController(ApplicationDbContext context, IExpensesRepository expensesRepository, ICashFlowRepository cashFlowRepository, IPaymentRepository paymentRepository, IUpadRepository upadRepository, ICompanyRepository companyRepository, IAccountMasterRepository accountMasterRepository, IOtherRepository otherRepository)
        {
            _context = context;
            this.expensesRepository = expensesRepository;
            this.cashFlowRepository = cashFlowRepository;
            this.paymentRepository = paymentRepository;
            this.upadRepository = upadRepository;
            this.companyRepository = companyRepository;
            this.accountMasterRepository = accountMasterRepository;
            this.otherRepository = otherRepository;
        }

        #region AccountMaster Index
        public IActionResult AccountMasterIndex(int? selectedCompanyId, int? month, int? year, string dateFilter, DateTime? startDate, DateTime? endDate)
        {
            var accountMasterQuery = _context.AccountMaster.AsQueryable();

            if (selectedCompanyId.HasValue)
            {
                accountMasterQuery = accountMasterQuery.Where(am => am.CompanyId == selectedCompanyId.Value);
            }

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

                accountMasterQuery = accountMasterQuery.Where(am => am.Date >= filterStartDate.Date && am.Date <= filterEndDate.Date);
            }

            if (month.HasValue && year.HasValue)
            {
                accountMasterQuery = accountMasterQuery.Where(am => am.Date.Month == month.Value && am.Date.Year == year.Value);
            }

            // Sort by Date in descending order to show latest entries first
            var accountMaster = accountMasterQuery
    .OrderByDescending(am => am.Id) // or use CreatedDate if available
    .ToList();

            ViewBag.Companies = _context.Companies.ToList();

            return View(accountMaster);
        }
        #endregion


        #region AccountMaster Create
        [HttpGet]
        public IActionResult AccountMasterCreate(DateTime? date, int? companyId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            // Retrieve the user ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Retrieve the user and associated company name
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            var companyName = user?.CompanyName;

            if (string.IsNullOrEmpty(companyName))
            {
                return View("Error");
            }

            // Retrieve the list of companies
            var companies = companyRepository.GetCompanyFromCompanyName(companyName)?.ToList() ?? new List<Company>();

            if (!companies.Any())
            {
                return View("Error");
            }

            // Initialize the view model
            var model = new AccountMasterVM
            {
                Companies = companies,
                Date = date ?? DateTime.Now, // Use the passed date or the current date
                SelectedCompanyId = companyId.HasValue ? companyId.Value : companies.FirstOrDefault()?.Id ?? 0 // Convert companyId to int and handle null case
            };



            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AccountMasterCreate(AccountMasterVM accountMasterVM)
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
            var companys = companyRepository.GetById(accountMasterVM.SelectedCompanyId);

            if (string.IsNullOrEmpty(companyName))
            {
                return View("Error");
            }

            // Extract selected description or custom description
            string description = accountMasterVM.DescriptionSelect == "Custom" && !string.IsNullOrEmpty(accountMasterVM.CustomDescription)
                ? accountMasterVM.CustomDescription
                : accountMasterVM.DescriptionSelect;

            // Validate that either a description is selected or a custom description is provided
            if (accountMasterVM.DescriptionSelect == "Custom" && string.IsNullOrEmpty(accountMasterVM.CustomDescription))
            {
                ModelState.AddModelError(nameof(accountMasterVM.CustomDescription), "Custom description is required when 'Custom' is selected.");
            }
            else if (string.IsNullOrEmpty(description))
            {
                ModelState.AddModelError(nameof(accountMasterVM.DescriptionSelect), "Description is required.");
            }

            if (ModelState.IsValid)
            {
                var company = companyRepository.GetById(accountMasterVM.SelectedCompanyId);
                // Create a new instance of CashFlow model
                var accountmaster = new AccountMaster
                {
                    UserId = userId,
                    Date = accountMasterVM.Date,
                    Description = description,
                    TransactionType = accountMasterVM.TransactionType,
                    CompanyId = accountMasterVM.SelectedCompanyId,
                    FirmName = company.FirmName,
                    TotalAmount = accountMasterVM.TotalAmount,
                    Category = accountMasterVM.Category,
                    PaidAmount = accountMasterVM.TransactionType == "Credit" ||
                        (accountMasterVM.Category == "Payments" && accountMasterVM.PaymentCategory == "Full Payment") ||
                        accountMasterVM.Category != "Payments"
                        ? accountMasterVM.TotalAmount
                        : accountMasterVM.PaidAmount,
                    UpadCategory = accountMasterVM.UpadCategory,
                    PaymentCategory = accountMasterVM.Category == "Payments" ? accountMasterVM.PaymentCategory : null,
                };

                _context.AccountMaster.Add(accountmaster);
                _context.SaveChanges();

                if (accountmaster.Category == "Expenses")
                {
                    var expense = new Expenses
                    {
                        UserId = userId,
                        AccountMasterId = accountmaster.Id,
                        Amount = (decimal)accountmaster.TotalAmount,
                        Description = accountmaster.Description,
                        Date = accountmaster.Date,
                        FirmName = company.FirmName,
                    };

                    expensesRepository.Add(expense);
                }

                else if (accountmaster.Category == "Others")
                {
                    var others = new Other
                    {
                        UserId = userId,
                        AccountMasterId = accountmaster.Id,
                        Amount = (decimal)accountmaster.TotalAmount,
                        Description = accountmaster.Description,
                        Date = accountmaster.Date,
                        FirmName = company.FirmName,
                    };

                    otherRepository.Add(others);
                }

                else if (accountmaster.Category == "Payments")
                {
                    if (accountmaster.TransactionType == "Debit")
                    {
                        var payments = new Payment
                        {
                            UserId = userId,
                            AccountMasterId = accountmaster.Id,
                            PaidAmount = accountmaster.PaidAmount,
                            TotalAmount = (decimal)accountmaster.TotalAmount,
                            Description = accountmaster.Description,
                            Date = accountmaster.Date,
                            FirmName = company.FirmName,
                            PaymentCategory = accountmaster.PaymentCategory
                        };

                        paymentRepository.Add(payments);
                    }
                }

                else if (accountmaster.Category == "Upad")
                {
                    var upad = new Upad
                    {
                        UserId = userId,
                        AccountMasterId = accountmaster.Id,
                        Amount = (decimal)accountmaster.TotalAmount,
                        Description = accountmaster.Description,
                        Date = accountmaster.Date,
                        UpadOption = accountmaster.UpadCategory,
                        FirmName = accountmaster.FirmName,
                        
                    };

                    upadRepository.Add(upad);
                }

                else if (accountmaster.Category == "Cash")
                {
                    var cashFlow = new CashFlow
                    {
                        UserId = userId,
                        Amount = (decimal)accountmaster.TotalAmount,
                        AccountMasterId= accountmaster.Id,
                        Description = accountmaster.Description,
                        Date = accountmaster.Date,
                        Category = accountmaster.Category,
                        TransactionType = "Credit"
                    };

                    cashFlowRepository.Add(cashFlow);
                }


                // Automatic entry for the next month, if applicable
                var now = DateTime.Now;
                var lastMonth = now.AddMonths(-1);
                var lastMonthBalance = accountMasterRepository.GetTotalAccountBalance(userId, companyName, lastMonth.Month);

                if (now.Day == 1 && lastMonthBalance > 0)
                {
                    var newEntry = new AccountMaster
                    {
                        UserId = userId,
                        Date = new DateTime(now.Year, now.Month, 1), // 1st of the current month
                        FirmName = companyName,
                        TransactionType = "Credit",
                        PaidAmount = lastMonthBalance,
                        Description = "Previous month Balance",
                        Category = "Payments",
                        
                    };

                    _context.AccountMaster.Add(newEntry);
                    _context.SaveChanges();
                }

                // Redirect back to AccountMasterCreate and retain the selected date and company
                return RedirectToAction("AccountMasterCreate", new { date = accountMasterVM.Date, companyId = accountMasterVM.SelectedCompanyId });
            }

            // If ModelState is not valid, return the view with validation errors
            return View(accountMasterVM);
        }


        #endregion


        #region AccountMaster Edit
        [HttpGet]
        public IActionResult AccountMasterEdit(int id)
        {
            // Retrieve the user ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Retrieve the user and associated company name
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            var companyName = user?.CompanyName;

            if (string.IsNullOrEmpty(companyName))
            {
                return View("Error");
            }

            // Retrieve the account master entry by ID
            var accountmaster = accountMasterRepository.GetById(id);

            if (accountmaster == null)
            {
                return View("Error");
            }

            // Retrieve the list of companies
            var companies = companyRepository.GetCompanyFromCompanyName(companyName)?.ToList();

            if (companies == null || !companies.Any())
            {
                return View("Error");  // If no companies are found, return an error view
            }

            // Create a ViewModel and populate it with account master details and the company list
            var viewModel = new AccountMasterVM
            {
                Id = accountmaster.Id,
                
                Date = accountmaster.Date,
                Description = accountmaster.Description,
                TransactionType = accountmaster.TransactionType,
                TotalAmount = accountmaster.TotalAmount ?? 0m, // Handle nullable decimal
                PaidAmount = accountmaster.PaidAmount,
                PaymentCategory = accountmaster.PaymentCategory,
                UpadCategory = accountmaster.UpadCategory,
                Category = accountmaster.Category,
                SelectedCompanyId = accountmaster.CompanyId,  // Assuming CompanyId exists
                
                Companies = companies  // Pass companies to the view
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AccountMasterEdit(int id, AccountMasterVM accountMaster)
        {
            var company = companyRepository.GetById(accountMaster.SelectedCompanyId);
            var existingAccountMaster = accountMasterRepository.GetById(id);

            if (existingAccountMaster == null)
            {
                return NotFound();
            }


            if (existingAccountMaster.Category != accountMaster.Category)
            {
                // If the category is changed from "Expenses" to "Upad"
                if (existingAccountMaster.Category == "Expenses" && accountMaster.Category == "Upad")
                {
                    // Remove the existing expense entry
                    var expense = _context.expenses.FirstOrDefault(e => e.AccountMasterId == existingAccountMaster.Id);
                    if (expense != null)
                    {
                        _context.expenses.Remove(expense);
                    }

                    // Add a new upad entry
                    var upad = new Upad
                    {
                        UserId = existingAccountMaster.UserId,
                        AccountMasterId = existingAccountMaster.Id,
                        Date = accountMaster.Date,
                        Description = accountMaster.Description,
                        Amount = (decimal)accountMaster.TotalAmount,
                        UpadOption = accountMaster.UpadCategory,
                        FirmName = existingAccountMaster.FirmName
                        
                    };
                    _context.Upads.Add(upad);
                }

                // If the category is changed from "Expenses" to "Payments"
                else if (existingAccountMaster.Category == "Expenses" && accountMaster.Category == "Payments")
                {
                    // Remove the existing expense entry
                    var expense = _context.expenses.FirstOrDefault(e => e.AccountMasterId == existingAccountMaster.Id);
                    if (expense != null)
                    {
                        _context.expenses.Remove(expense);
                    }

                    // Add a new upad entry
                    var payment = new Payment
                    {
                        UserId = existingAccountMaster.UserId,
                        AccountMasterId = existingAccountMaster.Id,
                        TotalAmount = (decimal)accountMaster.TotalAmount,
                        PaidAmount = accountMaster.TransactionType == "Credit" ||
                        (accountMaster.Category == "Payments" && accountMaster.PaymentCategory == "Full Payment") ||
                        accountMaster.Category != "Payments"
                        ? accountMaster.TotalAmount
                        : accountMaster.PaidAmount,
                        Description = accountMaster.Description,
                        Date = accountMaster.Date,
                        FirmName = existingAccountMaster.FirmName,
                        PaymentCategory = accountMaster.Category == "Payments" ? accountMaster.PaymentCategory : null,

                    };
                    _context.Payments.Add(payment);
                }

                else if (existingAccountMaster.Category == "Expenses" && accountMaster.Category == "Others")
                {
                    // Remove the existing expense entry
                    var expense = _context.expenses.FirstOrDefault(e => e.AccountMasterId == existingAccountMaster.Id);
                    if (expense != null)
                    {
                        _context.expenses.Remove(expense);
                    }

                    // Add a new upad entry
                    var other = new Other
                    {
                        UserId = existingAccountMaster.UserId,
                        AccountMasterId = existingAccountMaster.Id,
                        Date = accountMaster.Date,
                        Description = accountMaster.Description,
                        Amount = (decimal)accountMaster.TotalAmount,
                        
                        FirmName = existingAccountMaster.FirmName

                    };
                    _context.Others.Add(other);
                }



                // If the category is changed from "Upad" to "Payments"
                else if (existingAccountMaster.Category == "Upad" && accountMaster.Category == "Payments")
                {
                    // Remove the existing expense entry
                    var upad = _context.Upads.FirstOrDefault(e => e.AccountMasterId == existingAccountMaster.Id);
                    if (upad != null)
                    {
                        _context.Upads.Remove(upad);
                    }


                    // Add a new upad entry
                    var payment = new Payment
                    {
                        UserId = existingAccountMaster.UserId,
                        AccountMasterId = existingAccountMaster.Id,
                        TotalAmount = (decimal)accountMaster.TotalAmount,
                        PaidAmount = accountMaster.TransactionType == "Credit" ||
                        (accountMaster.Category == "Payments" && accountMaster.PaymentCategory == "Full Payment") ||
                        accountMaster.Category != "Payments"
                        ? accountMaster.TotalAmount
                        : accountMaster.PaidAmount,
                        Description = accountMaster.Description,
                        Date = accountMaster.Date,
                        FirmName = existingAccountMaster.FirmName,
                        PaymentCategory = accountMaster.Category == "Payments" ? accountMaster.PaymentCategory : null,

                    };
                    _context.Payments.Add(payment);
                }

                // If the category is changed from "Upad" to "Expenses"
                else if (existingAccountMaster.Category == "Upad" && accountMaster.Category == "Expenses")
                {
                    // Remove the existing expense entry
                    var upad = _context.Upads.FirstOrDefault(e => e.AccountMasterId == existingAccountMaster.Id);
                    if (upad != null)
                    {
                        _context.Upads.Remove(upad);
                    }


                    // Add a new expense entry
                    var expense = new Expenses
                    {
                        UserId = existingAccountMaster.UserId,
                        AccountMasterId = existingAccountMaster.Id,
                        Amount = (decimal)accountMaster.TotalAmount,
                        Description = accountMaster.Description,
                        Date = accountMaster.Date,
                        FirmName = existingAccountMaster.FirmName
                    };
                    _context.expenses.Add(expense);
                }

                else if (existingAccountMaster.Category == "Upad" && accountMaster.Category == "Others")
                {
                    // Remove the existing expense entry
                    var upad = _context.Upads.FirstOrDefault(e => e.AccountMasterId == existingAccountMaster.Id);
                    if (upad != null)
                    {
                        _context.Upads.Remove(upad);
                    }


                    // Add a new expense entry
                    var other = new Other
                    {
                        UserId = existingAccountMaster.UserId,
                        AccountMasterId = existingAccountMaster.Id,
                        Date = accountMaster.Date,
                        Description = accountMaster.Description,
                        Amount = (decimal)accountMaster.TotalAmount,
                        FirmName = existingAccountMaster.FirmName

                    };
                    _context.Others.Add(other);
                }




                // If the category is changed from "Payments" to "Expenses"
                else if (existingAccountMaster.Category == "Payments" && accountMaster.Category == "Expenses")
                {
                    // Remove the existing expense entry
                    var payment = _context.Payments.FirstOrDefault(e => e.AccountMasterId == existingAccountMaster.Id);
                    if (payment != null)
                    {
                        _context.Payments.Remove(payment);
                    }

                    // Add a new expense entry
                    var expense = new Expenses
                    {
                        UserId = existingAccountMaster.UserId,
                        AccountMasterId = existingAccountMaster.Id,
                        Amount = (decimal)accountMaster.TotalAmount,
                        Description = accountMaster.Description,
                        Date = accountMaster.Date,
                        FirmName = existingAccountMaster.FirmName
                    };
                    _context.expenses.Add(expense);
                }

                // If the category is changed from "Payments" to "Upad"
                else if (existingAccountMaster.Category == "Payments" && accountMaster.Category == "Upad")
                {
                    // Remove the existing expense entry
                    var payment = _context.Payments.FirstOrDefault(e => e.AccountMasterId == existingAccountMaster.Id);
                    if (payment != null)
                    {
                        _context.Payments.Remove(payment);
                    }


                    // Add a new expense entry
                    var upad = new Upad
                    {
                        UserId = existingAccountMaster.UserId,
                        AccountMasterId = existingAccountMaster.Id,
                        Amount = (decimal)accountMaster.TotalAmount,
                        Description = accountMaster.Description,
                        Date = accountMaster.Date,
                        UpadOption = accountMaster.UpadCategory,
                        FirmName = existingAccountMaster.FirmName,

                    };
                    _context.Upads.Add(upad);
                }

                else if (existingAccountMaster.Category == "Payments" && accountMaster.Category == "Others")
                {
                    // Remove the existing expense entry
                    var payment = _context.Payments.FirstOrDefault(e => e.AccountMasterId == existingAccountMaster.Id);
                    if (payment != null)
                    {
                        _context.Payments.Remove(payment);
                    }


                    // Add a new expense entry
                    var other = new Other
                    {
                        UserId = existingAccountMaster.UserId,
                        AccountMasterId = existingAccountMaster.Id,
                        Date = accountMaster.Date,
                        Description = accountMaster.Description,
                        Amount = (decimal)accountMaster.TotalAmount,
                        FirmName = existingAccountMaster.FirmName

                    };
                    _context.Others.Add(other);
                }



                else if (existingAccountMaster.Category == "Others" && accountMaster.Category == "Payments")
                {
                    // Remove the existing expense entry
                    var other = _context.Others.FirstOrDefault(e => e.AccountMasterId == existingAccountMaster.Id);
                    if (other != null)
                    {
                        _context.Others.Remove(other);
                    }


                    // Add a new upad entry
                    var payment = new Payment
                    {
                        UserId = existingAccountMaster.UserId,
                        AccountMasterId = existingAccountMaster.Id,
                        TotalAmount = (decimal)accountMaster.TotalAmount,
                        PaidAmount = accountMaster.TransactionType == "Credit" ||
                        (accountMaster.Category == "Payments" && accountMaster.PaymentCategory == "Full Payment") ||
                        accountMaster.Category != "Payments"
                        ? accountMaster.TotalAmount
                        : accountMaster.PaidAmount,
                        Description = accountMaster.Description,
                        Date = accountMaster.Date,
                        FirmName = existingAccountMaster.FirmName,
                        PaymentCategory = accountMaster.Category == "Payments" ? accountMaster.PaymentCategory : null,

                    };
                    _context.Payments.Add(payment);
                }

                else if (existingAccountMaster.Category == "Others" && accountMaster.Category == "Upad")
                {
                    // Remove the existing expense entry
                    var other = _context.Others.FirstOrDefault(e => e.AccountMasterId == existingAccountMaster.Id);
                    if (other != null)
                    {
                        _context.Others.Remove(other);
                    }


                    // Add a new expense entry
                    var upad = new Upad
                    {
                        UserId = existingAccountMaster.UserId,
                        AccountMasterId = existingAccountMaster.Id,
                        Amount = (decimal)accountMaster.TotalAmount,
                        Description = accountMaster.Description,
                        Date = accountMaster.Date,
                        UpadOption = accountMaster.UpadCategory,
                        FirmName = existingAccountMaster.FirmName,

                    };
                    _context.Upads.Add(upad);
                }

                else if (existingAccountMaster.Category == "Others" && accountMaster.Category == "Expenses")
                {
                    // Remove the existing expense entry
                    var other = _context.Others.FirstOrDefault(e => e.AccountMasterId == existingAccountMaster.Id);
                    if (other != null)
                    {
                        _context.Others.Remove(other);
                    }

                    // Add a new expense entry
                    var expense = new Expenses
                    {
                        UserId = existingAccountMaster.UserId,
                        AccountMasterId = existingAccountMaster.Id,
                        Amount = (decimal)accountMaster.TotalAmount,
                        Description = accountMaster.Description,
                        Date = accountMaster.Date,
                        FirmName = existingAccountMaster.FirmName
                    };
                    _context.expenses.Add(expense);
                }


            }

            // Update core cash flow details
            existingAccountMaster.Date = accountMaster.Date;
            existingAccountMaster.Description = accountMaster.Description;
            existingAccountMaster.TransactionType = accountMaster.TransactionType;
            existingAccountMaster.TotalAmount = accountMaster.TotalAmount;
            
            existingAccountMaster.PaymentCategory = accountMaster.PaymentCategory;
            // Add logic for FullPayment

            if (existingAccountMaster.PaymentCategory == "Payments") // Check if category is Payments
            {
                if (existingAccountMaster.PaymentCategory == "Full Payment")
                {
                    existingAccountMaster.PaidAmount = accountMaster.TotalAmount;
                }
                else
                {
                    existingAccountMaster.PaidAmount = accountMaster.PaidAmount; // Keep original PaidAmount logic
                }
            }
            
                existingAccountMaster.PaidAmount = accountMaster.TotalAmount;
            


            existingAccountMaster.Category = accountMaster.Category;
            existingAccountMaster.CompanyId = accountMaster.SelectedCompanyId;
            existingAccountMaster.FirmName = company.FirmName;
            existingAccountMaster.TotalAmount = accountMaster.TransactionType switch
            {
                "Credit" => accountMaster.TotalAmount,
                "Debit" => accountMaster.TotalAmount,
                _ => 0
            };

            // Update corresponding expense amount if necessary
            if (existingAccountMaster.Category == "Expenses")
            {
                var expense = _context.expenses.FirstOrDefault(e => e.AccountMasterId == existingAccountMaster.Id);
                if (expense != null)
                {
                    expense.Amount = (decimal)accountMaster.TotalAmount;
                    expense.Description = accountMaster.Description;
                    expense.Date = accountMaster.Date;
                    expense.FirmName = existingAccountMaster.FirmName;
                    _context.expenses.Update(expense);
                }
            }

            else if (existingAccountMaster.Category == "Payments")
            {
                var payment = _context.Payments.FirstOrDefault(e => e.AccountMasterId == existingAccountMaster.Id);
                if (payment != null)
                {

                    // Update other fields
                    payment.Description = accountMaster.Description;
                    payment.Date = accountMaster.Date;
                    payment.PaymentCategory = existingAccountMaster.PaymentCategory;
                    payment.TotalAmount = accountMaster.TotalAmount;
                    if (existingAccountMaster.PaymentCategory == "Full Payment")
                    {
                        payment.PaidAmount = accountMaster.TotalAmount;
                    }
                    else
                    {
                        existingAccountMaster.PaidAmount = accountMaster.PaidAmount; // Keep original PaidAmount logic
                    }
                    _context.Payments.Update(payment);
                }

            }


            else if (existingAccountMaster.Category == "Upad")
            {
                var upad = _context.Upads.FirstOrDefault(e => e.AccountMasterId == existingAccountMaster.Id);
                if (upad != null)
                {
                    upad.Amount = (decimal)accountMaster.TotalAmount;
                    upad.Description = accountMaster.Description;
                    upad.Date = accountMaster.Date;
                    _context.Upads.Update(upad);
                }
            }

            accountMasterRepository.Update(existingAccountMaster);
            _context.SaveChanges();

            return RedirectToAction("AccountMasterIndex", "AccountMaster");
        }






        #endregion



        #region AccountMaster Delete
        [HttpGet]
        public IActionResult AccountMasterDelete(int id)
        {
            var accountMaster = accountMasterRepository.GetById(id);
            if (accountMaster == null)
            {
                return NotFound();
            }

            return View(accountMaster); 
        }



        [HttpPost]

        public IActionResult ConfirmAccountMasterDelete(int id)
        {
            
            var accountMaster = accountMasterRepository.GetById(id);
            if (accountMaster == null)
            {
                return NotFound();
            }

            if (accountMaster.Category == "Cash")
            {
                var cashFlow = _context.CashFlows.FirstOrDefault(cf => cf.AccountMasterId == id);
                if (cashFlow != null)
                {
                    _context.CashFlows.Remove(cashFlow);
                }
            }

            // Delete the product
            accountMasterRepository.Delete(id);

            return RedirectToAction("AccountMasterIndex");
        }
        #endregion


        #region AccountMaster Excel
        public IActionResult ExportToExcel(DateTime? startDate, DateTime? endDate, int? selectedCompanyId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get current user's ID
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            var companyName = user?.CompanyName;

            // Get all companies associated with the user
            var companies = _context.Companies.Where(c => c.UserId == userId).ToList();

            using (var package = new ExcelPackage())
            {
                foreach (var company in companies)
                {
                    var firmName = company.FirmName;
                    var companyId = company.Id;

                    var accountMasters = _context.AccountMaster.Where(am => am.CompanyId == companyId);

                    if (startDate.HasValue && endDate.HasValue)
                    {
                        accountMasters = accountMasters.Where(p => p.Date >= startDate && p.Date <= endDate);
                    }

                    accountMasters = accountMasters.OrderBy(o => o.Date);

                    DateTime previousMonthStart = new DateTime(startDate.Value.Year, startDate.Value.Month, 1).AddMonths(-1); // 1st day of the previous month

                    // Calculate the end of the previous month
                    DateTime previousMonthEnd = new DateTime(startDate.Value.Year, startDate.Value.Month, 1).AddDays(-1); // Last day of the previous month


                    // Query for transactions in the previous month
                    var previousMonthTransactions = _context.AccountMaster
                        .Where(am => am.CompanyId == companyId && am.Date >= previousMonthStart && am.Date <= previousMonthEnd)
                        .OrderBy(am => am.Date)
                        .ToList(); // Ensure the result is executed and not deferred



                    decimal previousMonthBalance = 0;
                    foreach (var transaction in previousMonthTransactions)
                    {
                        if (transaction.TransactionType == "Credit")
                        {
                            previousMonthBalance += transaction.PaidAmount;
                        }
                        else if (transaction.TransactionType == "Debit")
                        {
                            previousMonthBalance -= transaction.PaidAmount;
                        }
                    }



                    // Create a worksheet
                    var worksheet = package.Workbook.Worksheets.Add(firmName ?? "AccountMaster");

                    // Set the title
                    string companySheetTitle = string.IsNullOrEmpty(firmName) ? "Account Report" : $"{firmName} Account Report";
                    string startDateString = startDate?.ToString("dd-MM-yyyy") ?? "StartDate";
                    string endDateString = endDate?.ToString("dd-MM-yyyy") ?? "EndDate";

                    worksheet.Cells["A1"].Value = $"{companySheetTitle} ({startDateString} to {endDateString})";
                    worksheet.Cells["A1:J1"].Merge = true;
                    worksheet.Cells["A1:J1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["A1:J1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells["A1:J1"].Style.Font.Size = 25;
                    worksheet.Cells["A1:J1"].Style.Font.Bold = true;

                    worksheet.Cells["A1:J1"].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                    worksheet.Cells["A1:J1"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                    worksheet.Cells["A1:J1"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                    worksheet.Cells["A1:J1"].Style.Border.Right.Style = ExcelBorderStyle.Medium;

                    worksheet.Cells["A3:E3"].Style.Font.Bold = true;
                    worksheet.Cells["A3:E3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["A3:E3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells["A3:E3"].Style.Font.Size = 16;
                    worksheet.Cells["A3:E3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells["A3:E3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                    worksheet.Cells["A3:E3"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["A3:E3"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["A3:E3"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["A3:E3"].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    worksheet.Cells["H3:J3"].Style.Font.Bold = true;
                    worksheet.Cells["H3:J3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells["H3:J3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["H3:J3"].Style.Font.Size = 17;
                    worksheet.Cells["H3:J3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells["H3:J3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                    worksheet.Cells["H3:J3"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["H3:J3"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["H3:J3"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["H3:J3"].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    worksheet.Cells["A3"].Value = "Date";
                    worksheet.Cells["B3"].Value = "Description";
                    worksheet.Cells["C3"].Value = "Credit";
                    worksheet.Cells["D3"].Value = "Debit";
                    worksheet.Cells["E3"].Value = "Account Balance";
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

                    // Add the "Previous Month Balance" entry
                    int row = 4;
                    worksheet.Cells[$"A{row}"].Value = startDate.Value.AddMonths(-1).ToString("MMMM yyyy") ;
                    worksheet.Cells[$"A{row}:D{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[$"A{row}:E{row}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells[$"B{row}"].Value = "Previous Month Balance";
                    worksheet.Cells[$"C{row}"].Value = previousMonthBalance;
                    worksheet.Cells[$"E{row}"].Value = previousMonthBalance;
                    worksheet.Cells[$"E{row}"].Style.Numberformat.Format = "₹ #,##0.00";
                    worksheet.Cells[$"C{row}"].Style.Numberformat.Format = "₹ #,##0.00";
                    worksheet.Cells[$"A{row}:E{row}"].Style.Font.Size = 14;
                    worksheet.Cells[$"B{row}"].Style.Font.Bold = true;
                    worksheet.Cells[$"B{row}"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[$"C{row}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[$"C{row}"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LawnGreen);
                    worksheet.Cells[$"H{row}:J{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[$"H{row}:J{row}"].Style.Font.Bold = true;
                    worksheet.Cells[$"H{row}:J{row}"].Style.Font.Size = 20;

                    worksheet.Row(row).Height = 28;

                    // Now add the data from the current month
                    row++;

                    decimal accountBalance = previousMonthBalance;
                    // Initialize totalCreditAmount and totalDebitAmount with previous month balance
                    decimal totalCreditAmount = previousMonthBalance > 0 ? previousMonthBalance : 0;
                    decimal totalDebitAmount = previousMonthBalance < 0 ? -previousMonthBalance : 0;

                    foreach (var account in accountMasters)
                    {
                        worksheet.Cells[$"A{row}:D{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[$"A{row}:E{row}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Row(row).Height = 28;
                        worksheet.Cells[$"A{row}"].Value = account.Date.ToString("dd-MM-yyyy");
                        worksheet.Cells[$"A{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Center-align Price
                        worksheet.Cells[$"B{row}"].Value = account.Description;
                        worksheet.Cells[$"B{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Center-align Date
                        worksheet.Cells[$"C{row}"].Value = account.PaidAmount;
                        worksheet.Cells[$"C{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Center-align ProductName
                        worksheet.Cells[$"D{row}"].Value = account.Category;
                        worksheet.Cells[$"D{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Center-align SKU
                        worksheet.Cells[$"H{row}:J{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        if (account.TransactionType == "Credit")
                        {
                            worksheet.Cells[$"C{row}"].Value = account.PaidAmount;
                            worksheet.Cells[$"D{row}"].Value = null;
                            worksheet.Cells[$"C{row}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[$"C{row}"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LawnGreen);
                            worksheet.Cells[$"C{row}"].Style.Numberformat.Format = "₹ #,##0.00";
                            accountBalance += account.PaidAmount;
                            totalCreditAmount += account.PaidAmount;
                        }
                        else if (account.TransactionType == "Debit")
                        {
                            worksheet.Cells[$"C{row}"].Value = null;
                            worksheet.Cells[$"D{row}"].Value = account.PaidAmount;
                            worksheet.Cells[$"D{row}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[$"D{row}"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightPink);
                            worksheet.Cells[$"D{row}"].Style.Numberformat.Format = "₹ #,##0.00";
                            accountBalance -= account.PaidAmount;
                            totalDebitAmount += account.PaidAmount;
                        }

                        worksheet.Cells[$"E{row}"].Value = accountBalance;
                        worksheet.Cells[$"E{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells[$"E{row}"].Style.Numberformat.Format = "₹ #,##,0.00";
                        worksheet.Cells[$"H4"].Value = totalCreditAmount;
                        worksheet.Cells[$"H4"].Style.Numberformat.Format = "₹ #,##0.00";
                        worksheet.Cells["I4"].Value = totalDebitAmount;
                        worksheet.Cells[$"I4"].Style.Numberformat.Format = "₹ #,##0.00";
                        worksheet.Cells["J4"].Value = totalCreditAmount - totalDebitAmount;
                        worksheet.Cells[$"J4"].Style.Numberformat.Format = "₹ #,##0.00";

                        worksheet.Cells[$"B{row}"].Style.Font.Bold = true;
                        
                        worksheet.Cells[$"A{row}:F{row}"].Style.Font.Size = 14;

                        worksheet.Cells[$"B{row}"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        row++;
                    }

                    // Fill background color for total amount cells
                    worksheet.Cells["H4:J4"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells["H4:J4"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.AntiqueWhite);

                    worksheet.Cells["H4"].Value = $"₹ {totalCreditAmount}";
                    worksheet.Cells["I4"].Value = $"₹ {totalDebitAmount}";
                    worksheet.Cells["J4"].Value = $"₹ {totalCreditAmount - totalDebitAmount}";
                }

                // Return the Excel file as a downloadable file
                byte[] excelData = package.GetAsByteArray();
                return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Account Report .xlsx");
            }
        }
        #endregion
    }
}
