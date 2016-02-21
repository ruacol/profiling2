using System.Collections.Generic;
using System.Linq;
using NHibernate.Envers.Configuration.Attributes;
using Profiling2.Domain.DTO;
using Profiling2.Domain.Extensions;
using Profiling2.Domain.Prf.Careers;
using Profiling2.Domain.Prf.Events;
using Profiling2.Domain.Prf.Organizations;
using Profiling2.Domain.Prf.Responsibility;
using Profiling2.Domain.Scr;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Prf.Units
{
    public class Unit : Entity, IIncompleteDate
    {
        [Audited]
        public virtual string UnitName { get; set; }
        [Audited]
        public virtual Organization Organization { get; set; }
        [Audited]
        public virtual string BackgroundInformation { get; set; }
        [Audited]
        public virtual int DayOfStart { get; set; }
        [Audited]
        public virtual int MonthOfStart { get; set; }
        [Audited]
        public virtual int YearOfStart { get; set; }
        [Audited]
        public virtual int DayOfEnd { get; set; }
        [Audited]
        public virtual int MonthOfEnd { get; set; }
        [Audited]
        public virtual int YearOfEnd { get; set; }
        [Audited]
        public virtual bool Archive { get; set; }
        [Audited]
        public virtual string Notes { get; set; }

        public virtual IList<Career> Careers { get; set; }
        public virtual IList<AdminUnitImport> AdminUnitImports { get; set; }
        public virtual IList<OrganizationResponsibility> OrganizationResponsibilities { get; set; }
        public virtual IList<UnitHierarchy> UnitHierarchies { get; set; }
        public virtual IList<UnitHierarchy> UnitHierarchyChildren { get; set; }
        public virtual IList<UnitLocation> UnitLocations { get; set; }

        [Audited]
        public virtual IList<UnitAlias> UnitAliases { get; set; }

        [Audited]
        public virtual IList<UnitSource> UnitSources { get; set; }

        [Audited]
        public virtual IList<UnitOperation> UnitOperations { get; set; }

        public virtual IList<RequestUnit> RequestUnits { get; set; }

        public Unit()
        {
            this.Careers = new List<Career>();
            this.AdminUnitImports = new List<AdminUnitImport>();
            this.OrganizationResponsibilities = new List<OrganizationResponsibility>();
            this.UnitHierarchies = new List<UnitHierarchy>();
            this.UnitHierarchyChildren = new List<UnitHierarchy>();
            this.UnitLocations = new List<UnitLocation>();
            this.UnitAliases = new List<UnitAlias>();
            this.UnitSources = new List<UnitSource>();
            this.UnitOperations = new List<UnitOperation>();
            this.RequestUnits = new List<RequestUnit>();
        }

        /// <summary>
        /// For audit purposes.
        /// </summary>
        public virtual string OrganizationNameSummary
        {
            get
            {
                return this.Organization == null ? string.Empty : this.Organization.ToString();
            }
        }

        public override string ToString()
        {
            return this.UnitName;
        }

        public virtual bool IsPartOf(Organization o)
        {
            return this.Organization == o;
        }

        public virtual IList<Career> GetCommanders(bool includeNameChanges)
        {
            IList<Career> careers = this.Careers;
            if (includeNameChanges)
                foreach (Unit u in this.GetNameChangeUnits())
                    careers = careers.Concat(u.Careers).ToList();
            return careers.Where(x => x.IsCurrentCareer && x.Role != null && x.Role.IsCommander)
                //.Select(x => x.Person)
                //.Distinct()
                .ToList();
        }

        public virtual IList<Career> GetDeputyCommanders(bool includeNameChanges)
        {
            IList<Career> careers = this.Careers;
            if (includeNameChanges)
                foreach (Unit u in this.GetNameChangeUnits())
                    careers = careers.Concat(u.Careers).ToList();
            return careers.Where(x => x.IsCurrentCareer && x.Role != null && x.Role.IsDeputyCommander)
                //.Select(x => x.Person)
                //.Distinct()
                .ToList();
        }

        /// <summary>
        /// Recursive method to find all parent name change unit hierarchies.
        /// </summary>
        /// <param name="hierarchies"></param>
        /// <returns></returns>
        public virtual IList<UnitHierarchy> GetParentChangedNameUnitHierarchiesRecursive(IList<UnitHierarchy> hierarchies)
        {
            var parents = this.UnitHierarchies
                .Where(x => !x.Archive && string.Equals(x.UnitHierarchyType.UnitHierarchyTypeName, UnitHierarchyType.NAME_CHANGED_NAME_TO))
                .OrderBy(x => x.ParentUnit.UnitName);

            if (parents.Any())
            {
                hierarchies = hierarchies.Concat(parents).ToList();
                foreach (UnitHierarchy parent in parents)
                    hierarchies = hierarchies.Concat(parent.ParentUnit.GetParentChangedNameUnitHierarchiesRecursive(hierarchies)).ToList();
            }

            return hierarchies.Distinct().ToList();
        }

        /// <summary>
        /// Recursive method to find all child name change unit hierarchies.
        /// </summary>
        /// <param name="hierarchies"></param>
        /// <returns></returns>
        public virtual IList<UnitHierarchy> GetChildChangedNameUnitHierarchiesRecursive(IList<UnitHierarchy> hierarchies)
        {
            var children = this.UnitHierarchyChildren
                .Where(x => !x.Archive && string.Equals(x.UnitHierarchyType.UnitHierarchyTypeName, UnitHierarchyType.NAME_CHANGED_NAME_TO))
                .OrderBy(x => x.ParentUnit.UnitName);

            if (children.Any())
            {
                hierarchies = hierarchies.Concat(children).ToList();
                foreach (UnitHierarchy child in children)
                    hierarchies = hierarchies.Concat(child.Unit.GetChildChangedNameUnitHierarchiesRecursive(hierarchies)).ToList();
            }

            return hierarchies.Distinct().ToList();
        }

        /// <summary>
        /// Get all (recursively) units linked to this one by name change.
        /// </summary>
        /// <returns></returns>
        protected virtual IList<Unit> GetNameChangeUnits()
        {
            return this.GetParentChangedNameUnitHierarchiesRecursive(new List<UnitHierarchy>()).Select(x => x.ParentUnit)
                .Concat(this.GetChildChangedNameUnitHierarchiesRecursive(new List<UnitHierarchy>()).Select(x => x.Unit))
                .Distinct()
                .ToList();
        }

        public virtual IList<UnitHierarchy> GetParentUnitHierarchies(bool includeNameChanges)
        {
            IList<UnitHierarchy> hierarchies = this.UnitHierarchies;
            if (includeNameChanges)
                foreach (Unit u in this.GetNameChangeUnits())
                    hierarchies = hierarchies.Concat(u.UnitHierarchies).ToList();
            return hierarchies
                .Where(x => !x.Archive && x.ParentUnit != null && string.Equals(x.UnitHierarchyType.UnitHierarchyTypeName, UnitHierarchyType.NAME_HIERARCHY))
                .OrderBy(x => x.ParentUnit.UnitName)
                .ToList();
        }

        public virtual IList<UnitHierarchy> GetChildUnitHierarchies(bool includeNameChanges)
        {
            IList<UnitHierarchy> hierarchies = this.UnitHierarchyChildren;
            if (includeNameChanges)
                foreach (Unit u in this.GetNameChangeUnits())
                    hierarchies = hierarchies.Concat(u.UnitHierarchyChildren).ToList();
            return hierarchies
                .Where(x => !x.Archive && x.Unit != null && string.Equals(x.UnitHierarchyType.UnitHierarchyTypeName, UnitHierarchyType.NAME_HIERARCHY))
                .OrderBy(x => x.Unit.UnitName)
                .ToList();
        }

        public virtual IList<UnitHierarchy> GetChildUnitHierarchiesRecursive(bool includeNameChanges, IList<UnitHierarchy> hierarchies)
        {
            var children = this.GetChildUnitHierarchies(includeNameChanges);

            if (children.Any())
            {
                hierarchies = hierarchies.Concat(children).ToList();
                foreach (UnitHierarchy child in children)
                    hierarchies = hierarchies.Concat(child.Unit.GetChildUnitHierarchiesRecursive(includeNameChanges, hierarchies)).ToList();
            }

            return hierarchies;
        }

        public virtual IList<Career> GetCareers(bool includeNameChanges)
        {
            IList<Career> careers = this.Careers;
            if (includeNameChanges)
                foreach (Unit u in this.GetNameChangeUnits())
                    careers = careers.Concat(u.Careers).ToList();
            return careers.Where(x => !x.Archive).ToList();
        }

        public virtual IList<OrganizationResponsibility> GetOrganizationResponsibilities(bool includeNameChanges)
        {
            IList<OrganizationResponsibility> rs = this.OrganizationResponsibilities;
            if (includeNameChanges)
                foreach (Unit u in this.GetNameChangeUnits())
                    rs = rs.Concat(u.OrganizationResponsibilities).ToList();
            return rs.Where(x => !x.Archive).ToList();
        }

        public virtual IList<UnitOperation> GetUnitOperations(bool includeNameChanges)
        {
            IList<UnitOperation> uos = this.UnitOperations;
            if (includeNameChanges)
                foreach (Unit u in this.GetNameChangeUnits())
                    uos = uos.Concat(u.UnitOperations).ToList();
            return uos.Distinct().ToList();
        }

        public virtual IList<UnitLocation> GetUnitLocations(bool includeNameChanges)
        {
            IList<UnitLocation> locs = this.UnitLocations;
            if (includeNameChanges)
                foreach (Unit u in this.GetNameChangeUnits())
                    locs = locs.Concat(u.UnitLocations).ToList();
            return locs.Where(x => !x.Archive).ToList();
        }

        public virtual IDictionary<Location, IList<EntityLocationDateDTO>> GetEntityLocationDTOs(bool includeNameChanges, bool onlyWithCoords)
        {
            IDictionary<Location, IList<EntityLocationDateDTO>> locs = new Dictionary<Location, IList<EntityLocationDateDTO>>();
            foreach (UnitLocation ul in this.GetUnitLocations(includeNameChanges).Where(x => x.Location != null))
            {
                if (!locs.ContainsKey(ul.Location))
                    locs[ul.Location] = new List<EntityLocationDateDTO>();
                locs[ul.Location].Add(new EntityLocationDateDTO(ul));
            }
            foreach (Career c in this.GetCareers(includeNameChanges).Where(x => x.Location != null))
            {
                if (!locs.ContainsKey(c.Location))
                    locs[c.Location] = new List<EntityLocationDateDTO>();
                locs[c.Location].Add(new EntityLocationDateDTO(c));
            }
            foreach (Event e in this.GetOrganizationResponsibilities(includeNameChanges).Select(x => x.Event).Where(x => x.Location != null))
            {
                if (!locs.ContainsKey(e.Location))
                    locs[e.Location] = new List<EntityLocationDateDTO>();
                locs[e.Location].Add(new EntityLocationDateDTO(e));
            }
            if (onlyWithCoords)
            {
                IDictionary<Location, IList<EntityLocationDateDTO>> filtered = new Dictionary<Location, IList<EntityLocationDateDTO>>();
                foreach (KeyValuePair<Location, IList<EntityLocationDateDTO>> kvp in locs.Where(x => x.Key.Longitude.HasValue && x.Key.Latitude.HasValue))
                {
                    filtered[kvp.Key] = kvp.Value;
                }
                return filtered;
            }
            return locs;
        }

        /// <summary>
        /// Returns name with dates in muted font (uses HTML tags).  Useful when displaying unit in dropdown results.
        /// </summary>
        /// <returns></returns>
        public virtual string GetNameWithDates()
        {
            string s = this.UnitName;
            if (this.HasStartDate() || this.HasEndDate())
            {
                s += " <span class='muted'>(";
                if (this.HasStartDate())
                    s += "from " + this.GetStartDateString();
                if (this.HasEndDate())
                    s += (this.HasStartDate() ? " " : string.Empty) + "until " + this.GetEndDateString();
                s += ")</span>";
            }
            return s;
        }

        // data modification methods below

        public virtual void AddUnitHierarchy(UnitHierarchy uh)
        {
            if (uh.ParentUnit == this)
            {
                if (this.UnitHierarchyChildren.Contains(uh))
                    return;
                this.UnitHierarchyChildren.Add(uh);
            }
            else if (uh.Unit == this)
            {
                if (this.UnitHierarchies.Contains(uh))
                    return;
                this.UnitHierarchies.Add(uh);
            }
        }

        public virtual void RemoveUnitHierarchy(UnitHierarchy uh)
        {
            if (uh.ParentUnit == this)
                this.UnitHierarchyChildren.Remove(uh);
            else if (uh.Unit == this)
                this.UnitHierarchies.Remove(uh);
        }

        public virtual void AddUnitSource(UnitSource us)
        {
            if (this.UnitSources.Contains(us))
                return;

            this.UnitSources.Add(us);
        }

        public virtual void RemoveUnitSource(UnitSource us)
        {
            this.UnitSources.Remove(us);
        }

        public virtual void AddOrganizationResponsibility(OrganizationResponsibility or)
        {
            if (this.OrganizationResponsibilities.Contains(or))
                return;

            this.OrganizationResponsibilities.Add(or);
        }

        public virtual void RemoveOrganizationResponsibility(OrganizationResponsibility or)
        {
            this.OrganizationResponsibilities.Remove(or);
        }

        public virtual void AddCareer(Career c)
        {
            if (this.Careers.Contains(c))
                return;

            this.Careers.Add(c);
        }

        public virtual void RemoveCareer(Career c)
        {
            this.Careers.Remove(c);
        }

        public virtual void AddUnitLocation(UnitLocation ul)
        {
            if (this.UnitLocations.Contains(ul))
                return;

            this.UnitLocations.Add(ul);
        }

        public virtual void RemoveUnitLocation(UnitLocation ul)
        {
            this.UnitLocations.Remove(ul);
        }

        public virtual void AddUnitAlias(UnitAlias ua)
        {
            if (this.UnitAliases.Contains(ua))
                return;

            this.UnitAliases.Add(ua);
        }

        public virtual void RemoveUnitAlias(UnitAlias ua)
        {
            this.UnitAliases.Remove(ua);
        }

        public virtual void AddAdminUnitImport(AdminUnitImport i)
        {
            if (this.AdminUnitImports.Contains(i))
                return;

            this.AdminUnitImports.Add(i);
        }

        public virtual void RemoveAdminUnitImport(AdminUnitImport i)
        {
            this.AdminUnitImports.Remove(i);
        }

        public virtual void AddUnitOperation(UnitOperation o)
        {
            if (this.UnitOperations.Contains(o))
                return;

            this.UnitOperations.Add(o);
        }

        public virtual void RemoveUnitOperation(UnitOperation o)
        {
            this.UnitOperations.Remove(o);
        }
    }
}
