using System.Collections.Generic;
using System.Linq;
using Profiling2.Domain.Extensions;
using Profiling2.Domain.Prf.Events;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Domain.Prf.Suggestions;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Infrastructure.Suggestions
{
    public class PersonRelationshipSuggestions : BaseSuggestions
    {
        public PersonRelationshipSuggestions(Person p, IEnumerable<PersonRelationship> relationships)
            : base(p)
        {
            // one-to-one mapping between relationship and feature
            foreach (PersonRelationship relationship in relationships)
            {
                Person other = null;
                if (relationship.SubjectPerson != p)
                    other = relationship.SubjectPerson;
                else if (relationship.ObjectPerson != p)
                    other = relationship.ObjectPerson;

                if (other != null)
                {
                    // TODO source of n+1 problem
                    IEnumerable<Event> events = other.PersonResponsibilities
                        .Select(x => x.Event)
                        .Where(x => !x.Archive && relationship.HasIntersectingDateWith(x));
                    if (events.Any())
                    {
                        SuggestedFeature feature = new SuggestedFeature()
                        {
                            FeatureID = this.GetSuggestionFeatureId(relationship.PersonRelationshipType.Code)
                        };
                        if (relationship.HasIncompleteDate())
                            feature.IncompleteDatePenalty = this.INCOMPLETE_DATE_PENALTY;

                        foreach (Event e in events)
                            this.AddSuggestionFeatureWithReason(feature, e, relationship);
                    }
                }
            }
        }

        protected override string ConstructSuggestionReason(Entity e)
        {
            if (e != null)
            {
                PersonRelationship pr = (PersonRelationship)e;
                return pr.RelationshipSummary(this.Person) + " and "
                    + (pr.SubjectPerson != this.Person ? pr.SubjectPerson.Name : pr.ObjectPerson.Name) + " has responsibility for this event"
                    + (pr.HasIncompleteDate() ? " (some dates are incomplete)." : ".");
            }
            return string.Empty;
        }

        protected int GetSuggestionFeatureId(string relationshipTypeName)
        {
            int featureId = 0;
            switch (relationshipTypeName)
            {
                case PersonRelationshipType.CODE_IS_A_SUPERIOR_TO:
                    featureId = this.Features[AdminSuggestionFeaturePersonResponsibility.IS_SUPERIOR_TO].Id;
                    break;
                case PersonRelationshipType.CODE_FOUGHT_WITH:
                    featureId = this.Features[AdminSuggestionFeaturePersonResponsibility.FOUGHT_WITH].Id;
                    break;
                case PersonRelationshipType.CODE_IS_A_SUBORDINATE_OF:
                    featureId = this.Features[AdminSuggestionFeaturePersonResponsibility.IS_A_SUBORDINATE].Id;
                    break;
                case PersonRelationshipType.CODE_ACTED_TOGETHER_WITH:
                    featureId = this.Features[AdminSuggestionFeaturePersonResponsibility.ACTED_WITH].Id;
                    break;
                case PersonRelationshipType.CODE_IS_THE_DEPUTY_OF:
                    featureId = this.Features[AdminSuggestionFeaturePersonResponsibility.IS_DEPUTY_OF].Id;
                    break;
                case PersonRelationshipType.CODE_IS_THE_BODYGUARD_OF:
                    featureId = this.Features[AdminSuggestionFeaturePersonResponsibility.IS_BODYGUARD_OF].Id;
                    break;
                case PersonRelationshipType.CODE_FOUGHT_IN_THE_SAME_GROUP_AS:
                    featureId = this.Features[AdminSuggestionFeaturePersonResponsibility.FOUGHT_IN_SAME_GROUP_AS].Id;
                    break;
                case PersonRelationshipType.CODE_BELONGED_TO_THE_SAME_GROUP_AS:
                    featureId = this.Features[AdminSuggestionFeaturePersonResponsibility.BELONGED_TO_SAME_GROUP_AS].Id;
                    break;
                case PersonRelationshipType.CODE_PROVIDED_WEAPONS_AND_AMMUNITION_TO:
                    featureId = this.Features[AdminSuggestionFeaturePersonResponsibility.PROVIDED_WEAPONS_AND_AMMUNITION_TO].Id;
                    break;
            }
            return featureId;
        }
    }
}
