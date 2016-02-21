using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Profiling2.Domain.Scr;

namespace Profiling2.Domain.Contracts.Export.Screening
{
    public interface IPdfExportRequestForInitiatorService
    {
        byte[] GetExport(Request request, bool sortByRank);
    }
}
