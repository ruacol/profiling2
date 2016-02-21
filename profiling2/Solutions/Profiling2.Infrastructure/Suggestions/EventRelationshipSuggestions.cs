using System.Collections.Generic;
using Profiling2.Domain.Prf.Events;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Domain.Prf.Suggestions;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Infrastructure.Suggestions
{
    public class EventRelationshipSuggestions : BaseSuggestions
    {
        public EventRelationshipSuggestions(Person p, IEnumerable<EventRelationship> relationships)
            : base(p)
        {
            foreach (EventRelationship er in relationships)
            {
                Event other = null;
                Event responsibleEvent = null;
                if (!er.SubjectEvent.IsPersonResponsible(p))
                {
                    other = er.SubjectEvent;
                    responsibleEvent = er.ObjectEvent;
                }
                else if (!er.ObjectEvent.IsPersonResponsible(p))
                {
                    other = er.ObjectEvent;
                    responsibleEvent = er.SubjectEvent;
                }

                if (other != null && !other.Archive)
                {
                    SuggestedFeature sf = new SuggestedFeature() { FeatureID = this.Features[AdminSuggestionFeaturePersonResponsibility.RESPONSIBLE_FOR_RELATED_EVENT].Id };
                    this.AddSuggestionFeatureWithReason(sf, other, responsibleEvent);
                }
            }
        }

        protected override string ConstructSuggestionReason(Entity entity)
        {
            if (entity != null)
            {
                Event e = (Event)entity;
                return "(Profiled) " + this.Person.Name + " bears responsibility for " + e.ToString() + " which is related to this event.";
            }
            return string.Empty;
        }
    }
}
