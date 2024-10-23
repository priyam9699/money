using FinanceManagement.IRepository;
using FinanceManagement.Models;
using FinanceManagement.ViewModels;

namespace FinanceManagement.SqlRepository
{
    public class SqlUpadRepository : IUpadRepository
    {
        private readonly ApplicationDbContext context;

        public SqlUpadRepository(ApplicationDbContext context)
        {
            this.context = context;
        }
        public Upad Add(Upad Upads)
        {
            context.Upads.Add(Upads);
            context.SaveChanges();
            return Upads;
        }

        public Upad Delete(int id)
        {
            var upad = context.Upads.Find(id);
            if (upad != null)
            {
                context.Upads.Remove(upad);
                context.SaveChanges();
            }
            return upad;
        }

        public Upad GetByCashFlowId(int cashFlowId)
        {
            throw new NotImplementedException();
        }

        public Upad GetById(int id)
        {
            return context.Upads.Find(id);
        }

        public decimal GetUpadAmountForMonth(string userId, string companyName, int month)
        {
            // Fetch the start and end dates for the specified month
            DateTime startDate = new DateTime(DateTime.Today.Year, month, 1);
            DateTime endDate = startDate.AddMonths(1).AddDays(-1);

            // Fetch user ids associated with the companyName
            var userIds = context.Users
                .Where(u => u.CompanyName == companyName)
                .Select(u => u.Id)
                .ToList();

            // Base query for Upads
            var upadsQuery = context.Upads
                .Where(upad => userIds.Contains(upad.UserId));

            var upadAmountQuery = context.Upads

                .Where(upad => upad.Date >= startDate && upad.Date <= endDate);

            // Sum the UPAD amounts
            var totalUpadAmount = upadAmountQuery.Sum(upad => (decimal?)upad.Amount) ?? 0;

            return totalUpadAmount;

        }

        public IEnumerable<Upad> GetUpadsFromCompanyName(string companyName)
        {
            // Retrieve products associated with users who have the specified company name
            var upads = context.Upads
                .Where(p => context.Users.Any(u => u.CompanyName == companyName && u.Id == p.UserId))
                .ToList();

            return upads;
        }

        public decimal GetUpadTotal(string companyName, DateTime? startDate, DateTime? endDate)
        {
            // Fetch user ids associated with the companyName
            var userIds = context.Users
                .Where(u => u.CompanyName == companyName)
                .Select(u => u.Id)
                .ToList();

            // Base query for Upads
            var upadsQuery = context.Upads
                .Where(upad => userIds.Contains(upad.UserId));

            // Apply date filters if provided
            if (startDate.HasValue)
            {
                upadsQuery = upadsQuery.Where(upad => upad.Date >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                upadsQuery = upadsQuery.Where(upad => upad.Date <= endDate.Value);
            }

            // Calculate the total amount
            var total = upadsQuery.Sum(upad => (decimal?)upad.Amount) ?? 0;

            return total;
        }


        public IEnumerable<Upad> SearchUpads(string userId, string companyName, string search)
        {
            return context.Upads
                .Where(p => p.UserId == userId && (p.Description.Contains(search)))
                .ToList();
        }

        public Upad Update(Upad UpdateUpads)
        {
            // Find the existing product in the context
            var existingUpad = context.Upads.Find(UpdateUpads.Id);

            // Check if the existing product is found
            if (existingUpad != null)
            {
                // Update the properties of the existing product
                existingUpad.Date = UpdateUpads.Date;
                existingUpad.Description = UpdateUpads.Description;
                existingUpad.Amount = UpdateUpads.Amount;


                // Save the changes to the database
                context.SaveChanges();

                // Return the updated product
                return existingUpad;
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
