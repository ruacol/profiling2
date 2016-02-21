using System.Collections.Generic;
using System.Linq;
using Profiling2.Domain.Contracts.Queries;
using Profiling2.Domain.Contracts.Queries.Search;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Prf;
using SharpArch.NHibernate.Contracts.Repositories;

namespace Profiling2.Tasks
{
    public class LocationTasks : ILocationTasks
    {
        protected readonly INHibernateRepository<Location> locationRepo;
        protected readonly INHibernateRepository<Region> regionRepo;
        protected readonly INHibernateRepository<Province> provinceRepo;
        protected readonly ILocationSearchQuery locationSearchQuery;
        protected readonly ILuceneTasks luceneTasks;
        protected readonly ILocationMergeQuery mergeQuery;

        public LocationTasks(INHibernateRepository<Location> locationRepo,
            INHibernateRepository<Region> regionRepo,
            INHibernateRepository<Province> provinceRepo,
            ILocationSearchQuery locationSearchQuery,
            ILuceneTasks luceneTasks,
            ILocationMergeQuery mergeQuery)
        {
            this.locationRepo = locationRepo;
            this.regionRepo = regionRepo;
            this.provinceRepo = provinceRepo;
            this.locationSearchQuery = locationSearchQuery;
            this.luceneTasks = luceneTasks;
            this.mergeQuery = mergeQuery;
        }

        public Location GetLocation(int id)
        {
            return this.locationRepo.Get(id);
        }

        public IList<Location> GetLocations(string name)
        {
            IDictionary<string, object> criteria = new Dictionary<string, object>();
            criteria.Add("LocationName", name);
            return this.locationRepo.FindAll(criteria);
        }

        protected IList<Location> GetLocations(string town, string territory, Province province)
        {
            IDictionary<string, object> criteria = new Dictionary<string, object>();
            criteria.Add("Town", town);
            criteria.Add("Territory", territory);
            criteria.Add("Province", province);
            return this.locationRepo.FindAll(criteria);
        }

        protected IList<Location> GetLocations(string town, string territory, Region region)
        {
            IDictionary<string, object> criteria = new Dictionary<string, object>();
            criteria.Add("Town", town);
            criteria.Add("Territory", territory);
            criteria.Add("Region", region);
            return this.locationRepo.FindAll(criteria);
        }

        public IList<Location> GetAllLocations()
        {
            return this.locationRepo.GetAll().OrderBy(x => x.LocationName).ToList<Location>();
        }

        public IList<Location> GetLocationsWithCoords()
        {
            return this.locationSearchQuery.GetLocationsWithCoords();
        }

        public Location GetOrCreateLocation(string address, string town, string territory, string province, float? latitude, float? longitude)
        {
            Location l = null;

            Province p = this.GetProvince(province);
            Region r = this.GetRegion(province);

            // check for existing Location using Location.Province
            IList<Location> candidates = this.GetLocations(town, territory, p);
            if (candidates == null || (candidates != null && !candidates.Any()))
                candidates = this.GetLocations(town, territory, r);  // check for existing Location using Location.Region

            if (candidates != null && candidates.Any())
                l = candidates[0];

            // create new Location if necessary
            if (l == null)
            {
                l = new Location();
                l.LocationName = !string.IsNullOrEmpty(address) ? address :
                    (!string.IsNullOrEmpty(town) ? town :
                        (!string.IsNullOrEmpty(territory) ? territory :
                            (!string.IsNullOrEmpty(province) ? province : "(no name)")));
                l.Town = town;
                l.Territory = territory;
                l.Region = r;
                l.Province = p;
                if (latitude.HasValue)
                    l.Latitude = latitude.Value;
                if (longitude.HasValue)
                    l.Longitude = longitude.Value;
                l = this.SaveLocation(l);
            }

            return l;
        }

        public Location SaveLocation(Location loc)
        {
            this.luceneTasks.UpdatePersons(loc);
            return this.locationRepo.SaveOrUpdate(loc);
        }

        public IList<Location> SearchLocations(string term)
        {
            return this.locationSearchQuery.GetResults(term);
        }

        public bool DeleteLocation(Location loc)
        {
            if (loc != null && loc.Careers.Count == 0 && loc.Events.Count == 0 && loc.UnitLocations.Count == 0)
            {
                this.locationRepo.Delete(loc);
                return true;
            }
            return false;
        }

        public Region GetRegion(int id)
        {
            return this.regionRepo.Get(id);
        }

        public Region GetRegion(string name)
        {
            IDictionary<string, object> criteria = new Dictionary<string, object>();
            criteria.Add("RegionName", name);
            return this.regionRepo.FindOne(criteria);
        }

        public IList<Region> GetAllRegions()
        {
            return this.regionRepo.GetAll().OrderBy(x => x.RegionName).ToList<Region>();
        }

        public IList<object> GetRegionsJson(string term)
        {
            IList<Region> regions = (from region in this.regionRepo.GetAll()
                                     where region.RegionName.ToUpper().Contains(term.ToUpper())
                                     orderby region.ToString()
                                     select region).ToList<Region>();

            IList<object> objList = new List<object>();
            foreach (Region r in regions)
                objList.Add(new
                {
                    id = r.Id,
                    text = r.ToString()
                });
            return objList;
        }

        public Region SaveRegion(Region reg)
        {
            return this.regionRepo.SaveOrUpdate(reg);
        }

        public void DeleteRegion(Region reg)
        {
            if (reg != null && !reg.Locations.Any() && !reg.Persons.Any())
                this.regionRepo.Delete(reg);
        }

        public void MergeLocations(int toKeepId, int toDeleteId)
        {
            this.mergeQuery.MergeLocations(toKeepId, toDeleteId);
        }

        public IList<Province> GetAllProvinces()
        {
            return this.provinceRepo.GetAll().OrderBy(x => x.ProvinceName).ToList<Province>();
        }

        public Province GetProvince(int id)
        {
            return this.provinceRepo.Get(id);
        }

        public Province GetProvince(string name)
        {
            IDictionary<string, object> criteria = new Dictionary<string, object>();
            criteria.Add("ProvinceName", name);
            return this.provinceRepo.FindOne(criteria);
        }
    }
}
