using System.Collections.Generic;
using Profiling2.Domain.Scr.PersonRecommendation;
using Profiling2.Domain.Scr.Person;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Profiling2.Domain.Scr;

namespace Profiling2.Web.Mvc.Areas.Screening.Controllers.ViewModels
{
    public class RecommendationViewModel
    {
        [Key]
        public int Id { get; set; }
        public int RequestPersonID { get; set; }
        public int ScreeningResultID { get; set; }
        public string Commentary { get; set; }
        public int Version { get; set; }

        public SelectList ScreeningResults { get; set; }

        public RecommendationViewModel() { }

        public RecommendationViewModel(ScreeningRequestPersonRecommendation srpr)
        {
            this.Id = srpr.Id;
            this.RequestPersonID = srpr.RequestPerson.Id;
            this.ScreeningResultID = srpr.ScreeningResult.Id;
            this.Commentary = srpr.Commentary;
            this.Version = srpr.Version;
        }

        public RecommendationViewModel(RequestPerson rp)
        {
            this.RequestPersonID = rp.Id;
        }

        public void PopulateDropDowns(IEnumerable<ScreeningResult> srs)
        {
            int selectedValue = this.ScreeningResultID;
            if (selectedValue <= 0)
                foreach (ScreeningResult sr in srs)
                    if (sr.ScreeningResultName == "Pending")
                        selectedValue = sr.Id;
            this.ScreeningResults = new SelectList(srs, "Id", "ScreeningResultName", selectedValue);
        }
    }
}