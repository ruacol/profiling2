
namespace Profiling2.Domain.Contracts.Tasks
{
    public interface IBackgroundTasks
    {
        void ResetLuceneIndexes();

        void CreatePersonIndex();

        void CreateUnitIndex();

        void CreateEventIndex();

        void CreateRequestIndex();

        void CreateScreeningResponseIndex();

        void UpdateSourceIndex();
    }
}
