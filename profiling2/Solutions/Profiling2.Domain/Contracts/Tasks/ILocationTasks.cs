using System.Collections.Generic;
using Profiling2.Domain.Prf;

namespace Profiling2.Domain.Contracts.Tasks
{
    public interface ILocationTasks
    {
        Location GetLocation(int id);

        IList<Location> GetLocations(string name);

        IList<Location> GetAllLocations();

        IList<Location> GetLocationsWithCoords();

        Location GetOrCreateLocation(string address, string town, string territory, string province, float? latitude, float? longitude);

        Location SaveLocation(Location loc);

        IList<Location> SearchLocations(string term);

        bool DeleteLocation(Location loc);

        Region GetRegion(int id);

        Region GetRegion(string name);

        IList<Region> GetAllRegions();

        IList<object> GetRegionsJson(string term);

        Region SaveRegion(Region reg);

        void DeleteRegion(Region reg);

        void MergeLocations(int toKeepId, int toDeleteId);

        IList<Province> GetAllProvinces();

        Province GetProvince(int id);

        Province GetProvince(string name);
    }
}
