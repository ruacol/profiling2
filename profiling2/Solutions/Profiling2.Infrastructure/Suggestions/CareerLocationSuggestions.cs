using System.Collections.Generic;
using System.Linq;
using Profiling2.Domain.Extensions;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Careers;
using Profiling2.Domain.Prf.Events;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Domain.Prf.Suggestions;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Infrastructure.Suggestions
{
    public class CareerLocationSuggestions : BaseSuggestions
    {
        public CareerLocationSuggestions(Person p, IList<Event> events)
            : base(p)
        {
            foreach (Event e in events)
            {
                IEnumerable<Career> careers = p.Careers
                    .Where(x => x.Location != null && x.Location.IsNotUnknown())
                    .Where(x => x.Location == e.Location || x.Location.Territory == e.Location.Territory)
                    .Where(x => x.HasIntersectingDateWith(e));

                if (careers != null && careers.Any())
                {
                    Career career = careers.First();
                    SuggestedFeature sf = new SuggestedFeature() { FeatureID = this.Features[AdminSuggestionFeaturePersonResponsibility.CAREER_IN_LOCATION].Id };
                    if (career.HasIncompleteDate())
                        sf.IncompleteDatePenalty = this.INCOMPLETE_DATE_PENALTY;
                    this.AddSuggestionFeatureWithReason(sf, e, career);
                }
            }
        }

        protected override string ConstructSuggestionReason(Entity e)
        {
            if (e != null)
            {
                Career c = (Career)e;
                return "(Profiled) " + this.Person.Name + " has a career in " + c.Location.ToString() + " where this event occurred"
                    + (c.HasIncompleteDate() ? " (some dates are incomplete)." : ".");
            }
            return string.Empty;
        }
    }
}
