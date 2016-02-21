using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Prf.Persons
{
    /// <summary>
    /// For statistical purposes.
    /// </summary>
    public class ProfileStatus : Entity
    {
        // unique index on name
        public const string ROUGH_OUTLINE = "Rough outline";
        public const string IN_PROGRESS = "In progress";
        public const string COMPLETE = "Complete";
        public const string FARDC_2007_LIST = "FARDC 2007 List";

        public virtual string ProfileStatusName { get; set; }
        public virtual bool Archive { get; set; }
        public virtual string Notes { get; set; }

        public override string ToString()
        {
            return this.ProfileStatusName;
        }
    }
}
