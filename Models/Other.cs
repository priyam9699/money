using System.ComponentModel.DataAnnotations;

namespace FinanceManagement.Models
{
    public class Other
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
        public decimal Amount { get; set; }

        // Foreign key relationship
        public int? CashFlowId { get; set; }
        public CashFlow CashFlow { get; set; }

        public string? FirmName { get; set; }

        public int? AccountMasterId { get; set; }  // This might be required
        public AccountMaster AccountMaster { get; set; }
    }
}
