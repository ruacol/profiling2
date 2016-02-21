using System;
using System.ComponentModel.DataAnnotations;
using Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels;

namespace Profiling2.Web.Mvc.Validators
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ValidDatePeriodAttribute : ValidationAttribute
    {
        public ValidDatePeriodAttribute() { }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                PeriodBaseViewModel vm = (PeriodBaseViewModel)value;

                if (vm.YearOfStart > 0 && vm.YearOfEnd > 0)
                    if (vm.YearOfStart > vm.YearOfEnd)
                        return new ValidationResult("Start year must be before end year.");

                if (vm.YearOfStart == vm.YearOfEnd)
                    if (vm.MonthOfStart > 0 && vm.MonthOfEnd > 0 && vm.MonthOfStart > vm.MonthOfEnd)
                        return new ValidationResult("Start month must be before end month.");

                if (vm.YearOfStart == vm.YearOfEnd)
                    if (vm.MonthOfStart == vm.MonthOfEnd)
                        if (vm.DayOfStart > 0 && vm.DayOfEnd > 0 && vm.DayOfStart > vm.DayOfEnd)
                            return new ValidationResult("Start day must be before end day.");
            }
            return ValidationResult.Success;
        }
    }
}