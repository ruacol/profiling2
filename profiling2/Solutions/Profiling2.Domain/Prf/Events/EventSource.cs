using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NHibernate.Envers.Configuration.Attributes;
using Profiling2.Domain.Prf.Sources;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Prf.Events
{
    public class EventSource : Entity
    {
        [Audited]
        public virtual Event Event { get; set; }
        [Audited]
        public virtual Source Source { get; set; }
        [Audited(TargetAuditMode = RelationTargetAuditMode.NotAudited)]
        public virtual Reliability Reliability { get; set; }
        [Audited]
        public virtual string Commentary { get; set; }
        [Audited]
        public virtual bool Archive { get; set; }
        [Audited]
        public virtual string Notes { get; set; }

        protected readonly string CASE_CODE_REGEX = @"DH[A-Z]{3}[0-9]+";

        public virtual bool HasCaseCode()
        {
            Regex regex = new Regex(CASE_CODE_REGEX);
            return (!string.IsNullOrEmpty(this.Commentary) && regex.IsMatch(this.Commentary))
                || (!string.IsNullOrEmpty(this.Notes) && regex.IsMatch(this.Notes));
        }

        public virtual IList<string> GetCaseCodes()
        {
            if (this.HasCaseCode())
            {
                IList<string> codes = new List<string>();

                Regex regex = new Regex(CASE_CODE_REGEX);
                if (!string.IsNullOrEmpty(this.Commentary))
                    foreach (Match match in regex.Matches(this.Commentary))
                        codes.Add(match.Value);
                if (!string.IsNullOrEmpty(this.Notes))
                    foreach (Match match in regex.Matches(this.Notes))
                        codes.Add(match.Value);

                return codes.Distinct().ToList();
            }
            return null;
        }

        public override string ToString()
        {
            return (this.Event != null ? "Event(ID=" + this.Event.Id.ToString() + ")" : string.Empty)
                + (this.Source != null ? " is linked with Source(ID=" + this.Source.Id.ToString() + ")" : string.Empty);
        }

        public virtual object ToJSON(SourceDTO dto)
        {
            return new
                {
                    Id = this.Id,
                    EventId = this.Event.Id,
                    Source = dto != null ? new
                        {
                            Id = dto.SourceID,
                            Name = dto.SourceName,
                            Archive = dto.Archive,
                            IsRestricted = dto.IsRestricted
                        } : null,
                    Reliability = this.Reliability != null ? this.Reliability.ToJSON() : null,
                    Commentary = this.Commentary,
                    Notes = this.Notes
                };
        }
    }
}
