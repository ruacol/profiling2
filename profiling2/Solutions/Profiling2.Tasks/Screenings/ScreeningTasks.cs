using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using Profiling2.Domain.Contracts.Queries;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Contracts.Tasks.Screenings;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Organizations;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Domain.Scr;
using Profiling2.Domain.Scr.Person;
using Profiling2.Domain.Scr.PersonEntity;
using Profiling2.Domain.Scr.PersonFinalDecision;
using Profiling2.Domain.Scr.PersonRecommendation;
using SharpArch.NHibernate.Contracts.Repositories;

namespace Profiling2.Tasks.Screenings
{
    public class ScreeningTasks : IScreeningTasks
    {
        private readonly IUserTasks userTasks;
        private readonly INHibernateRepository<ScreeningEntity> screeningEntityRepository;
        private readonly INHibernateRepository<ScreeningResult> screeningResultRepository;
        private readonly INHibernateRepository<ScreeningSupportStatus> screeningSupportStatusRepository;
        private readonly INHibernateRepository<ScreeningRequestPersonEntity> srpeRepository;
        private readonly INHibernateRepository<ScreeningRequestPersonEntityHistory> srpehRepository;
        private readonly INHibernateRepository<ScreeningRequestPersonRecommendation> srprRepo;
        private readonly INHibernateRepository<ScreeningRequestPersonRecommendationHistory> srprhRepo;
        private readonly INHibernateRepository<ScreeningRequestPersonFinalDecision> srpfdRepo;
        private readonly INHibernateRepository<ScreeningRequestPersonFinalDecisionHistory> srpfdhRepo;
        private readonly INHibernateRepository<ScreeningStatus> screeningStatusRepo;
        private readonly INHibernateRepository<ScreeningRequestEntityResponse> srerRepo;
        private readonly IScreeningEntityQueries screeningEntityQueries;
        private readonly ILuceneTasks luceneTasks;

        public ScreeningTasks(IUserTasks userTasks,
            INHibernateRepository<ScreeningEntity> screeningEntityRepository,
            INHibernateRepository<ScreeningResult> screeningResultRepository,
            INHibernateRepository<ScreeningSupportStatus> screeningSupportStatusRepository,
            INHibernateRepository<ScreeningRequestPersonEntity> srpeRepository,
            INHibernateRepository<ScreeningRequestPersonEntityHistory> srpehRepository,
            INHibernateRepository<ScreeningRequestPersonRecommendation> srprRepo,
            INHibernateRepository<ScreeningRequestPersonRecommendationHistory> srprhRepo,
            INHibernateRepository<ScreeningRequestPersonFinalDecision> srpfdRepo,
            INHibernateRepository<ScreeningRequestPersonFinalDecisionHistory> srpfdhRepo,
            INHibernateRepository<ScreeningStatus> screeningStatusRepo,
            INHibernateRepository<ScreeningRequestEntityResponse> srerRepo,
            IScreeningEntityQueries screeningEntityQueries,
            ILuceneTasks luceneTasks)
        {
            this.userTasks = userTasks;
            this.screeningEntityRepository = screeningEntityRepository;
            this.screeningResultRepository = screeningResultRepository;
            this.screeningSupportStatusRepository = screeningSupportStatusRepository;
            this.srpeRepository = srpeRepository;
            this.srpehRepository = srpehRepository;
            this.srprRepo = srprRepo;
            this.srprhRepo = srprhRepo;
            this.srpfdRepo = srpfdRepo;
            this.srpfdhRepo = srpfdhRepo;
            this.screeningStatusRepo = screeningStatusRepo;
            this.srerRepo = srerRepo;
            this.screeningEntityQueries = screeningEntityQueries;
            this.luceneTasks = luceneTasks;
        }

        public IEnumerable<ScreeningEntity> GetScreeningEntities()
        {
            return this.screeningEntityRepository.GetAll();
        }

        public ScreeningEntity GetScreeningEntity(int id)
        {
            return this.screeningEntityRepository.Get(id);
        }

        public ScreeningEntity GetScreeningEntity(string name)
        {
            IDictionary<string, object> criteria = new Dictionary<string, object>();
            criteria.Add("ScreeningEntityName", name);
            return this.screeningEntityRepository.FindOne(criteria);
        }

        public IEnumerable<ScreeningResult> GetScreeningResults()
        {
            return this.GetScreeningResults(DateTime.Now);
        }

