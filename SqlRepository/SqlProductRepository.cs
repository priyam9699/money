using FinanceManagement.IRepository;
using FinanceManagement.Models;

namespace FinanceManagement.SqlRepository
{
    public class SqlProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext context;

        public SqlProductRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public Product Add(Product product)
        {
            context.Products.Add(product);
            context.SaveChanges();
            return product;
        }

        public Product Delete(int id)
        {
            var product = context.Products.Find(id);
            if (product != null)
            {
                context.Products.Remove(product);
                context.SaveChanges();
            }
            return product;
        }

        public IEnumerable<Product> FilterProducts(string userId, DateTime? startDate, DateTime? endDate)
        {
            var allProducts = context.Products.Where(p => p.UserId == userId);

            if (startDate.HasValue && endDate.HasValue)
            {
                allProducts = allProducts.Where(p => p.Date >= startDate.Value && p.Date <= endDate.Value);
            }
            else if (startDate.HasValue)
            {
                allProducts = allProducts.Where(p => p.Date >= startDate.Value);
            }
            else if (endDate.HasValue)
            {
                allProducts = allProducts.Where(p => p.Date <= endDate.Value);
            }

            return allProducts.ToList();
        }


        public IEnumerable<Product> GetAllProduct(string userId)
        {
            //return context.Products.ToList();
            return context.Products.Where(p => p.UserId == userId).ToList();
        }



        public Product GetById(int id)
        {
            return context.Products.Find(id);
        }

        public int GetProductQuantityForMonth(int productId, int month)
        {
            int totalQuantityForMonth = context.Products
                .Where(p => p.Id == productId && p.Date.Month == month)
                .Sum(p => p.Quantity);

            return totalQuantityForMonth;
        }

        public int GetProductQuantityInDateRange(string companyName, int productId, DateTime startDate, DateTime endDate)
        {
            if (string.IsNullOrEmpty(companyName))
            {
                throw new ArgumentNullException(nameof(companyName), "Company name cannot be null or empty.");
            }

            if (startDate > endDate)
            {
                throw new ArgumentException("Start date must be less than or equal to end date.");
            }

            // Retrieve user IDs for the given company name
            var userIds = context.Users
                .Where(u => u.CompanyName == companyName)
                .Select(u => u.Id)
                .ToList();

            // Retrieve products within the specified date range, for the specified company, and with the specified productId
            var products = context.Products
                .Where(p => userIds.Contains(p.UserId) && p.Id == productId && p.Date >= startDate && p.Date <= endDate)
                .ToList(); // Execute the query to fetch the products from the database

            // Calculate the total quantity of all fetched products
            int totalQuantity = products.Sum(p => p.Quantity - (p.DamageQuantity ?? 0));

            return totalQuantity;
        }


        public IEnumerable<Product> GetProductsFromCompanyName(string companyName)
        {
            // Retrieve products associated with users who have the specified company name
            var products = context.Products
                .Where(p => context.Users.Any(u => u.CompanyName == companyName && u.Id == p.UserId))
                .ToList();

            return products;
        }

        public IEnumerable<Product> SearchProducts(string userId, string search)
        {
            return context.Products
                .Where(p => p.UserId == userId && (p.ProductName.Contains(search) || p.SKU.Contains(search)))
                .ToList();
        }




        public Product Update(Product UpdateProduct)
        {
            // Find the existing product in the context
            var existingProduct = context.Products.Find(UpdateProduct.Id);

            // Check if the existing product is found
            if (existingProduct != null)
            {
                // Update the properties of the existing product
                existingProduct.Date = UpdateProduct.Date;
                existingProduct.ProductName = UpdateProduct.ProductName;
                existingProduct.SKU = UpdateProduct.SKU;
                existingProduct.Price = UpdateProduct.Price;
                existingProduct.Quantity = UpdateProduct.Quantity;

                // Save the changes to the database
                context.SaveChanges();

                // Return the updated product
                return existingProduct;
            }
            else
            {
                // If the existing product is not found, you might handle this case according to your application's logic
                // For example, you could throw an exception or return null
                throw new InvalidOperationException("Product not found.");
            }
        }


        public Product GetBySKU(string sku)
        {
            return context.Products.FirstOrDefault(p => p.SKU == sku);
        }


    }
}
