using FinanceManagement.IRepository;
using FinanceManagement.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FinanceManagement.SqlRepository
{
    public class SqlAccountMasterRepository : IAccountMasterRepository
    {
        private readonly ApplicationDbContext context;

        public SqlAccountMasterRepository(ApplicationDbContext context)
        {
            this.context = context;
        }
        public AccountMaster Add(AccountMaster AccountMaster)
        {
            context.AccountMaster.Add(AccountMaster);
            context.SaveChanges();
            return AccountMaster;
        }

        public AccountMaster Delete(int id)
        {
            var accountmaster = context.AccountMaster.Find(id);
            if (accountmaster != null)
            {
                context.AccountMaster.Remove(accountmaster);
                context.SaveChanges();
            }
            return accountmaster;
        }

        public IEnumerable<AccountMaster> GetAccountMasterFromCompanyName(string companyName)
        {
            // Retrieve products associated with users who have the specified company name
            var accountmaster = context.AccountMaster
                .Where(p => context.Users.Any(u => u.CompanyName == companyName && u.Id == p.UserId))
                .ToList();

            return accountmaster;
        }

        public AccountMaster GetById(int id)
        {
            return context.AccountMaster.Find(id);
        }

        public IEnumerable<AccountMaster> SearchAccountMaster(string userId, string companyName, string search)
        {
            return context.AccountMaster
                .Where(p => p.UserId == userId && (p.Description.Contains(search) || p.Category.Contains(search)))
                .ToList();
        }

        public Dictionary<string, decimal> GetAccountBalancesForCompanyName(string companyName, DateTime? startDate, DateTime? endDate, int month)
        {
            var accountBalances = new Dictionary<string, decimal>();

            // Step 1: Retrieve the user IDs based on the company name from the user table
            var userIds = context.Users
                .Where(u => u.CompanyName == companyName)
                .Select(u => u.Id)
                .ToList();

            if (!userIds.Any())
            {
                // If no user IDs are found, return an empty dictionary
                return accountBalances;
            }

            // Step 2: Get all firm names associated with the retrieved user IDs from the company table
            var firmNames = context.Companies
                .Where(c => userIds.Contains(c.UserId))
                .Select(c => c.FirmName)
                .Distinct()
                .ToList();

            // Step 3: For each firm name, calculate the account balance
            foreach (var firmName in firmNames)
            {
                // Retrieve the company IDs associated with the current firm name and user IDs
                var companyIds = context.Companies
                    .Where(c => userIds.Contains(c.UserId) && c.FirmName == firmName)
                    .Select(c => c.Id)
                    .ToList();

                if (!companyIds.Any())
                {
                    // If no company IDs are found, set account balance to 0 for the firm name
                    accountBalances[firmName] = 0;
                    continue;
                }

                // Query for credits and debits, optionally filtering by the provided date range
                var creditQuery = context.AccountMaster
                    .Where(am => companyIds.Contains(am.CompanyId) && am.TransactionType == "Credit");

                var debitQuery = context.AccountMaster
                    .Where(am => companyIds.Contains(am.CompanyId) && am.TransactionType == "Debit");

                if (startDate.HasValue)
                {
                    creditQuery = creditQuery.Where(am => am.Date >= startDate.Value);
                    debitQuery = debitQuery.Where(am => am.Date >= startDate.Value);
                }

                if (endDate.HasValue)
                {
                    creditQuery = creditQuery.Where(am => am.Date <= endDate.Value);
                    debitQuery = debitQuery.Where(am => am.Date <= endDate.Value);
                }

                // Calculate the total credits and debits
                var totalCredits = creditQuery.Sum(am => (decimal?)am.TotalAmount) ?? 0;
                var totalDebits = debitQuery.Sum(am => (decimal?)am.TotalAmount) ?? 0;

                int previousMonth = month == 1 ? 12 : month - 1; // Handle January wrap-around
                var previousYear = month == 1 ? DateTime.Now.Year - 1 : DateTime.Now.Year; // Adjust year if necessary

                var creditQueryPreviousMonth = context.AccountMaster
                    .Where(am => companyIds.Contains(am.CompanyId) && am.TransactionType == "Credit"
                                 && am.Date.Month == previousMonth && am.Date.Year == previousYear);

                var debitQueryPreviousMonth = context.AccountMaster
                    .Where(am => companyIds.Contains(am.CompanyId) && am.TransactionType == "Debit"
                                 && am.Date.Month == previousMonth && am.Date.Year == previousYear);

                // Calculate the total credits and debits for the previous month
                var totalCreditsPreviousMonth = creditQueryPreviousMonth.Sum(am => (decimal?)am.PaidAmount) ?? 0;
                var totalDebitsPreviousMonth = debitQueryPreviousMonth.Sum(am => (decimal?)am.PaidAmount) ?? 0;

                // Calculate the account balance for the previous month
                var previousMonthBalance = totalCreditsPreviousMonth - totalDebitsPreviousMonth;




                // Calculate the account balance
                var accountBalance = previousMonthBalance + totalCredits - totalDebits;

                // Add the account balance to the dictionary
                accountBalances[firmName] = accountBalance;
            }

            return accountBalances;
        }





        public AccountMaster Update(AccountMaster UpdateAccountMaster)
        {
            // Find the existing product in the context
            var existingAccountMaster = context.AccountMaster.Find(UpdateAccountMaster.Id);

            // Check if the existing product is found
            if (existingAccountMaster != null)
            {
                // Update the properties of the existing product
                existingAccountMaster.Date = UpdateAccountMaster.Date;
                existingAccountMaster.Description = UpdateAccountMaster.Description;
                existingAccountMaster.TotalAmount = UpdateAccountMaster.TotalAmount;
                existingAccountMaster.FirmName = UpdateAccountMaster.FirmName;
                existingAccountMaster.Category = UpdateAccountMaster.Category;
                existingAccountMaster.TransactionType = UpdateAccountMaster.TransactionType;
                

                // Save the changes to the database
                context.SaveChanges();

                // Return the updated product
                return existingAccountMaster;
            }
            else
            {
                // If the existing product is not found, you might handle this case according to your application's logic
                // For example, you could throw an exception or return null
                throw new InvalidOperationException("CashFlow not found.");
            }
        }


        public decimal GetTotalAccountBalance(string userId,string companyName, int month)
        {
            
            
            // Step 1: Retrieve the user IDs based on the company name from the user table
            var userIds = context.Users
                .Where(u => u.CompanyName == companyName)
                .Select(u => u.Id)
                .ToList();

            if (!userIds.Any())
            {
                // If no user IDs are found, return 0
                return 0;
            }

            // Step 2: Get all company IDs associated with the retrieved user IDs from the company table
            var companyIds = context.Companies
                .Where(c => userIds.Contains(c.UserId))
                .Select(c => c.Id)
                .ToList();

            if (!companyIds.Any())
            {
                // If no company IDs are found, return 0
                return 0;
            }

            // Step 3: Query for credits and debits, filtering by the provided month and year
            var creditQuery = context.AccountMaster
                .Where(am => companyIds.Contains(am.CompanyId) && am.TransactionType == "Credit"
                             && am.Date.Month == month);

            var debitQuery = context.AccountMaster
                .Where(am => companyIds.Contains(am.CompanyId) && am.TransactionType == "Debit"
                             && am.Date.Month == month);

            // Calculate the total credits and debits
            var totalCredits = creditQuery.Sum(am => (decimal?)am.PaidAmount) ?? 0;
            var totalDebits = debitQuery.Sum(am => (decimal?)am.PaidAmount) ?? 0;

            int previousMonth = month == 1 ? 12 : month - 1; // Handle January wrap-around
            var previousYear = month == 1 ? DateTime.Now.Year - 1 : DateTime.Now.Year; // Adjust year if necessary

            var creditQueryPreviousMonth = context.AccountMaster
                .Where(am => companyIds.Contains(am.CompanyId) && am.TransactionType == "Credit"
                             && am.Date.Month == previousMonth && am.Date.Year == previousYear);

            var debitQueryPreviousMonth = context.AccountMaster
                .Where(am => companyIds.Contains(am.CompanyId) && am.TransactionType == "Debit"
                             && am.Date.Month == previousMonth && am.Date.Year == previousYear);

            // Calculate the total credits and debits for the previous month
            var totalCreditsPreviousMonth = creditQueryPreviousMonth.Sum(am => (decimal?)am.PaidAmount) ?? 0;
            var totalDebitsPreviousMonth = debitQueryPreviousMonth.Sum(am => (decimal?)am.PaidAmount) ?? 0;

            // Calculate the account balance for the previous month
            var previousMonthBalance = totalCreditsPreviousMonth - totalDebitsPreviousMonth;


            // Calculate the account balance
            var accountBalance = previousMonthBalance + totalCredits - totalDebits;

            return accountBalance;
        }


    }
}
