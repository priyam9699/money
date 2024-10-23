using FinanceManagement.Models;

namespace FinanceManagement.IRepository
{
    public interface ICompanyRepository
    {
        Company GetById(int id);
        Company Add(Company Company);
        Company Update(Company UpdateCompany);
        Company Delete(int id);
        IEnumerable<Company> SearchCompany(string userId, string companyName, string search);
        IEnumerable<Company> GetCompanyFromCompanyName(string companyName);
    }
}
