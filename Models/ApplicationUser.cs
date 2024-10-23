using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FinanceManagement.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [DisplayName("Name")]
        public string NameOfUser { get; set; }


        [Required]
        [DisplayName("CompanyName")]
        public string CompanyName { get; set; }



        [Required]
        [DisplayName("Branch Name")]
        public int BranchCode { get; set; }

        public DateTime CreateDateTime { get; set; } = DateTime.Now;

        public string? CreatedBy { get; set; }

        public string? CreatedTer { get; set; }

        public DateTime EditDateTime { get; set; }

        public string? EditedBy { get; set; }

        public string? EditedTer { get; set; }
    }
}
