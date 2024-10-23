using FinanceManagement.Models;

namespace FinanceManagement.IRepository
{
    public interface IExpensesRepository
    {
        Expenses GetById(int id);
        Expenses Add(Expenses expenses);
        Expenses Update(Expenses UpdateExpenses);
        Expenses Delete(int id);
        IEnumerable<Expenses> SearchExpenses(string userId, string companyName, string search);
        IEnumerable<Expenses> GetExpensesFromCompanyName(string companyName);
        public List<Expenses> GetOfficeExpenses(DateTime? startDate, DateTime? endDate);
        Expenses GetByCashFlowId(int cashFlowId);
        int GetTotalExpenseAmount(string firmName, DateTime? startDate, DateTime? endDate);
        decimal GetOverAllExpenseAmount(string companyName, DateTime? startDate, DateTime? endDate);
    }
}
