using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using HrdbWebServiceClient.Domain;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Contracts.Tasks.Sources;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Events;
using Profiling2.Domain.Prf.Sources;
using Profiling2.Infrastructure.Util;
using Profiling2.Web.Mvc.Areas.Hrdb.Controllers.ViewModels;
using Profiling2.Web.Mvc.Controllers;
using SharpArch.NHibernate.Web.Mvc;
using SharpArch.Web.Mvc.JsonNet;

namespace Profiling2.Web.Mvc.Areas.Hrdb.Controllers
{
    [PermissionAuthorize(AdminPermission.CanViewAndSearchEvents)]
    public class CasesController : BaseController
    {
        protected readonly ISourceTasks sourceTasks;
        protected readonly IEventTasks eventTasks;
        protected readonly IEventMatchingTasks eventMatchingTasks;

        public CasesController(ISourceTasks sourceTasks, IEventTasks eventTasks, IEventMatchingTasks eventMatchingTasks)
        {
            this.sourceTasks = sourceTasks;
            this.eventTasks = eventTasks;
            this.eventMatchingTasks = eventMatchingTasks;
        }

        public ActionResult Details(JhroCaseViewModel vm)
        {
            if (vm != null)
            {
                JhroCase jc = null;
                if (vm.Id > 0)
                    jc = this.sourceTasks.GetJhroCase(vm.Id);
                else if (!string.IsNullOrEmpty(vm.CaseNumber))
                    jc = this.sourceTasks.GetJhroCase(vm.CaseNumber);

                if (jc != null)
                {
                    ViewBag.JhroCase = jc;

                    HrdbCase hrdbCase = (HrdbCase)StreamUtil.Deserialize(jc.HrdbContentsSerialized);

                    return View(hrdbCase);
                }
            }
            return new HttpNotFoundResult();
        }

        public JsonNetResult Name(int id)
        {
            JhroCase jhroCase = this.sourceTasks.GetJhroCase(id);
            if (jhroCase != null)
            {
                return JsonNet(new
                {
                    Id = id,
                    Name = jhroCase.CaseNumber
                });
            }
            else
                return JsonNet(string.Empty);
        }

        public JsonNetResult Get()
        {
            string term = Request.QueryString["term"];
            IList<JhroCase> cases;

            if (string.IsNullOrEmpty(term))
                cases = this.sourceTasks.GetJhroCases();
            else
                cases = this.sourceTasks.SearchJhroCases(term.Trim());

            return JsonNet(cases.Select(x => new { id = x.Id, text = x.CaseNumber }).Take(50).ToList<object>());
        }

        public ActionResult CreateModal()
        {
            return View(new JhroCaseViewModel());
        }

        [HttpPost]
        [Transaction]
        public JsonNetResult CreateModal(JhroCaseViewModel vm)
        {
            JhroCase existing = this.sourceTasks.GetJhroCase(vm.CaseNumber);
            if (existing != null)
                ModelState.AddModelError("CaseNumber", "Case code already exists.");

            if (ModelState.IsValid)
            {
                JhroCase jhroCase = new JhroCase();
                jhroCase.CaseNumber = vm.CaseNumber;
                jhroCase = this.sourceTasks.SaveJhroCase(jhroCase);
                return JsonNet(new
                {
                    Id = jhroCase.Id,
                    CaseNumber = jhroCase.CaseNumber,
                    WasSuccessful = true
                });
            }
            else
            {
                return JsonNet(this.GetErrorsForJson());
            }
        }

        [Transaction]
        public ActionResult Link(int id)
        {
            JhroCase jc = this.sourceTasks.GetJhroCase(id);
            if (jc != null)
            {
                int eventId;
                if (int.TryParse(Request.QueryString["eventId"], out eventId))
                {
                    Event e = this.eventTasks.GetEvent(eventId);
                    if (e != null)
                    {
                        e.AddJhroCase(jc);
                        e = this.eventTasks.SaveEvent(e);
                        return RedirectToAction("Case", "Hrdb", new { area = "Profiling", id = id });
                    }
                }
            }
            return new HttpNotFoundResult();
        }

        public JsonNetResult FindMatchingEventCandidates(int id)
        {
            JhroCase jc = this.sourceTasks.GetJhroCase(id);
            if (jc != null)
            {
                // if not already linked to an Event...
                if (!jc.Events.Any())
                {
                    HrdbCase hrdbCase = (HrdbCase)StreamUtil.Deserialize(jc.HrdbContentsSerialized);

                    // ...find potential matching Events to link to this case
                    return JsonNet(new
                    {
                        Events = this.eventMatchingTasks.FindMatchingEventCandidates(jc, hrdbCase)
                    });
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    Response.StatusDescription = "That case already has an event linked.";
                    return JsonNet(Response.StatusDescription);
                }
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                Response.StatusDescription = "That case doesn't exist.";
                return JsonNet(Response.StatusDescription);
            }
        }
    }
}