using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using Mvc.JQuery.Datatables;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Contracts.Tasks.Screenings;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Units;
using Profiling2.Infrastructure.Security;
using Profiling2.Infrastructure.Util;
using Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels;
using Profiling2.Web.Mvc.Controllers;
using QueryInterceptor;
using SharpArch.NHibernate.Web.Mvc;
using SharpArch.Web.Mvc.JsonNet;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers
{
    public class OperationsController : BaseController
    {
        protected readonly IOrganizationTasks orgTasks;
        protected readonly IAuditTasks auditTasks;
        protected readonly IUserTasks userTasks;
        protected readonly IScreeningTasks screeningTasks;

        public OperationsController(IOrganizationTasks orgTasks, IAuditTasks auditTasks, IUserTasks userTasks, IScreeningTasks screeningTasks)
        {
            this.orgTasks = orgTasks;
            this.auditTasks = auditTasks;
            this.userTasks = userTasks;
            this.screeningTasks = screeningTasks;
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchUnits)]
        public ActionResult Index()
        {
            return View();
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchUnits)]
        public DataTablesResult<OperationDataTableView> DataTables(DataTablesParam p)
        {
            IQueryable<OperationDataTableView> q = this.orgTasks.GetAllOperations()
                .Select(x => new OperationDataTableView(x))
                .AsQueryable()
                .InterceptWith(new SetComparerExpressionVisitor(StringComparison.CurrentCultureIgnoreCase));

            return DataTablesResult.Create(q, p, x => new
                {
                    Objective = string.IsNullOrEmpty(x.Objective) ? string.Empty : x.Objective
                });
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchUnits)]
        public ActionResult Details(int id)
        {
            Operation op = this.orgTasks.GetOperation(id);
            if (op != null)
            {
                bool includeNameChanges = false;
                bool.TryParse(Request.QueryString["includeNameChanges"], out includeNameChanges);
                ViewBag.IncludeNameChanges = includeNameChanges;
                return View(op);
            }
            return new HttpNotFoundResult();
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchUnits)]
        public ActionResult Audit(int id)
        {
            Operation op = this.orgTasks.GetOperation(id);
            if (op != null)
            {
                ViewBag.Operation = op;
                ViewBag.AuditTrail = this.auditTasks.GetOperationAuditTrail(id);
                ViewBag.Users = this.userTasks.GetAllAdminUsers();
                return View();
            }
            return new HttpNotFoundResult();
        }

        [PermissionAuthorize(AdminPermission.CanChangeUnits)]
        public ActionResult Create()
        {
            return View(new OperationViewModel());
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangeUnits)]
        public ActionResult Create(OperationViewModel vm)
        {
            if (ModelState.IsValid)
            {
                if (this.orgTasks.GetOperation(vm.Name) == null)
                {
                    Operation op = Mapper.Map(vm, new Operation());
                    op = this.orgTasks.SaveOperation(op);

                    return RedirectToAction("Details", new { id = op.Id });
                }
                else
                    ModelState.AddModelError("Name", "Operation already exists with that name.");
            }
            return Create();
        }

        [PermissionAuthorize(AdminPermission.CanChangeUnits)]
        public ActionResult CreateModal()
        {
            return Create();
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangeUnits)]
        public JsonNetResult CreateModal(OperationViewModel vm)
        {
            if (ModelState.IsValid)
            {
                if (this.orgTasks.GetOperation(vm.Name) == null)
                {
                    Operation op = Mapper.Map(vm, new Operation());
                    op = this.orgTasks.SaveOperation(op);

                    return JsonNet(new
                    {
                        Id = op.Id,
                        Name = op.Name,
                        WasSuccessful = true
                    });
                }
                else
                    ModelState.AddModelError("Name", "Operation already exists with that name.");
            }
            return JsonNet(this.GetErrorsForJson());
        }

        [PermissionAuthorize(AdminPermission.CanChangeUnits)]
        public ActionResult Edit(int id)
        {
            Operation op = this.orgTasks.GetOperation(id);
            if (op != null)
                return View(new OperationViewModel(op));
            return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangeUnits)]
        public ActionResult Edit(OperationViewModel vm)
        {
            if (ModelState.IsValid)
            {
                Operation op = this.orgTasks.GetOperation(vm.Id);

                op = Mapper.Map(vm, op);
                op = this.orgTasks.SaveOperation(op);
                return RedirectToAction("Details", new { id = vm.Id });
            }
            return Edit(vm.Id);
        }

        [PermissionAuthorize(AdminPermission.CanChangeUnits)]
        public ActionResult EditModal(int id)
        {
            Operation op = this.orgTasks.GetOperation(id);
            if (op != null)
                return View(new OperationViewModel(op));
            return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangeUnits)]
        public JsonNetResult EditModal(OperationViewModel vm)
        {
            if (ModelState.IsValid)
            {
                Operation op = this.orgTasks.GetOperation(vm.Id);

                op = Mapper.Map(vm, op);
                op = this.orgTasks.SaveOperation(op);
                return JsonNet(string.Empty);
            }
            return JsonNet(this.GetErrorsForJson());
        }

        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangeUnits)]
        public ActionResult Delete(int id)
        {
            Operation op = this.orgTasks.GetOperation(id);
            if (op != null)
            {
                if (this.orgTasks.DeleteOperation(op))
                    return RedirectToAction("Index");
                else
                    return RedirectToAction("Details", new { id = id });
            }
            return new HttpNotFoundResult();
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchUnits)]
        public JsonNetResult Name(int id)
        {
            Operation op = this.orgTasks.GetOperation(id);
            if (op != null)
            {
                return JsonNet(new
                {
                    Id = id,
                    Name = op.ToString()
                });
            }
            else
                return JsonNet(string.Empty);
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchUnits)]
        public JsonNetResult Json()
        {
            IList<object> objList = new List<object>();
            string term = Request.QueryString["term"];
            if (!string.IsNullOrEmpty(term))
                term = term.Trim();
            foreach (Operation o in this.orgTasks.GetOperationsLike(term))
                objList.Add(new
                {
                    id = o.Id,
                    text = o.Name
                });
            return JsonNet(objList);
        }

        [PermissionAuthorize(AdminPermission.CanChangeUnits)]
        public ActionResult AddUnitModal(int operationId)
        {
            UnitOperationViewModel vm = new UnitOperationViewModel();
            Operation op = this.orgTasks.GetOperation(operationId);
            if (op != null)
            {
                vm.OperationName = op.Name;
            }
            return View(vm);
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangeUnits)]
        public JsonNetResult AddUnitModal(UnitOperationViewModel vm)
        {
            if (ModelState.IsValid)
            {
                Unit unit = this.orgTasks.GetUnit(vm.UnitId.Value);

                if (unit != null)
                {
                    if (vm.OperationId.HasValue)
                    {
                        Operation op = this.orgTasks.GetOperation(vm.OperationId.Value);
                        if (op != null)
                        {
                            UnitOperation uo = Mapper.Map(vm, new UnitOperation());
                            uo.Operation = op;
                            uo.Unit = unit;
                            uo = this.orgTasks.SaveUnitOperation(uo);
                            return JsonNet(string.Empty);
                        }
                        else
                            ModelState.AddModelError("OperationId", "That operation doesn't exist.");
                    }
                    else
                        ModelState.AddModelError("OperationId", "No operation selected.");
                }
                else
                    ModelState.AddModelError("UnitId", "That unit doesn't exist.");
            }
            return JsonNet(this.GetErrorsForJson());
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchUnits)]
        public ActionResult Screenings(int id)
        {
            Operation op = this.orgTasks.GetOperation(id);
            if (op != null)
            {
                bool includeNameChanges = false;
                bool.TryParse(Request.QueryString["includeNameChanges"], out includeNameChanges);

                ViewBag.Careers = op.GetAllCareers(includeNameChanges, ((PrfPrincipal)User).HasPermission(AdminPermission.CanViewAndSearchRestrictedPersons));
                ViewBag.ScreeningEntities = this.screeningTasks.GetScreeningEntities();

                return View(op);
            }
            return new HttpNotFoundResult();
        }

        [PermissionAuthorize(AdminPermission.CanChangeUnits)]
        public ActionResult EditFormerNameChange(int id)
        {
            Operation op = this.orgTasks.GetOperation(id);
            if (op != null)
            {
                ViewBag.CurrentOperation = op;
                OperationNameChangeViewModel vm = new OperationNameChangeViewModel();
                vm.CurrentOperationId = op.Id;
                vm.NewOperationId = op.Id;
                if (op.FormerOperations.Any())
                    vm.OldOperationId = op.GetFormerOperation().Id;
                return View(vm);
            }
            return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangeUnits)]
        public ActionResult EditFormerNameChange(OperationNameChangeViewModel vm)
        {
            if (ModelState.IsValid)
            {
                this.SaveOperationNameChangeViewModel(vm);
                return RedirectToAction("Details", new { id = vm.CurrentOperationId });
            }
            return EditFormerNameChange(vm.CurrentOperationId);
        }

        protected void SaveOperationNameChangeViewModel(OperationNameChangeViewModel vm)
        {
            Operation oldOp = vm.OldOperationId.HasValue ? this.orgTasks.GetOperation(vm.OldOperationId.Value) : null;
            Operation newOp = vm.NewOperationId.HasValue ? this.orgTasks.GetOperation(vm.NewOperationId.Value) : null;

            if (oldOp != null)
            {
                oldOp.NextOperation = newOp;
                oldOp = this.orgTasks.SaveOperation(oldOp);
            }

            if (newOp != null)
            {
                newOp.FormerOperations.Clear();
                newOp = this.orgTasks.SaveOperation(newOp);
            }
        }

        [PermissionAuthorize(AdminPermission.CanChangeUnits)]
        public ActionResult EditNewNameChange(int id)
        {
            Operation op = this.orgTasks.GetOperation(id);
            if (op != null)
            {
                ViewBag.CurrentOperation = op;
                OperationNameChangeViewModel vm = new OperationNameChangeViewModel();
                vm.CurrentOperationId = op.Id;
                vm.OldOperationId = op.Id;
                vm.NewOperationId = op.NextOperation.Id;
                return View(vm);
            }
            return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangeUnits)]
        public ActionResult EditNewNameChange(OperationNameChangeViewModel vm)
        {
            if (ModelState.IsValid)
            {
                this.SaveOperationNameChangeViewModel(vm);
                return RedirectToAction("Details", new { id = vm.CurrentOperationId });
            }
            return EditNewNameChange(vm.CurrentOperationId);
        }
    }
}