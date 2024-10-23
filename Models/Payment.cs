using System.ComponentModel.DataAnnotations;

namespace FinanceManagement.Models
{
    public class Payment
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
        public decimal TotalAmount { get; set; }

        [Required]
        public decimal PaidAmount { get; set; }
        public string? FirmName { get; set; }

        public string? PaymentCategory { get; set; }
        public int? CashFlowId { get; set; }  // This might be required
        public CashFlow CashFlow { get; set; }
        public int? AccountMasterId { get; set; }  // This might be required
        public AccountMaster AccountMaster { get; set; }
    }
}
