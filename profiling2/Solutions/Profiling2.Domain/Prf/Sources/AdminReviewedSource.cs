using System;
using System.Web.Script.Serialization;
using Profiling2.Domain.Prf.Events;
using Profiling2.Domain.Prf.Persons;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Prf.Sources
{
    /// <summary>
    /// Log of user actions once a user reviews the given Source as a search result of AdminSourceSearch.
    /// </summary>
    public class AdminReviewedSource : Entity
    {
        [ScriptIgnore]
        public virtual Source Source { get; set; }
        [ScriptIgnore]
        public virtual AdminSourceSearch AdminSourceSearch { get; set; }
        public virtual DateTime ReviewedDateTime { get; set; }
        public virtual bool? IsRelevant { get; set; }
        public virtual bool WasPreviewed { get; set; }
        public virtual bool WasDownloaded { get; set; }
        public virtual bool Archive { get; set; }
        public virtual string Notes { get; set; }
        public virtual Person AttachedToProfilePerson { get; set; }
        public virtual Event AttachedToProfileEvent { get; set; }
    }
}
