using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Envers.Configuration.Attributes;
using Profiling2.Domain.DTO;
using Profiling2.Domain.Prf.Careers;
using Profiling2.Domain.Prf.Events;
using Profiling2.Domain.Prf.Responsibility;
using Profiling2.Domain.Prf.Units;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Prf
{
    public class Location : Entity
    {
        [Audited]
        public virtual string LocationName { get; set; }
        [Audited]
        public virtual string Territory { get; set; }
        [Audited]
        public virtual string Town { get; set; }
        [Audited]
        public virtual Region Region { get; set; }
        [Audited(TargetAuditMode = RelationTargetAuditMode.NotAudited)]
        public virtual Province Province { get; set; }
        [Audited]
        public virtual float? Longitude { get; set; }
        [Audited]
        public virtual float? Latitude { get; set; }
        [Audited]
        public virtual bool Archive { get; set; }
        [Audited]
        public virtual string Notes { get; set; }

        public virtual IList<UnitLocation> UnitLocations { get; set; }
        public virtual IList<Career> Careers { get; set; }
        public virtual IList<Event> Events { get; set; }

        public Location()
        {
            this.UnitLocations = new List<UnitLocation>();
            this.Careers = new List<Career>();
            this.Events = new List<Event>();
        }

        public virtual bool HasNonZeroCoordinates()
        {
            return this.Latitude.HasValue && this.Longitude.HasValue 
                && this.Latitude.Value != 0 && this.Longitude.Value != 0;
        }

        public virtual bool IsNotUnknown()
        {
            return !new string[] 
            { 
                "Unknown", "unknown", "0", "00", "n/a", "N/A", "Not relevant", "not relevant"
            }.Contains(this.LocationName);
        }

        public virtual IDictionary<Unit, IList<EntityUnitDateDTO>> GetEntityUnitDTOs()
        {
            IDictionary<Unit, IList<EntityUnitDateDTO>> units = new Dictionary<Unit, IList<EntityUnitDateDTO>>();

            foreach (UnitLocation ul in this.UnitLocations.Where(x => !x.Archive && x.Unit != null))
            {
                if (!units.ContainsKey(ul.Unit))
                    units[ul.Unit] = new List<EntityUnitDateDTO>();
                units[ul.Unit].Add(new EntityUnitDateDTO(ul));
            }
            foreach (Career c in this.Careers.Where(x => x.Unit != null && !x.Archive))
            {
                if (!units.ContainsKey(c.Unit))
                    units[c.Unit] = new List<EntityUnitDateDTO>();
                units[c.Unit].Add(new EntityUnitDateDTO(c));
            }
            foreach (Event e in this.Events.Where(x => !x.Archive && x.OrganizationResponsibilities != null && x.OrganizationResponsibilities.Any()))
            {
                foreach (OrganizationResponsibility or in e.OrganizationResponsibilities.Where(x => !x.Archive && x.Unit != null))
                {
                    if (!units.ContainsKey(or.Unit))
                        units[or.Unit] = new List<EntityUnitDateDTO>();
                    units[or.Unit].Add(new EntityUnitDateDTO(or));
                }
            }

            return units;
        }

        /// <summary>
        /// Return a float between 0 and 1 where 1 is identical and 0 is not.
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        public virtual float GetSimilarity(Location l)
        {
            if (l != null)
            {
                if (this == l)
                    return 1;

                float f = 0;
                if (this.Province == l.Province)
                    f += (float)0.1;
                else if (this.Region == l.Region)
                    f += (float)0.1;
                if (this.Territory != "0" && string.Equals(this.Territory, l.Territory, StringComparison.InvariantCultureIgnoreCase))
                    f += (float)0.2;
                if (this.Town != "0" && string.Equals(this.Town, l.Town, StringComparison.InvariantCultureIgnoreCase))
                    f += (float)0.3;
                if (this.LocationName != "0" && string.Equals(this.LocationName, l.LocationName, StringComparison.InvariantCultureIgnoreCase))
                    f += (float)0.4;
                return f;
            }
            return 0;
        }

        public override string ToString()
        {
            string[] parts = new string[] { this.Town, this.Territory, this.Region != null ? this.Region.ToString() : string.Empty };
            string suffix = string.Join(", ", (from s in parts
                                               where !string.IsNullOrEmpty(s) && !string.Equals(s, "0") && !string.Equals(s, "unknown", StringComparison.InvariantCultureIgnoreCase)
                                               select s).ToArray<string>());
            return this.LocationName + (string.IsNullOrEmpty(suffix) ? string.Empty : ": " + suffix);
        }

        public virtual object ToJSON(bool includeStats)
        {
            IDictionary<string, object> json = new Dictionary<string, object>();
            json["Id"] = this.Id;
            json["Name"] = this.LocationName;
            json["Territory"] = this.Territory;
            json["Town"] = this.Town;
            json["Region"] = this.Region != null ? this.Region.RegionName : string.Empty;
            json["Province"] = this.Province != null ? this.Province.ProvinceName : string.Empty;
            json["Latitude"] = this.Latitude;
            json["Longitude"] = this.Longitude;
            json["Notes"] = this.Notes;

            if (includeStats)
            {
                json["NumEvents"] = this.Events.Count;
                json["NumCareers"] = this.Careers.Count;
                json["NumUnitLocations"] = this.UnitLocations.Count;
            }

            return json;
        }

        public virtual object ToShortJSON()
        {
            return new
                {
                    Id = this.Id,
                    Name = this.LocationName,
                    Label = !string.IsNullOrEmpty(this.Territory) ? this.Territory : 
                        (this.Province != null ? this.Province.ProvinceName : 
                            (this.Region != null ? this.Region.RegionName : this.LocationName)),  // select field to use as label in human rights record
                    FullName = this.ToString()
                };
        }

        public virtual void AddEvent(Event ev)
        {
            if (this.Events != null && !this.Events.Contains(ev))
                this.Events.Add(ev);
        }

        public virtual void RemoveEvent(Event ev)
        {
            if (this.Events != null && this.Events.Contains(ev))
                this.Events.Remove(ev);
        }

        public virtual void AddCareer(Career career)
        {
            if (this.Careers != null && !this.Careers.Contains(career))
                this.Careers.Add(career);
        }

        public virtual void RemoveCareer(Career career)
        {
            if (this.Careers != null && this.Careers.Contains(career))
                this.Careers.Remove(career);
        }

        public virtual void AddUnitLocation(UnitLocation ul)
        {
            if (this.UnitLocations != null && !this.UnitLocations.Contains(ul))
                this.UnitLocations.Add(ul);
        }

        public virtual void RemoveUnitLocation(UnitLocation ul)
        {
            if (this.UnitLocations != null && this.UnitLocations.Contains(ul))
                this.UnitLocations.Remove(ul);
        }
    }
}
