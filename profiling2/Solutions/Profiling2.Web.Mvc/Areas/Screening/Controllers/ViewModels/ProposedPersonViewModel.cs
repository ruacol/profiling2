using System.ComponentModel.DataAnnotations;
using Profiling2.Domain.Scr.Proposed;

namespace Profiling2.Web.Mvc.Areas.Screening.Controllers.ViewModels
{
    public class ProposedPersonViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "A request is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "A valid request is required.")]
        public int RequestId { get; set; }

        [Required(ErrorMessage = "A name is required.")]
        [StringLength(1000, ErrorMessage = "Name must not be longer than 1000 characters.")]
        public string Name { get; set; }

        [StringLength(255, ErrorMessage = "ID number must not be longer than 255 characters.")]
        public string MilitaryIDNumber { get; set; }

        public string Notes { get; set; }

        public ProposedPersonViewModel() { }

        public ProposedPersonViewModel(RequestProposedPerson rpp)
        {
            if (rpp != null)
            {
                this.Id = rpp.Id;
                this.RequestId = rpp.Request.Id;
                this.Name = rpp.PersonName;
                this.MilitaryIDNumber = rpp.MilitaryIDNumber;
                this.Notes = rpp.Notes;
            }
        }
    }
}