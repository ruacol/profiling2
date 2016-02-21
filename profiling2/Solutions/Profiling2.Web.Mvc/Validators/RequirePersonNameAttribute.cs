using System;
using System.ComponentModel.DataAnnotations;
using Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels;

namespace Profiling2.Web.Mvc.Validators
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class RequirePersonNameAttribute : ValidationAttribute
    {
        public RequirePersonNameAttribute() 
        {
            this.ErrorMessage = "A first or last name is required.";
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            PersonViewModel vm = (PersonViewModel)validationContext.ObjectInstance;
            if (string.IsNullOrEmpty(vm.FirstName) && string.IsNullOrEmpty(vm.LastName))
                return new ValidationResult(this.ErrorMessage, new string[] { validationContext.MemberName });
            return ValidationResult.Success;
        }
    }
}