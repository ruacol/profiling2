using System.Collections.Generic;
using System.Linq;
using Profiling2.Domain.Scr;
using Profiling2.Domain.Scr.Person;
using Profiling2.Domain.Scr.PersonFinalDecision;
using Profiling2.Web.Mvc.Validators;

namespace Profiling2.Web.Mvc.Areas.Screening.Controllers.ViewModels
{
    [IsNotPending]
    public class FinalizeViewModel
    {
        public int Id { get; set; }

        public bool Finalize { get; set; }

        // indexed by RequestPersonID
        public IDictionary<int, FinalDecisionViewModel> FinalDecisions { get; set; }

        public FinalizeViewModel()
        {
            this.Finalize = false;
            this.FinalDecisions = new Dictionary<int, FinalDecisionViewModel>();
        }

        public FinalizeViewModel(Request request, IEnumerable<ScreeningResult> screeningResults, IEnumerable<ScreeningSupportStatus> screeningSupportStatuses)
        {
            this.Finalize = false;
            this.FinalDecisions = new Dictionary<int, FinalDecisionViewModel>();
            if (request != null)
            {
                this.Id = request.Id;
                foreach (RequestPerson rp in request.Persons.Where(x => !x.Archive))
                {
                    FinalDecisionViewModel vm;
                    if (rp.ScreeningRequestPersonFinalDecisions != null && rp.ScreeningRequestPersonFinalDecisions.Count > 0)
                    {
                        vm = new FinalDecisionViewModel(rp.ScreeningRequestPersonFinalDecisions[0]);
                    }
                    else
                    {
                        // doesn't exist in database, create blank 
                        vm = new FinalDecisionViewModel(rp);
                    }
                    vm.PopulateDropDowns(screeningResults, screeningSupportStatuses);
                    this.FinalDecisions[rp.Id] = vm;
                }
            }
        }
    }
}