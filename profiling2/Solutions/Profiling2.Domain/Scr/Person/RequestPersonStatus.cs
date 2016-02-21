using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Scr.Person
{
    public class RequestPersonStatus : Entity
    {
        public const string NAME_ADDED = "Added to request";
        public const string NAME_REMOVED = "Removed from request";
        public const string NAME_NOMINATED = "Nominated for PWG-Conditionality Meeting";
        public const string NAME_NOMINATION_WITHDRAWN = "Nomination withdrawn from PWG-Conditionality Meeting";

        public virtual string RequestPersonStatusName { get; set; }
        public virtual bool Archive { get; set; }
        public virtual string Notes { get; set; }

        public override string ToString()
        {
            return this.RequestPersonStatusName;
        }
    }
}
