using FinanceManagement.IRepository;
using FinanceManagement.Models;
using FinanceManagement.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FinanceManagement.SqlRepository
{
    public class SqlPaymentRepository : IPaymentRepository
    {
        private readonly ApplicationDbContext context;
        private readonly ICompanyRepository companyRepository;

        public SqlPaymentRepository(ApplicationDbContext context, ICompanyRepository companyRepository)
        {
            this.context = context;
            this.companyRepository = companyRepository;
        }
        public Payment Add(Payment Payments)
        {
            context.Payments.Add(Payments);
            context.SaveChanges();
            return Payments;
        }

        public Payment Delete(int id)
        {
            var payment = context.Payments.Find(id);
            if (payment != null)
            {
                context.Payments.Remove(payment);
                context.SaveChanges();
            }
            return payment;
        }

        public Payment GetByCashFlowId(int cashFlowId)
        {
            throw new NotImplementedException();
        }

        public Payment GetById(int id)
        {
            return context.Payments.Find(id);
        }

        public IEnumerable<Payment> GetPaymentFromCompanyName(string firmName)
        {
            // Retrieve expenses associated with the specified firm name
            var payment = context.Payments
                .Where(p => p.FirmName == firmName)
                .ToList();

            return payment;
        }

        public decimal GetTotalPendingPayment(string userId, string companyName, int month)
        {
            // Retrieve the firms associated with the specified company name and user ID
            var firms = companyRepository.GetCompanyFromCompanyName(companyName)
                .Where(c => c.UserId == userId)
                .Select(c => c.FirmName)
                .ToList();

            if (!firms.Any())
            {
                return 0;
            }

            // Set the start and end date for the specified month
            DateTime startDate = new DateTime(DateTime.Today.Year, month, 1);
            DateTime endDate = startDate.AddMonths(1).AddDays(-1);

            decimal totalPendingPayment = 0;

            foreach (var firm in firms)
            {
                // Create base queries for total amount and paid amount
                var totalAmountQuery = context.Payments
                    .Where(p => p.FirmName == firm && p.Date >= startDate && p.Date <= endDate);

                var paidAmountQuery = context.Payments
                    .Where(p => p.FirmName == firm && p.Date >= startDate && p.Date <= endDate);

                // Calculate the total amount and paid amount
                var totalAmount = totalAmountQuery.Sum(p => (decimal?)p.TotalAmount) ?? 0;
                var paidAmount = paidAmountQuery.Sum(p => (decimal?)p.PaidAmount) ?? 0;

                // Calculate the pending payment for the firm
                var pendingPayment = (totalAmount == 0) ? paidAmount : totalAmount - paidAmount;


                // Sum up the pending payments for all firms
                totalPendingPayment += pendingPayment;
            }

            return totalPendingPayment;
        }


        public IEnumerable<Payment> SearchPayment(string userId, string companyName, string search)
        {
            return context.Payments
                .Where(p => p.UserId == userId && (p.Description.Contains(search)))
                .ToList();
        }

        public Payment Update(Payment UpdatePayments)
        {
            // Find the existing product in the context
            var existingPayment = context.expenses.Find(UpdatePayments.Id);

            // Check if the existing product is found
            if (existingPayment != null)
            {
                // Update the properties of the existing product
                existingPayment.Date = UpdatePayments.Date;
                existingPayment.Description = UpdatePayments.Description;
                existingPayment.Amount = UpdatePayments.TotalAmount;


                // Save the changes to the database
                context.SaveChanges();

                // Return the updated product
                return UpdatePayments;
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
    

