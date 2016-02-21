using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Profiling2.Domain.Scr;
using Profiling2.Domain.Scr.PersonEntity;

namespace Profiling2.Web.Mvc.Areas.Screening.Controllers.ViewModels
{
    public class ScreeningRequestPersonEntityViewModel
    {
        [Required]
        // i.e. If it it doesn't exist, this entity should be created before page is returned to user; 
        // ScreeningRequestPersonEntity.Id is a required parameter of Profiling1's ConditionalityParticipantScreeningRequestPersonEntityHistoryOther.aspx.
        public int Id { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "No person ID was received.")]
        public int RequestPersonID { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "No screening result was received.")]
        public int ScreeningResultID { get; set; }
        public string Reason { get; set; }
        public string Commentary { get; set; }
        public int Version { get; set; }

        public SelectList ScreeningResults { get; set; }

        public ScreeningRequestPersonEntityViewModel() { }

        public ScreeningRequestPersonEntityViewModel(ScreeningRequestPersonEntity srpe)
        {
            if (srpe != null)
            {
                this.Id = srpe.Id;
                if (srpe.RequestPerson != null)
                    this.RequestPersonID = srpe.RequestPerson.Id;
                if (srpe.ScreeningResult != null)
                    this.ScreeningResultID = srpe.ScreeningResult.Id;
                this.Reason = srpe.Reason;
                this.Commentary = srpe.Commentary;
                this.Version = srpe.Version;
            }
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