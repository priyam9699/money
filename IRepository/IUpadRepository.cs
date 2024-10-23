using FinanceManagement.Models;

namespace FinanceManagement.IRepository
{
    public interface IUpadRepository
    {
        Upad GetById(int id);
        Upad Add(Upad Upads);
        Upad Update(Upad UpdateUpads);
        Upad Delete(int id);
        IEnumerable<Upad> SearchUpads(string userId, string companyName, string search);
        IEnumerable<Upad> GetUpadsFromCompanyName(string companyName);
        Upad GetByCashFlowId(int cashFlowId);
        decimal GetUpadTotal(string companyName, DateTime? startDate, DateTime? endDate);
        decimal GetUpadAmountForMonth(string userId, string companyName, int month);
    }
}
