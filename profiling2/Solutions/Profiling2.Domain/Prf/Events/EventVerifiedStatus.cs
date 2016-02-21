using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Prf.Events
{
    public class EventVerifiedStatus : Entity
    {
        public const string ALLEGATION = "allegation";
        public const string JHRO_VERIFIED = "JHRO-verified";
        public const string NOT_JHRO_VERIFIED = "not JHRO-verified";

        public virtual string EventVerifiedStatusName { get; set; }
        public virtual string Notes { get; set; }
        public virtual bool Archive { get; set; }

        public override string ToString()
        {
            return this.EventVerifiedStatusName;
        }

        public virtual object ToJSON()
        {
            return new
                {
                    Id = this.Id,
                    Name = this.EventVerifiedStatusName
                };
        }
    }
}
