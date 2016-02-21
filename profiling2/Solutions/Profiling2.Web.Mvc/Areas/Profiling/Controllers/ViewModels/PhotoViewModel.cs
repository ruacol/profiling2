using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Profiling2.Domain.Prf;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels
{
    public class PhotoViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "A photo name is required.")]
        [StringLength(255, ErrorMessage = "Photo name must not be longer than 255 characters.")]
        public string PhotoName { get; set; }

        [StringLength(4000, ErrorMessage = "File URL must not be longer than 4000 characters.")]
        public string FileURL { get; set; }

        public bool Archive { get; set; }
        public string Notes { get; set; }

        [Required(ErrorMessage = "A person is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "A valid person is required.")]
        public int PersonId { get; set; }

        public PhotoViewModel() { }

        public PhotoViewModel(Photo photo)
        {
            this.Id = photo.Id;
            this.PhotoName = photo.PhotoName;
            this.FileURL = photo.FileURL;
            this.Archive = photo.Archive;
            this.Notes = photo.Notes;
        }
    }
}