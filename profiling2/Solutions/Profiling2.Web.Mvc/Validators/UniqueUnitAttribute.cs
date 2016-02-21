using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Practices.ServiceLocation;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Extensions;
using Profiling2.Domain.Prf.Units;
using Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels;

namespace Profiling2.Web.Mvc.Validators
{
    /// <summary>
    /// A unique unit is defined not only by its name attribute, but its start and end dates; a unit may exist with the same name, but in different time periods.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class UniqueUnitAttribute : ValidationAttribute
    {
        public UniqueUnitAttribute() { }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            UnitViewModel vm = (UnitViewModel)validationContext.ObjectInstance;

            IOrganizationTasks orgTasks = ServiceLocator.Current.GetInstance<IOrganizationTasks>();
            Unit existing = orgTasks.GetUnit(vm.UnitName);
            if (vm.Id > 0)  // editing a unit
            {
                Unit thisUnit = orgTasks.GetUnit(vm.Id);
                if (thisUnit != null && existing != null && thisUnit.Id != existing.Id)
                {
                    return CheckDates(vm, existing);
                }
            }
            else  // creating a new unit
            {
                return CheckDates(vm, existing);
            }
            
            return ValidationResult.Success;
        }

        /// <summary>
        /// Pre-condition: only check dates when a unit with the same name exists.
        /// </summary>
        private ValidationResult CheckDates(UnitViewModel vm, Unit existing)
        {
            if (existing != null)
            {
                if (!vm.HasStartDate() && !vm.HasEndDate())
                {
                    return new ValidationResult("Unit name already exists; either change the name or add dates.", new string[] { "UnitName" });
                }

                if (vm.HasStartDate() && vm.HasEndDate() && existing.HasStartDate() && existing.HasEndDate()
                    && vm.GetStartDateTime() < existing.GetEndDateTime() && existing.GetStartDateTime() < vm.GetEndDateTime())
                {
                    return new ValidationResult("A unit already exists with the same name, as well as overlapping dates.", new string[] { "UnitName" });
                }

                if (vm.HasStartDate() && vm.HasEndDate())
                {
                    if (existing.HasStartDate() && existing.GetStartDateTime() > vm.GetStartDateTime() && existing.GetStartDateTime() < vm.GetEndDateTime())
                    {
                        return new ValidationResult("A unit already exists with the same name and an overlapping start date.", new string[] { "UnitName" });
                    }
                    if (existing.HasEndDate() && existing.GetEndDateTime() > vm.GetStartDateTime() && existing.GetEndDateTime() < vm.GetEndDateTime())
                    {
                        return new ValidationResult("A unit already exists with the same name and an overlapping end date.", new string[] { "UnitName" });
                    }
                }

                if (existing.HasStartDate() && existing.HasEndDate())
                {
                    if (vm.HasStartDate() && vm.GetStartDateTime() > existing.GetStartDateTime() && vm.GetStartDateTime() < existing.GetEndDateTime())
                    {
                        return new ValidationResult("A unit already exists with the same name and an overlapping start date.", new string[] { "UnitName" });
                    }
                    if (vm.HasEndDate() && vm.GetEndDateTime() > existing.GetStartDateTime() && vm.GetEndDateTime() < existing.GetEndDateTime())
                    {
                        return new ValidationResult("A unit already exists with the same name and an overlapping end date.", new string[] { "UnitName" });
                    }
                }

                if (string.IsNullOrEmpty(vm.Notes))
                {
                    return new ValidationResult("Unit with the same name exists; justification is necessary in the Notes.", new string[] { "Notes" });
                }
            }

            return ValidationResult.Success;
        }
    }
}