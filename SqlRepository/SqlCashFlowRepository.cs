using FinanceManagement.IRepository;
using FinanceManagement.Models;

namespace FinanceManagement.SqlRepository
{
    public class SqlCashFlowRepository : ICashFlowRepository
    {
        private readonly ApplicationDbContext context;

        public SqlCashFlowRepository(ApplicationDbContext context)
        {
            this.context = context;
        }
        public CashFlow Add(CashFlow CashFlow)
        {
            context.CashFlows.Add(CashFlow);
            context.SaveChanges();
            return CashFlow;
        }

        public CashFlow Delete(int id)
        {
            var cashflow = context.CashFlows.Find(id);
            if (cashflow != null)
            {
                context.CashFlows.Remove(cashflow);
                context.SaveChanges();
            }
            return cashflow;
        }

        public CashFlow GetById(int id)
        {
            return context.CashFlows.Find(id);
        }

        public IEnumerable<CashFlow> GetCashFlowFromCompanyName(string companyName)
        {
            // Retrieve products associated with users who have the specified company name
            var cashflows = context.CashFlows
                .Where(p => context.Users.Any(u => u.CompanyName == companyName && u.Id == p.UserId))
                .ToList();

            return cashflows;
        }

        public decimal GetCashFlowTotal(string companyName, DateTime? startDate, DateTime? endDate)
        {
            // Fetch user ids associated with the companyName
            var userIds = context.Users
                .Where(u => u.CompanyName == companyName)
                .Select(u => u.Id)
                .ToList();

            // Base query for cash flows
            var cashFlowsQuery = context.CashFlows
                .Where(cf => userIds.Contains(cf.UserId));

            // Apply date filters if provided
            if (startDate.HasValue)
            {
                cashFlowsQuery = cashFlowsQuery.Where(cf => cf.Date >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                cashFlowsQuery = cashFlowsQuery.Where(cf => cf.Date <= endDate.Value);
            }

            // Calculate total credits
            var totalCredits = cashFlowsQuery
                .Where(cf => cf.TransactionType == "Credit")
                .Sum(cf => (decimal?)cf.Amount) ?? 0;


            // Calculate total debits
            var totalDebits = cashFlowsQuery
                .Where(cf => cf.TransactionType == "Debit")
                .Sum(cf => (decimal?)cf.Amount) ?? 0;

            // Calculate the cash balance
            var cashBalance = totalCredits - totalDebits;

            return cashBalance;
        }

        public decimal GetTotalCashFlow(string userId, string companyName, int month)
        {
            // Fetch user ids associated with the companyName
            var userIds = context.Users
                .Where(u => u.CompanyName == companyName)
                .Select(u => u.Id)
                .ToList();

            // Base query for cash flows
            var cashFlowsQuery = context.CashFlows
                .Where(cf => userIds.Contains(cf.UserId));

            

            // Calculate total credits
            
            var creditQuery = context.CashFlows
                .Where(am => am.TransactionType == "Credit"
                             && am.Date.Month == month).Sum(cf => (decimal?)cf.Amount) ?? 0;


            // Calculate total debits
            var debitQuery = context.CashFlows
                .Where(am => am.TransactionType == "Debit"
                             && am.Date.Month == month).Sum(cf => (decimal?)cf.Amount) ?? 0;

            // Calculate the cash balance
            var cashBalance = creditQuery - debitQuery;

            return cashBalance;
        }

        public IEnumerable<CashFlow> SearchCashFlow(string userId,string companyName, string search)
        {
            return context.CashFlows
                .Where(p => p.UserId == userId && (p.Description.Contains(search) || p.Category.Contains(search)))
                .ToList();
        }

        public CashFlow Update(CashFlow UpdateCashFlow)
        {
            // Find the existing product in the context
            var existingCashFlow = context.CashFlows.Find(UpdateCashFlow.Id);

            // Check if the existing product is found
            if (existingCashFlow != null)
            {
                // Update the properties of the existing product
                existingCashFlow.Date = UpdateCashFlow.Date;
                existingCashFlow.Description = UpdateCashFlow.Description;
                existingCashFlow.Amount = UpdateCashFlow.Amount;
                existingCashFlow.Category = UpdateCashFlow.Category;
                existingCashFlow.TransactionType = UpdateCashFlow.TransactionType;

                // Save the changes to the database
                context.SaveChanges();

                // Return the updated product
                return existingCashFlow;
            }
            else
            {
                // If the existing product is not found, you might handle this case according to your application's logic
                // For example, you could throw an exception or return null
                throw new InvalidOperationException("CashFlow not found.");
            }
        }
    }
}
