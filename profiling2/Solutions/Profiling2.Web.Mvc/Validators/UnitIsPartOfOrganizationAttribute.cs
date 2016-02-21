using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Practices.ServiceLocation;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Prf.Organizations;
using Profiling2.Domain.Prf.Units;
using Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels;

namespace Profiling2.Web.Mvc.Validators
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class UnitIsPartOfOrganizationAttribute : ValidationAttribute
    {
        public UnitIsPartOfOrganizationAttribute() 
        {
            this.ErrorMessage = "Unit is not part of organization.";
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                OrgResponsibilityViewModel vm = (OrgResponsibilityViewModel)value;  // could generalize this by adding an interface to the view model

                // don't allow a responsibility where the unit is not a part of the organization
                if (vm.OrganizationId.HasValue && vm.UnitId.HasValue)
                {
                    Organization o = ServiceLocator.Current.GetInstance<IOrganizationTasks>().GetOrganization(vm.OrganizationId.Value);
                    Unit u = ServiceLocator.Current.GetInstance<IOrganizationTasks>().GetUnit(vm.UnitId.Value);
                    if (!u.IsPartOf(o))
                    {
                        return new ValidationResult(this.ErrorMessage);
                    }
                }
            }
            return ValidationResult.Success;
        }
    }
}