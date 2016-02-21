using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Documentation;

namespace Profiling2.Web.Mvc.Areas.Documents.Controllers.ViewModels
{
    public class DocumentationFileTagViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int AdminPermissionId { get; set; }

        public IEnumerable<SelectListItem> PermissionSelectItems { get; set; }

        public DocumentationFileTagViewModel() { }

        public DocumentationFileTagViewModel(DocumentationFileTag tag)
        {
            if (tag != null)
            {
                this.Id = tag.Id;
                this.Name = tag.Name;
                this.AdminPermissionId = tag.AdminPermission.Id;
            }
        }

        public void PopulateDropDowns(IEnumerable<AdminPermission> permissions)
        {
            this.PermissionSelectItems = permissions.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text);
        }
    }
}