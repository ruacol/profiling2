using System;
using System.Collections.Generic;
using System.Linq;
using Profiling2.Domain.Contracts.Queries.Stats;
using Profiling2.Domain.Contracts.Tasks.Screenings;
using Profiling2.Domain.DTO;
using Profiling2.Domain.Scr;
using Profiling2.Domain.Scr.Person;
using Profiling2.Domain.Scr.PersonFinalDecision;
using SharpArch.NHibernate.Contracts.Repositories;

namespace Profiling2.Tasks.Screenings
{
    public class ScreeningStatisticTasks : IScreeningStatisticTasks
    {
        protected readonly IScreeningCountsQuery screeningCountsQuery;
        protected readonly INHibernateRepository<ScreeningRequestPersonFinalDecision> srpfdRepo;
        protected readonly IScreeningTasks screeningTasks;
        protected readonly IRequestPersonTasks requestPersonTasks;

        public ScreeningStatisticTasks(IScreeningCountsQuery screeningCountsQuery,
            INHibernateRepository<ScreeningRequestPersonFinalDecision> srpfdRepo,
            IScreeningTasks screeningTasks,
            IRequestPersonTasks requestPersonTasks)
        {
            this.screeningCountsQuery = screeningCountsQuery;
            this.srpfdRepo = srpfdRepo;
            this.screeningTasks = screeningTasks;
            this.requestPersonTasks = requestPersonTasks;
        }

        public IList<object[]> GetFinalDecisionCountByMonth()
        {
            return this.screeningCountsQuery.GetFinalDecisionCountByMonth();
        }

        public IDictionary<string, int> GetFinalDecisionCountByRequestEntity(DateTime start, DateTime end)
        {
            IDictionary<string, int> requestEntities = new Dictionary<string, int>();
            foreach (object[] row in this.screeningCountsQuery.GetFinalDecisionCountByRequestEntity(start, end))
                requestEntities.Add(Convert.ToString(row[0]), Convert.ToInt32(row[1]));
            return requestEntities;
        }

        public IDictionary<int, IDictionary<string, int>> GetFinalDecisionCountByRequestEntity(int year)
        {
            IDictionary<int, IDictionary<string, int>> months = new Dictionary<int, IDictionary<string, int>>();

            for (int i = 1; i <= 12; i++)
            {
                DateTime start = new DateTime(year, i, 1);
                DateTime end = start.AddMonths(1);
                months.Add(i, this.GetFinalDecisionCountByRequestEntity(start, end));
            }

            return months;
        }

        public IDictionary<RequestEntity, int> GetFinalDecisionIndividualCountByRequestEntity(DateTime start, DateTime end)
        {
            IDictionary<RequestEntity, int> counts = new Dictionary<RequestEntity, int>();

            IDictionary<int, ScreeningRequestPersonFinalDecision> individualFds = this.GetCompletedFinalDecisionsByIndividual(start, end);
            foreach (ScreeningRequestPersonFinalDecision fd in individualFds.Values)
            {
                if (!counts.ContainsKey(fd.RequestPerson.Request.RequestEntity))
                    counts[fd.RequestPerson.Request.RequestEntity] = 0;

                counts[fd.RequestPerson.Request.RequestEntity]++;
            }

            return counts;
        }

        protected IList<ScreeningRequestPersonFinalDecision> GetCompletedFinalDecisions(DateTime start, DateTime end)
        {
            return this.srpfdRepo.GetAll()
                .Where(x => x.MostRecentHistory.DateStatusReached >= start
                    && x.MostRecentHistory.DateStatusReached < end
                    && !x.Archive
                    && x.RequestPerson.Request.CurrentStatus.RequestStatusName == RequestStatus.NAME_COMPLETED
                    && !x.RequestPerson.Request.Archive)
                .ToList();
        }

        protected IDictionary<int, ScreeningRequestPersonFinalDecision> GetCompletedFinalDecisionsByIndividual(DateTime start, DateTime end)
        {
            // arrange final decisions by unique individuals (dictionary keyed by person ID)
            IDictionary<int, ScreeningRequestPersonFinalDecision> individualFds = new Dictionary<int, ScreeningRequestPersonFinalDecision>();
            foreach (ScreeningRequestPersonFinalDecision srpfd in this.GetCompletedFinalDecisions(start, end))
            {
                if (individualFds.ContainsKey(srpfd.RequestPerson.Person.Id))
                {
                    if (individualFds[srpfd.RequestPerson.Person.Id].MostRecentDate.HasValue && srpfd.MostRecentDate.HasValue
                        && individualFds[srpfd.RequestPerson.Person.Id].MostRecentDate.Value < srpfd.MostRecentDate.Value)
                    {
                        individualFds[srpfd.RequestPerson.Person.Id] = srpfd;
                    }
                }
                else
                {
                    individualFds[srpfd.RequestPerson.Person.Id] = srpfd;
                }
            }

            return individualFds;
        }

