using System;
using System.Web;

namespace Profiling2.Web.Mvc.Areas.Sources.Controllers.ViewModels
{
    public class MultipleUploadViewModel
    {
        public bool Restricted { get; set; }
        public HttpPostedFileBase[] FileData { get; set; }
        public DateTime[] FileModifiedDateTime { get; set; }
        public string UploadNotes { get; set; }
        public bool IsReadOnly { get; set; }
        public bool IsPublic { get; set; }
        public string AuthorIds { get; set; }
        public string OwnerIds { get; set; }

        public MultipleUploadViewModel()
        {
            this.Restricted = false;
            this.IsReadOnly = false;
            this.IsPublic = false;
        }
    }
}