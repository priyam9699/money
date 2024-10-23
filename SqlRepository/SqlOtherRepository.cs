using FinanceManagement.IRepository;
using FinanceManagement.Models;
using FinanceManagement.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FinanceManagement.SqlRepository
{
    public class SqlOtherRepository : IOtherRepository
    {
        private readonly ApplicationDbContext context;

        // Constructor to initialize the context
        public SqlOtherRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public Other Add(Other other)
        {
            context.Others.Add(other);
            context.SaveChanges();
            return other;
        }

        public Other Delete(int id)
        {
            var other = context.Others.Find(id);
            if (other != null)
            {
                context.Others.Remove(other);
                context.SaveChanges();
            }
            return other;
        }

        public Other GetById(int id)
        {
            return context.Others.Find(id);
        }

        public IEnumerable<Other> GetOtherFromCompanyName(string firmName)
        {
            var other = context.Others
                .Where(p => p.FirmName == firmName)
                .ToList();

            return other;
        }

        public IEnumerable<Other> SearchOther(string userId, string companyName, string search)
        {
            return context.Others
                .Where(p => p.UserId == userId && (p.Description.Contains(search)))
                .ToList();
        }

        public Other Update(Other UpdateOthers)
        {
            // Find the existing product in the context
            var existingOther = context.Others.Find(UpdateOthers.Id);

            // Check if the existing product is found
            if (existingOther != null)
            {
                // Update the properties of the existing product
                existingOther.Date = UpdateOthers.Date;
                existingOther.Description = UpdateOthers.Description;
                existingOther.Amount = UpdateOthers.Amount;


                // Save the changes to the database
                context.SaveChanges();

                // Return the updated product
                return existingOther;
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
