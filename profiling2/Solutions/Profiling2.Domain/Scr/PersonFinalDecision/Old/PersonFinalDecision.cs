using SharpArch.Domain.DomainModel;
using System.Collections.Generic;

namespace Profiling2.Domain.Scr.PersonFinalDecision.Old
{
    public class PersonFinalDecision : Entity
    {
        //public virtual Person Person { get; set; }
        public virtual ScreeningResult ScreeningResult { get; set; }
        public virtual ScreeningSupportStatus ScreeningSupportStatus { get; set; }
        public virtual string Commentary { get; set; }
        public virtual string Notes { get; set; }
        public virtual bool Archive { get; set; }
        public virtual IList<PersonFinalDecisionHistory> Histories { get; set; }
        public virtual IList<AdminPersonFinalDecisionImport> AdminPersonFinalDecisionImports { get; set; }

        public PersonFinalDecision()
        {
            this.Histories = new List<PersonFinalDecisionHistory>();
            this.AdminPersonFinalDecisionImports = new List<AdminPersonFinalDecisionImport>();
        }
    }
}
