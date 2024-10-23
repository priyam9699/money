using FinanceManagement.ViewModels;

namespace FinanceManagement.IRepository
{
    public interface IDashboardRepository
    {
        IEnumerable<DashboardIndexVM> GetDashboardData(string userId,string companyName, DateTime? startDate, DateTime? endDate, int SelectedMonth);
        Dictionary<string, decimal> GetTotalAmount(string userId, string companyName, int selectedMonth);
    }
}
