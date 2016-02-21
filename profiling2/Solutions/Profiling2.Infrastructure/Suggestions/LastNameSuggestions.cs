using System.Collections.Generic;
using Profiling2.Domain.Prf.Events;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Domain.Prf.Suggestions;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Infrastructure.Suggestions
{
    public class LastNameSuggestions : BaseSuggestions
    {
        public LastNameSuggestions(Person p, IList<Event> events)
            : base(p)
        {
            foreach (Event e in events)
            {
                SuggestedFeature sf = new SuggestedFeature() { FeatureID = this.Features[AdminSuggestionFeaturePersonResponsibility.LAST_NAME_APPEARS].Id };
                this.AddSuggestionFeatureWithReason(sf, e, null);
            }
        }

        protected override string ConstructSuggestionReason(Entity entity)
        {
            return "(Profiled) " + this.Person.Name + " has a last name which appears in this event narrative.";
        }
    }
}
