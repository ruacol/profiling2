using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Mvc.JQuery.Datatables;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Events;
using Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels;
using Profiling2.Web.Mvc.Controllers;
using SharpArch.NHibernate.Web.Mvc;
using SharpArch.Web.Mvc.JsonNet;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers
{
    public class ViolationsController : BaseController
    {
        protected readonly IEventTasks eventTasks;

        public ViolationsController(IEventTasks eventTasks)
        {
            this.eventTasks = eventTasks;
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchEvents)]
        public ActionResult Index()
        {
            ViewBag.Roots = this.eventTasks.GetRootParentViolations();
            return View();
        }

        /// <summary>
        /// Deprecated for the moment.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [PermissionAuthorize(AdminPermission.CanViewAndSearchEvents)]
        public JsonNetResult DataTables(DataTablesParam p)
        {
            //IQueryable<Violation> queryable = this.eventTasks.GetViolations().AsQueryable<Violation>();
            //return DataTablesResult.Create<Violation, ViolationDataTableView>(queryable, parameters, x => new ViolationDataTableView(x));
            int iTotalRecords = this.eventTasks.GetViolationDataTablesCount(p.sSearch);
            IList<ViolationDataTableView> violations = this.eventTasks.GetViolationDataTablesPaginated(p.iDisplayStart, p.iDisplayLength, p.sSearch,
                p.iSortingCols, p.iSortCol, p.sSortDir);
            //object[] aaData = (from v in violations select new ViolationDataTableView(v)).ToArray<ViolationDataTableView>();

            return JsonNet(new DataTablesData
            {
                iTotalRecords = iTotalRecords,
                iTotalDisplayRecords = iTotalRecords,
                sEcho = p.sEcho,
                aaData = violations.ToArray()
            });
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchEvents)]
        public ActionResult Details(int id)
        {
            Violation v = this.eventTasks.GetViolation(id);
            if (v != null)
                return View(v);
            else
                return new HttpNotFoundResult();
        }

        //
        // GET: /Profiling/Violations/Create

        [PermissionAuthorize(AdminPermission.CanChangeEvents)]
        public ActionResult Create()
        {
            ViolationViewModel vm = new ViolationViewModel();
            vm.PopulateDropDowns(this.eventTasks.GetViolations());
            return View(vm);
        } 

        //
        // POST: /Profiling/Violations/Create

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangeEvents)]
        public ActionResult Create(ViolationViewModel vm)
        {
            if (ModelState.IsValid)
            {
                // TODO would be nice to have a unique constraint annotation we could use on the view model
                if (this.eventTasks.GetViolation(vm.Name) == null)
                {
                    Violation violation = new Violation();
                    violation.Name = vm.Name;
                    violation.Keywords = vm.Keywords;
                    violation.Description = vm.Description;
                    violation.ConditionalityInterest = vm.ConditionalityInterest;
                    if (vm.ParentViolationID.HasValue)
                        violation.ParentViolation = this.eventTasks.GetViolation(vm.ParentViolationID.Value);
                    violation = this.eventTasks.SaveViolation(violation);
                    return RedirectToAction("Details", new { id = violation.Id });
                }
                else
                    ViewData.ModelState.AddModelError("Name", "A violation with that name already exists.");
            }
            return Create();
        }
        
        //
        // GET: /Profiling/Violations/Edit/5

        [PermissionAuthorize(AdminPermission.CanChangeEvents)]
        public ActionResult Edit(int id)
        {
            Violation v = this.eventTasks.GetViolation(id);
            if (v != null)
            {
                ViolationViewModel vm = new ViolationViewModel();
                vm.Id = v.Id;
                vm.Name = v.Name;
                vm.Keywords = v.Keywords;
                vm.Description = v.Description;
                vm.ConditionalityInterest = v.ConditionalityInterest;
                if (v.ParentViolation != null)
                    vm.ParentViolationID = v.ParentViolation.Id;
                IList<Violation> parentCandidates = this.eventTasks.GetViolations();
                parentCandidates.Remove(v);
                vm.PopulateDropDowns(parentCandidates);
                return View(vm);
            }
            else
                return new HttpNotFoundResult();
        }

        //
        // POST: /Profiling/Violations/Edit/5

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangeEvents)]
        public ActionResult Edit(ViolationViewModel vm)
        {
            if (ModelState.IsValid)
            {
                Violation existingViolation = this.eventTasks.GetViolation(vm.Name);
                if (existingViolation == null || (existingViolation != null && existingViolation.Id == vm.Id))
                {
                    Violation violation = this.eventTasks.GetViolation(vm.Id);
                    violation.Name = vm.Name;
                    violation.Keywords = vm.Keywords;
                    violation.Description = vm.Description;
                    violation.ConditionalityInterest = vm.ConditionalityInterest;
                    if (vm.ParentViolationID.HasValue)
                        violation.ParentViolation = this.eventTasks.GetViolation(vm.ParentViolationID.Value);
                    else
                        violation.ParentViolation = null;
                    violation = this.eventTasks.SaveViolation(violation);
                    return RedirectToAction("Details", new { id = vm.Id });
                }
                else
                    ViewData.ModelState.AddModelError("Name", "A violation with that name already exists.");
            }
            return Edit(vm.Id);
        }

        //
        // GET: /Profiling/Violations/Delete/5

        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangeEvents)]
        public ActionResult Delete(int id)
        {
            Violation v = this.eventTasks.GetViolation(id);
            if (v != null)
            {
                this.eventTasks.DeleteViolation(v);
                return RedirectToAction("Index");
            }
            else
                return new HttpNotFoundResult();
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchEvents)]
        public JsonNetResult Name(int id)
        {
            Violation v = this.eventTasks.GetViolation(id);
            if (v != null)
            {
                return JsonNet(new
                {
                    Id = id,
                    Name = v.Name
                });
            }
            else
                return JsonNet(string.Empty);
        }
    }
}
