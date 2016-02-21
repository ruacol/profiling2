using System;
using System.ComponentModel.DataAnnotations;
using Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels;

namespace Profiling2.Web.Mvc.Validators
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class SensibleOperationNameChangeAttribute : ValidationAttribute
    {
        public SensibleOperationNameChangeAttribute()
        {
            this.ErrorMessage = "A validation error exists: please change one of the operations.";
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                OperationNameChangeViewModel vm = (OperationNameChangeViewModel)value;

                // one of the operations must be the current one
                if (vm.CurrentOperationId != vm.OldOperationId && vm.CurrentOperationId != vm.NewOperationId)
                    return new ValidationResult("Either the former or new operation must be the current one.");

                // operations must not be the same
                if (vm.OldOperationId.HasValue && vm.OldOperationId == vm.NewOperationId)
                    return new ValidationResult("Former and new operations can not be the same.");
            }
            return ValidationResult.Success;
        }
    }
}