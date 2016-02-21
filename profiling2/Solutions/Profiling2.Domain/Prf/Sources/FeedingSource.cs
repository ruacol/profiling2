using System;
using System.Collections.Generic;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Prf.Sources
{
    public class FeedingSource : Entity
    {
        public virtual string Name { get; set; }
        public virtual bool Restricted { get; set; }
        public virtual byte[] FileData { get; set; }
        public virtual DateTime FileModifiedDateTime { get; set; }
        public virtual AdminUser UploadedBy { get; set; }
        public virtual DateTime UploadDate { get; set; }
        public virtual AdminUser ApprovedBy { get; set; }
        public virtual DateTime? ApprovedDate { get; set; }
        public virtual Source Source { get; set; }
        public virtual AdminUser RejectedBy { get; set; }
        public virtual DateTime? RejectedDate { get; set; }
        public virtual string RejectedReason { get; set; }
        public virtual string UploadNotes { get; set; }
        public virtual bool IsReadOnly { get; set; }
        public virtual bool IsPublic { get; set; }
        public virtual IList<SourceAuthor> SourceAuthors { get; set; }
        public virtual IList<SourceOwningEntity> SourceOwningEntities { get; set; }

        public FeedingSource()
        {
            this.SourceAuthors = new List<SourceAuthor>();
            this.SourceOwningEntities = new List<SourceOwningEntity>();
        }
    }
}
