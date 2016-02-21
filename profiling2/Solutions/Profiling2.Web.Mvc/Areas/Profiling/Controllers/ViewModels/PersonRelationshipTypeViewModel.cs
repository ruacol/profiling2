using System.ComponentModel.DataAnnotations;
using Profiling2.Domain.Prf.Persons;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels
{
    public class PersonRelationshipTypeViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "A name is required for this relationship type.")]
        [StringLength(255, ErrorMessage = "Name must not be longer than 255 characters.")]
        public string PersonRelationshipTypeName { get; set; }
        public bool IsCommutative { get; set; }
        public bool Archive { get; set; }
        public string Notes { get; set; }

        // display
        public int NumRelationships { get; set; }

        public PersonRelationshipTypeViewModel() { }

        public PersonRelationshipTypeViewModel(PersonRelationshipType t)
        {
            if (t != null)
            {
                this.Id = t.Id;
                this.PersonRelationshipTypeName = t.PersonRelationshipTypeName;
                this.IsCommutative = t.IsCommutative;
                this.Archive = t.Archive;
                this.Notes = t.Notes;
                this.NumRelationships = t.PersonRelationships.Count;
            }
        }
    }
}