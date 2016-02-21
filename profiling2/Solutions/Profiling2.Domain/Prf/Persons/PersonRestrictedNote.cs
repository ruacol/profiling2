using NHibernate.Envers.Configuration.Attributes;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Prf.Persons
{
    /// <summary>
    /// Person text that is only visible to Profiling Internationals and Profiling Nationals.
    /// </summary>
    public class PersonRestrictedNote : Entity
    {
        [Audited]
        public virtual Person Person { get; set; }
        [Audited]
        public virtual string Note { get; set; }

        public override string ToString()
        {
            return this.Note;
        }
    }
}
