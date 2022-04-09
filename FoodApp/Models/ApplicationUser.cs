using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FoodApp.Models
{
    /* *here user inhirt from identityuser so we can add more fields 
    /on identityUser base class
    so our user will have properties that exsit in IdentityUser
    like ID,username,email,... and also fullName
    */
    
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "Full Name")]
        public string FullName { get; set; }
    }
}
