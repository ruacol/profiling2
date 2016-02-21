using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using HrdbWebServiceClient.Domain;
using Microsoft.Practices.ServiceLocation;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Contracts.Tasks.Sources;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Events;
using Profiling2.Domain.Prf.Sources;
using Profiling2.Infrastructure.Util;
using Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels;

namespace Profiling2.Web.Mvc.Areas.Hrdb.Controllers.ViewModels
{
    public class HrdbCaseViewModel
    {
        public int Id { get; set; }  // JhroCaseId
        public int? EventId { get; set; }
        public EventViewModel Event { get; set; }
        public IList<HrdbPerpetratorViewModel> HrdbPerpetrators { get; set; }

        public IList<SelectListItem> OrganizationResponsibilityTypes { get; set; }
        public IList<SelectListItem> PersonResponsibilityTypes { get; set; }

        // for display purposes
        public HrdbCase HrdbCase { get; set; }

        public HrdbCaseViewModel() { }

        public HrdbCaseViewModel(JhroCase jc)
        {
            if (jc != null)
            {
                this.Id = jc.Id;
                if (jc.Events != null && jc.Events.Any())
                {
                    // currently assuming one Event (but data model can take multiple)
                    this.EventId = jc.Events[0].Id;
                }

                HrdbCase hc = (HrdbCase)StreamUtil.Deserialize(jc.HrdbContentsSerialized);
                if (hc != null)
                {
                    this.HrdbCase = hc;

                    this.HrdbPerpetrators = hc.Perpetrators.Select(x => new HrdbPerpetratorViewModel(x)).ToList();

                    // pre-populate new Event fields
                    this.Event = new EventViewModel();
                    this.Event.PopulateDropDowns(ServiceLocator.Current.GetInstance<IEventTasks>().GetAllEventVerifiedStatuses());
                    this.Event.ViolationIds = string.Join(",", this.HrdbPerpetrators.Select(x => x.GetViolationIds()).Aggregate(new List<int>(), (x, y) => x.Concat(y).ToList()));
                    this.Event.NarrativeEn = string.Join("\n\n", new string[] { "Summary", hc.Summary, "Analysis", hc.AnalysisDesc, "Facts", hc.FactAnalysis, "Legal", hc.LegalAnalysis, "Methodology", hc.Methodology });
                    if (hc.StartDate.HasValue)
                    {
                        this.Event.YearOfStart = hc.StartDate.Value.Year;
                        this.Event.MonthOfStart = hc.StartDate.Value.Month;
                        this.Event.DayOfStart = hc.StartDate.Value.Day;
                    }
                    if (hc.EndDate.HasValue)
                    {
                        this.Event.YearOfEnd = hc.EndDate.Value.Year;
                        this.Event.MonthOfEnd = hc.EndDate.Value.Month;
                        this.Event.DayOfEnd = hc.EndDate.Value.Day;
                    }
                    // location
                    Location loc = ServiceLocator.Current.GetInstance<ILocationTasks>().GetOrCreateLocation(hc.IncidentAddr, hc.TownVillage, hc.Subregion, hc.Region, hc.GetLatitude(), hc.GetLongitude());
                    if (loc != null)
                        this.Event.LocationId = loc.Id;
                    // notes
                    this.Event.EventVerifiedStatusId = ServiceLocator.Current.GetInstance<IEventTasks>().GetEventVerifiedStatus(
                        hc.IsComplaintCode() ? EventVerifiedStatus.ALLEGATION : EventVerifiedStatus.JHRO_VERIFIED).Id;
                    this.Event.JhroCaseIds = jc.Id.ToString();
                }
            }

            this.OrganizationResponsibilityTypes = ServiceLocator.Current.GetInstance<IResponsibilityTasks>().GetOrgResponsibilityTypes()
                .Select(x => new SelectListItem() { Text = x.OrganizationResponsibilityTypeName, Value = x.Id.ToString() })
                .ToList();

            this.PersonResponsibilityTypes = ServiceLocator.Current.GetInstance<IResponsibilityTasks>().GetPersonResponsibilityTypes()
                .Select(x => new SelectListItem() { Text = x.PersonResponsibilityTypeName, Value = x.Id.ToString() })
                .ToList();
        }
    }
}