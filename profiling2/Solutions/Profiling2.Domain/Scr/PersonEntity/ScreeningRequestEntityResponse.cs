using System;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Scr.PersonEntity
{
    public class ScreeningRequestEntityResponse : Entity
    {
        public virtual Request Request { get; set; }
        public virtual ScreeningEntity ScreeningEntity { get; set; }
        public virtual DateTime ResponseDateTime { get; set; }
        public virtual bool Archive { get; set; }
        public virtual string Notes { get; set; }
    }
}
