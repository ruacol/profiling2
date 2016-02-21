using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Prf.Events
{
    public class EventRelationshipType : Entity
    {
        public virtual string EventRelationshipTypeName { get; set; }
        public virtual bool Archive { get; set; }
        public virtual string Notes { get; set; }

        public override string ToString()
        {
            return this.EventRelationshipTypeName;
        }
    }
}
