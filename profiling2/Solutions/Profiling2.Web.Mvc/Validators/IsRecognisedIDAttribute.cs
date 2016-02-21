using System;
using System.ComponentModel.DataAnnotations;
using Profiling2.Infrastructure.Util;
using Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels;

namespace Profiling2.Web.Mvc.Validators
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class IsRecognisedIDAttribute : ValidationAttribute
    {
        public IsRecognisedIDAttribute() 
        {
            this.ErrorMessage = "ID number must be in a recognized format.";  // doesn't allow for incomplete ID numbers
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                PersonViewModel vm = (PersonViewModel)validationContext.ObjectInstance;
                if (!IDNumber.IsRecognised(vm.MilitaryIDNumber))
                    return new ValidationResult(this.ErrorMessage);

            }
            return ValidationResult.Success;
        }
    }
}