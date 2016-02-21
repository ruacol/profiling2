using SharpArch.Domain.DomainModel;
using Profiling2.Domain.Scr.Person;
using System.Collections.Generic;
using System.Linq;

namespace Profiling2.Domain.Scr.PersonEntity
{
    public class ScreeningRequestPersonEntity : Entity
    {
        public virtual RequestPerson RequestPerson { get; set; }
        public virtual ScreeningEntity ScreeningEntity { get; set; }
        public virtual ScreeningResult ScreeningResult { get; set; }
        public virtual string Reason { get; set; }
        public virtual string Commentary { get; set; }
        public virtual string Notes { get; set; }
        public virtual bool Archive { get; set; }
        public virtual int Version { get; set; }
        public virtual IList<ScreeningRequestPersonEntityHistory> Histories { get; set; }
        public virtual IList<AdminScreeningRequestPersonEntityImport> Imports { get; set; }

        public ScreeningRequestPersonEntity()
        {
            this.Histories = new List<ScreeningRequestPersonEntityHistory>();
            this.Imports = new List<AdminScreeningRequestPersonEntityImport>();
        }

        public virtual ScreeningRequestPersonEntityHistory MostRecentHistory
        {
            get
            {
                if (this.Histories != null && this.Histories.Any())
                {
                    return (from h in this.Histories
                            orderby h.DateStatusReached
                            select h).Last<ScreeningRequestPersonEntityHistory>();
                }
                else
                {
                    return null;
                }
            }
        }

        public virtual void AddHistory(ScreeningRequestPersonEntityHistory srpeh)
        {
            this.Histories.Add(srpeh);
        }

        public override string ToString()
        {
            if (this.ScreeningEntity != null)
            {
                if (this.ScreeningResult != null)
                    return this.ScreeningEntity + ": " + this.ScreeningResult;
                else
                    return this.ScreeningEntity.ToString();
            }
            else
            {
                // good cause to throw an exception here, any screened person would necessarily have been screened by one of the 
                // screening entities.
                return this.ScreeningResult.ToString();
            }
        }
    }
}
