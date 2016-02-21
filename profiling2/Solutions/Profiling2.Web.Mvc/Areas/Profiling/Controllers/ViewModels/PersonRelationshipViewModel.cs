using System.ComponentModel.DataAnnotations;
using Profiling2.Domain.Prf.Persons;
using AutoMapper;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels
{
    public class PersonRelationshipViewModel : PeriodBaseViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "A subject person is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "A valid subject person is required.")]
        public int? SubjectPersonId { get; set; }

        [Required(ErrorMessage = "An object person is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "A valid object person is required.")]
        public int? ObjectPersonId { get; set; }

        [Required(ErrorMessage = "A relationship is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "A valid relationship is required.")]
        public int? PersonRelationshipTypeId { get; set; }

        public bool Archive { get; set; }
        public string Notes { get; set; }

        // for display purposes
        public string SubjectPersonName { get; set; }
        public string ObjectPersonName { get; set; }
        public string PersonRelationshipTypeName { get; set; }

        public PersonRelationshipViewModel() { }

        public PersonRelationshipViewModel(PersonRelationship pr)
        {
            Mapper.Map<PersonRelationship, PersonRelationshipViewModel>(pr, this);
            this.SubjectPersonId = pr.SubjectPerson.Id;
            this.ObjectPersonId = pr.ObjectPerson.Id;
            this.PersonRelationshipTypeId = pr.PersonRelationshipType.Id;

            this.SubjectPersonName = pr.SubjectPerson.Name;
            this.ObjectPersonName = pr.ObjectPerson.Name;
            this.PersonRelationshipTypeName = pr.PersonRelationshipType.ToString();
        }
    }
}