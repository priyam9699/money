using FinanceManagement.IRepository;
using FinanceManagement.Models;
using FinanceManagement.ViewModels;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.Style;

namespace FinanceManagement.SqlRepository
{
    public class SqlDailyBeltUpdateRepository : IDailyBeltUpdateRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IProductRepository productRepository;

        public SqlDailyBeltUpdateRepository(ApplicationDbContext context, IProductRepository productRepository)
        {
            this.context = context;
            this.productRepository = productRepository;
        }
        public DailyBeltUpdate Add(DailyBeltUpdate dailyBeltUpdate)
        {
            context.dailyBeltUpdates.Add(dailyBeltUpdate);
            context.SaveChanges();
            return dailyBeltUpdate;
        }

        public DailyBeltUpdate Delete(int id)
        {
            var dailybelt = context.dailyBeltUpdates.Find(id);
            if (dailybelt != null)
            {
                context.dailyBeltUpdates.Remove(dailybelt);
                context.SaveChanges();
            }
            return dailybelt;
        }

        public IEnumerable<DailyBeltUpdate> FilterDailyBeltUpdate(string userId, DateTime? startDate, DateTime? endDate)
        {
            var allDailyBelt = GetAllDailyBeltUpdate(userId);

            if (startDate.HasValue && endDate.HasValue)
            {
                allDailyBelt = allDailyBelt.Where(p => p.Date >= startDate.Value && p.Date <= endDate.Value);
            }
            else if (startDate.HasValue)
            {
                allDailyBelt = allDailyBelt.Where(p => p.Date >= startDate.Value);
            }
            else if (endDate.HasValue)
            {
                allDailyBelt = allDailyBelt.Where(p => p.Date <= endDate.Value);
            }

            return allDailyBelt.ToList();
        }

        public IEnumerable<DailyBeltUpdate> GetAllDailyBeltUpdate(string userId)
        {
            return context.dailyBeltUpdates.Where(p => p.UserId == userId).ToList();
        }

        public DailyBeltUpdate GetById(int id)
        {
            return context.dailyBeltUpdates.Find(id);
        }

        public Dictionary<string, int> GetMonthlyDailyBeltUpdateCounts(string userId)
        {
            var monthlydailybeltCounts = new Dictionary<string, int>();

            // Fetch order counts grouped by month
            var dailybeltByMonth = context.dailyBeltUpdates
                .GroupBy(o => o.Date.Month)
                .Select(g => new { Month = g.Key, TotalOrders = g.Sum(o => o.Quantity) })
                .ToList();

            // Map month numbers to month names
            var monthNames = new string[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };

            foreach (var item in dailybeltByMonth)
            {
                var monthName = monthNames[item.Month - 1]; // Month numbers are 1-indexed
                monthlydailybeltCounts.Add(monthName, item.TotalOrders);
            }

            return monthlydailybeltCounts;
        }

        public IEnumerable<DailyBeltUpdate> GetDailyBeltUpdateFromCompanyName(string companyName)
        {
            var orders = context.dailyBeltUpdates
                .Where(p => context.Users.Any(u => u.CompanyName == companyName && u.Id == p.UserId))
                .ToList();

            return orders;
        }

        public int GetTotalDailyBeltUpdateCountForProduct(string companyName, int productId)
        {
            // Retrieve user IDs for the given company name
            var userIds = context.Users
                .Where(u => u.CompanyName == companyName)
                .Select(u => u.Id)
                .ToList();

            // Calculate the total orders count for the product and company
            return context.dailyBeltUpdates
                .Where(o => userIds.Contains(o.UserId) && o.ProductId == productId)
                .Sum(o => o.Quantity);
        }

        public int GetTotalDailyBeltUpdateCountForProductByDateRange(string companyName, int productId, DateTime startDate, DateTime endDate)
        {
            // Retrieve user IDs for the given company name
            var userIds = context.Users
                .Where(u => u.CompanyName == companyName)
                .Select(u => u.Id)
                .ToList();

            // Calculate the total order quantity for the specified product, date range, and company
            return context.dailyBeltUpdates
                .Where(o => userIds.Contains(o.UserId) && o.ProductId == productId && o.Date >= startDate && o.Date <= endDate)
                .Sum(o => o.Quantity);
        }

