using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Mvc.JQuery.Datatables;
using Profiling2.Domain;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Contracts.Tasks.Screenings;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Domain.Scr;
using Profiling2.Web.Mvc.Areas.Screening.Controllers.ViewModels;
using Profiling2.Web.Mvc.Areas.System.Controllers.ViewModels;
using Profiling2.Web.Mvc.Controllers;
using SharpArch.Web.Mvc.JsonNet;

namespace Profiling2.Web.Mvc.Areas.Screening.Controllers
{
    public class ScreeningEntityController : ScreeningBaseController
    {
        protected readonly IScreeningTasks screeningTasks;
        protected readonly IUserTasks userTasks;
        protected readonly ILuceneTasks luceneTasks;

        public ScreeningEntityController(IScreeningTasks screeningTasks, IUserTasks userTasks, ILuceneTasks luceneTasks)
        {
            this.screeningTasks = screeningTasks;
            this.userTasks = userTasks;
            this.luceneTasks = luceneTasks;
        }

        public ActionResult Details(int id)
        {
            return View(this.screeningTasks.GetScreeningEntity(id));
        }

        [MultiRoleAuthorize(AdminRole.ScreeningRequestConditionalityParticipant)]
        public ActionResult SearchResponses()
        {
            ViewData["screeningEntity"] = this.GetUserScreeningEntityName();
            return View();
        }

        protected string GetUserScreeningEntityName()
        {
            AdminUser user = this.userTasks.GetAdminUser(User.Identity.Name);
            if (user != null)
            {
                ScreeningEntity entity = user.GetScreeningEntity();
                if (entity != null)
                    return entity.ScreeningEntityName;
            }
            return null;
        }

        [MultiRoleAuthorize(AdminRole.ScreeningRequestConditionalityParticipant)]
        public JsonNetResult DataTables(DataTablesParam p)
        {
            // calculate total results to request from lucene search
            int numResults = (p.iDisplayStart >= 0 && p.iDisplayLength > 0) ? (p.iDisplayStart + 1) * p.iDisplayLength : 10;

            IList<LuceneSearchResult> results = this.luceneTasks.ScreeningResponseSearch(p.sSearch, this.GetUserScreeningEntityName(), numResults);

            int iTotalRecords = 0;
            if (results != null && results.Count > 0)
                iTotalRecords = results.First().TotalHits;

            object[] aaData = results
                .Select(x => new ScreeningResponseDataTableLuceneView(x))
                .Skip(p.iDisplayStart)
                .Take(p.iDisplayLength)
                .ToArray<ScreeningResponseDataTableLuceneView>();

            return JsonNet(new DataTablesData
            {
                iTotalRecords = iTotalRecords,
                iTotalDisplayRecords = iTotalRecords,
                sEcho = p.sEcho,
                aaData = aaData.ToArray()
            });
        }
    }
}