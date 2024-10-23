using FinanceManagement.IRepository;
using FinanceManagement.Models;

namespace FinanceManagement.SqlRepository
{
    public class SqlCompanyRepository : ICompanyRepository
    {
        private readonly ApplicationDbContext context;

        public SqlCompanyRepository(ApplicationDbContext context)
        {
            this.context = context;
        }
        public Company Add(Company Company)
        {
            context.Companies.Add(Company);
            context.SaveChanges();
            return Company;
        }

        public Company Delete(int id)
        {
            var company = context.Companies.Find(id);
            if (company != null)
            {
                context.Companies.Remove(company);
                context.SaveChanges();
            }
            return company;
        }

        public Company GetById(int id)
        {
            return context.Companies.Find(id);
        }

        public IEnumerable<Company> GetCompanyFromCompanyName(string companyName)
        {
            // Retrieve products associated with users who have the specified company name
            var company = context.Companies
                .Where(p => context.Users.Any(u => u.CompanyName == companyName && u.Id == p.UserId))
                .ToList();

            return company;
        }

        public IEnumerable<Company> SearchCompany(string userId, string companyName, string search)
        {
            return context.Companies
                .Where(p => p.UserId == userId && (p.FirmName.Contains(search) || p.Owner.Contains(search)))
                .ToList();
        }

        public Company Update(Company UpdateCompany)
        {
            // Find the existing product in the context
            var existingCompany = context.Companies.Find(UpdateCompany.Id);

            // Check if the existing product is found
            if (existingCompany != null)
            {
                // Update the properties of the existing product
                existingCompany.FirmName = UpdateCompany.FirmName;
                existingCompany.Email = UpdateCompany.Email;
                existingCompany.Owner = UpdateCompany.Owner;
                

                // Save the changes to the database
                context.SaveChanges();

                // Return the updated product
                return existingCompany;
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
