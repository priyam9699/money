using System.ComponentModel.DataAnnotations;

namespace FinanceManagement.Models
{
    public class Company
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public string FirmName { get; set; }
        [Required]
        public string Email { get; set;}

        [Required]
        public string Owner { get; set;}
    }
}
