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
    public class OperationSourcesController : BaseController
    {
        protected readonly IOrganizationTasks orgTasks;
        protected readonly ISourceTasks sourceTasks;
        protected readonly ISourceAttachmentTasks sourceAttachmentTasks;

        public OperationSourcesController(IOrganizationTasks orgTasks, ISourceTasks sourceTasks, ISourceAttachmentTasks sourceAttachmentTasks)
        {
            this.orgTasks = orgTasks;
            this.sourceTasks = sourceTasks;
            this.sourceAttachmentTasks = sourceAttachmentTasks;
        }

        public ActionResult Edit(int id)
        {
            OperationSource os = this.sourceAttachmentTasks.GetOperationSource(id);
            if (os != null)
            {
                OperationSourceViewModel vm = new OperationSourceViewModel(os);
                vm.PopulateDropDowns(this.sourceTasks.GetReliabilities());
                vm.PopulateSource(this.sourceTasks.GetSourceDTO(os.Source.Id));
                return View(vm);
            }
            return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        public JsonNetResult Edit(OperationSourceViewModel vm)
        {
            if (ModelState.IsValid)
            {
                OperationSource os = this.sourceAttachmentTasks.GetOperationSource(vm.Id.Value);
                if (os != null)
                {
                    if (vm.ReliabilityId.HasValue)
                        os.Reliability = this.sourceTasks.GetReliability(vm.ReliabilityId.Value);
                    os.Commentary = vm.Commentary;
                    os.Notes = vm.Notes;
                    os = this.sourceAttachmentTasks.SaveOperationSource(os);
                    return JsonNet(string.Empty);
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return JsonNet("Operation source not found.");
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
            OperationSource os = this.sourceAttachmentTasks.GetOperationSource(id);
            if (os != null)
            {
                this.sourceAttachmentTasks.DeleteOperationSource(os.Id);
                Response.StatusCode = (int)HttpStatusCode.OK;
                return JsonNet("Source successfully detached from operation.");
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return JsonNet("Operation source not found.");
            }
        }

        public ActionResult Add(int operationId)
        {
            Operation o = this.orgTasks.GetOperation(operationId);
            if (o != null)
            {
                OperationSourceViewModel vm = new OperationSourceViewModel(o);
                vm.PopulateDropDowns(this.sourceTasks.GetReliabilities());
                return View(vm);
            }
            return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        public JsonNetResult Add(OperationSourceViewModel vm)
        {
            if (ModelState.IsValid)
            {
                Source s = this.sourceTasks.GetSource(vm.SourceId.Value);
                Operation o = this.orgTasks.GetOperation(vm.OperationId.Value);
                if (s != null && o != null)
                {
                    if (s.GetOperationSource(o) == null)
                    {
                        OperationSource os = new OperationSource()
                        {
                            Source = s,
                            Operation = o,
                            Commentary = vm.Commentary,
                            Notes = vm.Notes
                        };
                        if (vm.ReliabilityId.HasValue)
                            os.Reliability = this.sourceTasks.GetReliability(vm.ReliabilityId.Value);
                        this.sourceAttachmentTasks.SaveOperationSource(os);
                        return JsonNet(string.Empty);
                    }
                    else
                    {
                        Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        return JsonNet("Source already attached to this operation.");
                    }
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return JsonNet("Operation or source not found.");
                }
            }
            else
            {
                return JsonNet(this.GetErrorsForJson());
            }
        }

        public JsonNetResult Json(int id)
        {
            Operation op = this.orgTasks.GetOperation(id);
            if (op != null)
            {
                IDictionary<string, object> jsonObj = new Dictionary<string, object>();

                IList<OperationSourceViewModel> sources = new List<OperationSourceViewModel>();
                foreach (OperationSource os in op.OperationSources.Where(x => !x.Archive))
                {
                    OperationSourceViewModel vm = new OperationSourceViewModel(os);
                    vm.PopulateSource(this.sourceTasks.GetSourceDTO(os.Source.Id));
                    sources.Add(vm);
                }

                jsonObj.Add("OperationSources", sources);

                return JsonNet(jsonObj);
            }

            Response.StatusCode = (int)HttpStatusCode.NotFound;
            return JsonNet(string.Empty);
        }
    }
}