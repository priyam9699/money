using FinanceManagement.Models;
using System.ComponentModel.DataAnnotations;

namespace FinanceManagement.ViewModels
{
    public class DailyBeltUpdateVM
    {
        [Key]
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public string? SKU { get; set; }

        [Required]
        public int Quantity { get; set; }
        public IEnumerable<Product>? Products { get; set; }
        public int SelectedProductId { get; set; }


        // Navigation property for the related Product
        //[ForeignKey("ProductId")]
        //public Product Product { get; set; }
    }
}
