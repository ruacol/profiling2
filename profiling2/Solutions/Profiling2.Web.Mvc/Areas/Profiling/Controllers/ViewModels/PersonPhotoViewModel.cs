using Profiling2.Domain.Prf.Persons;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels
{
    public class PersonPhotoViewModel
    {
        public int PersonId { get; set; }
        public int PhotoId { get; set; }
        public bool Archive { get; set; }
        public string Notes { get; set; }

        // display
        public string PhotoName { get; set; }

        public PersonPhotoViewModel() { }

        public PersonPhotoViewModel(PersonPhoto pp)
        {
            if (pp != null)
            {
                if (pp.Person != null)
                {
                    this.PersonId = pp.Person.Id;
                }

                if (pp.Photo != null)
                {
                    this.PhotoId = pp.Photo.Id;
                    this.PhotoName = pp.Photo.PhotoName;
                }

                this.Archive = pp.Archive;
                this.Notes = pp.Notes;
            }
        }
    }
}