using System;
using System.Collections.Generic;
using System.Linq;
using Profiling2.Domain.Scr.Person;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Scr.PersonFinalDecision
{
    public class ScreeningRequestPersonFinalDecision : Entity
    {
        public virtual RequestPerson RequestPerson { get; set; }
        public virtual ScreeningResult ScreeningResult { get; set; }
        public virtual ScreeningSupportStatus ScreeningSupportStatus { get; set; }
        public virtual string Commentary { get; set; }
        public virtual string Notes { get; set; }
        public virtual bool Archive { get; set; }
        /// <summary>
        /// Used as concurrency lock during validation.
        /// </summary>
        public virtual int Version { get; set; }
        public virtual IList<ScreeningRequestPersonFinalDecisionHistory> Histories { get; set; }
        public virtual IList<AdminScreeningRequestPersonFinalDecisionImport> AdminScreeningRequestPersonFinalDecisionImports { get; set; }

        public ScreeningRequestPersonFinalDecision()
        {
            this.Histories = new List<ScreeningRequestPersonFinalDecisionHistory>();
            this.AdminScreeningRequestPersonFinalDecisionImports = new List<AdminScreeningRequestPersonFinalDecisionImport>();
        }

        public virtual ScreeningRequestPersonFinalDecisionHistory MostRecentHistory
        {
            get
            {
                if (this.Histories != null)
                {
                    return (from h in this.Histories
                            orderby h.DateStatusReached
                            select h).Last<ScreeningRequestPersonFinalDecisionHistory>();
                }
                else
                {
                    return null;
                }
            }
        }

        public virtual DateTime? MostRecentDate
        {
            get
            {
                if (this.MostRecentHistory != null)
                    return this.MostRecentHistory.DateStatusReached;
                return null;
            }
        }

        /// <summary>
        /// Whether the commentary of this final decision contains the text 'waiver'.  Usage of this information
        /// in the front-end should be properly caveated.
        /// </summary>
        /// <returns></returns>
        public virtual bool ContainsWaiver()
        {
            return !string.IsNullOrEmpty(this.Commentary) && this.Commentary.Contains("waiver");
        }
    }
}
