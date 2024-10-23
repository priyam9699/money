using System.ComponentModel.DataAnnotations;

namespace FinanceManagement.Models
{
    public class CashFlow
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string TransactionType { get; set; }

        
        public decimal Amount { get; set; }

        [Required]
        public string Category { get; set; } 

        public decimal TotalAmount { get; set; }
        
        public string? PaymentCategory { get; set; }
        public string? UpadCategory { get; set; }

        public int? AccountMasterId { get; set; }

        // Navigation property for one-to-many relationship
        public ICollection<Expenses>? Expenses { get; set; }
        public ICollection<Payment>? Payments { get; set; }
        public ICollection<Upad>? Upads { get; set; }
        public ICollection<Other>? Others { get; set; }
    }
}
