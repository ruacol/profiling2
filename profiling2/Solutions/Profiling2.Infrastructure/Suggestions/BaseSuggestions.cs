using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Prf.Events;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Domain.Prf.Suggestions;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Infrastructure.Suggestions
{
    public abstract class BaseSuggestions
    {
        protected double INCOMPLETE_DATE_PENALTY = 0.5;
        protected Person Person;
        public IList<SuggestedFeatureWithReason> Suggestions;
        protected IDictionary<string, AdminSuggestionFeaturePersonResponsibility> Features;

        public BaseSuggestions(Person p)
        {
            this.Features = ServiceLocator.Current.GetInstance<ISuggestionTasks>().GetAdminSuggestionFeaturePersonResponsibilities();
            this.Person = p;
            this.Suggestions = new List<SuggestedFeatureWithReason>();
        }

        protected abstract string ConstructSuggestionReason(Entity e);

        protected void AddSuggestionFeatureWithReason(SuggestedFeature feature, Event suggestedEvent, Entity entity)
        {
            SuggestedFeatureWithReason s = new SuggestedFeatureWithReason()
            {
                SuggestedFeature = feature,
                EventID = suggestedEvent.Id,
                EventHeadline = suggestedEvent.Headline,
                Reason = this.ConstructSuggestionReason(entity)
            };

            this.Suggestions.Add(s);
        }
    }
}
