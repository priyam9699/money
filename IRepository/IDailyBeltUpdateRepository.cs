using FinanceManagement.Models;

namespace FinanceManagement.IRepository
{
    public interface IDailyBeltUpdateRepository
    {
        DailyBeltUpdate GetById(int id);

        IEnumerable<DailyBeltUpdate> GetAllDailyBeltUpdate(string userId);
        IEnumerable<DailyBeltUpdate> GetDailyBeltUpdateFromCompanyName(string companyName);
        DailyBeltUpdate Add(DailyBeltUpdate dailyBeltUpdate);
        DailyBeltUpdate Update(DailyBeltUpdate dailyBeltUpdate);
        DailyBeltUpdate Delete(int id);
        IEnumerable<DailyBeltUpdate> SearchDailyBeltUpdate(string userId, string search);
        IEnumerable<DailyBeltUpdate> FilterDailyBeltUpdate(string userId, DateTime? startDate, DateTime? endDate);
        int GetTotalDailyBeltUpdateQuantity(string userId);
        int GetTotalDailyBeltUpdateQuantityForCompanyName(string companyName);
        int GetTotalDailyBeltUpdateQuantityForMOnth(string companyName, int Month);
        int GetTotalDailyBeltUpdateCountForProduct(string companyName, int productId);
        int GetTotalDailyBeltUpdateCountForProductForMonth(string companyName, int productId, int Month);
        int GetTotalDailyBeltUpdateQuantityByDateRange(string companyName, DateTime startDate, DateTime endDate);
        int GetTotalDailyBeltUpdateCountForProductByDateRange(string companyName, int productId, DateTime startDate, DateTime endDate);
        Dictionary<string, int> GetMonthlyDailyBeltUpdateCounts(string companyName);
        int GetTotalDailyBeltUpdateYesterday(string companyName, DateTime yesterday);
        int GetTotalDailyBeltUpdateWeekly(string companyName, DateTime startOfWeek, DateTime endOfWeek);
        int GetTotalDailyBeltUpdateFor15Days(string companyName, DateTime startOf15, DateTime endOf15);
        int GetLastPendingAmount(string companyName, DateTime? startDate, DateTime? endDate);
    }
}
