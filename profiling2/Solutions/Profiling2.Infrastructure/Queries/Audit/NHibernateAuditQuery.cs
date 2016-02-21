using System;
using System.Collections.Generic;
using System.Linq;
using KellermanSoftware.CompareNetObjects;
using log4net;
using NHibernate.Envers;
using Profiling2.Domain;
using Profiling2.Domain.Prf.Events;
using Profiling2.Domain.Prf.Units;
using SharpArch.Domain.DomainModel;
using SharpArch.NHibernate;

namespace Profiling2.Infrastructure.Queries.Audit
{
    public class NHibernateAuditQuery : NHibernateQuery
    {
        protected readonly ILog log = LogManager.GetLogger(typeof(NHibernateAuditQuery));

        protected CompareLogic _compareLogic { get; set; }
        public CompareLogic CompareLogic
        {
            get
            {
                if (this._compareLogic == null)
                {
                    this._compareLogic = new CompareLogic();
                    this._compareLogic.Config.TreatStringEmptyAndNullTheSame = true;
                    this._compareLogic.Config.MaxDifferences = 100;
                    this._compareLogic.Config.MembersToInclude = new List<string>();
                    this._compareLogic.Config.MembersToIgnore = new List<string>()
                    {
                        "REVINFO", "RevisionType"
                    };
                }
                return this._compareLogic;
            }
        }

        protected IList<AuditTrailDTO> TransformToDto(IList<object[]> revisions)
        {
            return (from row in revisions
                    orderby ((REVINFO)row[1]).REVTSTMP descending
                    select new AuditTrailDTO()
                    {
                        Entity = (Entity)row[0],
                        REVINFO = (REVINFO)row[1],
                        RevisionType = (RevisionType)row[2]
                    }).ToList<AuditTrailDTO>();
        }

        protected IList<AuditTrailDTO> AddDifferences<T>(IList<AuditTrailDTO> trail)
        {
            CompareLogic compareLogic = new CompareLogic();
            compareLogic.Config.CompareChildren = false;
            compareLogic.Config.TreatStringEmptyAndNullTheSame = true;
            compareLogic.Config.MaxDifferences = 100;

            if (typeof(T).Equals(typeof(Event)))
            {
                compareLogic.Config.MembersToInclude = new List<string>()
                {
                    "EventName", "NarrativeEn", "NarrativeFr", "Commentary", "Archive", "Notes", "StartDate", "EndDate", "EventVerifiedStatusName", "JhroCaseNumbers",
                    "LocationNameSummary", "ViolationNames"
                };
            }
            else if (typeof(T).Equals(typeof(Unit)))
            {
                compareLogic.Config.MembersToInclude = new List<string>()
                {
                    "UnitName", "BackgroundInformation", "DayOfStart", "MonthOfStart", "YearOfStart", "DayOfEnd", "MonthOfEnd", "YearOfEnd", "Notes",
                    "OrganizationNameSummary"
                };
            }
            else if (typeof(T).Equals(typeof(Operation)))
            {
                compareLogic.Config.MembersToInclude = new List<string>()
                {
                    "Name", "Objective", "DayOfStart", "MonthOfStart", "YearOfStart", "DayOfEnd", "MonthOfEnd", "YearOfEnd", "Notes"
                };
            }

            for (int i = 0; i < trail.Count; i++)
            {
                trail[i].Differences = new List<Difference>();
                Entity current = trail[i].Entity as Entity;

                // select last Entity to compare with (last as in the previous revision of the current Entity instance)
                Entity last = null;
                if (trail[i].REVINFO.REVTSTMP.HasValue)
                {
                    IList<AuditTrailDTO> previousRevisions = trail
                        .Where(x => x.REVINFO.REVTSTMP.HasValue
                            && x.REVINFO.REVTSTMP.Value < trail[i].REVINFO.REVTSTMP.Value
                            && x.Entity.Id == trail[i].Entity.Id)
                        .ToList();

                    if (previousRevisions.Any())
                        last = previousRevisions
                            .Aggregate((agg, next) => next.REVINFO.REVTSTMP.Value > agg.REVINFO.REVTSTMP.Value ? next : agg)  // max
                            .Entity;
                }

                try
                {
                    if (trail[i].REVINFO.Id != AuditableExtensions.BASE_REVISION_ID)  // don't show diffs for base revision
                    {
                        ComparisonResult result = compareLogic.Compare(current, last);
                        if (!result.AreEqual)
                            trail[i].Differences = trail[i].Differences.Concat(result.Differences).ToList();
                    }
                }
                catch (Exception ex)
                {
                    this.log.Error("Object doesn't exist in audit history...", ex);
                }
            }

            return trail;
        }

        protected int GetRevisionNumberForDate(DateTime date)
        {
            // The following doesn't work due to the funny way we use REVINFO.REVINFOID instead of REVINFO.REV.
            //long revisionNumber = AuditReaderFactory.Get(Session).GetRevisionNumberForDate(date);

            int revisionNumber = Session.CreateSQLQuery("SELECT max(this_.REVINFOID) as y0_ FROM REVINFO this_ WHERE this_.REVTSTMP <= :date")
                 .SetDateTime("date", date)
                 .UniqueResult<int>();

            // for those historical requests that pre-date nhibernate envers auditing, use base revision 100 not 0
            return revisionNumber > 0 ? revisionNumber : AuditableExtensions.BASE_REVISION_ID;
        }
    }
}
