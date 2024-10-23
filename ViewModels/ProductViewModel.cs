using System.ComponentModel.DataAnnotations;

namespace FinanceManagement.ViewModels
{
    public class ProductViewModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public string ProductName { get; set; }
        [Required]
        public string SKU { get; set; }
        [Required]
        public int Price { get; set; }

        [Required]
        public int Quantity { get; set; }

        public int? DamageQuantity { get; set; }

        public int Totalquantity { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalOrdersCount { get; set; }
        public int TotalOrdersCountForMonth { get; set; }
        public int TotalReturnsCountForMonth { get; set; }
        public int TotalShipmentInCountForMonth { get; set; }
        public int TotalShipmentOutCountForMonth { get; set; }
        public int TotalStockInCountForMonth { get; set; }
        public int TotalReturnsCount { get; set; }
        public int TotalShipmentsInCount { get; set; }
        public int TotalShipmentsOutCount { get; set; }

        public int TotalProductOnHoldCount { get; set; }
        public int TotalProductOnHoldCountForMonth { get; set; }

        public int TotalAvailable { get; set; }
        public int TotalAvailableByDateRange { get; set; }
        public int TotalOrderForProductByDateRange { get; set; }
        public int TotalReturnForProductByDateRange { get; set; }
        public int TotalShipmentInForProductByDateRange { get; set; }
        public int TotalShipmentOutForProductByDateRange { get; set; }
        public int TotalProductOnHoldForProductByDateRange { get; set; }
        public int TotalOrdersLastMonth { get; set; }
        public int NumofOrders { get; set; }
        public double ReturnPercentage { get; set; }

        public int TotalStocks { get; set; }
        public int TotalStocksByDateRange { get; set; }
    }
}
