using System.Collections.Generic;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Prf.Actions
{
    public class ActionTakenType : Entity
    {
        public virtual string ActionTakenTypeName { get; set; }
        public virtual bool Archive { get; set; }
        public virtual string Notes { get; set; }
        public virtual bool IsRemedial { get; set; }
        public virtual bool IsDisciplinary { get; set; }

        public virtual IList<ActionTaken> ActionsTaken { get; set; }

        public ActionTakenType()
        {
            this.ActionsTaken = new List<ActionTaken>();
        }

        public override string ToString()
        {
            return this.ActionTakenTypeName;
        }

        public virtual object ToJSON()
        {
            return new
                {
                    Id = this.Id,
                    Name = this.ActionTakenTypeName,
                    IsRemedial = this.IsRemedial,
                    IsDisciplinary = this.IsDisciplinary
                };
        }
    }
}
