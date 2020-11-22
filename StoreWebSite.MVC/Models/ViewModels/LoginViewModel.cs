using System.ComponentModel.DataAnnotations;

namespace StoreWebSite.MVC.Models
{
    //Viewmodel for login form.
    public class LoginViewModel
    {

        [Display(Name = "User name")]
        [Required(ErrorMessage = "User name must be filled")]
        public string UserName { get; set; }

        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password must be filled")]
        public string Password { get; set; }
    }
}
