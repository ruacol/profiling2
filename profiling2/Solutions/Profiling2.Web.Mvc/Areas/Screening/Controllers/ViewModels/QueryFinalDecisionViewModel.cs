using System.Collections.Generic;
using System.Web.Mvc;
using Profiling2.Domain.Scr;

namespace Profiling2.Web.Mvc.Areas.Screening.Controllers.ViewModels
{
    public class QueryFinalDecisionViewModel
    {
        public int? OrganizationId { get; set; }
        public int ScreeningResultId { get; set; }

        public SelectList ScreeningResults { get; set; }

        public QueryFinalDecisionViewModel() { }

        public void PopulateDropDowns(IEnumerable<ScreeningResult> srs)
        {
            this.ScreeningResults = new SelectList(srs, "Id", "ScreeningResultName", this.ScreeningResultId);
        }
    }
}