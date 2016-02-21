using Profiling2.Domain.Prf.Persons;
using System.ComponentModel.DataAnnotations;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels
{
    public class PersonAliasViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "A person is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "A valid person is required.")]
        public int PersonId { get; set; }

        [StringLength(500, ErrorMessage = "Last name must not be longer than 500 characters.")]
        public string LastName { get; set; }

        [StringLength(500, ErrorMessage = "First name must not be longer than 500 characters.")]
        public string FirstName { get; set; }

        public bool Archive { get; set; }

        public string Notes { get; set; }

        public PersonAliasViewModel() { }

        public PersonAliasViewModel(PersonAlias pa)
        {
            this.Id = pa.Id;
            this.PersonId = pa.Person.Id;
            this.LastName = pa.LastName;
            this.FirstName = pa.FirstName;
            this.Archive = pa.Archive;
            this.Notes = pa.Notes;
        }
    }
}