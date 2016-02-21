using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using AutoMapper;
using Mvc.JQuery.Datatables;
using Profiling2.Domain;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Contracts.Tasks.Screenings;
using Profiling2.Domain.Contracts.Tasks.Sources;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Responsibility;
using Profiling2.Domain.Prf.Units;
using Profiling2.Infrastructure.Security;
using Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels;
using Profiling2.Web.Mvc.Controllers;
using SharpArch.NHibernate.Web.Mvc;
using SharpArch.Web.Mvc.JsonNet;
using StackExchange.Profiling;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers
{
    public class UnitsController : BaseController
    {
        protected readonly IOrganizationTasks orgTasks;
        protected readonly IScreeningTasks screeningTasks;
        protected readonly ILuceneTasks luceneTasks;
        protected readonly IUserTasks userTasks;
        protected readonly ISourceTasks sourceTasks;
        protected readonly IAuditTasks auditTasks;

        public UnitsController(IOrganizationTasks orgTasks, 
            IScreeningTasks screeningTasks, 
            ILuceneTasks luceneTasks, 
            IUserTasks userTasks, 
            ISourceTasks sourceTasks, 
            IAuditTasks auditTasks)
        {
            this.orgTasks = orgTasks;
            this.screeningTasks = screeningTasks;
            this.luceneTasks = luceneTasks;
            this.userTasks = userTasks;
            this.sourceTasks = sourceTasks;
            this.auditTasks = auditTasks;
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchUnits)]
        public JsonNetResult Get()
        {
            string term = Request.QueryString["term"];
            if (!string.IsNullOrEmpty(term))
            {
                IList<LuceneSearchResult> results = this.luceneTasks.UnitSearch(term.Trim(), 50);
                object[] objects = results.Select(x =>
                    {
                        UnitDataTableLuceneView view = new UnitDataTableLuceneView(x);
                        Unit unit = this.orgTasks.GetUnit(int.Parse(view.Id));
                        return new
                        {
                            id = view.Id,
                            text = unit == null ? view.Name : unit.GetNameWithDates()
                        };
                    }).ToArray();
                return JsonNet(objects);
            }
            return JsonNet(string.Empty);
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchUnits)]
        public JsonNetResult Name(int id)
        {
            Unit unit = this.orgTasks.GetUnit(id);
            if (unit != null)
            {
                return JsonNet(new
                {
                    Id = id,
                    Name = unit.ToString()
                });
            }
            else
                return JsonNet(string.Empty);
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchUnits)]
        public JsonNetResult Json(int id)
        {
            Unit unit = this.orgTasks.GetUnit(id);
            if (unit != null)
            {
                IDictionary<string, object> jsonObj = new Dictionary<string, object>();

                UnitViewModel vm = new UnitViewModel(unit);
                jsonObj.Add("Unit", vm);

                IList<UnitAliasViewModel> aliases = (from a in unit.UnitAliases
                                                     where !a.Archive
                                                     select new UnitAliasViewModel(a)).ToList<UnitAliasViewModel>();
                jsonObj.Add("UnitAliases", aliases);

                if (((PrfPrincipal)User).HasPermission(AdminPermission.CanViewAndSearchSources))
                {
                    IList<UnitSourceViewModel> sources = new List<UnitSourceViewModel>();
                    foreach (UnitSource us in unit.UnitSources.Where(x => !x.Archive))
                    {
                        UnitSourceViewModel usvm = new UnitSourceViewModel(us);
                        usvm.PopulateSource(this.sourceTasks.GetSourceDTO(us.Source.Id));
                        sources.Add(usvm);
                    }

                    jsonObj.Add("UnitSources", sources);
                }

                jsonObj.Add("OrganizationResponsibilities", (from or in unit.OrganizationResponsibilities
                                                             where !or.Archive
                                                             select new OrgResponsibilityViewModel(or)).ToList<OrgResponsibilityViewModel>());

                jsonObj.Add("Careers", (from c in unit.Careers
                                        where !c.Archive
                                        select new CareerViewModel(c)).ToList<CareerViewModel>());

                jsonObj.Add("UnitHierarchies", (from uh in unit.UnitHierarchies
                                                where !uh.Archive
                                                select new UnitHierarchyViewModel(uh)).ToList<UnitHierarchyViewModel>());

                jsonObj.Add("UnitHierarchyChildren", (from uh in unit.UnitHierarchyChildren
                                                      where !uh.Archive
                                                      select new UnitHierarchyViewModel(uh)).ToList<UnitHierarchyViewModel>());

                jsonObj.Add("UnitLocations", (from l in unit.UnitLocations
                                              where !l.Archive
                                              select new UnitLocationViewModel(l)).ToList<UnitLocationViewModel>());

                return JsonNet(jsonObj);
            }

            Response.StatusCode = (int)HttpStatusCode.NotFound;
            return JsonNet(string.Empty);
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchUnits)]
        public ActionResult Details(int id)
        {
            Unit u = this.orgTasks.GetUnit(id);
            if (u != null)
            {
                bool includeNameChanges = false;
                bool.TryParse(Request.QueryString["includeNameChanges"], out includeNameChanges);

                ViewBag.Commanders = u.GetCommanders(includeNameChanges);
                ViewBag.DeputyCommanders = u.GetDeputyCommanders(includeNameChanges);
                ViewBag.ParentNameChanges = u.GetParentChangedNameUnitHierarchiesRecursive(new List<UnitHierarchy>()).Distinct().ToList();
                ViewBag.ChildNameChanges = u.GetChildChangedNameUnitHierarchiesRecursive(new List<UnitHierarchy>()).Distinct().ToList();
                ViewBag.ParentUnitHierarchies = u.GetParentUnitHierarchies(includeNameChanges);
                ViewBag.ChildUnitHierarchies = u.GetChildUnitHierarchies(includeNameChanges);
                ViewBag.Careers = ((PrfPrincipal)User).HasPermission(AdminPermission.CanViewAndSearchRestrictedPersons)
                    ? u.GetCareers(includeNameChanges)
                    : u.GetCareers(includeNameChanges).Where(x => !x.Person.IsRestrictedProfile).ToList();
                ViewBag.OrganizationResponsibilities = u.GetOrganizationResponsibilities(includeNameChanges);
                ViewBag.UnitOperations = u.GetUnitOperations(includeNameChanges);
                ViewBag.UnitLocations = u.GetUnitLocations(includeNameChanges);
                ViewBag.CombinedLocations = u.GetEntityLocationDTOs(includeNameChanges, false);
                return View(u);
            }
            return new HttpNotFoundResult();
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchUnits)]
        public ActionResult Indirects(int id)
        {
            Unit u = this.orgTasks.GetUnit(id);
            if (u != null)
            {
                ViewBag.Unit = u;

                bool includeNameChanges = false;
                bool.TryParse(Request.QueryString["includeNameChanges"], out includeNameChanges);

                var profiler = MiniProfiler.Current;

                IList<UnitHierarchy> hierarchies;
                using (profiler.Step("GetChildUnitHierarchiesRecursive"))
                    hierarchies = u.GetChildUnitHierarchiesRecursive(includeNameChanges, new List<UnitHierarchy>());

                IEnumerable<IList<OrganizationResponsibility>> responsibilityLists;
                using (profiler.Step("GetOrganizationResponsibilities"))
                    responsibilityLists = hierarchies.Select(x => x.Unit.GetOrganizationResponsibilities(includeNameChanges));

                // this turns out to be much slower
                //IList<OrganizationResponsibility> indirectOrganizationResponsibilities;
                //using (profiler.Step("Aggregate and distinct ors"))
                //    indirectOrganizationResponsibilities = ors
                //        .Aggregate(new List<OrganizationResponsibility>(), (x, y) => x.Concat(y).ToList())
                //        .Distinct().ToList();

                IList<OrganizationResponsibility> responsibilities = new List<OrganizationResponsibility>();
                using (profiler.Step("Manual aggregate"))
                    foreach (IList<OrganizationResponsibility> list in responsibilityLists)
                        foreach (OrganizationResponsibility or in list)
                            if (!responsibilities.Contains(or))
                                responsibilities.Add(or);
                return View(responsibilities);
            }
            return new HttpNotFoundResult();
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchUnits)]
        public ActionResult Locations(int id)
        {
            Unit u = this.orgTasks.GetUnit(id);
            if (u != null)
            {
                return View(u);
            }
            return new HttpNotFoundResult();
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchUnits)]
        public JsonNetResult GetCombinedLocations(int id)
        {
            Unit u = this.orgTasks.GetUnit(id);
            if (u != null)
            {
                bool includeNameChanges = false;
                bool.TryParse(Request.QueryString["includeNameChanges"], out includeNameChanges);

                return JsonNet(u.GetEntityLocationDTOs(includeNameChanges, true).Select(x => new
                    {
                        Id = x.Key.Id,
                        Name = x.Key.LocationName,
                        Latitude = x.Key.Latitude,
                        Longitude = x.Key.Longitude,
                        Dates = x.Value.Distinct().Select(y => new
                            {
                                StartDateString = y.StartDateString,
                                AsOfDateString = y.AsOfDateString,
                                EndDateString = y.EndDateString
                            })
                    }));
            }
            return JsonNet(string.Empty);
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchUnits)]
        public ActionResult Audit(int id)
        {
            Unit u = this.orgTasks.GetUnit(id);
            if (u != null)
            {
                ViewBag.Unit = u;
                ViewBag.OldAuditTrail = this.auditTasks.GetUnitOldAuditTrail(id);
                ViewBag.AuditTrail = this.auditTasks.GetUnitAuditTrail(id);
                ViewBag.Users = this.userTasks.GetAllAdminUsers();
                return View();
            }
            return new HttpNotFoundResult();
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchUnits)]
        public ActionResult Members(int id)
        {
            Unit u = this.orgTasks.GetUnit(id);
            if (u != null)
                return View(u);
            return new HttpNotFoundResult();
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchUnits)]
        public ActionResult Index()
        {
            return View(this.orgTasks.GetAllUnits());
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchUnits)]
        public JsonNetResult DataTables(DataTablesParam p)
        {
            // calculate total results to request from lucene search
            int numResults = (p.iDisplayStart >= 0 && p.iDisplayLength > 0) ? (p.iDisplayStart + 1) * p.iDisplayLength : 10;

            IList<LuceneSearchResult> results = this.luceneTasks.UnitSearch(p.sSearch, numResults);

            int iTotalRecords = 0;
            if (results != null && results.Count > 0)
                iTotalRecords = results.First().TotalHits;

            object[] aaData = results
                .Select(x => new UnitDataTableLuceneView(x))
                .Skip(p.iDisplayStart)
                .Take(p.iDisplayLength)
                .ToArray<UnitDataTableLuceneView>();

            return JsonNet(new DataTablesData
            {
                iTotalRecords = iTotalRecords,
                iTotalDisplayRecords = iTotalRecords,
                sEcho = p.sEcho,
                aaData = aaData.ToArray()
            });
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchUnits)]
        public ActionResult Current()
        {
            return View(this.orgTasks.GetAllUnits());
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchUnits)]
        public ActionResult Chart(int id)
        {
            Unit u = this.orgTasks.GetUnit(id);
            if (u != null)
                return View(u);
            return new HttpNotFoundResult();
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchUnits)]
        public JsonNetResult Hierarchy(int id)
        {
            Unit u = this.orgTasks.GetUnit(id);
            if (u != null)
            {
                string hierarchyType = "Hierarchy";
                if (!string.IsNullOrEmpty(Request.QueryString["hierarchyType"]))
                    hierarchyType = Request.QueryString["hierarchyType"];

                return JsonNet(new
                {
                    UnitHierarchies = (from uh in u.UnitHierarchies.Where(x => string.Equals(hierarchyType, x.UnitHierarchyType.UnitHierarchyTypeName)) 
                                       select new UnitHierarchyViewModel(uh)).ToArray<UnitHierarchyViewModel>(),
                    UnitHierarchyChildren = (from uh in u.UnitHierarchyChildren.Where(x => string.Equals(hierarchyType, x.UnitHierarchyType.UnitHierarchyTypeName)) 
                                             select new UnitHierarchyViewModel(uh)).ToArray<UnitHierarchyViewModel>()
                });
            }
            return JsonNet(string.Empty);
        }

        [PermissionAuthorize(AdminPermission.CanChangeUnits)]
        public ActionResult Edit(int id)
        {
            Unit unit = this.orgTasks.GetUnit(id);
            if (unit != null)
                return View(Mapper.Map(unit, new UnitViewModel()));
            return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangeUnits)]
        public ActionResult Edit(UnitViewModel vm)
        {
            if (ModelState.IsValid)
            {
                Unit unit = this.orgTasks.GetUnit(vm.Id);
                unit = Mapper.Map(vm, unit);
                if (vm.OrganizationId.HasValue)
                    unit.Organization = this.orgTasks.GetOrganization(vm.OrganizationId.Value);
                else
                    unit.Organization = null;
                unit = this.orgTasks.SaveUnit(unit);
                return RedirectToAction("Details", new { id = vm.Id });
            }
            return Edit(vm.Id);
        }

        [PermissionAuthorize(AdminPermission.CanChangeUnits)]
        public ActionResult EditModal(int id)
        {
            return Edit(id);
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangeUnits)]
        public JsonNetResult EditModal(UnitViewModel vm)
        {
            if (ModelState.IsValid)
            {
                Unit unit = this.orgTasks.GetUnit(vm.Id);
                unit = Mapper.Map(vm, unit);
                if (vm.OrganizationId.HasValue)
                    unit.Organization = this.orgTasks.GetOrganization(vm.OrganizationId.Value);
                else
                    unit.Organization = null;
                unit = this.orgTasks.SaveUnit(unit);
                return JsonNet(string.Empty);
            }
            return JsonNet(this.GetErrorsForJson());
        }

        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangeUnits)]
        public ActionResult Delete(int id)
        {
            Unit unit = this.orgTasks.GetUnit(id);
            if (unit != null)
            {
                if (this.orgTasks.DeleteUnit(unit))
                    return RedirectToAction("Index");
                else
                    return RedirectToAction("Details", new { id = id });
            }
            return new HttpNotFoundResult();
        }

        [PermissionAuthorize(AdminPermission.CanChangeUnits)]
        public ActionResult Create()
        {
            UnitViewModel vm = new UnitViewModel();
            return View(vm);
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangeUnits)]
        public ActionResult Create(UnitViewModel vm)
        {
            if (ModelState.IsValid)
            {
                Unit unit = Mapper.Map(vm, new Unit());
                if (vm.OrganizationId.HasValue)
                    unit.Organization = this.orgTasks.GetOrganization(vm.OrganizationId.Value);
                unit = this.orgTasks.SaveUnit(unit);
                return RedirectToAction("Details", new { id = unit.Id });
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
        public JsonNetResult CreateModal(UnitViewModel vm)
        {
            if (ModelState.IsValid)
            {
                Unit unit = Mapper.Map(vm, new Unit());
                if (vm.OrganizationId.HasValue)
                    unit.Organization = this.orgTasks.GetOrganization(vm.OrganizationId.Value);
                unit = this.orgTasks.SaveUnit(unit);
                return JsonNet(new
                {
                    Id = unit.Id,
                    Name = unit.UnitName,
                    WasSuccessful = true
                });
            }
            return JsonNet(this.GetErrorsForJson());
        }

        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangeUnits)]
        public ActionResult PopulateOrganization()
        {
            this.orgTasks.PopulateUnitOrganization();
            return RedirectToAction("Index");
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchUnits)]
        public ActionResult Screenings(int id)
        {
            Unit u = this.orgTasks.GetUnit(id);
            if (u != null)
            {
                ViewBag.Careers = ((PrfPrincipal)User).HasPermission(AdminPermission.CanViewAndSearchRestrictedPersons)
                    ? u.GetCareers(false)
                    : u.GetCareers(false).Where(x => !x.Person.IsRestrictedProfile).ToList();

                ViewBag.ScreeningEntities = this.screeningTasks.GetScreeningEntities();

                return View(u);
            }
            return new HttpNotFoundResult();
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchUnits)]
        public ActionResult EmptyUnits()
        {
            return View(this.orgTasks.GetEmptyUnits());
        }

        [PermissionAuthorize(AdminPermission.CanChangeUnits)]
        public ActionResult Merge()
        {
            return View();
        }

        [HttpPost]
        [PermissionAuthorize(AdminPermission.CanChangeUnits)]
        public JsonNetResult Merge(MergeViewModel vm)
        {
            if (ModelState.IsValid)
            {
                AdminUser user = this.userTasks.GetAdminUser(User.Identity.Name);

                if (user != null)
                {
                    if (this.orgTasks.MergeUnits(vm.ToKeepId, vm.ToDeleteId, user.UserID, ((PrfPrincipal)User).HasPermission(AdminPermission.CanViewPersonResponsibilities)) == 1)
                    {
                        return JsonNet(null);
                    }
                    else
                    {
                        Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        return JsonNet(new { merge = "There was a database error merging the units." });
                    }
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return JsonNet(new { user = "There was a problem identifying the logged-in user." });
                }
            }
            Response.StatusCode = (int)HttpStatusCode.NotFound;
            return JsonNet(GetErrorsForJson());
        }

        [PermissionAuthorize(AdminPermission.CanChangeUnits)]
        public ActionResult AddOperationModal(int unitId)
        {
            UnitOperationViewModel vm = new UnitOperationViewModel();
            Unit unit = this.orgTasks.GetUnit(unitId);
            if (unit != null)
            {
                vm.UnitName = unit.UnitName;
            }
            return View(vm);
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangeUnits)]
        public JsonNetResult AddOperationModal(UnitOperationViewModel vm)
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
    }
}