using Profiling2.Domain.Scr;

namespace Profiling2.Domain.Contracts.Export.Screening
{
    public interface IPdfExportRequestForConditionalityParticipantService
    {
        byte[] GetExport(Request request, ScreeningEntity screeningEntity, bool sortByRank);
    }
}