        public int GetTotalDailyBeltUpdateCountForProductForMonth(string companyName, int productId, int Month)
        {
            var userIds = context.Users
        .Where(u => u.CompanyName == companyName)
        .Select(u => u.Id)
        .ToList();

            // Calculate the total orders count for the product and month, based on the retrieved user IDs
            return context.dailyBeltUpdates
                .Where(o => userIds.Contains(o.UserId) && o.ProductId == productId && o.Date.Month == Month)
                .Sum(o => o.Quantity);
        }

        public int GetTotalDailyBeltUpdateFor15Days(string companyName, DateTime startOf15, DateTime endOf15)
        {
            // Retrieve user IDs for the given company name
            var userIds = context.Users
                .Where(u => u.CompanyName == companyName)
                .Select(u => u.Id)
                .ToList();

            // Calculate the total orders quantity for the specified 15 days, based on the retrieved user IDs
            int totalOrdersLast15Days = context.dailyBeltUpdates
                .Where(order => userIds.Contains(order.UserId) && order.Date.Date >= startOf15.Date && order.Date.Date <= endOf15.Date)
                .Sum(order => order.Quantity);

            return totalOrdersLast15Days;

        }

        public int GetTotalDailyBeltUpdateQuantity(string userId)
        {
            var totalQuantity = context.dailyBeltUpdates
        .Where(o => o.UserId == userId)
        .Sum(o => o.Quantity);

            return totalQuantity;
        }

        public int GetTotalDailyBeltUpdateQuantityByDateRange(string companyName, DateTime startDate, DateTime endDate)
        {
            // Retrieve user IDs for the given company name
            var userIds = context.Users
                .Where(u => u.CompanyName == companyName)
                .Select(u => u.Id)
                .ToList();

            // Calculate the total order quantity for the specified date range and company
            return context.dailyBeltUpdates
                .Where(o => userIds.Contains(o.UserId) && o.Date >= startDate && o.Date <= endDate)
                .Sum(o => o.Quantity);
        }

        public int GetTotalDailyBeltUpdateQuantityForCompanyName(string companyName)
        {
            var orderQuantity = context.dailyBeltUpdates
        .Where(p => context.Users.Any(u => u.CompanyName == companyName && u.Id == p.UserId))
        .Sum(p => p.Quantity);

            return orderQuantity;
        }


        public int GetTotalDailyBeltUpdateQuantityForMOnth(string companyName, int month)
        {




            // Retrieve user IDs for the given company name
            var userIds = context.Users
                .Where(u => u.CompanyName == companyName)
                .Select(u => u.Id)
                .ToList();

            // Calculate the total order quantity for the specified month and company
            var orderQuantity = context.dailyBeltUpdates
                .Where(o => userIds.Contains(o.UserId) && o.Date.Month == month)
                .Sum(o => o.Quantity);

            return orderQuantity;
        }


        //public int GetTotalOrdersQuantityForMOnth(string userId, int Month)
        //{
        //    return context.OrderOuts
        //.Where(o => o.UserId == userId && o.Date.Month == Month)
        //.Sum(o => o.Quantity);
        //}

        public int GetTotalDailyBeltUpdateWeekly(string companyName, DateTime startOfWeek, DateTime endOfWeek)
        {
            // Retrieve user IDs for the given company name
            var userIds = context.Users
                .Where(u => u.CompanyName == companyName)
                .Select(u => u.Id)
                .ToList();

            // Calculate the total orders quantity for the specified week, based on the retrieved user IDs
            int totalOrdersWeekly = context.dailyBeltUpdates
                .Where(order => userIds.Contains(order.UserId) && order.Date.Date >= startOfWeek.Date && order.Date.Date <= endOfWeek.Date)
                .Sum(order => order.Quantity);

            return totalOrdersWeekly;
        }

        public int GetTotalDailyBeltUpdateYesterday(string companyName, DateTime yesterday)
        {

            // Retrieve user IDs for the given company name
            var userIds = context.Users
                .Where(u => u.CompanyName == companyName)
                .Select(u => u.Id)
                .ToList();

            // Calculate the total orders quantity for yesterday, based on the retrieved user IDs
            int totalOrdersYesterday = context.dailyBeltUpdates
                .Where(order => userIds.Contains(order.UserId) && order.Date.Date == yesterday.Date)
                .Sum(order => order.Quantity);

            return totalOrdersYesterday;
        }

