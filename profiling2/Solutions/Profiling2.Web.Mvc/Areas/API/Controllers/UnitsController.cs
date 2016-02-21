using System.Collections.Generic;
using System.Linq;
using System.Net;
using Profiling2.Domain;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Prf.Units;
using Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels;
using SharpArch.Web.Mvc.JsonNet;

namespace Profiling2.Web.Mvc.Areas.API.Controllers
{
    public class UnitsController : BaseApiController
    {
        protected readonly IOrganizationTasks orgTasks;
        protected readonly ILuceneTasks luceneTasks;

        public UnitsController(IOrganizationTasks orgTasks, ILuceneTasks luceneTasks)
        {
            this.orgTasks = orgTasks;
            this.luceneTasks = luceneTasks;
        }

        public JsonNetResult Get(int id)
        {
            Unit unit = this.orgTasks.GetUnit(id);
            if (unit != null)
            {
                return JsonNet(new
                {
                    Id = unit.ToString(),
                    Name = unit.UnitName
                });
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return JsonNet(string.Empty);
            }
        }

        public JsonNetResult Search()
        {
            string term = Request.QueryString["term"];
            if (!string.IsNullOrEmpty(term))
            {
                term = term.Trim();

                // conduct search
                IList<LuceneSearchResult> results = this.luceneTasks.UnitSearch(term, 50);
                return JsonNet(results.Select(x => new UnitDataTableLuceneView(x)).ToArray());
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return JsonNet(string.Empty);
            }
        }
    }
}