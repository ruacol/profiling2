using System.Collections.Generic;
using System.Linq;
using Profiling2.Domain.Prf.Events;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Prf
{
    public class Tag : Entity
    {
        public virtual string TagName { get; set; }
        public virtual bool Archive { get; set; }
        public virtual string Notes { get; set; }

        public virtual IList<Event> Events { get; set; }

        public Tag()
        {
            this.Events = new List<Event>();
        }

        public override string ToString()
        {
            return this.TagName;
        }

        public virtual object ToJSON()
        {
            return new
                {
                    Id = this.Id,
                    TagName = this.TagName,
                    Events = this.Events.Select(x => x.ToShortJSON())
                };
        }
    }
}