        public IEnumerable<DailyBeltUpdate> SearchDailyBeltUpdate(string userId, string search)
        {
            return context.dailyBeltUpdates
                .Where(p => p.UserId == userId && (p.ProductName.Contains(search)))
                .ToList();
        }

        public DailyBeltUpdate Update(DailyBeltUpdate dailyBeltUpdate)
        {
            // Check if an existing entity with the same key is being tracked
            var existingEntity = context.Set<DailyBeltUpdate>().Local.FirstOrDefault(e => e.Id == dailyBeltUpdate.Id);
            if (existingEntity != null)
            {
                // Detach the existing entity to avoid conflicts
                context.Entry(existingEntity).State = EntityState.Detached;
            }

            // Attach the new entity
            context.Entry(dailyBeltUpdate).State = EntityState.Modified;

            // Save changes to the database
            context.SaveChanges();

            return dailyBeltUpdate;
        }


        private double GetProductPriceById(int productId)
        {
            var product = productRepository.GetById(productId); // Fetch the product by ID
            if (product != null)
            {
                return product.Price; // Return the product's price
            }
            return 0; // Return 0 or handle as needed if product not found
        }

        public int GetLastPendingAmount(string companyName, DateTime? startDate, DateTime? endDate)
        {

            // Check if the dates are null and handle accordingly
            if (!startDate.HasValue || !endDate.HasValue)
            {
                // Handle the null case (e.g., return 0 or throw an exception)
                return 0; // Or any other default behavior you want
            }
            // Define the date range for the previous month
            // Use the value of the nullable dates
            DateTime previousMonthStart = startDate.Value.AddMonths(-1);
            DateTime previousMonthEnd = new DateTime(previousMonthStart.Year, previousMonthStart.Month, DateTime.DaysInMonth(previousMonthStart.Year, previousMonthStart.Month));


            // Fetch all the entries for the previous month for the given company
            var previousMonthEntries = context.AccountMaster
                .Where(a => a.Date >= previousMonthStart && a.Date <= previousMonthEnd )
                .ToList();
            var dailyBeltUpdate = context.dailyBeltUpdates
                .Where(a => a.Date >= previousMonthStart && a.Date <= previousMonthEnd )
                .ToList();

            var previousMonthBeltUpdates = context.dailyBeltUpdates
    .Where(d => d.Date >= previousMonthStart && d.Date <= previousMonthEnd)
    .ToList();

            var productPrices = new Dictionary<string, decimal>();

            // Assuming dailyBeltUpdate has ProductID, ProductName, and Quantity properties
            foreach (var dailyUpdate in dailyBeltUpdate)
            {
                string productName = dailyUpdate.ProductName;

                // If the price is not already fetched, get it from the database or repository
                if (!productPrices.ContainsKey(productName))
                {
                    double price = GetProductPriceById(dailyUpdate.ProductId); // Replace with your method to fetch the price
                    productPrices[productName] = (decimal)price;
                }
            }

            var productDetails = dailyBeltUpdate
                        .GroupBy(d => d.ProductName)
                        .ToDictionary(
                            g => g.Key,
                            g => new
                            {
                                TotalQuantity = g.Sum(d => d.Quantity),
                                Price = productPrices[g.Key] // Get price for the product from the dictionary
                            });

            // Now, populate the worksheet
            decimal grandTotalAmount = 0; // To hold the total amount for all products
            foreach (var product in productDetails)
            {
                string productName = product.Key;
                int totalQuantity = product.Value.TotalQuantity;
                decimal productPrice = product.Value.Price;

                // Calculate the total amount
                decimal totalAmount = totalQuantity * productPrice;
                grandTotalAmount += totalAmount; // Add to grand total
            }

            decimal grandTotalPreviousMonth = grandTotalAmount;

            // Calculate the total paid amount for the previous month
            decimal paidAmountPreviousMonth = previousMonthEntries
                .Where(a => a.Category == "Payments" && a.Description == "Belt Payment")
                .Sum(a => a.PaidAmount);

            // Calculate the last pending amount (remaining balance)
            decimal lastPendingAmount = grandTotalPreviousMonth - paidAmountPreviousMonth;

            return (int)lastPendingAmount; // Return the result as an integer
        }
    }
}