        public IEnumerable<ScreeningResult> GetScreeningResults(DateTime requestCreated)
        {
            if (requestCreated >= new DateTime(2015, 5, 21))
                return this.screeningResultRepository.GetAll();
            else
                return this.screeningResultRepository.GetAll()
                    .Where(x => new string[] { ScreeningResult.GREEN, ScreeningResult.YELLOW, ScreeningResult.RED, ScreeningResult.PENDING }.Contains(x.ScreeningResultName));
        }

        public ScreeningSupportStatus GetScreeningSupportStatus(int id)
        {
            return this.screeningSupportStatusRepository.Get(id);
        }

        public IEnumerable<ScreeningSupportStatus> GetScreeningSupportStatuses()
        {
            return this.screeningSupportStatusRepository.GetAll();
        }

        public ScreeningRequestPersonEntity GetScreeningRequestPersonEntity(int id)
        {
            return this.srpeRepository.Get(id);
        }

        public ScreeningRequestPersonEntity SaveOrUpdateScreeningRequestPersonEntity(ScreeningRequestPersonEntity srpe, string username, int screeningStatusId)
        {
            srpe.RequestPerson.AddScreeningRequestPersonEntity(srpe);
            srpe = this.srpeRepository.SaveOrUpdate(srpe);

            this.luceneTasks.UpdateResponse(srpe);

            ScreeningRequestPersonEntityHistory h = new ScreeningRequestPersonEntityHistory();
            h.AdminUser = this.userTasks.GetAdminUser(username);
            h.DateStatusReached = DateTime.Now;
            h.ScreeningRequestPersonEntity = srpe;
            h.ScreeningStatus = this.screeningStatusRepo.Get(screeningStatusId);
            srpe.AddHistory(h);
            this.srpehRepository.SaveOrUpdate(h);

            return srpe;
        }

        public IList<ScreeningRequestPersonEntity> GetScreeningResponsesByEntity(string screeningEntityName)
        {
            ScreeningEntity se = this.GetScreeningEntity(screeningEntityName);
            IDictionary<string, object> criteria = new Dictionary<string, object>();
            criteria.Add("ScreeningEntity", se);
            criteria.Add("Archive", false);
            return this.srpeRepository.FindAll(criteria)
                .Where(x => x.RequestPerson != null && x.RequestPerson.Request != null && x.RequestPerson.Request.ResponseDate(se.ScreeningEntityName).HasValue)
                .ToList();
        }

        public void CreateScreeningRequestPersonEntitiesForRequest(Request request, ScreeningEntity screeningEntity, string username)
        {
            if (request != null && screeningEntity != null)
            {
                ScreeningResult screeningResult = this.GetScreeningResult(ScreeningResult.PENDING);
                foreach (RequestPerson rp in request.Persons.Where(x => !x.Archive))
                {
                    // only create new ScreeningRequestPersonEntity if one doesn't already exist
                    if (rp.GetMostRecentScreeningRequestPersonEntity(screeningEntity.ScreeningEntityName) == null)
                    {
                        // persist new ScreeningRequestPersonEntity for this RequestPerson
                        ScreeningRequestPersonEntity newSrpe = new ScreeningRequestPersonEntity()
                            {
                                RequestPerson = rp,
                                ScreeningEntity = screeningEntity,
                                ScreeningResult = screeningResult
                            };

                        // copy screening results from last known screening by this screening entity
                        ScreeningRequestPersonEntity lastSrpe = rp.Person.GetLatestScreeningEntityResponse(screeningEntity.ScreeningEntityName);
                        if (lastSrpe != null)
                        {
                            newSrpe.ScreeningResult = lastSrpe.ScreeningResult;
                            newSrpe.Reason = lastSrpe.Reason;
                            newSrpe.Commentary = lastSrpe.Commentary;
                        }
                        newSrpe = this.SaveOrUpdateScreeningRequestPersonEntity(newSrpe, username, ScreeningStatus.ADDED);
                    }
                }
            }
        }

        public ScreeningRequestPersonRecommendation GetRecommendation(int id)
        {
            return this.srprRepo.Get(id);
        }

        public ScreeningResult GetScreeningResult(int id)
        {
            return this.screeningResultRepository.Get(id);
        }

        public ScreeningResult GetScreeningResult(string name)
        {
            IDictionary<string, object> criteria = new Dictionary<string, object>();
            criteria.Add("ScreeningResultName", name);
            return this.screeningResultRepository.FindOne(criteria);
        }