        // TODO runs slow
        public IDictionary<ScreeningResult, int> GetFinalDecisionCountByResult(DateTime start, DateTime end)
        {
            IDictionary<ScreeningResult, int> counts = new Dictionary<ScreeningResult, int>();

            IList<ScreeningRequestPersonFinalDecision> fds = this.GetCompletedFinalDecisions(start, end);
            foreach (ScreeningResult sr in this.screeningTasks.GetScreeningResults())
                counts.Add(sr, fds.Where(x => x.ScreeningResult == sr).Count());

            return counts;
        }

        public IDictionary<int, IDictionary<ScreeningResult, int>> GetFinalDecisionCountByResult(int year)
        {
            IDictionary<int, IDictionary<ScreeningResult, int>> months = new Dictionary<int, IDictionary<ScreeningResult, int>>();

            for (int i = 1; i <= 12; i++)
            {
                DateTime start = new DateTime(year, i, 1);
                DateTime end = start.AddMonths(1);
                months.Add(i, this.GetFinalDecisionCountByResult(start, end));
            }

            return months;
        }

        public IDictionary<ScreeningResult, int> GetFinalDecisionIndividualCountByResult(DateTime start, DateTime end)
        {
            IDictionary<ScreeningResult, int> counts = new Dictionary<ScreeningResult, int>();

            IDictionary<int, ScreeningRequestPersonFinalDecision> individualFds = this.GetCompletedFinalDecisionsByIndividual(start, end);
            foreach (ScreeningResult sr in this.screeningTasks.GetScreeningResults())
                counts.Add(sr, individualFds.Values.Where(x => x.ScreeningResult == sr).Count());

            return counts;
        }

        public IDictionary<ScreeningEntity, ScreeningEntityStatisticDTO> GetScreeningEntityStatistics(DateTime start, DateTime end)
        {
            IDictionary<ScreeningEntity, ScreeningEntityStatisticDTO> dtos = new Dictionary<ScreeningEntity, ScreeningEntityStatisticDTO>();

            // get all RequestPersons for given date period
            IList<RequestPerson> rps = this.requestPersonTasks.GetCompletedRequestPersons()
                .Where(x => x.GetScreeningRequestPersonFinalDecision().MostRecentDate >= start
                    && x.GetScreeningRequestPersonFinalDecision().MostRecentDate < end)
                .ToList();

            // count all ScreeningResults from ConditionalityGroup and FinalDecision that differed and agreed to each ScreeningEntity's results.
            foreach (ScreeningEntity entity in this.screeningTasks.GetScreeningEntities())
            {
                ScreeningEntityStatisticDTO dto = new ScreeningEntityStatisticDTO() { ScreeningEntity = entity.ScreeningEntityName };
                IEnumerable<RequestPerson> theseRps = rps.Where(x => x.GetScreeningRequestPersonEntity(entity.ScreeningEntityName) != null);
                dto.TotalPersonScreenings = theseRps.Count();

                foreach (ScreeningResult result in this.screeningTasks.GetScreeningResults())
                {
                    ScreeningDifferenceDTO cgDiff = new ScreeningDifferenceDTO() { ScreeningResult = result.ScreeningResultName };
                    ScreeningDifferenceDTO fdDiff = new ScreeningDifferenceDTO() { ScreeningResult = result.ScreeningResultName };
                    foreach (RequestPerson rp in theseRps.Where(x => x.GetScreeningRequestPersonEntity(entity.ScreeningEntityName).ScreeningResult == result))
                    {
                        ScreeningResult entityResult = rp.GetScreeningRequestPersonEntity(entity.ScreeningEntityName).ScreeningResult;
                        if (entityResult == rp.GetScreeningRequestPersonRecommendation().ScreeningResult)
                        {
                            cgDiff.NumAgreed++;
                        }
                        else
                        {
                            cgDiff.NumDisagreed++;
                        }
                        if (entityResult == rp.GetScreeningRequestPersonFinalDecision().ScreeningResult)
                        {
                            fdDiff.NumAgreed++;
                        }
                        else
                        {
                            fdDiff.NumDisagreed++;
                        }
                    }
                    dto.ConditionalityGroupDifferences.Add(cgDiff);
                    dto.FinalDecisionDifferences.Add(fdDiff);
                }

                dtos.Add(entity, dto);
            }

            return dtos;
        }
    }
}
