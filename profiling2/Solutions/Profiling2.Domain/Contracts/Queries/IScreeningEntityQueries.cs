using System.Collections.Generic;
using NHibernate;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Domain.Scr;
using Profiling2.Domain.Scr.PersonEntity;

namespace Profiling2.Domain.Contracts.Queries
{
    public interface IScreeningEntityQueries
    {
        /// <summary>
        /// Searches all reasoning and commentary of the given screening entity using given search term.
        /// 
        /// Note that all reasoning and commentary are searched, not just the latest.
        /// </summary>
        /// <param name="term"></param>
        /// <param name="screeningEntityId"></param>
        /// <returns></returns>
        IList<ScreeningRequestPersonEntity> SearchScreenings(string term, int screeningEntityId);

        IList<ScreeningEntity> GetAllScreeningEntities(ISession session);

        IList<ScreeningRequestPersonEntity> GetAllScreeningRequestPersonEntities(ISession session);
    }
}
