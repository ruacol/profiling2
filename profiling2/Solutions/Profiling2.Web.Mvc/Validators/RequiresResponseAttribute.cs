using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Practices.ServiceLocation;
using Profiling2.Domain.Contracts.Tasks.Screenings;
using Profiling2.Domain.Scr;
using Profiling2.Web.Mvc.Areas.Screening.Controllers.ViewModels;

namespace Profiling2.Web.Mvc.Validators
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class RequiresResponseAttribute : ValidationAttribute
    {
        public RequiresResponseAttribute() 
        {
            this.ErrorMessage = "This request does not currently require a response.";
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                if (value.GetType() == typeof(RespondViewModel))
                {
                    RespondViewModel vm = (RespondViewModel)value;
                    Request r = ServiceLocator.Current.GetInstance<IRequestTasks>().Get(vm.Id);
                    if (r != null)
                        if (!r.RequiresResponse)
                            return new ValidationResult(this.ErrorMessage);
                }
            }
            return ValidationResult.Success;
        }
    }
}