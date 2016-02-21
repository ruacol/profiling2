using System.ComponentModel.DataAnnotations;
using Profiling2.Domain.Prf.Sources;

namespace Profiling2.Web.Mvc.Areas.Sources.Controllers.ViewModels
{
    public class SourceOwningEntityViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "A name is required.")]
        public string Name { get; set; }

        public string SourcePathPrefix { get; set; }

        public SourceOwningEntityViewModel() { }

        public SourceOwningEntityViewModel(SourceOwningEntity soe)
        {
            if (soe != null)
            {
                this.Id = soe.Id;
                this.Name = soe.Name;
                this.SourcePathPrefix = soe.SourcePathPrefix;
            }
        }
    }
}