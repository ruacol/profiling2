using System;
using System.ComponentModel.DataAnnotations;
using Profiling2.Domain.Scr;
using Profiling2.Domain.Scr.PersonFinalDecision;
using Profiling2.Web.Mvc.Areas.Screening.Controllers.ViewModels;

namespace Profiling2.Web.Mvc.Validators
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class IsNotPendingAttribute : ValidationAttribute
    {
        protected const string COLOR_CODING_ERROR_MESSAGE = "One or more color codings are still 'Pending'.";
        protected const string SUPPORT_STATUS_ERROR_MESSAGE = "One or more support statuses are still 'Pending'.";

        public IsNotPendingAttribute() { }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                if (value.GetType() == typeof(RespondViewModel))
                {
                    RespondViewModel vm = (RespondViewModel)value;
                    if (vm.SubmitResponse)
                        foreach (ScreeningRequestPersonEntityViewModel srpevm in vm.Responses.Values)
                            if (srpevm.ScreeningResultID == ScreeningResult.ID_PENDING)  // TODO decouple from hard-coded id numbers
                                return new ValidationResult(COLOR_CODING_ERROR_MESSAGE);
                }
                else if (value.GetType() == typeof(ConsolidateViewModel))
                {
                    ConsolidateViewModel vm = (ConsolidateViewModel)value;
                    if (vm.SendForFinalDecision)
                        foreach (RecommendationViewModel rvm in vm.Recommendations.Values)
                            if (rvm.ScreeningResultID == ScreeningResult.ID_PENDING)
                                return new ValidationResult(COLOR_CODING_ERROR_MESSAGE);
                }
                else if (value.GetType() == typeof(FinalizeViewModel))
                {
                    FinalizeViewModel vm = (FinalizeViewModel)value;
                    if (vm.Finalize)
                        foreach (FinalDecisionViewModel fdvm in vm.FinalDecisions.Values)
                        {
                            if (fdvm.ScreeningResultID == ScreeningResult.ID_PENDING)
                                return new ValidationResult(COLOR_CODING_ERROR_MESSAGE);
                            if (fdvm.ScreeningSupportStatusID == ScreeningSupportStatus.ID_PENDING)
                                return new ValidationResult(SUPPORT_STATUS_ERROR_MESSAGE);
                        }
                }
            }
            return ValidationResult.Success;
        }
    }
}