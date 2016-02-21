using System.Collections.Generic;
using System.Linq;
using Profiling2.Domain.Scr.Person;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Scr.PersonRecommendation
{
    public class ScreeningRequestPersonRecommendation : Entity
    {
        public virtual RequestPerson RequestPerson { get; set; }
        public virtual ScreeningResult ScreeningResult { get; set; }
        public virtual string Commentary { get; set; }
        public virtual string Notes { get; set; }
        public virtual bool Archive { get; set; }
        /// <summary>
        /// Used as concurrency lock during validation.
        /// </summary>
        public virtual int Version { get; set; }
        public virtual IList<ScreeningRequestPersonRecommendationHistory> Histories { get; set; }
        public virtual IList<AdminScreeningRequestPersonRecommendationImport> Imports { get; set; }

        public ScreeningRequestPersonRecommendation()
        {
            this.Histories = new List<ScreeningRequestPersonRecommendationHistory>();
            this.Imports = new List<AdminScreeningRequestPersonRecommendationImport>();
        }

        public virtual ScreeningRequestPersonRecommendationHistory MostRecentHistory
        {
            get
            {
                if (this.Histories != null)
                {
                    return (from h in this.Histories
                            orderby h.DateStatusReached
                            select h).Last<ScreeningRequestPersonRecommendationHistory>();
                }
                else
                {
                    return null;
                }
            }
        }

        public override string ToString()
        {
            if (this.ScreeningResult != null)
                return "PWG: " + this.ScreeningResult;
            else
                return "PWG";
        }
    }
}
