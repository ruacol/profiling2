using System;
using System.ComponentModel.DataAnnotations;
using Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels;

namespace Profiling2.Web.Mvc.Validators
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class ApproximateBirthDateAttribute : ValidationAttribute
    {
        public ApproximateBirthDateAttribute() 
        {
            this.ErrorMessage = "Approximate birth date must be empty if any of year/month/day of birth is specified.";
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                PersonViewModel vm = (PersonViewModel)validationContext.ObjectInstance;
                if (vm.DayOfBirth > 0 || vm.MonthOfBirth > 0 || vm.YearOfBirth > 0)
                    return new ValidationResult(this.ErrorMessage);
            }
            return ValidationResult.Success;
        }
    }
}