using System;
using SharpArch.Domain.DomainModel;
using Profiling2.Domain.Prf;

namespace Profiling2.Domain.Scr.Proposed
{
    public class RequestProposedPersonHistory : Entity
    {
        public virtual RequestProposedPersonStatus RequestProposedPersonStatus { get; set; }
        public virtual DateTime DateStatusReached { get; set; }
        public virtual bool Archive { get; set; }
        public virtual RequestProposedPerson RequestProposedPerson { get; set; }
        public virtual string Notes { get; set; }
        public virtual AdminUser AdminUser { get; set; }
    }
}
