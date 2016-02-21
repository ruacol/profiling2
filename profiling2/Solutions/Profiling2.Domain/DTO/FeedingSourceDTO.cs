using System;
using System.Collections.Generic;
using System.Linq;

namespace Profiling2.Domain.DTO
{
    /// <summary>
    /// Used in the feeding source page to feed DataTable of manually uploaded sources.
    /// </summary>
    public class FeedingSourceDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Restricted { get; set; }
        public DateTime FileModifiedDateTime { get; set; }
        public string UploadedBy { get; set; }
        public DateTime UploadDate { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApproveDate { get; set; }
        public string RejectedBy { get; set; }
        public DateTime? RejectedDate { get; set; }
        public string RejectedReason { get; set; }
        public string UploadNotes { get; set; }

        public string Status
        {
            get
            {
                if (this.RejectedBy != null)
                {
                    return "Rejected";
                }
                else if (this.ApprovedBy != null)
                {
                    return "Approved";
                }
                else
                {
                    return "Waiting";
                }
            }
        }

        public DateTime LastUpdateDate
        {
            get
            {
                DateTime last = this.UploadDate;
                if (this.ApproveDate.HasValue && this.ApproveDate.Value > last)
                    last = this.ApproveDate.Value;
                if (this.RejectedDate.HasValue && this.RejectedDate.Value > last)
                    last = this.RejectedDate.Value;
                return last;
            }
        }

        public string LastUpdatedBy
        {
            get
            {
                string last = this.UploadedBy;
                if (!string.IsNullOrEmpty(this.ApprovedBy))
                    last = this.ApprovedBy;
                if (!string.IsNullOrEmpty(this.RejectedBy))
                    last = this.RejectedBy;
                return last;
            }
        }

        public int PersonSourceCount { get; set; }
        public int EventSourceCount { get; set; }
        public int UnitSourceCount { get; set; }
        public int OperationSourceCount { get; set; }
        public int TotalSourceCount
        {
            get
            {
                return this.PersonSourceCount + this.EventSourceCount + this.UnitSourceCount + this.OperationSourceCount;
            }
        }
        public int SourceID { get; set; }
        public bool IsReadOnly { get; set; }
        public bool IsPublic { get; set; }

        public FeedingSourceDTO() { }

        public virtual object ToJSON()
        {
            return new
                {
                    // Without this transform, Mvc.Jquery.DataTables doesn't include attributes with null values.
                    //
                    // String format "u" assumes UTC time as input; since the server is located in GMT+1, the date time conversion chain goes like this:
                    // 1. Database value stored as GMT+1;
                    // 2. ToUniversalTime() called on value here (assuming server still located in GMT+1);
                    // 3. Use 'u' string format to flag value as UTC;
                    // 4. Value passed to momentjs in the frontend to be converted into user's local timezone.
                    UploadDate = string.Format("{0:u}", this.UploadDate.ToUniversalTime()),
                    ApprovedBy = string.IsNullOrEmpty(this.ApprovedBy) ? string.Empty : this.ApprovedBy,
                    ApproveDate = this.ApproveDate.HasValue ? string.Format("{0:u}", this.ApproveDate.Value.ToUniversalTime()) : string.Empty,
                    RejectedBy = string.IsNullOrEmpty(this.RejectedBy) ? string.Empty : this.RejectedBy,
                    RejectedDate = this.RejectedDate.HasValue ? string.Format("{0:u}", this.RejectedDate.Value.ToUniversalTime()) : string.Empty,
                    RejectedReason = string.IsNullOrEmpty(this.RejectedReason) ? string.Empty : this.RejectedReason,
                    LastUpdateDate = string.Format("{0:u}", this.LastUpdateDate.ToUniversalTime()),
                    LastUpdatedBy = string.IsNullOrEmpty(this.LastUpdatedBy) ? string.Empty : this.LastUpdatedBy,
                    UploadNotes = string.IsNullOrEmpty(this.UploadNotes) ? string.Empty : this.UploadNotes
                };
        }
    }
}
