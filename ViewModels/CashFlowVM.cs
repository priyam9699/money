using System;
using System.ComponentModel.DataAnnotations;

namespace FinanceManagement.ViewModels
{
    public class CashFlowVM
    {
        

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string TransactionType { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string Category { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        public decimal PaidAmount { get; set; } 
        
        public string? PaymentCategory { get; set; }
        
        public string? UpadCategory { get; set; }
    }
}
