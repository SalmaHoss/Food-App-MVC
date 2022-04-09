using System.ComponentModel.DataAnnotations;

namespace FoodApp.Data.ViewModel
{
    public class RegisterVM
    {
        [Display(Name = "Full name")]
        [Required(ErrorMessage = "Full name is required")]
        public string FullName { get; set; }

        [Display(Name = "Email address")]
        [Required(ErrorMessage = "Email address is required")]
        public string EmailAddress { get; set; }

        [Required]
        [StringLength(10, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 8)]
        [RegularExpression(@"^.*(?=.{8,})(?=.*[A-Z])(?=.*[@#$%^&+=]).*$",
         ErrorMessage = "At least one speacial charchter and one Uppercase.")]

        [DataType(DataType.Password)]

        public string Password { get; set; }

        [Display(Name = "Confirm password")]
        [Required(ErrorMessage = "Confirm password is required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }
}
