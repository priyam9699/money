using System.ComponentModel.DataAnnotations;

namespace Online_Management.ViewModels
{
    public class ForgotPasswordVM
    {
        [Required, EmailAddress, Display(Name ="Registered Email Address")]
        public string Email { get; set; }
        public bool EmailSent { get; set; }
        
    }
}
