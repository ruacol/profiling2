using SharpArch.Domain.DomainModel;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Envers.Configuration.Attributes;
using Profiling2.Domain.Prf.Responsibility;
using ScorePhrase;

namespace Profiling2.Domain.Prf.Events
{
    public class Violation : Entity
    {
        [Audited]
        public virtual string Name { get; set; }
        [Audited]
        public virtual string Description { get; set; }
        [Audited]
        public virtual string Keywords { get; set; }
        [Audited]
        public virtual Violation ParentViolation { get; set; }
        [Audited]
        public virtual bool ConditionalityInterest { get; set; }

        public virtual IList<Event> Events { get; set; }
        public virtual IList<PersonResponsibility> PersonResponsibilities { get; set; }
        public virtual IList<Violation> ChildrenViolations { get; set; }

        public Violation()
        {
            this.Events = new List<Event>();
            this.PersonResponsibilities = new List<PersonResponsibility>();
            this.ChildrenViolations = new List<Violation>();
        }

        public virtual int PersonResponsibilityCount
        {
            get
            {
                return (from e in this.Events
                 select e.PersonResponsibilities.Count).Sum();
            }
        }

        public virtual int OrganizationResponsibilityCount
        {
            get
            {
                return (from e in this.Events
                        select e.OrganizationResponsibilities.Count).Sum();
            }
        }

        /// <summary>
        /// Score similarity of given text with this Violation.  Lower is better.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public virtual int ScoreMatchingText(string text)
        {
            IList<int> phraseScores = new List<int>();

            if (!string.IsNullOrEmpty(this.Keywords))
            {
                // score against each keyword
                foreach (string vKeyphrase in this.Keywords.Split(','))
                    if (!string.IsNullOrEmpty(vKeyphrase))
                        phraseScores.Add(Scorer.GetScore(vKeyphrase, text));
            }
            else
            {
                phraseScores.Add(Scorer.GetScore(this.Name, text));
            }

            return phraseScores.Any() ? phraseScores.Min() : 1000;
        }

        public override string ToString()
        {
            return this.Name;
        }

        public virtual object ToJSON()
        {
            return new
            {
                Id = this.Id,
                Name = this.Name,
                ConditionalityInterest = this.ConditionalityInterest
            };
        }

        // data modification methods

        public virtual void AddPersonResponsibility(PersonResponsibility pr)
        {
            if (this.PersonResponsibilities.Contains(pr))
                return;

            this.PersonResponsibilities.Add(pr);
        }

        public virtual void RemovePersonResponsibility(PersonResponsibility pr)
        {
            this.PersonResponsibilities.Remove(pr);
        }
    }
}
