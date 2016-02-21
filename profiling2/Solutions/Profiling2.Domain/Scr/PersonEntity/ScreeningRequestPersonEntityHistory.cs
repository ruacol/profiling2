using System;
using SharpArch.Domain.DomainModel;
using Profiling2.Domain.Prf;

namespace Profiling2.Domain.Scr.PersonEntity
{
    public class ScreeningRequestPersonEntityHistory : Entity
    {
        public virtual ScreeningRequestPersonEntity ScreeningRequestPersonEntity { get; set; }
        public virtual ScreeningStatus ScreeningStatus { get; set; }
        public virtual DateTime DateStatusReached { get; set; }
        public virtual string Notes { get; set; }
        public virtual bool Archive { get; set; }
        public virtual AdminUser AdminUser { get; set; }

        public override string ToString()
        {
            if (this.ScreeningStatus != null)
                return this.ScreeningStatus + " on " + this.DateStatusReached.ToShortDateString();
            else
                return this.DateStatusReached.ToShortDateString();
        }
    }
}
