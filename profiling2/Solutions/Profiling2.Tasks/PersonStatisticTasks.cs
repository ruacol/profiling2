using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using Profiling2.Domain.Contracts.Queries.Audit;
using Profiling2.Domain.Contracts.Queries.Stats;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.DTO;
using Profiling2.Domain.Prf.Careers;
using Profiling2.Domain.Prf.Persons;

namespace Profiling2.Tasks
{
    public class PersonStatisticTasks : IPersonStatisticTasks
    {
        protected readonly IPersonStatisticsQuery personStatisticsQuery;
        protected readonly ICountsQuery countsQuery;
        protected readonly IEventRevisionsQuery eventRevisionsQuery;
        protected readonly IPersonAuditable<Career> auditCareerQuery;
        protected readonly IPersonTasks personTasks;
        protected readonly IAuditTasks auditTasks;

        public PersonStatisticTasks(IPersonStatisticsQuery personStatisticsQuery,
            ICountsQuery countsQuery,
            IEventRevisionsQuery eventRevisionsQuery,
            IPersonAuditable<Career> auditCareerQuery,
            IPersonTasks personTasks,
            IAuditTasks auditTasks)
        {
            this.personStatisticsQuery = personStatisticsQuery;
            this.countsQuery = countsQuery;
            this.eventRevisionsQuery = eventRevisionsQuery;
            this.auditCareerQuery = auditCareerQuery;
            this.personTasks = personTasks;
            this.auditTasks = auditTasks;
        }

        public IList<object[]> GetCreatedProfilesCountByMonth()
        {
            return this.personStatisticsQuery.GetCreatedProfilesCountByMonth();
        }

        public ProfilingCountsView GetProfilingCountsView(DateTime? date, ISession session)
        {
            if (date.HasValue)
            {
                return new ProfilingCountsView()
                {
                    AsOfDate = string.Format("{0:yyyy-MM-dd}", date.Value),
                    ProfileStatus = this.GetPersonCount(date.Value)//,
                    //Career = ((IHistoricalCareerQuery)this.auditCareerQuery).GetCareerCount(date.Value),  // this figure inaccurate for unknown reason
                    //Event = this.eventRevisionsQuery.GetEventCount(date.Value)  // this figure inaccurate as it doesn't account for merged events
                };
            }
            else
            {
                return new ProfilingCountsView()
                {
                    AsOfDate = "now",
                    ProfileStatus = this.GetCurrentPersonCount(session),
                    Career = this.countsQuery.GetCareerCount(session),
                    Organization = this.countsQuery.GetOrganizationCount(session),
                    Event = this.countsQuery.GetEventCount(session),
                    PersonResponsibility = this.countsQuery.GetPersonResponsibilityCount(session),
                    OrganizationResponsibility = this.countsQuery.GetOrganizationResponsibilityCount(session),
                    Source = this.countsQuery.GetSourceCount(session),
                };
            }
        }

        protected IDictionary<ProfileStatus, int> GetPersonCount(DateTime date)
        {
            IDictionary<ProfileStatus, int> counts = new Dictionary<ProfileStatus, int>();

            // get deleted profiles from Profiling1 audit trail - required because deletions due to person merges are only recorded there (and not in PRF_Person_AUD).
            IList<int> deleted = this.auditTasks.GetOldDeletedProfiles().Where(x => x.WhenDate.Value <= date).Select(x => Convert.ToInt32(x.PersonID)).ToList();

            foreach (ProfileStatus ps in this.personTasks.GetAllProfileStatuses())
            {
                // get list of persons according to envers at given date
                IList<Person> enversPersonList = this.auditTasks.GetPersons(date, ps);

                // filter out those that were deleted, but whose deletion wasn't recorded in PRF_Person_AUD table (REVTYPE=2).
                counts.Add(ps, enversPersonList.Where(x => !deleted.Contains(x.Id)).Count());
            }

            // do the same as above, but for FARDC_2007_List
            ProfileStatus fardcPs = this.personTasks.GetProfileStatus(ProfileStatus.FARDC_2007_LIST);
            if (fardcPs != null)
            {
                IList<Person> fardcList = this.auditTasks.GetPersons(date, fardcPs);
                counts.Add(fardcPs, fardcList.Where(x => !deleted.Contains(x.Id)).Count());
            }

            return counts;
        }

        protected IDictionary<ProfileStatus, int> GetCurrentPersonCount(ISession session)
        {
            IList<object[]> counts = this.personStatisticsQuery.GetLiveCreatedProfilesCount(session);
            IDictionary<ProfileStatus, int> dict = new Dictionary<ProfileStatus, int>();

            foreach (ProfileStatus ps in this.personTasks.GetAllProfileStatuses(session))
            {
                IEnumerable<object[]> psCounts = counts.Where(x => Convert.ToInt32(x[0]) == ps.Id);
                if (psCounts != null && psCounts.Any())
                {
                    object[] row = psCounts.First();
                    dict.Add(ps, Convert.ToInt32(row[1]));
                }
                else
                {
                    dict.Add(ps, 0);
                }
            }

            ProfileStatus fardc2007List = this.personTasks.GetProfileStatus(ProfileStatus.FARDC_2007_LIST, session);
            IEnumerable<object[]> fardc2007Counts = counts.Where(x => Convert.ToInt32(x[0]) == fardc2007List.Id);
            if (fardc2007Counts != null && fardc2007Counts.Any())
            {
                object[] row = fardc2007Counts.First();
                dict.Add(fardc2007List, Convert.ToInt32(row[1]));
            }
            else
            {
                dict.Add(fardc2007List, 0);
            }

            return dict;
        }

        public IDictionary<string, IDictionary<string, int>> GetPersonCountByStatusAndOrganization()
        {
            IDictionary<string, IDictionary<string, int>> orgs = new Dictionary<string, IDictionary<string, int>>();

            // person counts by organization (those with careers) and status 
            foreach (object[] row in this.personStatisticsQuery.GetProfileStatusCountsByOrganization())
            {
                string psName = Convert.ToString(row[0]);
                string orgName = Convert.ToString(row[1]);
                int count = Convert.ToInt32(row[2]);

                if (!orgs.ContainsKey(orgName))
                {
                    orgs[orgName] = new Dictionary<string, int>();

                    foreach (ProfileStatus ps in this.personTasks.GetAllProfileStatuses())
                        orgs[orgName][ps.ProfileStatusName] = 0;
                }

                if (!string.Equals(psName, ProfileStatus.FARDC_2007_LIST))
                    orgs[orgName][psName] = count;
            }

            // person counts by status only (i.e. no careers)
            foreach (object[] row in this.personStatisticsQuery.GetProfileStatusCountsNoOrganization())
            {
                string psName = Convert.ToString(row[0]);
                int count = Convert.ToInt32(row[1]);

                if (!orgs.ContainsKey(string.Empty))
                {
                    orgs[string.Empty] = new Dictionary<string, int>();

                    foreach (ProfileStatus ps in this.personTasks.GetAllProfileStatuses())
                        orgs[string.Empty][ps.ProfileStatusName] = 0;
                }

                if (!string.Equals(psName, ProfileStatus.FARDC_2007_LIST))
                    orgs[string.Empty][psName] += count;
            }

            return orgs;
        }
    }
}
