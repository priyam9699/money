using System.ComponentModel.DataAnnotations;

namespace FinanceManagement.Models
{
    public class DailyBeltUpdate
    {
        [Key]
        public int Id { get; set; }

        // Other properties for OrderOut
        [Required]
        public string UserId { get; set; }

        [Required]
        public int ProductId { get; set; } // Foreign key

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string ProductSKU { get; set; }

        [Required]
        public int Quantity { get; set; }

        public string ProductName { get; set; }



    }
}
