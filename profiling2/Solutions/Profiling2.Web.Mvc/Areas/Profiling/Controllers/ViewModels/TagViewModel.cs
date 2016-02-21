using System.ComponentModel.DataAnnotations;
using Profiling2.Domain.Prf;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels
{
    public class TagViewModel
    {
        public int Id { get; set; }
        [Required]
        public string TagName { get; set; }
        public string Notes { get; set; }
        public bool Archive { get; set; }

        public TagViewModel() { }

        public TagViewModel(Tag tag)
        {
            if (tag != null)
            {
                this.Id = tag.Id;
                this.TagName = tag.TagName;
                this.Notes = tag.Notes;
                this.Archive = tag.Archive;
            }
        }
    }
}