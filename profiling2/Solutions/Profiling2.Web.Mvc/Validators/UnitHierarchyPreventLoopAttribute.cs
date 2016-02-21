using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Practices.ServiceLocation;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels;

namespace Profiling2.Web.Mvc.Validators
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class UnitHierarchyPreventLoopAttribute : ValidationAttribute
    {
        public UnitHierarchyPreventLoopAttribute() 
        {
            this.ErrorMessage = "A unit loop exists: please change one.";
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                UnitHierarchyViewModel vm = (UnitHierarchyViewModel)value;

                // no hierarchy type should allow both units to be the same
                if (vm.ParentUnitId == vm.UnitId)
                    return new ValidationResult(this.ErrorMessage);

                // don't allow duplicates or reverse relationships if already exists
                if (ServiceLocator.Current.GetInstance<IOrganizationTasks>().GetUnitHierarchy(vm.UnitId, vm.ParentUnitId) != null
                    || ServiceLocator.Current.GetInstance<IOrganizationTasks>().GetUnitHierarchy(vm.ParentUnitId, vm.UnitId) != null)
                    return new ValidationResult(this.ErrorMessage);
            }
            return ValidationResult.Success;
        }
    }
}