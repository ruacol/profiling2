using System;
using SharpArch.Domain.DomainModel;
using Profiling2.Domain.Prf;

namespace Profiling2.Domain.Scr.Person
{
    public class RequestPersonHistory : Entity
    {
        public virtual RequestPersonStatus RequestPersonStatus { get; set; }
        public virtual DateTime DateStatusReached { get; set; }
        public virtual bool Archive { get; set; }
        public virtual RequestPerson RequestPerson { get; set; }
        public virtual string Notes { get; set; }
        public virtual AdminUser AdminUser { get; set; }
    }
}
