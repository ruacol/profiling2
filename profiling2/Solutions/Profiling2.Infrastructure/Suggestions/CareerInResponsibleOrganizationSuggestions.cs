using System.Collections.Generic;
using System.Linq;
using Profiling2.Domain.Extensions;
using Profiling2.Domain.Prf.Careers;
using Profiling2.Domain.Prf.Events;
using Profiling2.Domain.Prf.Organizations;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Domain.Prf.Responsibility;
using Profiling2.Domain.Prf.Suggestions;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Infrastructure.Suggestions
{
    public class CareerInResponsibleOrganizationSuggestions : BaseSuggestions
    {
        protected Organization CurrentOrganization { get; set; }

        public CareerInResponsibleOrganizationSuggestions(Person p, IList<OrganizationResponsibility> responsibilities)
            : base(p)
        {
            foreach (OrganizationResponsibility or in responsibilities)
            {
                // multiple OrganizationResponsibilities may exist in the database, which we aren't interested in
                if (!this.Suggestions.Where(x => x.EventID == or.Event.Id).Any())
                {
                    IEnumerable<Career> careers = p.Careers
                        .Where(x => !x.Archive && x.Organization == or.Organization)
                        .OrderByDescending(x => x.YearOfStart + x.YearOfEnd);  // attempt at ordering by most-complete-date-first
                    if (careers != null && careers.Any())
                    {
                        // perform suggestion based only on 1 career linked to organization
                        Career career = careers.First();
                        SuggestedFeature sf = new SuggestedFeature() { FeatureID = this.Features[AdminSuggestionFeaturePersonResponsibility.CAREER_IN_ORG_RESPONSIBLE].Id };
                        if (career.HasIncompleteDate())
                            sf.IncompleteDatePenalty = this.INCOMPLETE_DATE_PENALTY;
                        this.CurrentOrganization = or.Organization;
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
                return "(Profiled) " + this.Person.Name + " has a career in " + this.CurrentOrganization.OrgShortName + " which bears responsibility for this event"
                    + (c.HasIncompleteDate() ? " (some dates are incomplete)." : ".");
            }
            return string.Empty;
        }
    }
}
