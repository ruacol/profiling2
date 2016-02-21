using System;
using System.Collections.Generic;
using Profiling2.Domain.DTO;
using Profiling2.Domain.Scr;

namespace Profiling2.Domain.Contracts.Tasks.Screenings
{
    public interface IScreeningStatisticTasks
    {
        IList<object[]> GetFinalDecisionCountByMonth();

        IDictionary<string, int> GetFinalDecisionCountByRequestEntity(DateTime start, DateTime end);

        /// <summary>
        /// Count the number of screening final decisions made in the given year (including individual persons more than once).
        /// 
        /// Returns a dictionary of dictionaries indexed first by month (1-12), then by request entity name.
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        IDictionary<int, IDictionary<string, int>> GetFinalDecisionCountByRequestEntity(int year);

        IDictionary<RequestEntity, int> GetFinalDecisionIndividualCountByRequestEntity(DateTime start, DateTime end);

        IDictionary<ScreeningResult, int> GetFinalDecisionCountByResult(DateTime start, DateTime end);

        /// <summary>
        /// Count the number of screening final decisions made in the given year (including individual persons more than once).
        /// 
        /// Returns a dictionary of dictionaries indexed first by month (1-12), then by screening result.
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        IDictionary<int, IDictionary<ScreeningResult, int>> GetFinalDecisionCountByResult(int year);

        IDictionary<ScreeningResult, int> GetFinalDecisionIndividualCountByResult(DateTime start, DateTime end);

        /// <summary>
        /// Retrieve agreements rates of conditionality group and final decision colour codings with that of each screening entity.
        /// </summary>
        /// <param name="start">Start date (inclusive).</param>
        /// <param name="end">End date (exclusive).</param>
        /// <returns></returns>
        IDictionary<ScreeningEntity, ScreeningEntityStatisticDTO> GetScreeningEntityStatistics(DateTime start, DateTime end);
    }
}
