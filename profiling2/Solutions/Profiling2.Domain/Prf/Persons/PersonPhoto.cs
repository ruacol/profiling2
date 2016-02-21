using SharpArch.Domain.DomainModel;
using NHibernate.Envers.Configuration.Attributes;

namespace Profiling2.Domain.Prf.Persons
{
    public class PersonPhoto : Entity
    {
        [Audited]
        public virtual Person Person { get; set; }
        [Audited(TargetAuditMode = RelationTargetAuditMode.NotAudited)]
        public virtual Photo Photo { get; set; }
        [Audited]
        public virtual bool Archive { get; set; }
        [Audited]
        public virtual string Notes { get; set; }

        public PersonPhoto()
        {

        }

        public override string ToString()
        {
            //return (this.Person != null ? "Person(ID=" + this.Person.Id.ToString() + ")" : string.Empty)
            //    + (this.Photo != null ? " has Photo(ID=" + this.Photo.Id.ToString() + ")" : string.Empty);
            return "PersonPhotoID(ID=" + this.Id + ")";
        }
    }
}
