using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Practices.ServiceLocation;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Prf.Units;
using Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels;

namespace Profiling2.Web.Mvc.Validators
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class UniqueOperationNameAttribute : ValidationAttribute
    {
        public UniqueOperationNameAttribute() 
        {
            this.ErrorMessage = "Operation already exists with that name.";
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            OperationViewModel vm = (OperationViewModel)validationContext.ObjectInstance;

            if (!string.IsNullOrEmpty(vm.Name))
            {
                Operation existing = ServiceLocator.Current.GetInstance<IOrganizationTasks>().GetOperation(vm.Name);

                if (existing != null)
                {
                    if (vm.Id == 0 || existing.Id != vm.Id)  // for new or existing entity
                    {
                        return new ValidationResult(this.ErrorMessage, new string[] { validationContext.MemberName });
                    }
                }
            }

            return ValidationResult.Success;
        }
    }
}