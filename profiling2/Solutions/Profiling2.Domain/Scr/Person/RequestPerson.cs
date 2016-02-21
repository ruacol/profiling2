using System.Collections.Generic;
using System.Linq;
using Profiling2.Domain.Scr.PersonEntity;
using Profiling2.Domain.Scr.PersonFinalDecision;
using Profiling2.Domain.Scr.PersonRecommendation;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Scr.Person
{
    /// <summary>
    /// An instance of when the given Person has been screened for the given Request.
    /// </summary>
    public class RequestPerson : Entity
    {
        public virtual Request Request { get; set; }
        public virtual Profiling2.Domain.Prf.Persons.Person Person { get; set; }
        public virtual string Notes { get; set; }
        public virtual bool Archive { get; set; }
        public virtual IList<RequestPersonHistory> RequestPersonHistories { get; set; }
        public virtual IList<AdminRequestPersonImport> AdminRequestPersonImports { get; set; }
        public virtual IList<ScreeningRequestPersonFinalDecision> ScreeningRequestPersonFinalDecisions { get; set; }
        public virtual IList<ScreeningRequestPersonEntity> ScreeningRequestPersonEntities { get; set; }
        public virtual IList<ScreeningRequestPersonRecommendation> ScreeningRequestPersonRecommendations { get; set; }

        public RequestPerson()
        {
            this.RequestPersonHistories = new List<RequestPersonHistory>();
            this.AdminRequestPersonImports = new List<AdminRequestPersonImport>();
            this.ScreeningRequestPersonFinalDecisions = new List<ScreeningRequestPersonFinalDecision>();
            this.ScreeningRequestPersonEntities = new List<ScreeningRequestPersonEntity>();
            this.ScreeningRequestPersonRecommendations = new List<ScreeningRequestPersonRecommendation>();
        }

        public virtual RequestPersonHistory MostRecentHistory
        {
            get
            {
                IEnumerable<RequestPersonHistory> responses = this.RequestPersonHistories
                    .OrderByDescending(x => x.DateStatusReached);
                if (responses != null && responses.Any())
                    return responses.First();
                else
                    return null;
            }
        }

        public virtual bool IsNominated
        {
            get
            {
                RequestPersonHistory h = this.MostRecentHistory;
                if (h != null)
                    return h.RequestPersonStatus.RequestPersonStatusName == RequestPersonStatus.NAME_NOMINATED;
                return false;
            }
        }

        public virtual ScreeningRequestPersonEntity GetScreeningRequestPersonEntity(string screeningEntityName)
        {
            // if we don't check the SCR_ScreeningRequestEntityResponse table (i.e. Request.HasResponded()),
            // then we risk returning a response here that doesn't belong to this request.
            if (this.Request.HasResponded(screeningEntityName))
                return this.GetMostRecentScreeningRequestPersonEntity(screeningEntityName);
            return null;
        }

        public virtual ScreeningRequestPersonEntity GetMostRecentScreeningRequestPersonEntity(string screeningEntityName)
        {
            IEnumerable<ScreeningRequestPersonEntity> responses = this.GetResponses(screeningEntityName)
                .OrderByDescending(x => x.MostRecentHistory.DateStatusReached);
            if (responses != null && responses.Any())
                return responses.First();
            else
                return null;
        }

        protected virtual IEnumerable<ScreeningRequestPersonEntity> GetResponses(string screeningEntityName)
        {
            return this.ScreeningRequestPersonEntities
                .Where(x => !x.Archive && x.ScreeningEntity != null
                    && string.Equals(x.ScreeningEntity.ScreeningEntityName, screeningEntityName));
        }

        public virtual ScreeningRequestPersonRecommendation GetScreeningRequestPersonRecommendation()
        {
            if (this.ScreeningRequestPersonRecommendations.Where(x => !x.Archive).Count() > 0)
                return this.ScreeningRequestPersonRecommendations.Where(x => !x.Archive).First();
            return null;
        }

        public virtual ScreeningRequestPersonFinalDecision GetScreeningRequestPersonFinalDecision()
        {
            if (this.ScreeningRequestPersonFinalDecisions.Where(x => !x.Archive).Count() > 0)
                return this.ScreeningRequestPersonFinalDecisions.Where(x => !x.Archive).First();
            return null;
        }

        public virtual void AddRequestPersonHistory(RequestPersonHistory rph)
        {
            this.RequestPersonHistories.Add(rph);
        }

        public virtual void AddScreeningRequestPersonEntity(ScreeningRequestPersonEntity srpe)
        {
            this.ScreeningRequestPersonEntities.Add(srpe);
        }

        public virtual void AddScreeningRequestPersonFinalDecision(ScreeningRequestPersonFinalDecision srpfd)
        {
            this.ScreeningRequestPersonFinalDecisions.Add(srpfd);
        }
    }
}
