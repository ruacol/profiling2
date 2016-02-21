
namespace Profiling2.Domain.Contracts.Queries.Procs
{
    public interface IMergeStoredProcQueries
    {
        /// <summary>
        /// Call to stored procedure written by Miki as part of Profiling1.
        /// </summary>
        /// <param name="toKeepPersonId"></param>
        /// <param name="toDeletePersonId"></param>
        /// <param name="userId">This is 'UN ID', in the form 'I-0001'.</param>
        /// <param name="isProfilingChange"></param>
        /// <returns></returns>
        int MergePersons(int toKeepPersonId, int toDeletePersonId, string userId, bool isProfilingChange);

        /// <summary>
        /// Call to stored procedure written by Miki as part of Profiling1.
        /// </summary>
        /// <param name="toKeepUnitId"></param>
        /// <param name="toDeleteUnitId"></param>
        /// <param name="userId">This is 'UN ID', in the form 'I-0001'.</param>
        /// <param name="isProfilingChange"></param>
        /// <returns></returns>
        int MergeUnits(int toKeepUnitId, int toDeleteUnitId, string userId, bool isProfilingChange);

        /// <summary>
        /// Call to stored procedure written by Miki as part of Profiling1.
        /// </summary>
        /// <param name="toKeepEventId"></param>
        /// <param name="toDeleteEventId"></param>
        /// <param name="userId">This is 'UN ID', in the form 'I-0001'.</param>
        /// <param name="isProfilingChange"></param>
        /// <returns></returns>
        int MergeEvents(int toKeepEventId, int toDeleteEventId, string userId, bool isProfilingChange);
    }
}
