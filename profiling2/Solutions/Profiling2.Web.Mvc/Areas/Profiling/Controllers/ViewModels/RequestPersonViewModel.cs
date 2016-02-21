using System.Collections.Generic;
using Profiling2.Domain.Scr;
using Profiling2.Domain.Scr.Person;
using Profiling2.Domain.Scr.PersonEntity;
using Profiling2.Domain.Scr.PersonFinalDecision;
using Profiling2.Domain.Scr.PersonRecommendation;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels
{
    public class RequestPersonViewModel
    {
        public int Id { get; set; }
        public int RequestId { get; set; }
        public string Notes { get; set; }

        // for display purposes only
        public string RequestName { get; set; }
        public IList<ScreeningResultViewModel> EntityResults { get; set; }
        public ScreeningResultViewModel RecommendationResult { get; set; }
        public ScreeningResultViewModel FinalResult { get; set; }
        public string FinalSupportStatus { get; set; }

        public RequestPersonViewModel() { }

        public RequestPersonViewModel(RequestPerson rp)
        {
            this.Id = rp.Id;
            this.Notes = rp.Notes;
            if (rp.Request != null)
            {
                this.RequestId = rp.Request.Id;
                this.RequestName = rp.Request.Headline;
            }

            this.EntityResults = new List<ScreeningResultViewModel>();
            string[] entities = ScreeningEntity.GetNames(rp.Request.GetCreatedDate());
            foreach (string entity in entities)
            {
                ScreeningRequestPersonEntity srpe = rp.GetScreeningRequestPersonEntity(entity);
                if (srpe != null)
                {
                    this.EntityResults.Add(new ScreeningResultViewModel()
                    {
                        Name = srpe.ScreeningEntity.ToString(),
                        Result = srpe.ScreeningResult.ToString(),
                        Reason = srpe.Reason,
                        Commentary = srpe.Commentary,
                        Date = srpe.MostRecentHistory.DateStatusReached
                    });
                }
                else
                {
                    this.EntityResults.Add(new ScreeningResultViewModel()
                    {
                        Name = entity
                    });
                }
            }

            ScreeningRequestPersonRecommendation srpr = rp.GetScreeningRequestPersonRecommendation();
            if (srpr != null)
                this.RecommendationResult = new ScreeningResultViewModel()
                {
                    Name = "Recommended",
                    Result = srpr.ScreeningResult.ToString(),
                    Commentary = srpr.Commentary,
                    Date = srpr.MostRecentHistory.DateStatusReached
                };

            ScreeningRequestPersonFinalDecision srpfd = rp.GetScreeningRequestPersonFinalDecision();
            if (srpfd != null)
            {
                this.FinalResult = new ScreeningResultViewModel()
                {
                    Name = "Final Decision",
                    Result = srpfd.ScreeningResult.ToString(),
                    Commentary = srpfd.Commentary,
                    Date = srpfd.MostRecentHistory.DateStatusReached
                };
                this.FinalSupportStatus = srpfd.ScreeningSupportStatus.ToString();
            }
        }
    }
}