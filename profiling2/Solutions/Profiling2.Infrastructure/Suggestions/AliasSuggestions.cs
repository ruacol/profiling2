using System.Collections.Generic;
using Profiling2.Domain.Prf.Events;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Domain.Prf.Suggestions;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Infrastructure.Suggestions
{
    public class AliasSuggestions : BaseSuggestions
    {
        public AliasSuggestions(Person p, IList<Event> events)
            : base(p)
        {
            foreach (Event e in events)
            {
                SuggestedFeature sf = new SuggestedFeature() { FeatureID = this.Features[AdminSuggestionFeaturePersonResponsibility.ALIAS_APPEARS].Id };
                this.AddSuggestionFeatureWithReason(sf, e, null);
            }
        }

        protected override string ConstructSuggestionReason(Entity entity)
        {
            return "(Profiled) " + this.Person.Name + " has an alias which appears in this event narrative.";
        }
    }
}
