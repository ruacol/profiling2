using System.Collections.Generic;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Prf
{
    public class AdminPermission : Entity
    {
        public const string CanViewAndSearchPersons = "CanViewAndSearchPersons";
        public const string CanChangePersons = "CanChangePersons";
        public const string CanChangePersonPublicSummaries = "CanChangePersonPublicSummaries";
        public const string CanChangePersonNotes = "CanChangePersonNotes";
        public const string CanViewAndChangePersonRestrictedNotes = "CanViewAndChangePersonRestrictedNotes";
        public const string CanViewBackgroundInformation = "CanViewBackgroundInformation";
        public const string CanViewPersonRelationships = "CanViewPersonRelationships";
        public const string CanChangePersonBackground = "CanChangePersonBackground";
        public const string CanDeletePersons = "CanDeletePersons";
        public const string CanExportPersons = "CanExportPersons";
        public const string CanViewPersonReports = "CanViewPersonReports";
        public const string CanViewAndSearchRestrictedPersons = "CanViewAndSearchRestrictedPersons";
        public const string CanViewPersonResponsibilities = "CanViewPersonResponsibilities";
        public const string CanChangePersonResponsibilities = "CanChangePersonResponsibilities";
        public const string CanViewAndSearchRequests = "CanViewAndSearchRequests";

        public const string CanViewAndSearchEvents = "CanViewAndSearchEvents";
        public const string CanChangeEvents = "CanChangeEvents";
        public const string CanApproveEvents = "CanApproveEvents";
        public const string CanChangeActionsTaken = "CanChangeActionsTaken";
        public const string CanLinkEvents = "CanLinkEvents";
        public const string CanLinkEventsAndSources = "CanLinkEventsAndSources";

        public const string CanViewAndSearchUnits = "CanViewAndSearchUnits";
        public const string CanChangeUnits = "CanChangeUnits";
        public const string CanViewAndSearchOrganizations = "CanViewAndSearchOrganizations";
        public const string CanChangeOrganizations = "CanChangeOrganizations";

        public const string CanViewAndSearchSources = "CanViewAndSearchSources";
        public const string CanUploadSources = "CanUploadSources";
        public const string CanChangeSources = "CanChangeSources";
        public const string CanApproveAndRejectSources = "CanApproveAndRejectSources";
        public const string CanViewAndSearchRestrictedSources = "CanViewAndSearchRestrictedSources";
        public const string CanViewAndSearchAllSources = "CanViewAndSearchAllSources";

        public const string CanPerformScreeningInput = "CanPerformScreeningInput";

        public const string CanAdministrate = "CanAdministrate";

        public virtual string Name { get; set; }

        public virtual IList<AdminRole> AdminRoles { get; set; }

        public AdminPermission()
        {
            this.AdminRoles = new List<AdminRole>();
        }
    }
}
