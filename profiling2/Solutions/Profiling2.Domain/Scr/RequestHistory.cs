using System;
using SharpArch.Domain.DomainModel;
using Profiling2.Domain.Prf;

namespace Profiling2.Domain.Scr
{
    public class RequestHistory : Entity
    {
        public virtual RequestStatus RequestStatus { get; set; }
        public virtual DateTime DateStatusReached { get; set; }
        public virtual Boolean Archive { get; set; }
        public virtual Request Request { get; set; }
        public virtual string Notes { get; set; }
        public virtual AdminUser AdminUser { get; set; }

        public override string ToString()
        {
            return this.RequestStatus + " on " + string.Format("{0:yyyy-MM-dd HH:mm}", this.DateStatusReached) + " by " + this.AdminUser;
        }
    }
}
