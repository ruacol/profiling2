using System.ComponentModel.DataAnnotations;
using Profiling2.Domain.Prf.Persons;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels
{
    public class PersonRestrictedNoteViewModel
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        [Required]
        public string Note { get; set; }

        public PersonRestrictedNoteViewModel() { }

        public PersonRestrictedNoteViewModel(Person p)
        {
            if (p != null)
                this.PersonId = p.Id;
        }

        public PersonRestrictedNoteViewModel(PersonRestrictedNote note)
        {
            if (note != null)
            {
                this.Id = note.Id;
                if (note.Person != null)
                    this.PersonId = note.Person.Id;
                this.Note = note.Note;
            }
        }
    }
}