        public ScreeningRequestPersonRecommendation SaveOrUpdateRecommendation(ScreeningRequestPersonRecommendation srpr, string username)
        {
            srpr = this.srprRepo.SaveOrUpdate(srpr);

            ScreeningRequestPersonRecommendationHistory h = new ScreeningRequestPersonRecommendationHistory();
            h.AdminUser = this.userTasks.GetAdminUser(username);
            h.DateStatusReached = DateTime.Now;
            h.ScreeningRequestPersonRecommendation = srpr;
            h.ScreeningStatus = (srpr.Id > 0 ? this.screeningStatusRepo.Get(ScreeningStatus.UPDATED) : this.screeningStatusRepo.Get(ScreeningStatus.ADDED));
            this.srprhRepo.SaveOrUpdate(h);

            return srpr;
        }

        // will only allow one ScreeningEntity per user.
        public bool SetScreeningEntity(int screeningEntityId, int userId)
        {
            ScreeningEntity se = this.screeningEntityRepository.Get(screeningEntityId);
            AdminUser u = this.userTasks.GetAdminUser(userId);
            if (u != null)
            {
                foreach (ScreeningEntity e in this.screeningEntityRepository.GetAll())
                    u.RemoveScreeningEntity(e);
                if (se != null)
                    u.AddScreeningEntity(se);
                return true;
            }
            return false;
        }

        public bool HasAllResponses(Request request)
        {
            if (request != null)
            {
                int numResponses = request.ScreeningRequestEntityResponses.Where(x => !x.Archive).Select(x => x.ScreeningEntity).Distinct().Count();
                if (numResponses >= this.GetScreeningEntities().Where(x => !x.Archive).Count())
                    return true;
            }
            return false;
        }

        public ScreeningRequestEntityResponse SetEntityResponse(Request request, ScreeningEntity entity)
        {
            IDictionary<string, object> criteria = new Dictionary<string, object>();
            criteria.Add("Request", request);
            criteria.Add("ScreeningEntity", entity);
            ScreeningRequestEntityResponse response = this.srerRepo.FindOne(criteria);  // unique index exists over these two columns
            if (response != null)
            {
                response.Archive = false;
            }
            else
            {
                response = new ScreeningRequestEntityResponse();
                response.Request = request;
                response.ScreeningEntity = entity;
            }
            response.ResponseDateTime = DateTime.Now;
            return this.srerRepo.SaveOrUpdate(response);
        }

        public void UndoEntityResponse(ScreeningRequestEntityResponse response)
        {
            if (response != null)
                response.Archive = true;
        }

        public ScreeningRequestPersonFinalDecision GetFinalDecision(int id)
        {
            return this.srpfdRepo.Get(id);
        }

        public ScreeningRequestPersonFinalDecision SaveOrUpdateFinalDecision(ScreeningRequestPersonFinalDecision srpfd, string username)
        {
            srpfd = this.srpfdRepo.SaveOrUpdate(srpfd);

            ScreeningRequestPersonFinalDecisionHistory h = new ScreeningRequestPersonFinalDecisionHistory();
            h.AdminUser = this.userTasks.GetAdminUser(username);
            h.DateStatusReached = DateTime.Now;
            h.ScreeningRequestPersonFinalDecision = srpfd;
            h.ScreeningStatus = (srpfd.Id > 0 ? this.screeningStatusRepo.Get(ScreeningStatus.UPDATED) : this.screeningStatusRepo.Get(ScreeningStatus.ADDED));
            this.srpfdhRepo.SaveOrUpdate(h);

            return srpfd;
        }

