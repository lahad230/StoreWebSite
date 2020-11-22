using StoreWebSite.DAL.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace StoreWebSite.MVC.Validators
{
    //Validates Username is unique, and doesnt exist in the database.
    public class UniqueUserNameValidationAttribute
    {        
        public static ValidationResult UserNameUnique(string userName, ValidationContext pValidationContext)
        {
            var service = (IUnitOfWork)pValidationContext.GetService(typeof(IUnitOfWork));

            if (service.UserRepository.Get().Any(u => u.UserName == userName))
            {
                return new ValidationResult("User name is taken, try another");
            }
            return ValidationResult.Success;
        }
    }
}
