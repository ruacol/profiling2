using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Profiling2.Domain.Prf.Documentation;

namespace Profiling2.Web.Mvc.Areas.Documents.Controllers.ViewModels
{
    public class DocumentationFileViewModel
    {
        public int Id { get; set; }
        public HttpPostedFileBase FileData { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int TagId { get; set; }

        public IEnumerable<SelectListItem> TagSelectItems { get; set; }

        public DocumentationFileViewModel() { }

        public void PopulateDropDowns(IEnumerable<DocumentationFileTag> tags)
        {
            this.TagSelectItems = tags.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text);
        }
    }
}