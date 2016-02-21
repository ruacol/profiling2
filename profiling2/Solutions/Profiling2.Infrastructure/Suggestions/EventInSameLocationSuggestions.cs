using System.Collections.Generic;
using Profiling2.Domain.Prf.Events;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Domain.Prf.Suggestions;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Infrastructure.Suggestions
{
    public class EventInSameLocationSuggestions : BaseSuggestions
    {
        public EventInSameLocationSuggestions(Person p, IList<Event> events)
            : base(p)
        {
            foreach (Event e in events)
            {
                SuggestedFeature sf = new SuggestedFeature() { FeatureID = this.Features[AdminSuggestionFeaturePersonResponsibility.RESPONSIBILITY_IN_LOCATION].Id };
                this.AddSuggestionFeatureWithReason(sf, e, e);
            }
        }

        protected override string ConstructSuggestionReason(Entity e)
        {
            if (e != null)
            {
                Event ev = (Event)e;
                return "(Profiled) " + this.Person.Name + " bears responsibility for an event occurring in " + ev.Location.ToString() + " where this event occurred.";
            }
            return string.Empty;
        }
    }
}
