using System.Collections.Generic;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Prf
{
    public class AdminRole : Entity
    {
        // TODO deprecate these constants
        public const string ProfilingInternational = "ProfilingInternational";
        public const string ProfilingNational = "ProfilingNational";
        public const string ProfilingAdmin = "ProfilingAdmin";
        public const string ProfilingLimitedPersonEdit = "ProfilingLimitedPersonEdit";
        public const string ProfilingReadOnlyPersonView = "ProfilingReadOnlyPersonView";

        public const string ScreeningRequestInitiator = "ScreeningRequestInitiator";
        public const string ScreeningRequestValidator = "ScreeningRequestValidator";
        public const string ScreeningRequestConditionalityParticipant = "ScreeningRequestConditionalityParticipant";
        public const string ScreeningRequestConsolidator = "ScreeningRequestConsolidator";
        public const string ScreeningRequestFinalDecider = "ScreeningRequestFinalDecider";

        public virtual string Name { get; set; }

        public virtual IList<AdminPermission> AdminPermissions { get; set; }

        public AdminRole()
        {
            this.AdminPermissions = new List<AdminPermission>();
        }
    }
}
