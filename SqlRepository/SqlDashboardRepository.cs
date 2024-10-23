using FinanceManagement.IRepository;
using FinanceManagement.Models;
using FinanceManagement.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FinanceManagement.SqlRepository
{
    public class SqlDashboardRepository : IDashboardRepository
    {

        private readonly ApplicationDbContext _context;
        private readonly ICompanyRepository companyRepository;
        private readonly IPaymentRepository paymentRepository;
        private readonly IAccountMasterRepository accountMasterRepository;
        private readonly ICashFlowRepository cashFlowRepository;
        private readonly IUpadRepository upadRepository;

        public SqlDashboardRepository(ApplicationDbContext context, ICompanyRepository companyRepository, IPaymentRepository paymentRepository, IAccountMasterRepository accountMasterRepository, ICashFlowRepository cashFlowRepository, IUpadRepository upadRepository)
        {
            _context = context;
            this.companyRepository = companyRepository;
            this.paymentRepository = paymentRepository;
            this.accountMasterRepository = accountMasterRepository;
            this.cashFlowRepository = cashFlowRepository;
            this.upadRepository = upadRepository;
        }
        #region Dashboard
        public IEnumerable<DashboardIndexVM> GetDashboardData(string userId, string companyName, DateTime? startDate, DateTime? endDate, int SelectedMonth)
        {
            // Retrieve the firm names
            var firmNames = companyRepository.GetCompanyFromCompanyName(companyName)
                .Select(c => c.FirmName)
                .ToList();

            var accountBalances = accountMasterRepository.GetAccountBalancesForCompanyName(companyName, startDate, endDate, SelectedMonth);
            var TotalUpad = upadRepository.GetUpadTotal(companyName, startDate, endDate);
            decimal totalPendingPayment = 0;

            var dashboardDataList = new List<DashboardIndexVM>();

            foreach (var firmName in firmNames)
            {
                // Get the account balance for the current firm name
                decimal? accountBalance = accountBalances.ContainsKey(firmName) ? accountBalances[firmName] : (decimal?)null;


                // Get the pending payment for the current company
                decimal? pendingPayment = GetPendingPayment(userId, firmName, startDate, endDate);
                // Sum up the pending payments
                totalPendingPayment += pendingPayment ?? 0;


                decimal? cashBalance = cashFlowRepository.GetCashFlowTotal(companyName,startDate,endDate);
                decimal? totalUpad = upadRepository.GetUpadTotal(companyName, startDate, endDate);

                // Create DashboardIndexVM object for the current company
                var dashboardData = new DashboardIndexVM
                {
                    Company = firmName,
                    AccountBalance = accountBalance,
                    FlipkartOutstandingPayment = null, // Replace with actual outstanding payment calculation for Flipkart
                    AmazonOutstandingPayment = null, // Replace with actual outstanding payment calculation for Amazon
                    MeeshoOutstandingPayment = null, // Replace with actual outstanding payment calculation for Meesho
                    OtherOutstandingPayment = null, // Replace with actual outstanding payment calculation for other companies
                    PendingPayment = pendingPayment,
                    CashBalance = cashBalance,
                    TotalUpad = totalUpad,
                    FirmAccountBalances = accountBalances,

                };

                // Add the DashboardIndexVM object to the list
                dashboardDataList.Add(dashboardData);
            }

            // Return the list of DashboardIndexVM objects
            return dashboardDataList;
        }

        public Dictionary<string, decimal> GetTotalAmount(string userId,string companyName, int SelectedMonth)
        {
            // Initialize a dictionary to store total orders and shipments for each month
            var totalAmount = new Dictionary<string, decimal>();
            

            // Retrieve orders and shipments for each month
            for (int month = 1; month <= 12; month++)
            {
                DateTime firstDayOfMonth = new DateTime(DateTime.Today.Year, month, 1);
                DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);



                decimal TotalAccountBalance = accountMasterRepository.GetTotalAccountBalance(userId,companyName, month);
                decimal TotalCashFlow = cashFlowRepository.GetTotalCashFlow(userId, companyName, month);
                decimal TotalPendingPayment = paymentRepository.GetTotalPendingPayment(userId,companyName, month);
                decimal TotalUpadAmount = upadRepository.GetUpadAmountForMonth(userId, companyName, month);
                decimal advancePayment = _context.Payments
                                    .Where(p => p.UserId == userId && p.PaymentCategory == "Advance Payment" && p.Date >= firstDayOfMonth && p.Date <= lastDayOfMonth)
                                    .Sum(p => (decimal)p.PaidAmount);

                var totalPlusAmount = TotalAccountBalance + TotalCashFlow + advancePayment;
                var totalMinusAmount = TotalPendingPayment + TotalUpadAmount;
                var TOTAL = totalPlusAmount - totalMinusAmount;

                

                // Add the total to the dictionary with the month name as the key
                totalAmount.Add(firstDayOfMonth.ToString("MMMM"), TOTAL);
            }

            return totalAmount;
        }

        private decimal? GetPendingPayment(string userId, string firmName, DateTime? startDate , DateTime? endDate)
        {
            // Retrieve the company associated with the specified user ID and company name
            var company = _context.Companies
                .FirstOrDefault(c => c.UserId == userId && c.FirmName == firmName);

            if (company == null)
            {
                return null;
            }

            // Create base queries for total amount and paid amount
            var totalAmountQuery = _context.Payments
                .Where(p => p.FirmName == company.FirmName);

            var paidAmountQuery = _context.Payments
                .Where(p => p.FirmName == company.FirmName);

            // Apply date filters if provided
            if (startDate.HasValue)
            {
                totalAmountQuery = totalAmountQuery.Where(p => p.Date >= startDate.Value);
                paidAmountQuery = paidAmountQuery.Where(p => p.Date >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                totalAmountQuery = totalAmountQuery.Where(p => p.Date <= endDate.Value);
                paidAmountQuery = paidAmountQuery.Where(p => p.Date <= endDate.Value);
            }

            // Calculate the total amount and paid amount
            var totalAmount = totalAmountQuery.Sum(p => (decimal?)p.TotalAmount) ?? 0;
            var paidAmount = paidAmountQuery.Sum(p => (decimal?)p.PaidAmount) ?? 0;

            // Calculate the pending payment
            var pendingPayment = totalAmount - paidAmount;

            return pendingPayment;
        }

        

        #endregion

    }
}
