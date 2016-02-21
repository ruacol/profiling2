using System.Collections.Generic;
using System.Linq;
using HrdbWebServiceClient.Domain;
using Microsoft.Practices.ServiceLocation;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Prf.Events;

namespace Profiling2.Web.Mvc.Areas.Hrdb.Controllers.ViewModels
{
    public class HrdbPerpetratorViewModel
    {
        public int? PersonId { get; set; }
        public int? OrganizationId { get; set; }
        public int? OrganizationResponsibilityTypeId { get; set; }
        public int? PersonResponsibilityTypeId { get; set; }
        public string ViolationIds { get; set; }

        // for display purposes
        public HrdbPerpetrator HrdbPerpetrator { get; set; }

        public HrdbPerpetratorViewModel() { }

        public HrdbPerpetratorViewModel(HrdbPerpetrator hp)
        {
            if (hp != null)
            {
                this.HrdbPerpetrator = hp;
                if (hp.Violations != null && hp.Violations.Any())
                {
                    IList<Violation> violations = new List<Violation>();
                    foreach (HrdbViolation hv in hp.Violations)
                    {
                        foreach (KeyValuePair<Violation, int> kvp in ServiceLocator.Current.GetInstance<IEventTasks>().ScoreViolations(hv.ViolationDesc, new string[] { "/" }))
                            if (kvp.Value <= 2)
                                violations.Add(kvp.Key);
                    }
                    this.ViolationIds = string.Join(",", violations.Select(x => x.Id.ToString()));
                }
            }
        }

        public IList<int> GetViolationIds()
        {
            IList<int> list = new List<int>();

            if (!string.IsNullOrEmpty(this.ViolationIds))
            {
                string[] ids = this.ViolationIds.Split(',');
                foreach (string id in ids)
                {
                    int result;
                    if (int.TryParse(id, out result))
                    {
                        list.Add(result);
                    }
                }
            }

            return list;
        }
    }
}