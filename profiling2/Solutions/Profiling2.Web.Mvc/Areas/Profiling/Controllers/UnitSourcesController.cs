using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Contracts.Tasks.Sources;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Sources;
using Profiling2.Domain.Prf.Units;
using Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels;
using Profiling2.Web.Mvc.Controllers;
using SharpArch.NHibernate.Web.Mvc;
using SharpArch.Web.Mvc.JsonNet;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers
{
    [PermissionAuthorize(AdminPermission.CanChangeUnits, AdminPermission.CanViewAndSearchSources)]
    public class UnitSourcesController : BaseController
    {
        protected readonly IOrganizationTasks orgTasks;
        protected readonly ISourceTasks sourceTasks;
        protected readonly ISourceAttachmentTasks sourceAttachmentTasks;

        public UnitSourcesController(IOrganizationTasks orgTasks, ISourceTasks sourceTasks, ISourceAttachmentTasks sourceAttachmentTasks)
        {
            this.orgTasks = orgTasks;
            this.sourceTasks = sourceTasks;
            this.sourceAttachmentTasks = sourceAttachmentTasks;
        }

        public ActionResult Edit(int id)
        {
            UnitSource us = this.sourceAttachmentTasks.GetUnitSource(id);
            if (us != null)
            {
                UnitSourceViewModel vm = new UnitSourceViewModel(us);
                vm.PopulateDropDowns(this.sourceTasks.GetReliabilities());
                vm.PopulateSource(this.sourceTasks.GetSourceDTO(us.Source.Id));
                return View(vm);
            }
            return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        public JsonNetResult Edit(UnitSourceViewModel vm)
        {
            if (ModelState.IsValid)
            {
                UnitSource us = this.sourceAttachmentTasks.GetUnitSource(vm.Id.Value);
                if (us != null)
                {
                    if (vm.ReliabilityId.HasValue)
                        us.Reliability = this.sourceTasks.GetReliability(vm.ReliabilityId.Value);
                    us.Commentary = vm.Commentary;
                    us.Notes = vm.Notes;
                    us = this.sourceAttachmentTasks.SaveUnitSource(us);
                    return JsonNet(string.Empty);
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return JsonNet("Unit source not found.");
                }
            }
            else
            {
                return JsonNet(this.GetErrorsForJson());
            }
        }

        [Transaction]
        public JsonNetResult Delete(int id)
        {
            UnitSource us = this.sourceAttachmentTasks.GetUnitSource(id);
            if (us != null)
            {
                this.sourceAttachmentTasks.DeleteUnitSource(us.Id);
                Response.StatusCode = (int)HttpStatusCode.OK;
                return JsonNet("Source successfully detached from unit.");
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return JsonNet("Unit source not found.");
            }
        }

        public ActionResult Add(int unitId)
        {
            Unit u = this.orgTasks.GetUnit(unitId);
            if (u != null)
            {
                UnitSourceViewModel vm = new UnitSourceViewModel(u);
                vm.PopulateDropDowns(this.sourceTasks.GetReliabilities());
                return View(vm);
            }
            return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        public JsonNetResult Add(UnitSourceViewModel vm)
        {
            if (ModelState.IsValid)
            {
                Source s = this.sourceTasks.GetSource(vm.SourceId.Value);
                Unit u = this.orgTasks.GetUnit(vm.UnitId.Value);
                if (s != null && u != null)
                {
                    if (s.GetUnitSource(u) == null)
                    {
                        UnitSource us = new UnitSource()
                        {
                            Source = s,
                            Unit = u,
                            Commentary = vm.Commentary,
                            Notes = vm.Notes
                        };
                        if (vm.ReliabilityId.HasValue)
                            us.Reliability = this.sourceTasks.GetReliability(vm.ReliabilityId.Value);
                        this.sourceAttachmentTasks.SaveUnitSource(us);
                        return JsonNet(string.Empty);
                    }
                    else
                    {
                        Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        return JsonNet("Source already attached to this unit.");
                    }
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return JsonNet("Unit or source not found.");
                }
            }
            else
            {
                return JsonNet(this.GetErrorsForJson());
            }
        }

        public JsonNetResult UnitSources(int id)
        {
            Unit unit = this.orgTasks.GetUnit(id);
            if (unit != null)
            {
                IDictionary<string, object> jsonObj = new Dictionary<string, object>();

                IList<UnitSourceViewModel> sources = new List<UnitSourceViewModel>();
                foreach (UnitSource us in unit.UnitSources.Where(x => !x.Archive))
                {
                    UnitSourceViewModel usvm = new UnitSourceViewModel(us);
                    usvm.PopulateSource(this.sourceTasks.GetSourceDTO(us.Source.Id));
                    sources.Add(usvm);
                }

                jsonObj.Add("UnitSources", sources);

                return JsonNet(jsonObj);
            }

            Response.StatusCode = (int)HttpStatusCode.NotFound;
            return JsonNet(string.Empty);
        }
    }
}