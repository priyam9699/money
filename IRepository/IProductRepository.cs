using FinanceManagement.Models;

namespace FinanceManagement.IRepository
{
    public interface IProductRepository
    {
        Product GetById(int id);

        IEnumerable<Product> GetAllProduct(string userId);

        Product Add(Product Product);
        Product Update(Product UpdateProduct);
        Product Delete(int id);
        IEnumerable<Product> SearchProducts(string userId, string search);
        IEnumerable<Product> FilterProducts(string userId, DateTime? startDate, DateTime? endDate);
        int GetProductQuantityInDateRange(string companyName, int productId, DateTime startDate, DateTime endDate);
        int GetProductQuantityForMonth(int productId, int Month);

        Product GetBySKU(string sku);

        IEnumerable<Product> GetProductsFromCompanyName(string companyName);
    }
}
