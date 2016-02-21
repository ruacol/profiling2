using System.Net;
using System.Web.Mvc;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Events;
using Profiling2.Domain.Prf.Organizations;
using Profiling2.Domain.Prf.Responsibility;
using Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels;
using Profiling2.Web.Mvc.Controllers;
using SharpArch.NHibernate.Web.Mvc;
using SharpArch.Web.Mvc.JsonNet;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers
{
    [PermissionAuthorize(AdminPermission.CanChangePersonResponsibilities, AdminPermission.CanChangeEvents)]
    public class OrgResponsibilitiesController : BaseController
    {
        protected readonly IEventTasks eventTasks;
        protected readonly IResponsibilityTasks responsibilityTasks;
        protected readonly IOrganizationTasks organizationTasks;

        public OrgResponsibilitiesController(IEventTasks eventTasks,
            IResponsibilityTasks responsibilityTasks,
            IOrganizationTasks organizationTasks)
        {
            this.eventTasks = eventTasks;
            this.responsibilityTasks = responsibilityTasks;
            this.organizationTasks = organizationTasks;
        }

        public ActionResult Add(int eventId)
        {
            Event e = this.eventTasks.GetEvent(eventId);
            if (e != null)
            {
                OrgResponsibilityViewModel vm = new OrgResponsibilityViewModel(e);
                vm.PopulateDropDowns(this.responsibilityTasks.GetOrgResponsibilityTypes());
                return View(vm);
            }
            return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        public JsonNetResult Add(OrgResponsibilityViewModel vm)
        {
            if (ModelState.IsValid)
            {
                Event e = this.eventTasks.GetEvent(vm.EventId);
                Organization o = this.organizationTasks.GetOrganization(vm.OrganizationId.Value);
                if (e != null && o != null)
                {
                    OrganizationResponsibility or = new OrganizationResponsibility();
                    or.Event = e;
                    or.Organization = o;
                    or.Unit = vm.UnitId.HasValue ? this.organizationTasks.GetUnit(vm.UnitId.Value) : null;
                    or.OrganizationResponsibilityType = this.responsibilityTasks.GetOrgResponsibilityType(vm.OrganizationResponsibilityTypeId);
                    or.Commentary = vm.Commentary;
                    or.Notes = vm.Notes;
                    or.Archive = false;
                    if (e.AddOrganizationResponsibility(or))
                    {
                        e = this.eventTasks.SaveEvent(e);
                        return JsonNet(string.Empty);
                    }
                    else
                    {
                        Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        return JsonNet("Organization/unit responsibility already exists.");
                    }
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return JsonNet("Event or organization does not exist.");
                }
            }
            else
            {
                return JsonNet(this.GetErrorsForJson());
            }
        }

        public ActionResult Edit(int id)
        {
            OrganizationResponsibility or = responsibilityTasks.GetOrganizationResponsibility(id);
            if (or != null)
            {
                OrgResponsibilityViewModel vm = new OrgResponsibilityViewModel(or);
                vm.PopulateDropDowns(this.responsibilityTasks.GetOrgResponsibilityTypes());
                return View(vm);
            }
            return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        public JsonNetResult Edit(OrgResponsibilityViewModel vm)
        {
            if (ModelState.IsValid)
            {
                OrganizationResponsibility or = responsibilityTasks.GetOrganizationResponsibility(vm.Id);
                if (or != null)
                {
                    // assuming event, org are the same
                    or.Unit = vm.UnitId.HasValue ? this.organizationTasks.GetUnit(vm.UnitId.Value) : null;
                    or.OrganizationResponsibilityType = this.responsibilityTasks.GetOrgResponsibilityType(vm.OrganizationResponsibilityTypeId);
                    or.Commentary = vm.Commentary;
                    or.Notes = vm.Notes;
                    or = this.responsibilityTasks.SaveOrganizationResponsibility(or);
                    return JsonNet(string.Empty);
                }
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return JsonNet("Organization responsibility does not exist.");
            }
            else
            {
                return JsonNet(this.GetErrorsForJson());
            }
        }

        [Transaction]
        public JsonNetResult Delete(int id)
        {
            OrganizationResponsibility or = responsibilityTasks.GetOrganizationResponsibility(id);
            if (or != null)
            {
                this.responsibilityTasks.DeleteOrganizationResponsibility(or);
                Response.StatusCode = (int)HttpStatusCode.OK;
                return JsonNet("Organization and/or unit's responsibility for event successfully removed.");
            }
            Response.StatusCode = (int)HttpStatusCode.NotFound;
            return JsonNet("Organization responsibility not found.");
        }
    }
}