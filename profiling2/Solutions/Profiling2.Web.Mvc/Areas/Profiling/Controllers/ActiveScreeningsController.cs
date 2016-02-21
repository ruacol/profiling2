using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Contracts.Tasks.Screenings;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Domain.Scr;
using Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels;
using Profiling2.Web.Mvc.Controllers;
using SharpArch.NHibernate.Web.Mvc;
using SharpArch.Web.Mvc.JsonNet;
using StackExchange.Profiling;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers
{
    [PermissionAuthorize(AdminPermission.CanChangePersons, AdminPermission.CanPerformScreeningInput)]
    public class ActiveScreeningsController : BaseController
    {
        protected readonly IPersonTasks personTasks;
        protected readonly IRequestTasks requestTasks;
        protected readonly IUserTasks userTasks;

        public ActiveScreeningsController(IPersonTasks personTasks, IRequestTasks requestTasks, IUserTasks userTasks)
        {
            this.personTasks = personTasks;
            this.requestTasks = requestTasks;
            this.userTasks = userTasks;
        }

        public ActionResult CreateModal(int personId)
        {
            Person p = this.personTasks.GetPerson(personId);
            if (p != null)
            {
                ActiveScreeningViewModel vm = new ActiveScreeningViewModel(p);
                vm.DateActivelyScreened = string.Format("{0:yyyy-MM-dd}", DateTime.Now);
                AdminUser user = this.userTasks.GetAdminUser(User.Identity.Name);
                if (user != null)
                {
                    vm.ScreenedById = user.Id;
                    vm.ScreenedByName = user.ToString();
                }

                IList<Request> requests;
                IEnumerable<AdminUser> users;
                var profiler = MiniProfiler.Current;
                
                using (profiler.Step("Getting valid requests"))
                    requests = this.requestTasks.GetValidRequests();

                using (profiler.Step("Getting active users"))
                    users = this.userTasks.GetAllAdminUsers().Where(x => !x.Archive);

                vm.PopulateDropDowns(requests, users);
                return View(vm);
            }
            return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        public JsonNetResult CreateModal(ActiveScreeningViewModel vm)
        {
            if (ModelState.IsValid)
            {
                Person p = this.personTasks.GetPerson(vm.PersonId);
                if (p != null)
                {
                    ActiveScreening a = new ActiveScreening();
                    if (vm.RequestId.HasValue)
                        a.Request = this.requestTasks.Get(vm.RequestId.Value);
                    a.Person = p;
                    a.DateActivelyScreened = DateTime.ParseExact(vm.DateActivelyScreened, "yyyy-MM-dd", CultureInfo.CurrentCulture);
                    if (vm.ScreenedById.HasValue)
                        a.ScreenedBy = this.userTasks.GetAdminUser(vm.ScreenedById.Value);
                    a.Notes = vm.Notes;
                    this.personTasks.SaveActiveScreening(a);
                    return JsonNet(string.Empty);
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return JsonNet("Person does not exist.");
                }
            }
            else
                return JsonNet(this.GetErrorsForJson());
        }

        public ActionResult EditModal(int personId, int id)
        {
            Person p = this.personTasks.GetPerson(personId);
            ActiveScreening a = this.personTasks.GetActiveScreening(id);
            if (p != null && a != null)
            {
                ActiveScreeningViewModel vm = new ActiveScreeningViewModel(a);
                vm.PopulateDropDowns(this.requestTasks.GetValidRequests(), this.userTasks.GetAllAdminUsers().Where(x => !x.Archive));
                return View(vm);
            }
            return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        public JsonNetResult EditModal(ActiveScreeningViewModel vm)
        {
            if (ModelState.IsValid)
            {
                Person p = this.personTasks.GetPerson(vm.PersonId);
                ActiveScreening a = this.personTasks.GetActiveScreening(vm.Id);
                if (p != null && a != null)
                {
                    if (vm.RequestId.HasValue)
                        a.Request = this.requestTasks.Get(vm.RequestId.Value);
                    else
                        a.Request = null;
                    a.Person = p;
                    a.DateActivelyScreened = DateTime.ParseExact(vm.DateActivelyScreened, "yyyy-MM-dd", CultureInfo.CurrentCulture);
                    if (vm.ScreenedById.HasValue)
                        a.ScreenedBy = this.userTasks.GetAdminUser(vm.ScreenedById.Value);
                    else
                        a.ScreenedBy = null;
                    a.Notes = vm.Notes;
                    this.personTasks.SaveActiveScreening(a);
                    return JsonNet(string.Empty);
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return JsonNet("Person or active screening does not exist.");
                }
            }
            else
                return JsonNet(this.GetErrorsForJson());
        }

        [Transaction]
        public JsonNetResult Delete(int id)
        {
            ActiveScreening a = this.personTasks.GetActiveScreening(id);
            if (a != null)
            {
                this.personTasks.DeleteActiveScreening(a);
                Response.StatusCode = (int)HttpStatusCode.OK;
                return JsonNet("Active screening successfully deleted.");
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return JsonNet("Active screening not found.");
            }
        }
    }
}