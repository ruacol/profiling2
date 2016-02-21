using System.Collections.Generic;
using System.Web.Mvc;
using Profiling2.Domain.Scr;
using Profiling2.Domain.Scr.Person;
using Profiling2.Domain.Scr.PersonFinalDecision;

namespace Profiling2.Web.Mvc.Areas.Screening.Controllers.ViewModels
{
    public class FinalDecisionViewModel
    {
        public int Id { get; set; }
        public int RequestPersonID { get; set; }
        public int ScreeningResultID { get; set; }
        public int ScreeningSupportStatusID { get; set; }
        public string Commentary { get; set; }
        public int Version { get; set; }

        public SelectList ScreeningResults { get; set; }
        public SelectList ScreeningSupportStatuses { get; set; }

        public FinalDecisionViewModel() { }

        public FinalDecisionViewModel(ScreeningRequestPersonFinalDecision srpfd)
        {
            if (srpfd != null)
            {
                this.Id = srpfd.Id;
                this.RequestPersonID = srpfd.RequestPerson.Id;
                this.ScreeningResultID = srpfd.ScreeningResult.Id;
                this.ScreeningSupportStatusID = srpfd.ScreeningSupportStatus.Id;
                this.Commentary = srpfd.Commentary;
                this.Version = srpfd.Version;
            }
        }

        public FinalDecisionViewModel(RequestPerson rp)
        {
            if (rp != null)
                this.RequestPersonID = rp.Id;
        }

        public void PopulateDropDowns(IEnumerable<ScreeningResult> screeningResults, IEnumerable<ScreeningSupportStatus> screeningSupportStatuses)
        {
            int selectedResultValue = this.ScreeningResultID;
            if (selectedResultValue <= 0)  // if no selected value, default to Pending
                foreach (ScreeningResult sr in screeningResults)
                    if (sr.ScreeningResultName == "Pending")
                        selectedResultValue = sr.Id;
            this.ScreeningResults = new SelectList(screeningResults, "Id", "ScreeningResultName", selectedResultValue);

            int selectedStatusValue = this.ScreeningSupportStatusID;
            if (selectedStatusValue <= 0)  // if no selected value, default to Pending
                foreach (ScreeningSupportStatus sss in screeningSupportStatuses)
                    if (sss.ScreeningSupportStatusName == "Pending")
                        selectedStatusValue = sss.Id;
            this.ScreeningSupportStatuses = new SelectList(screeningSupportStatuses, "Id", "ScreeningSupportStatusName", selectedStatusValue);
        }
    }
}