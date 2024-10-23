using System.ComponentModel.DataAnnotations;

namespace FinanceManagement.ViewModels
{
    public class ExpensesVM
    {
        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string Description { get; set; }
        [Required]
        public decimal Amount { get; set; }
    }
}
