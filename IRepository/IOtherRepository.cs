using FinanceManagement.Models;

namespace FinanceManagement.IRepository
{
    public interface IOtherRepository
    {
        Other GetById(int id);
        Other Add(Other others);
        Other Update(Other UpdateOthers);
        Other Delete(int id);
        IEnumerable<Other> SearchOther(string userId, string companyName, string search);
        IEnumerable<Other> GetOtherFromCompanyName(string companyName);
    }
}
