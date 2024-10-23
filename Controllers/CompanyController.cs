using FinanceManagement.IRepository;
using FinanceManagement.Models;
using FinanceManagement.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FinanceManagement.Controllers
{

    public class CompanyController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IExpensesRepository expensesRepository;
        private readonly ICashFlowRepository cashFlowRepository;
        private readonly IPaymentRepository paymentRepository;
        private readonly IUpadRepository upadRepository;
        private readonly ICompanyRepository companyRepository;

        public CompanyController(ApplicationDbContext context, IExpensesRepository expensesRepository, ICashFlowRepository cashFlowRepository, IPaymentRepository paymentRepository, IUpadRepository upadRepository, ICompanyRepository companyRepository)
        {
            _context = context;
            this.expensesRepository = expensesRepository;
            this.cashFlowRepository = cashFlowRepository;
            this.paymentRepository = paymentRepository;
            this.upadRepository = upadRepository;
            this.companyRepository = companyRepository;
        }


        #region CompanyIndex
        public IActionResult CompanyIndex()
        {

            var company = _context.Companies;
            return View(company);
        }
        #endregion

        #region CompanyCreate
        [HttpGet]
        public IActionResult CompanyCreate()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CompanyCreate(CompanyVM companyVM)
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

            if (ModelState.IsValid)
            {
                // Create a new instance of CashFlow model
                var company = new Company
                {
                    UserId = userId,
                    FirmName = companyVM.FirmName,
                    Owner = companyVM.Owner,
                    Email = companyVM.Email
                    
                };

                _context.Companies.Add(company);
                _context.SaveChanges();

                

                // Redirect to CashFlowIndex action
                return RedirectToAction("CompanyIndex", "Company");
            }

            // If ModelState is not valid, return the view with validation errors
            return View(companyVM);
        }

        #endregion

        #region CompanyEdit
        [HttpGet]
        public IActionResult CompanyEdit(int id)
        {
            var company = companyRepository.GetById(id);
            return View(company);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CompanyEdit(int id, Company company)
        {
            var existingCompany = companyRepository.GetById(id);

            if (existingCompany == null)
            {
                return NotFound();
            }
            existingCompany.FirmName = company.FirmName;
            existingCompany.Owner = company.Owner;
            existingCompany.Email = company.Email;


            companyRepository.Update(existingCompany);
            _context.SaveChanges();

            return RedirectToAction("CompanyIndex", "Company");
        }
        #endregion

        #region Company Delete
        [HttpGet]
        public IActionResult CompanyDelete(int id)
        {
            // Ensure that the product exists
            var company = companyRepository.GetById(id);
            if (company == null)
            {
                return NotFound();
            }

            return View(company); // Assuming you have a view to confirm the deletion
        }



        [HttpPost]

        public IActionResult ConfirmCompanyDelete(int id)
        {
            // Ensure that the product exists
            var company = companyRepository.GetById(id);
            if (company == null)
            {
                return NotFound();
            }

            // Delete the product
            companyRepository.Delete(id);

            return RedirectToAction("CompanyIndex");
        }
        #endregion

    }
}
