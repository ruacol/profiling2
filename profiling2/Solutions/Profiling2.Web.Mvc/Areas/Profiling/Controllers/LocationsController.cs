using AutoMapper;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Prf;
using Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels;
using Profiling2.Web.Mvc.Controllers;
using SharpArch.NHibernate.Web.Mvc;
using SharpArch.Web.Mvc.JsonNet;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers
{
    public class LocationsController : BaseController
    {
        protected readonly ILocationTasks locationTasks;
        protected readonly IEventTasks eventTasks;
        protected readonly ICareerTasks careerTasks;
        protected readonly IOrganizationTasks orgTasks;

        public LocationsController(ILocationTasks locationTasks, IEventTasks eventTasks, ICareerTasks careerTasks, IOrganizationTasks orgTasks)
        {
            this.locationTasks = locationTasks;
            this.eventTasks = eventTasks;
            this.careerTasks = careerTasks;
            this.orgTasks = orgTasks;
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchPersons)]
        public JsonNetResult Name(int id)
        {
            Location loc = this.locationTasks.GetLocation(id);
            if (loc != null)
            {
                return JsonNet(new
                {
                    Id = id,
                    Name = loc.ToString()
                });
            }
            else
                return JsonNet(string.Empty);
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchPersons)]
        public JsonNetResult Get()
        {
            string term = Request.QueryString["term"];
            IList<Location> locations;
            
            if (string.IsNullOrEmpty(term))
                locations = this.locationTasks.GetAllLocations();
            else
                locations = this.locationTasks.SearchLocations(term.Trim());

            return JsonNet((from loc in locations
                            select new { id = loc.Id, text = loc.ToString() }).ToList<object>());
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchPersons)]
        public JsonNetResult Json(int id)
        {
            Location loc = this.locationTasks.GetLocation(id);
            if (loc != null)
            {
                return JsonNet(loc.ToJSON(true));
            }
            else
                return JsonNet(string.Empty);
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchPersons)]
        public ActionResult Index()
        {
            return View(this.locationTasks.GetAllLocations());
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchPersons)]
        public ActionResult Details(int id)
        {
            Location loc = this.locationTasks.GetLocation(id);
            if (loc != null)
                return View(loc);
            return new HttpNotFoundResult();
        }

        [PermissionAuthorize(AdminPermission.CanChangePersons)]
        public ActionResult Create()
        {
            LocationViewModel vm = new LocationViewModel();
            vm.PopulateDropDowns(this.locationTasks.GetAllRegions(), this.locationTasks.GetAllProvinces());
            return View(vm);
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangePersons)]
        public ActionResult Create(LocationViewModel vm)
        {
            IList<Location> existing = this.locationTasks.GetLocations(vm.LocationName);
            if (existing != null && existing.Count > 0)
                ModelState.AddModelError("LocationName", "Location name already exists.");

            if (ModelState.IsValid)
            {
                Location loc = new Location();
                Mapper.Map<LocationViewModel, Location>(vm, loc);
                if (vm.RegionId.HasValue)
                    loc.Region = this.locationTasks.GetRegion(vm.RegionId.Value);
                if (vm.ProvinceId.HasValue)
                    loc.Province = this.locationTasks.GetProvince(vm.ProvinceId.Value);
                loc = this.locationTasks.SaveLocation(loc);
                return RedirectToAction("Details", new { id = loc.Id });
            }
            else
            {
                return Create();
            }
        }

        [PermissionAuthorize(AdminPermission.CanChangePersons)]
        public ActionResult CreateModal()
        {
            return Create();
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangePersons)]
        public JsonNetResult CreateModal(LocationViewModel vm)
        {
            IList<Location> existing = this.locationTasks.GetLocations(vm.LocationName);
            if (existing != null && existing.Count > 0)
                ModelState.AddModelError("LocationName", "Location name already exists.");

            if (ModelState.IsValid)
            {
                Location loc = new Location();
                Mapper.Map<LocationViewModel, Location>(vm, loc);
                if (vm.RegionId.HasValue)
                    loc.Region = this.locationTasks.GetRegion(vm.RegionId.Value);
                if (vm.ProvinceId.HasValue)
                    loc.Province = this.locationTasks.GetProvince(vm.ProvinceId.Value);
                loc = this.locationTasks.SaveLocation(loc);
                return JsonNet(new
                {
                    Id = loc.Id,
                    Name = loc.LocationName,
                    WasSuccessful = true
                });
            }
            else
            {
                return JsonNet(this.GetErrorsForJson());
            }
        }

        [PermissionAuthorize(AdminPermission.CanChangePersons)]
        public ActionResult Edit(int id)
        {
            Location loc = this.locationTasks.GetLocation(id);
            if (loc != null)
            {
                LocationViewModel vm = new LocationViewModel();
                Mapper.Map<Location, LocationViewModel>(loc, vm);
                if (loc.Region != null)
                    vm.RegionId = loc.Region.Id;
                vm.PopulateDropDowns(this.locationTasks.GetAllRegions(), this.locationTasks.GetAllProvinces());
                return View(vm);
            }
            return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangePersons)]
        public ActionResult Edit(LocationViewModel vm)
        {
            Location loc = this.locationTasks.GetLocation(vm.Id);
            IList<Location> existing = this.locationTasks.GetLocations(vm.LocationName);
            if (loc != null && existing != null && existing.Count > 0 && !existing.Select(x => x.Id).Contains(loc.Id))
                ModelState.AddModelError("LocationName", "Location name already exists.");

            if (ModelState.IsValid)
            {
                Mapper.Map<LocationViewModel, Location>(vm, loc);
                if (vm.RegionId.HasValue)
                    loc.Region = this.locationTasks.GetRegion(vm.RegionId.Value);
                if (vm.ProvinceId.HasValue)
                    loc.Province = this.locationTasks.GetProvince(vm.ProvinceId.Value);
                loc = this.locationTasks.SaveLocation(loc);
                return RedirectToAction("Details", new { id = loc.Id });
            }
            else
            {
                return Edit(vm.Id);
            }
        }

        [PermissionAuthorize(AdminPermission.CanChangePersons)]
        public ActionResult EditModal(int id)
        {
            return Edit(id);
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangePersons)]
        public JsonNetResult EditModal(LocationViewModel vm)
        {
            Location loc = this.locationTasks.GetLocation(vm.Id);
            IList<Location> existing = this.locationTasks.GetLocations(vm.LocationName);
            if (loc != null && existing != null && existing.Count > 0 && !existing.Select(x => x.Id).Contains(loc.Id))
                ModelState.AddModelError("LocationName", "Location name already exists.");

            if (ModelState.IsValid)
            {
                Mapper.Map<LocationViewModel, Location>(vm, loc);
                if (vm.RegionId.HasValue)
                    loc.Region = this.locationTasks.GetRegion(vm.RegionId.Value);
                if (vm.ProvinceId.HasValue)
                    loc.Province = this.locationTasks.GetProvince(vm.ProvinceId.Value);
                loc = this.locationTasks.SaveLocation(loc);
                return JsonNet(string.Empty);
            }
            else
            {
                return JsonNet(this.GetErrorsForJson());
            }
        }

        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangePersons)]
        public ActionResult Delete(int id)
        {
            Location loc = this.locationTasks.GetLocation(id);
            if (loc != null)
            {
                if (this.locationTasks.DeleteLocation(loc))
                    return RedirectToAction("Index");
                else
                    return RedirectToAction("Details", new { id = id });
            }
            return new HttpNotFoundResult();
        }

        [PermissionAuthorize(AdminPermission.CanChangePersons)]
        public ActionResult MergeInto(int id)
        {
            Location loc = this.locationTasks.GetLocation(id);
            if (loc != null)
            {
                LocationMergeViewModel vm = new LocationMergeViewModel();
                Mapper.Map<Location, LocationMergeViewModel>(loc, vm);
                if (loc.Region != null)
                    vm.RegionId = loc.Region.Id;
                vm.NumEvents = loc.Events.Count;
                vm.NumCareers = loc.Careers.Count;
                vm.NumUnitLocations = loc.UnitLocations.Count;
                vm.PopulateDropDowns(this.locationTasks.GetAllRegions(), this.locationTasks.GetAllProvinces());
                return View(vm);
            }
            return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangePersons)]
        public ActionResult MergeInto(LocationMergeViewModel vm)
        {
            if (ModelState.IsValid)
            {
                if (vm.Id != vm.ToDeleteLocationId)
                {
                    Location toKeep = this.locationTasks.GetLocation(vm.Id);
                    Mapper.Map<LocationMergeViewModel, Location>(vm, toKeep);
                    if (vm.RegionId.HasValue)
                        toKeep.Region = this.locationTasks.GetRegion(vm.RegionId.Value);
                    if (vm.ProvinceId.HasValue)
                        toKeep.Province = this.locationTasks.GetProvince(vm.ProvinceId.Value);

                    Location toDelete = this.locationTasks.GetLocation(vm.ToDeleteLocationId);

                    // currently talks straight to database; Lucene indexes not updated, normal audit trail skipped
                    this.locationTasks.MergeLocations(toKeep.Id, toDelete.Id);

                    this.locationTasks.DeleteLocation(toDelete);

                    return RedirectToAction("Details", new { id = toKeep.Id });
                }
                else
                {
                    ModelState.AddModelError("ToDeleteLocationId", "Can't merge same location.");
                    return MergeInto(vm.Id);
                }
            }
            else
            {
                return MergeInto(vm.Id);
            }
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchPersons)]
        public ActionResult Map()
        {
            return View();
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchPersons)]
        public JsonNetResult GetLocationswithCoords()
        {
            return JsonNet(this.locationTasks.GetLocationsWithCoords().Select(x => x.ToJSON(false)));
        }
    }
}