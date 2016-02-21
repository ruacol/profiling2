using System.Collections.Generic;
using System.Linq;
using NHibernate.Envers.Configuration.Attributes;
using Profiling2.Domain.Extensions;
using Profiling2.Domain.Prf.Careers;
using Profiling2.Domain.Prf.Responsibility;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Prf.Units
{
    public class Operation : Entity, IIncompleteDate
    {
        [Audited]
        public virtual string Name { get; set; }
        [Audited]
        public virtual string Objective { get; set; }
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
        [Audited]
        public virtual Operation NextOperation { get; set; }

        [Audited]
        public virtual IList<OperationAlias> OperationAliases { get; set; }
        [Audited]
        public virtual IList<UnitOperation> UnitOperations { get; set; }
        [Audited]
        public virtual IList<OperationSource> OperationSources { get; set; }
        [Audited]
        public virtual IList<Operation> FormerOperations { get; set; }

        public Operation()
        {
            this.OperationAliases = new List<OperationAlias>();
            this.UnitOperations = new List<UnitOperation>();
            this.OperationSources = new List<OperationSource>();
            this.FormerOperations = new List<Operation>();
        }

        public virtual IList<Career> GetCommanders(bool includeNameChanges)
        {
            return this.UnitOperations.Where(x => x.IsCommandUnit)
                .Select(x => x.Unit.GetCommanders(includeNameChanges))
                .Aggregate(new List<Career>(), (x, y) => x.Concat(y).ToList())
                .ToList();
        }

        public virtual IList<Career> GetDeputyCommanders(bool includeNameChanges)
        {
            return this.UnitOperations.Where(x => x.IsCommandUnit)
                .Select(x => x.Unit.GetDeputyCommanders(includeNameChanges))
                .Aggregate(new List<Career>(), (x, y) => x.Concat(y).ToList())
                .ToList();
        }

        /// <summary>
        /// Group this operation, the immediate previous operation(s), and the next operation.
        /// 
        /// TODO: recursively traverse chain of operations forward and back.
        /// </summary>
        /// <returns></returns>
        protected virtual IList<Operation> GetNameChangeOperations()
        {
            IList<Operation> list = this.FormerOperations;
            list.Add(this);
            if (this.NextOperation != null)
                list.Add(this.NextOperation);
            return list;
        }

        public virtual IList<UnitOperation> GetUnitOperations(bool includeNameChanges)
        {
            IList<UnitOperation> list = this.UnitOperations;
            if (includeNameChanges)
                foreach (Operation o in this.GetNameChangeOperations())
                    list = list.Concat(o.UnitOperations).ToList();
            return list;
        }

        /// <summary>
        /// Aggregate all OrganizationResponsibilities from member Units.  Includes OrganizationResponsibilities from units linked by name changes.
        /// </summary>
        /// <param name="excludeEventsOutsideInvolvement">Don't include a responsibility if the Event occurred outside the Unit's involvement with this Operation.</param>
        /// <returns></returns>
        public virtual IList<OrganizationResponsibility> GetCombinedOrganizationResponsibilities(bool excludeEventsOutsideInvolvement, bool includeNameChanges)
        {
            IList<UnitOperation> list = this.GetUnitOperations(includeNameChanges);

            if (list != null && list.Count > 0)
            {
                if (excludeEventsOutsideInvolvement)
                {
                    IList<OrganizationResponsibility> filtered = new List<OrganizationResponsibility>();
                    foreach (UnitOperation uo in list)
                    {
                        if (uo.Unit != null && uo.Unit.OrganizationResponsibilities != null)
                        {
                            foreach (OrganizationResponsibility or in uo.Unit.GetOrganizationResponsibilities(true))
                            {
                                if (or.Event != null && uo.HasIntersectingDateWith(or.Event) && uo.Operation.HasIntersectingDateWith(or.Event))
                                {
                                    filtered.Add(or);
                                }
                            }
                        }
                    }
                    return filtered.Distinct().ToList();
                }
                else
                {
                    return this.UnitOperations.Select(x => x.Unit)
                        .Select(x => x.GetOrganizationResponsibilities(true))
                        .Aggregate((x, y) => x.Concat(y).ToList());
                }
            }
            return null;
        }

        public virtual IList<Career> GetAllCareers(bool includeNameChanges, bool canViewAndSearchRestrictedPersons)
        {
            return this.UnitOperations.Select(x => x.Unit)
                .Where(x => !x.Archive)
                .Select(x => x.GetCareers(includeNameChanges))
                .Aggregate((x, y) => x.Concat(y).ToList())
                .Where(x => !x.Archive)
                .Where(x => !x.Person.IsRestrictedProfile || canViewAndSearchRestrictedPersons)
                .ToList();
        }

        /// <summary>
        /// Get the Operation this Operation was formerly known as.  There should only be one, but the data model allows for several.
        /// </summary>
        /// <returns></returns>
        public virtual Operation GetFormerOperation()
        {
            if (this.FormerOperations.Any())
                return this.FormerOperations.First();
            return null;
        }

        public virtual bool HasNameChange()
        {
            return this.NextOperation != null || this.FormerOperations.Any();
        }

        public override string ToString()
        {
            string s = this.Name;
            foreach (OperationAlias a in this.OperationAliases)
                s += "<br />(a.k.a.) " + a.Name;
            if (this.FormerOperations.Any())
                s += "<br />(formerly known as) " + this.GetFormerOperation().Name;
            if (this.NextOperation != null)
                s += "<br />(changed name to) " + this.NextOperation.Name;
            return s;
        }

        // data modification methods below

        public virtual void AddOperationSource(OperationSource os)
        {
            if (this.OperationSources.Contains(os))
                return;

            this.OperationSources.Add(os);
        }

        public virtual void RemoveOperationSource(OperationSource os)
        {
            this.OperationSources.Remove(os);
        }
    }
}
