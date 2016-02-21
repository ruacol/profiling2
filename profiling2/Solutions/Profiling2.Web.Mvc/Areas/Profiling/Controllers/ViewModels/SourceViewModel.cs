using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Sources;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels
{
    public class SourceViewModel
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Valid source ID is required.")]
        public int Id { get; set; }
        public string FullReference { get; set; }
        public int? FileLanguageId { get; set; }
        public string Notes { get; set; }
        public bool IsRestricted { get; set; }
        public bool IsReadOnly { get; set; }
        public bool IsPublic { get; set; }
        public bool Archive { get; set; }
        public string SourceAuthorIds { get; set; }
        public string SourceOwningEntityIds { get; set; }

        // for display purposes only
        public string FileLanguageName { get; set; }

        public SelectList Languages { get; set; }

        public SourceViewModel() { }

        public SourceViewModel(Source s)
        {
            this.Id = s.Id;
            this.FullReference = s.FullReference;
            this.Notes = s.Notes;
            this.IsRestricted = s.IsRestricted;
            this.IsReadOnly = s.IsReadOnly;
            this.IsPublic = s.IsPublic;
            this.Archive = s.Archive;
            this.SourceAuthorIds = string.Join(",", s.SourceAuthors.Select(x => x.Id));
            this.SourceOwningEntityIds = string.Join(",", s.SourceOwningEntities.Select(x => x.Id));
        }

        public void PopulateDropDowns(IEnumerable<Language> languages)
        {
            this.Languages = new SelectList(languages, "Id", "LanguageName", this.FileLanguageId);
        }
    }
}