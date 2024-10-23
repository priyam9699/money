using FinanceManagement.Models;

namespace FinanceManagement.IRepository
{
    public interface IPaymentRepository
    {
        Payment GetById(int id);
        Payment Add(Payment Payments);
        Payment Update(Payment UpdatePayments);
        Payment Delete(int id);
        IEnumerable<Payment> SearchPayment(string userId, string companyName, string search);
        IEnumerable<Payment> GetPaymentFromCompanyName(string firmName);
        Payment GetByCashFlowId(int cashFlowId);
        
        decimal GetTotalPendingPayment(string userId, string companyName, int month);
    }
}
