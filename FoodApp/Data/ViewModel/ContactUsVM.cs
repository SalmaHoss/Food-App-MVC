using System.ComponentModel.DataAnnotations;

namespace FoodApp.Data.ViewModel
{
    public class ContactUsVM
    {
        [Display(Name = "Full name:")]
        [Required(ErrorMessage = "Full name is required")]
        public string FullName { get; set; }

        [Display(Name = "Email address:")]
        [Required(ErrorMessage = "Email address is required")]
        public string EmailAddress { get; set; }

        [Display(Name = "Message:")]
        [Required(ErrorMessage = "Message is required")]
        public string Notes { get; set; }

    }
}
