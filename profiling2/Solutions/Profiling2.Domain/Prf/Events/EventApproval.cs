using System;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Prf.Events
{
    /// <summary>
    /// A record of when an Event has been looked over and approved by a user.
    /// </summary>
    public class EventApproval : Entity
    {
        public virtual Event Event { get; set; }
        public virtual AdminUser AdminUser { get; set; }
        public virtual DateTime ApprovalDateTime { get; set; }
        public virtual string Notes { get; set; }
    }
}
