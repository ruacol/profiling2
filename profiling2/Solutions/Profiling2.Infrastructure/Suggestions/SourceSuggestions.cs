using System.Collections.Generic;
using Profiling2.Domain.Prf.Events;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Domain.Prf.Sources;
using Profiling2.Domain.Prf.Suggestions;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Infrastructure.Suggestions
{
    public class SourceSuggestions : BaseSuggestions
    {
        public SourceSuggestions(Person p, IList<EventSource> eventSources)
            : base(p)
        {
            foreach (EventSource es in eventSources)
            {
                if (es.Source != null && es.Event != null && !es.Source.Archive)
                {
                    SuggestedFeature sf = new SuggestedFeature() { FeatureID = this.Features[AdminSuggestionFeaturePersonResponsibility.COMMON_SOURCE].Id };
                    this.AddSuggestionFeatureWithReason(sf, es.Event, es.Source);
                }
            }
        }

        protected override string ConstructSuggestionReason(Entity e)
        {
            if (e != null)
            {
                Source s = (Source)e;
                return "(Profiled) " + this.Person.Name
                    + " shares source '" + s.SourceName + "' with this event.";
            }
            return string.Empty;
        }
    }
}
