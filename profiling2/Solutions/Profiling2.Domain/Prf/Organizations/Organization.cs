using System;
using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;
using Profiling2.Domain.Prf.Careers;
using Profiling2.Domain.Prf.Responsibility;
using Profiling2.Domain.Prf.Units;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Prf.Organizations
{
    public class Organization : Entity, IIncompleteDate
    {
        [Audited]
        public virtual string OrgShortName { get; set; }
        [Audited]
        public virtual string OrgLongName { get; set; }
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
        public virtual DateTime? Created { get; set; }

        [Audited]
        public virtual IList<Career> Careers { get; set; }
        [Audited]
        public virtual IList<OrganizationResponsibility> OrganizationResponsibilities { get; set; }
        public virtual IList<OrganizationRelationship> OrganizationRelationshipsAsSubject { get; set; }
        public virtual IList<OrganizationRelationship> OrganizationRelationshipsAsObject { get; set; }
        public virtual IList<OrganizationAlias> OrganizationAliases { get; set; }
        public virtual IList<OrganizationSource> OrganizationSources { get; set; }
        public virtual IList<OrganizationPhoto> OrganizationPhotos { get; set; }
        public virtual IList<Unit> Units { get; set; }

        public Organization()
        {
            this.Careers = new List<Career>();
            this.OrganizationResponsibilities = new List<OrganizationResponsibility>();
            this.OrganizationRelationshipsAsSubject = new List<OrganizationRelationship>();
            this.OrganizationRelationshipsAsObject = new List<OrganizationRelationship>();
            this.OrganizationAliases = new List<OrganizationAlias>();
            this.OrganizationSources = new List<OrganizationSource>();
            this.OrganizationPhotos = new List<OrganizationPhoto>();
            this.Units = new List<Unit>();
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(this.OrgShortName))
                return this.OrgLongName;
            else
            {
                if (string.IsNullOrEmpty(this.OrgLongName))
                    return this.OrgShortName;
                else
                    return this.OrgLongName + " (" + this.OrgShortName + ")";
            }
        }

        public virtual bool AddOrganizationResponsibility(OrganizationResponsibility or)
        {
            foreach (OrganizationResponsibility o in this.OrganizationResponsibilities)
            {
                if (o.Event.Id == or.Event.Id)
                {
                    if (o.Unit == null && or.Unit == null)
                        return false;
                    else if (o.Unit != null && or.Unit != null && o.Unit.Id == or.Unit.Id)
                        return false;
                }
            }

            this.OrganizationResponsibilities.Add(or);
            or.Event.AddOrganizationResponsibility(or);
            return true;
        }

        public virtual void RemoveOrganizationResponsibility(OrganizationResponsibility or)
        {
            this.OrganizationResponsibilities.Remove(or);
        }

        public virtual void AddOrganizationSource(OrganizationSource os)
        {
            if (this.OrganizationSources.Contains(os))
                return;

            this.OrganizationSources.Add(os);
        }

        public virtual void RemoveOrganizationSource(OrganizationSource os)
        {
            this.OrganizationSources.Remove(os);
        }
    }
}
