using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Profiling2.Domain.Scr;
using Profiling2.Domain.Scr.Person;
using Profiling2.Web.Mvc.Validators;

namespace Profiling2.Web.Mvc.Areas.Screening.Controllers.ViewModels
{
    [IsNotPending]
    public class ConsolidateViewModel
    {
        [Key]
        public int Id { get; set; }

        public bool SendForFinalDecision { get; set; }

        // indexed by RequestPersonID
        public IDictionary<int, RecommendationViewModel> Recommendations { get; set; }

        public ConsolidateViewModel() 
        {
            this.SendForFinalDecision = false;
            this.Recommendations = new Dictionary<int, RecommendationViewModel>();
        }

        public ConsolidateViewModel(Request request, IEnumerable<ScreeningResult> srs)
        {
            this.Id = request.Id;
            this.SendForFinalDecision = false;
            this.Recommendations = new Dictionary<int, RecommendationViewModel>();
            foreach (RequestPerson rp in request.Persons)
            {
                if (!rp.Archive)
                {
                    RecommendationViewModel rvm;
                    if (rp.ScreeningRequestPersonRecommendations != null && rp.ScreeningRequestPersonRecommendations.Count > 0)
                    {
                        rvm = new RecommendationViewModel(rp.ScreeningRequestPersonRecommendations[0]);
                    }
                    else
                    {
                        // doesn't exist in database, create blank 
                        rvm = new RecommendationViewModel(rp);
                    }
                    rvm.PopulateDropDowns(srs);
                    this.Recommendations[rp.Id] = rvm;
                }
            }
        }
    }
}