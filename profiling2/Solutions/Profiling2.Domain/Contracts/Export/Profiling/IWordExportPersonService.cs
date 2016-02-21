using Profiling2.Domain.Prf.Persons;

namespace Profiling2.Domain.Contracts.Export.Profiling
{
    public interface IWordExportPersonService
    {
        byte[] GetExport(Person person, bool includeBackground);
    }
}
