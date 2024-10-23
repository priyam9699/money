using FinanceManagement.Models;
using System.ComponentModel.DataAnnotations;

namespace FinanceManagement.ViewModels
{
    public class AccountMasterVM
    {
        [Required]
        public DateTime Date { get; set; }

        public string? FirmName { get; set; }

        public int Id { get; set; }

        public string? Description { get; set; }

        [Required]
        public string TransactionType { get; set; }

        public decimal PaidAmount { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        public string? PaymentCategory { get; set; }

        public string? UpadCategory { get; set; }

        // List of companies to display in dropdown
        public List<Company> Companies { get; set; } = new List<Company>();

        // The selected company ID from the dropdown
        [Required(ErrorMessage = "Please select a company.")]
        public int SelectedCompanyId { get; set; }
        


        [Required]
        public string Category { get; set; }

        public string? DescriptionSelect { get; set; }
        public string? CustomDescription { get; set; }
    }
}
