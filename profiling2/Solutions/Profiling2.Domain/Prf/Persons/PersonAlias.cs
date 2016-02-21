using SharpArch.Domain.DomainModel;
using NHibernate.Envers.Configuration.Attributes;

namespace Profiling2.Domain.Prf.Persons
{
    public class PersonAlias : Entity
    {
        [Audited]
        public virtual Person Person { get; set; }
        [Audited]
        public virtual string LastName { get; set; }
        [Audited]
        public virtual string FirstName { get; set; }
        [Audited]
        public virtual bool Archive { get; set; }
        [Audited]
        public virtual string Notes { get; set; }

        public virtual string Name
        {
            get
            {
                if (!string.IsNullOrEmpty(this.FirstName))
                {
                    if (!string.IsNullOrEmpty(this.LastName))
                        return this.FirstName + " " + this.LastName;
                    else
                        return this.FirstName;
                }
                else
                {
                    return this.LastName;
                }
            }
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
