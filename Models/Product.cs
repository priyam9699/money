using System.ComponentModel.DataAnnotations;

namespace FinanceManagement.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public DateTime Date { get; set; }
        [Required]
        public string ProductName { get; set; }
        [Required]
        public string SKU { get; set; }
        [Required]
        public double Price { get; set; }

        [Required]
        public int Quantity { get; set; }

        public int? DamageQuantity { get; set; }
        public int Totalquantity => Quantity - (DamageQuantity ?? 0);

        



    }
}
