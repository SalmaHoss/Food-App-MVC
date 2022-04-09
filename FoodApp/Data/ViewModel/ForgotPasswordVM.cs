using System.ComponentModel.DataAnnotations;

namespace FoodApp.Data.ViewModel
{
    public class ForgotPasswordVM
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
