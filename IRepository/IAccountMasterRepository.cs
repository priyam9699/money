using FinanceManagement.Models;

namespace FinanceManagement.IRepository
{
    public interface IAccountMasterRepository
    {
        AccountMaster GetById(int id);
        AccountMaster Add(AccountMaster AccountMaster);
        AccountMaster Update(AccountMaster UpdateAccountMaster);
        AccountMaster Delete(int id);
        IEnumerable<AccountMaster> SearchAccountMaster(string userId, string companyName, string search);
        public Dictionary<string, decimal> GetAccountBalancesForCompanyName(string companyName, DateTime? startDate, DateTime? endDate, int month);
        IEnumerable<AccountMaster> GetAccountMasterFromCompanyName(string companyName);
        decimal GetTotalAccountBalance(string userId, string companyName, int month);
    }
}
