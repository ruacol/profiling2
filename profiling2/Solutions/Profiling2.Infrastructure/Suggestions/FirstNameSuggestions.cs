using System.Collections.Generic;
using Profiling2.Domain.Prf.Events;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Domain.Prf.Suggestions;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Infrastructure.Suggestions
{
    public class FirstNameSuggestions : BaseSuggestions
    {
        public FirstNameSuggestions(Person p, IList<Event> events)
            : base(p)
        {
            foreach (Event e in events)
            {
                SuggestedFeature sf = new SuggestedFeature() { FeatureID = this.Features[AdminSuggestionFeaturePersonResponsibility.FIRST_NAME_APPEARS].Id };
                this.AddSuggestionFeatureWithReason(sf, e, null);
            }
        }

        protected override string ConstructSuggestionReason(Entity entity)
        {
            return "(Profiled) " + this.Person.Name + " has a first name which appears in this event narrative.";
        }
    }
}
