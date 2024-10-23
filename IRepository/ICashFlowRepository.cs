using FinanceManagement.Models;

namespace FinanceManagement.IRepository
{
    public interface ICashFlowRepository
    {
        CashFlow GetById(int id);
        CashFlow Add(CashFlow CashFlow);
        CashFlow Update(CashFlow UpdateCashFlow);
        CashFlow Delete(int id);
        IEnumerable<CashFlow> SearchCashFlow(string userId,string companyName, string search);
        IEnumerable<CashFlow> GetCashFlowFromCompanyName(string companyName);
        decimal GetCashFlowTotal(string companyName, DateTime? startDate, DateTime? endDate);
        decimal GetTotalCashFlow(string userId, string companyName, int month);
    }
}
