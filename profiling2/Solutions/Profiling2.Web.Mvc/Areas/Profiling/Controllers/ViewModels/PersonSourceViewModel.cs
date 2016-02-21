
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Domain.Prf.Sources;
namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels
{
    public class PersonSourceViewModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "A source is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "A valid source is required.")]
        public int? SourceId { get; set; }

        [Required(ErrorMessage = "A person is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "A valid person is required.")]
        public int? PersonId { get; set; }

        public int? ReliabilityId { get; set; }
        public string Commentary { get; set; }
        public string Notes { get; set; }

        public SelectList Reliabilities { get; set; }

        //for display purposes only
        public string PersonName { get; set; }
        public string SourceName { get; set; }
        public string ReliabilityName { get; set; }

        public PersonSourceViewModel() { }

        public PersonSourceViewModel(Person p)
        {
            if (p != null)
            {
                this.PersonId = p.Id;
                this.PersonName = p.Name;
            }
        }

        public PersonSourceViewModel(PersonSource ps)
        {
            this.Id = ps.Id;
            if (ps.Person != null)
            {
                this.PersonId = ps.Person.Id;
                this.PersonName = ps.Person.Name;
            }
            if (ps.Reliability != null)
            {
                this.ReliabilityId = ps.Reliability.Id;
                this.ReliabilityName = ps.Reliability.ToString();
            }
            this.Commentary = ps.Commentary;
            this.Notes = ps.Notes;
        }

        public void PopulateDropDowns(IEnumerable<Reliability> reliabilities)
        {
            this.Reliabilities = new SelectList(reliabilities, "Id", "ReliabilityName", this.ReliabilityId);
        }

        public void PopulateSource(SourceDTO s)
        {
            if (s != null)
            {
                this.SourceId = s.SourceID;
                this.SourceName = s.SourceName;
            }
        }
    }
}