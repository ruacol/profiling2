using System;
using System.Collections.Generic;
using NHibernate;
using Profiling2.Domain.Prf.Organizations;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Domain.Scr;
using Profiling2.Domain.Scr.PersonEntity;
using Profiling2.Domain.Scr.PersonFinalDecision;
using Profiling2.Domain.Scr.PersonRecommendation;

namespace Profiling2.Domain.Contracts.Tasks.Screenings
{
    public interface IScreeningTasks
    {
        IEnumerable<ScreeningEntity> GetScreeningEntities();

        ScreeningEntity GetScreeningEntity(int id);

        ScreeningEntity GetScreeningEntity(string name);

        IEnumerable<ScreeningResult> GetScreeningResults();

        IEnumerable<ScreeningResult> GetScreeningResults(DateTime requestCreated);

        ScreeningSupportStatus GetScreeningSupportStatus(int id);

        IEnumerable<ScreeningSupportStatus> GetScreeningSupportStatuses();

        ScreeningRequestPersonEntity GetScreeningRequestPersonEntity(int id);

        ScreeningRequestPersonEntity SaveOrUpdateScreeningRequestPersonEntity(ScreeningRequestPersonEntity srpe, string username, int screeningStatusId);

        IList<ScreeningRequestPersonEntity> GetScreeningResponsesByEntity(string screeningEntityName);

        void CreateScreeningRequestPersonEntitiesForRequest(Request request, ScreeningEntity screeningEntity, string username);

        ScreeningRequestPersonRecommendation GetRecommendation(int id);

        ScreeningResult GetScreeningResult(int id);

        ScreeningResult GetScreeningResult(string name);

        ScreeningRequestPersonRecommendation SaveOrUpdateRecommendation(ScreeningRequestPersonRecommendation srpr, string username);

        bool SetScreeningEntity(int screeningEntityId, int userId);

        bool HasAllResponses(Request request);

        /// <summary>
        /// Mark the fact that the given screening entity has responded to the given request by saving a ScreeningRequestEntityResponse.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        ScreeningRequestEntityResponse SetEntityResponse(Request request, ScreeningEntity entity);

        /// <summary>
        /// Allow a screening entity to edit their response by archiving their ScreeningRequestEntityResponse.
        /// </summary>
        /// <param name="response"></param>
        void UndoEntityResponse(ScreeningRequestEntityResponse response);

        ScreeningRequestPersonFinalDecision GetFinalDecision(int id);

        ScreeningRequestPersonFinalDecision SaveOrUpdateFinalDecision(ScreeningRequestPersonFinalDecision srpfd, string username);

        /// <summary>
        /// Checks and ensures that each RequestPerson has a ScreeningRequestPersonFinalDecision.  Pre-populates the ScreeningResult with that 
        /// from the person's ScreeningRequestPersonRecommendation.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="username"></param>
        void CreateScreeningRequestPersonFinalDecisionsForRequest(Request request, string username);

        IList<Person> SearchScreeningEntityResponses(string term, int screeningEntityId);

        /// <summary>
        /// Return only most recent responses by each screening entity of all screened persons.
        /// </summary>
        /// <returns></returns>
        IList<ScreeningRequestPersonEntity> GetMostRecentScreeningRequestPersonEntities(ISession session);

        /// <summary>
        /// Return a unique list of Persons for which a screening final decision was made with the given colour coding.
        /// </summary>
        /// <param name="resultName"></param>
        /// <returns></returns>
        IList<Person> GetFinalDecisions(string resultName);

        /// <summary>
        /// Return a unique list of Persons for which a screening final decision was made with the given colour coding;
        /// each Person must have been, at some stage, a member of the given organisation.
        /// </summary>
        /// <param name="resultName"></param>
        /// <param name="org"></param>
        /// <returns></returns>
        IList<Person> GetFinalDecisions(string resultName, Organization org);
    }
}
