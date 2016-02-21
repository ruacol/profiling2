using System;
using System.Linq;
using System.Web;
using Profiling2.Domain.Prf.Sources;

namespace Profiling2.Web.Mvc.Areas.Sources.Controllers.ViewModels
{
    public class FeedingSourceViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Restricted { get; set; }
        public HttpPostedFileBase FileData { get; set; }
        public DateTime FileModifiedDateTime { get; set; }
        public string UploadedBy { get; set; }
        public DateTime UploadDate { get; set; }
        public bool Approved { get; set; }
        public bool Rejected { get; set; }
        public string RejectedReason { get; set; }
        public string UploadNotes { get; set; }
        public bool IsReadOnly { get; set; }
        public bool IsPublic { get; set; }
        public string AuthorIds { get; set; }
        public string OwnerIds { get; set; }

        public FeedingSourceViewModel()
        {
            this.Restricted = false;
            this.Approved = false;
            this.Rejected = false;
            this.IsReadOnly = false;
            this.IsPublic = false;
        }

        public FeedingSourceViewModel(FeedingSource fs)
        {
            this.Id = fs.Id;
            this.Name = fs.Name;
            this.Restricted = fs.Restricted;
            this.FileModifiedDateTime = fs.FileModifiedDateTime;
            this.UploadedBy = fs.UploadedBy.UserName;
            this.UploadDate = fs.UploadDate;
            this.Approved = fs.ApprovedBy != null;
            this.Rejected = fs.RejectedBy != null;
            this.RejectedReason = fs.RejectedReason;
            this.UploadNotes = fs.UploadNotes;
            this.IsReadOnly = fs.IsReadOnly;
            this.IsPublic = fs.IsPublic;
            if (fs.SourceAuthors != null && fs.SourceAuthors.Any())
            {
                this.AuthorIds = string.Join(",", fs.SourceAuthors.Select(x => x.Id));
            }
            if (fs.SourceOwningEntities != null && fs.SourceOwningEntities.Any())
            {
                this.OwnerIds = string.Join(",", fs.SourceOwningEntities.Select(x => x.Id));
            }
        }
    }
}