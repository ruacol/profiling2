using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Prf.Units;
using Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels;

namespace Profiling2.Web.Mvc.Validators
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class UniqueUnitOperationAttribute : ValidationAttribute
    {
        public UniqueUnitOperationAttribute()
        {
            this.ErrorMessage = "The unit is already part of that operation.";
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                UnitOperationViewModel vm = (UnitOperationViewModel)value;

                if (vm.UnitId.HasValue && vm.OperationId.HasValue)
                {
                    Unit u = ServiceLocator.Current.GetInstance<IOrganizationTasks>().GetUnit(vm.UnitId.Value);
                    Operation o = ServiceLocator.Current.GetInstance<IOrganizationTasks>().GetOperation(vm.OperationId.Value);

                    if (u != null && o != null)
                    {
                        if (vm.Id == 0)
                        {
                            // new UnitOperation
                            if (u.UnitOperations.Where(x => x.Operation == o).Any())
                                return new ValidationResult(this.ErrorMessage);
                        }
                        else
                        {
                            // existing UnitOperation
                            if (u.UnitOperations.Where(x => x.Id != vm.Id && x.Operation == o).Any())
                                return new ValidationResult(this.ErrorMessage);
                        }
                    }
                }
            }
            return ValidationResult.Success;
        }
    }
}