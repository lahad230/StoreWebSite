using System.ComponentModel.DataAnnotations;

namespace StoreWebSite.MVC.Validators
{
    public class DecimalValidationAttribute : ValidationAttribute
    {
        //Validates if input is decimal and above 0.1
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!(value.GetType() == typeof(decimal)))
            {
                return new ValidationResult("Price must be above 0.1");
            }

            if (!((decimal)value > (decimal)0.1))
            {
                return new ValidationResult("Price must be above 0.1");
            }

            return ValidationResult.Success;
        }

    }
}
