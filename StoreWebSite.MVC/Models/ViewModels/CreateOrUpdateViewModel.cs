using StoreWebSite.DAL.Models;
using StoreWebSite.MVC.Validators;
using System;
using System.ComponentModel.DataAnnotations;

namespace StoreWebSite.MVC.Models
{
    //Viewmodel for creating or updating User form.
    public class CreateOrUpdateViewModel
    {
        public CreateOrUpdateViewModel()
        {

        }

        public CreateOrUpdateViewModel(User user)
        {
            FirstName = user.FirstName;
            LastName = user.LastName;
            UserName = user.UserName;
            Password = user.Password;
            Email = user.Email;
            BirthDay = user.BirthDay;

        }

        [Display(Name = "First name")]
        [Required(ErrorMessage = "First name must be filled")]
        [MaxLength(50, ErrorMessage = "First name can't be longer than 50 characters")]
        public string FirstName { get; set; }

        [Display(Name = "Last name")]
        [Required(ErrorMessage = "Last name must be filled")]
        [MaxLength(50, ErrorMessage = "Last name can't be longer than 50 characters")]
        public string LastName { get; set; }

        [Display(Name = "Birthday")]
        public DateTime BirthDay { get; set; }

        [Display(Name = "Email Address")]
        [Required(ErrorMessage = "Email must be filled")]
        [MaxLength(50, ErrorMessage = "Email can't be longer than 50 characters")]
        [RegularExpression(@"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z"
                            , ErrorMessage = "Input doesn't match Email pattern")]
        public string Email { get; set; }

        [Display(Name = "User name")]
        [Required(ErrorMessage = "User name must be filled")]
        [CustomValidation(typeof(UniqueUserNameValidationAttribute), "UserNameUnique")]
        [MaxLength(50, ErrorMessage = "User name can't be longer than 50 characters")]
        public string UserName { get; set; }

        
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password must be filled")]
        [MaxLength(50, ErrorMessage ="Password can't be longer than 50 characters")]
        public string Password { get; set; }

        
        [Display(Name = "Validate Password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage ="Validate Password must match password")]
        public string ValPassword { get; set; }
    }
}
