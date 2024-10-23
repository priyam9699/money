using FinanceManagement.IRepository;
using FinanceManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceManagement.SqlRepository
{
    public class SqlExpensesRepository : IExpensesRepository
    {
        private readonly ApplicationDbContext context;

        public SqlExpensesRepository(ApplicationDbContext context)
        {
            this.context = context;
        }
        public Expenses Add(Expenses expenses)
        {
            context.expenses.Add(expenses);
            context.SaveChanges();
            return expenses;
        }

        public Expenses Delete(int id)
        {
            var expense = context.expenses.Find(id);
            if (expense != null)
            {
                context.expenses.Remove(expense);
                context.SaveChanges();
            }
            return expense;
        }

        public Expenses GetByCashFlowId(int cashFlowId)
        {
            throw new NotImplementedException();
        }

        public Expenses GetById(int id)
        {
            return context.expenses.Find(id);
        }

        public IEnumerable<Expenses> GetExpensesFromCompanyName(string firmName)
        {
            // Retrieve expenses associated with the specified firm name
            var expenses = context.expenses
                .Where(p => p.FirmName == firmName)
                .ToList();

            return expenses;
        }

        public int GetTotalExpenseAmount(string firmName, DateTime? startDate, DateTime? endDate)
        {
            // Query the expenses for the specified firmName
            var expensesQuery = context.expenses
                                       .Where(expense => expense.FirmName == firmName);

            // Apply date filters if provided
            if (startDate.HasValue)
            {
                expensesQuery = expensesQuery.Where(expense => expense.Date >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                expensesQuery = expensesQuery.Where(expense => expense.Date <= endDate.Value);
            }

            // Sum up the total amount of expenses
            var totalExpenses = expensesQuery.Sum(expense => (int?)expense.Amount) ?? 0;

            return totalExpenses;
        }


        public List<Expenses> GetOfficeExpenses(DateTime? startDate, DateTime? endDate)
        {
            // Query the expenses where CashFlowId is not null
            var expensesQuery = context.expenses
                                       .Where(expense => expense.CashFlowId != null);

            // Apply date filters if provided
            if (startDate.HasValue)
            {
                expensesQuery = expensesQuery.Where(expense => expense.Date >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                expensesQuery = expensesQuery.Where(expense => expense.Date <= endDate.Value);
            }

            // Execute the query and return the result
            return expensesQuery.ToList();
        }




        public IEnumerable<Expenses> SearchExpenses(string userId, string companyName, string search)
        {
            return context.expenses
                .Where(p => p.UserId == userId && (p.Description.Contains(search)))
                .ToList();
        }

        public Expenses Update(Expenses UpdateExpenses)
        {
            // Find the existing product in the context
            var existingExpense = context.expenses.Find(UpdateExpenses.Id);

            // Check if the existing product is found
            if (existingExpense != null)
            {
                // Update the properties of the existing product
                existingExpense.Date = UpdateExpenses.Date;
                existingExpense.Description = UpdateExpenses.Description;
                existingExpense.Amount = UpdateExpenses.Amount;
                

                // Save the changes to the database
                context.SaveChanges();

                // Return the updated product
                return existingExpense;
            }
            else
            {
                // If the existing product is not found, you might handle this case according to your application's logic
                // For example, you could throw an exception or return null
                throw new InvalidOperationException("CashFlow not found.");
            }
        }

        public decimal GetOverAllExpenseAmount(string companyName, DateTime? startDate, DateTime? endDate)
        {
            // Get the user IDs for the specified company
            var userIds = context.Users
                                 .Where(u => u.CompanyName == companyName)
                                 .Select(u => u.Id)
                                 .ToList();

            // Query the expenses
            var expensesQuery = context.expenses
                                       .Where(e => userIds.Contains(e.UserId));

            // Apply date filters if provided
            if (startDate.HasValue)
            {
                expensesQuery = expensesQuery.Where(e => e.Date >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                expensesQuery = expensesQuery.Where(e => e.Date <= endDate.Value);
            }

            // Sum the amount
            var expenseAmount = expensesQuery.Sum(e => e.Amount);

            return expenseAmount;
        }

    }
}