        public void CreateScreeningRequestPersonFinalDecisionsForRequest(Request request, string username)
        {
            if (request != null)
            {
                ScreeningResult screeningResult = this.GetScreeningResult(ScreeningResult.PENDING);
                ScreeningSupportStatus screeningSupportStatus = this.GetScreeningSupportStatus(ScreeningSupportStatus.ID_PENDING);
                foreach (RequestPerson rp in request.Persons.Where(x => !x.Archive))
                {
                    // only create new ScreeningRequestPersonFinalDecision if one doesn't already exist
                    if (rp.GetScreeningRequestPersonFinalDecision() == null)
                    {
                        // persist new ScreeningRequestPersonFinalDecision for this RequestPerson
                        ScreeningRequestPersonFinalDecision finalDecision = new ScreeningRequestPersonFinalDecision()
                        {
                            RequestPerson = rp,
                            ScreeningResult = screeningResult,
                            ScreeningSupportStatus = screeningSupportStatus
                        };

                        // copy screening result made during recommendation/consolidation phase
                        ScreeningRequestPersonRecommendation recommendation = rp.GetScreeningRequestPersonRecommendation();
                        if (recommendation != null && recommendation.ScreeningResult != null)
                        {
                            finalDecision.ScreeningResult = recommendation.ScreeningResult;

                            // use same level screening result to pre-populate support status as well
                            switch (recommendation.ScreeningResult.Id)
                            {
                                case ScreeningResult.ID_GREEN:
                                    finalDecision.ScreeningSupportStatus = this.GetScreeningSupportStatus(ScreeningSupportStatus.ID_SUPPORT_RECOMMENDED);
                                    break;
                                case ScreeningResult.ID_YELLOW:
                                    finalDecision.ScreeningSupportStatus = this.GetScreeningSupportStatus(ScreeningSupportStatus.ID_MONITORED_SUPPORT);
                                    break;
                                case ScreeningResult.ID_RED:
                                    finalDecision.ScreeningSupportStatus = this.GetScreeningSupportStatus(ScreeningSupportStatus.ID_SUPPORT_NOT_RECOMMENDED);
                                    break;
                                case ScreeningResult.ID_PENDING:
                                    finalDecision.ScreeningSupportStatus = this.GetScreeningSupportStatus(ScreeningSupportStatus.ID_PENDING);
                                    break;
                            }
                        }
                        finalDecision = this.SaveOrUpdateFinalDecision(finalDecision, username);
                        rp.AddScreeningRequestPersonFinalDecision(finalDecision);
                    }
                }
            }
        }

        public IList<Person> SearchScreeningEntityResponses(string term, int screeningEntityId)
        {
            IList<Person> persons = new List<Person>();

            ScreeningEntity entity = this.GetScreeningEntity(screeningEntityId);
            if (entity != null)
            {
                IList<ScreeningRequestPersonEntity> srpes = this.screeningEntityQueries.SearchScreenings(term, screeningEntityId);
                foreach (ScreeningRequestPersonEntity srpe in srpes)
                    if (srpe.RequestPerson.Person.GetLatestScreeningEntityResponse(entity.ScreeningEntityName) == srpe)
                        persons.Add(srpe.RequestPerson.Person);
            }

            return persons;
        }

        public IList<ScreeningRequestPersonEntity> GetMostRecentScreeningRequestPersonEntities(ISession session)
        {
            // use dictionaries to ensure unique personId/screeningEntity responses - we only want the most recent response of each screening entity.
            // might not scale that well - TODO later
            IDictionary<int, IDictionary<int, ScreeningRequestPersonEntity>> entities = new Dictionary<int, IDictionary<int, ScreeningRequestPersonEntity>>();
            foreach (ScreeningEntity se in this.screeningEntityQueries.GetAllScreeningEntities(session))
                entities.Add(se.Id, new Dictionary<int, ScreeningRequestPersonEntity>());

            foreach (ScreeningRequestPersonEntity srpe in this.screeningEntityQueries.GetAllScreeningRequestPersonEntities(session))
            {
                if (entities[srpe.ScreeningEntity.Id].ContainsKey(srpe.RequestPerson.Person.Id))
                {
                    // replace response in dictionary with more recent response
                    if (srpe.Id > entities[srpe.ScreeningEntity.Id][srpe.RequestPerson.Person.Id].Id)
                    {
                        entities[srpe.ScreeningEntity.Id][srpe.RequestPerson.Person.Id] = srpe;
                    }
                }
                else
                {
                    entities[srpe.ScreeningEntity.Id][srpe.RequestPerson.Person.Id] = srpe;
                }
            }

            IList<ScreeningRequestPersonEntity> results = new List<ScreeningRequestPersonEntity>();
            foreach (KeyValuePair<int, IDictionary<int, ScreeningRequestPersonEntity>> kvp in entities)
                results = results.Concat(kvp.Value.Values).ToList();
            return results;
        }

        protected IList<ScreeningRequestPersonFinalDecision> GetFinalDecisions(ScreeningResult result)
        {
            IDictionary<string, object> criteria = new Dictionary<string, object>();
            criteria.Add("ScreeningResult", result);
            criteria.Add("Archive", false);
            return this.srpfdRepo.FindAll(criteria);
        }

        public IList<Person> GetFinalDecisions(string resultName)
        {
            return this.GetFinalDecisions(this.GetScreeningResult(resultName))
                .Where(x => !x.RequestPerson.Archive)
                .Select(x => x.RequestPerson.Person).Distinct()
                .ToList();
        }

        public IList<Person> GetFinalDecisions(string resultName, Organization org)
        {
            return this.GetFinalDecisions(resultName)
                .Where(x => x.WasEverAMemberOf(org))
                .ToList();
        }
    }
}
