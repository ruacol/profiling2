using System.Collections.Generic;
using System.Linq;
using Profiling2.Domain.Extensions;
using Profiling2.Domain.Prf.Careers;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Domain.Prf.Responsibility;
using Profiling2.Domain.Prf.Suggestions;
using Profiling2.Domain.Prf.Units;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Infrastructure.Suggestions
{
    public class CareerInResponsibleUnitSuggestions : BaseSuggestions
    {
        protected Unit CurrentUnit { get; set; }

        public CareerInResponsibleUnitSuggestions(Person p, IList<OrganizationResponsibility> responsibilities)
            : base(p)
        {
            foreach (OrganizationResponsibility or in responsibilities)
            {
                // multiple OrganizationResponsibilities may exist in the database, we only need one
                if (!this.Suggestions.Where(x => x.EventID == or.Event.Id).Any())
                {
                    IEnumerable<Career> careers = p.Careers
                        .Where(x => !x.Archive && x.Unit == or.Unit)
                        .OrderByDescending(x => x.YearOfStart + x.YearOfEnd);  // attempt at ordering by most-complete-date-first
                    if (careers != null && careers.Any())
                    {
                        // perform suggestion based only on 1 career linked to unit
                        Career career = careers.First();
                        SuggestedFeature sf = new SuggestedFeature() { FeatureID = this.Features[AdminSuggestionFeaturePersonResponsibility.CAREER_IN_UNIT_RESPONSIBLE].Id };
                        if (career.HasIncompleteDate())
                            sf.IncompleteDatePenalty = this.INCOMPLETE_DATE_PENALTY;
                        this.CurrentUnit = or.Unit;
                        this.AddSuggestionFeatureWithReason(sf, or.Event, career);
                    }
                }
            }
        }

        protected override string ConstructSuggestionReason(Entity e)
        {
            if (e != null)
            {
                Career c = (Career)e;
                return "(Profiled) " + this.Person.Name + " has a career in " + this.CurrentUnit.UnitName + " which bears responsibility for this event"
                    + (c.HasIncompleteDate() ? " (some dates are incomplete)." : ".");
            }
            return string.Empty;
        }
    }
}
