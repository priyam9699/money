using System.ComponentModel.DataAnnotations;

namespace FinanceManagement.Models
{
    public class AccountMaster
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

        
        public decimal PaidAmount { get; set; }

        [Required] 
        public int CompanyId { get; set; }

        [Required]
        public string FirmName { get; set;}
        public decimal? TotalAmount { get; set; }


        [Required]
        public string Category { get; set; } // This will store the selected category

        public string? PaymentCategory { get; set; }
        public string? UpadCategory { get; set; }

        public ICollection<Expenses>? Expenses { get; set; }
        public ICollection<Payment>? Payments { get; set; }
        public ICollection<Upad>? Upads { get; set; }
        public ICollection<Other>? Others { get; set; }

    }
}
