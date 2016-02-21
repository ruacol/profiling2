using System.Collections.Generic;
using Profiling2.Domain.Scr.PersonEntity;

namespace Profiling2.Domain.Contracts.Search
{
    public interface IScreeningResponseIndexer
    {
        void Add<T>(T response);

        void AddMultiple<T>(IEnumerable<T> multiple);

        void DeleteIndex();

        void DeleteResponse(int personId, string screeningEntityName);

        void UpdateResponse(ScreeningRequestPersonEntity response);
    }
}
