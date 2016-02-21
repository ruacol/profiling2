using Profiling2.Domain.Prf.Events;
using System;

namespace Profiling2.Domain.Contracts.Export.Profiling
{
    public interface IWordExportEventService
    {
        byte[] GetExport(Event ev, DateTime lastModifiedDate);
    }
}
