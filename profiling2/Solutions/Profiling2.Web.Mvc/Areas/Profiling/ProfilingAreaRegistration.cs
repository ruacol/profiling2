using System.Web.Mvc;

namespace Profiling2.Web.Mvc.Areas.Profiling
{
    public class ProfilingAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Profiling";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Add Operation Source",
                "Profiling/Operations/{operationId}/Sources/Add",
                new { controller = "OperationSources", action = "Add", operationId = UrlParameter.Optional },
                null,
                new string[] { "Profiling2.Web.Mvc.Areas.Profiling.Controllers" }
            );

            context.MapRoute(
                "Person Notes",
                "Profiling/Persons/{personId}/Notes/{action}/{id}",
                new { controller = "PersonRestrictedNotes", action = "Index", personId = UrlParameter.Optional, id = UrlParameter.Optional },
                null,
                new string[] { "Profiling2.Web.Mvc.Areas.Profiling.Controllers" }
            );

            context.MapRoute(
                "Add Unit to Operation",
                "Profiling/Operations/AddUnitModal/{operationId}",
                new { controller = "Operations", action = "AddUnitModal", operationId = UrlParameter.Optional },
                null,
                new string[] { "Profiling2.Web.Mvc.Areas.Profiling.Controllers" }
            );

            context.MapRoute(
                "OperationAliases",
                "Profiling/Operations/{operationId}/Aliases/{action}/{id}",
                new { controller = "OperationAliases", action = "Create", id = UrlParameter.Optional },
                null,
                new string[] { "Profiling2.Web.Mvc.Areas.Profiling.Controllers" }
            );

            context.MapRoute(
                "Add Operation to Unit",
                "Profiling/Units/AddOperationModal/{unitId}",
                new { controller = "Units", action = "AddOperationModal", unitId = UrlParameter.Optional },
                null,
                new string[] { "Profiling2.Web.Mvc.Areas.Profiling.Controllers" }
            );

            context.MapRoute(
                "UnitRemoveOperation",
                "Profiling/Units/RemoveOperation/{id}/{operationId}",
                new { controller = "Units", action = "RemoveOperation", operationId = UrlParameter.Optional },
                null,
                new string[] { "Profiling2.Web.Mvc.Areas.Profiling.Controllers" }
            );

            context.MapRoute(
                "AddUnitSource",
                "Profiling/Units/{unitId}/Sources/Add",
                new { controller = "UnitSources", action = "Add", unitId = UrlParameter.Optional },
                null,
                new string[] { "Profiling2.Web.Mvc.Areas.Profiling.Controllers" }
            );

            context.MapRoute(
                "CreateUnitAlias",
                "Profiling/Units/{unitId}/Aliases/Create",
                new { controller = "UnitAliases", action = "Create", unitId = UrlParameter.Optional },
                null,
                new string[] { "Profiling2.Web.Mvc.Areas.Profiling.Controllers" }
            );

            context.MapRoute(
                "Active Screenings",
                "Profiling/Persons/{personId}/ActiveScreenings/{action}/{id}",
                new { controller = "ActiveScreenings", action = "Index", personId = UrlParameter.Optional, id = UrlParameter.Optional },
                null,
                new string[] { "Profiling2.Web.Mvc.Areas.Profiling.Controllers" }
            );

            context.MapRoute(
                "Unit Locations",
                "Profiling/Units/{currentUnitId}/Locations/{action}/{id}",
                new { controller = "UnitLocations", action = "Create", currentUnitId = UrlParameter.Optional, id = UrlParameter.Optional },
                null,
                new string[] { "Profiling2.Web.Mvc.Areas.Profiling.Controllers" }
            );

            context.MapRoute(
                "Unit Hierarchies",
                "Profiling/Units/{currentUnitId}/Hierarchies/{action}/{id}",
                new { controller = "Hierarchies", action = "Create", currentUnitId = UrlParameter.Optional, id = UrlParameter.Optional },
                null,
                new string[] { "Profiling2.Web.Mvc.Areas.Profiling.Controllers" }
            );

            context.MapRoute(
                "AddCareer",
                "Profiling/Persons/{personId}/Careers/Add",
                new { controller = "Careers", action = "Add", personId = UrlParameter.Optional },
                null,
                new string[] { "Profiling2.Web.Mvc.Areas.Profiling.Controllers" }
            );

            context.MapRoute(
                "Careers",
                "Profiling/Persons/{personId}/Careers/{action}/{careerId}",
                new { controller = "Careers", action = "Index", personId = UrlParameter.Optional, careerId = UrlParameter.Optional },
                null,
                new string[] { "Profiling2.Web.Mvc.Areas.Profiling.Controllers" }
            );

            context.MapRoute(
                "AddPersonRelationship",
                "Profiling/Persons/{personId}/Relationships/Add",
                new { controller = "PersonRelationships", action = "Add", personId = UrlParameter.Optional },
                null,
                new string[] { "Profiling2.Web.Mvc.Areas.Profiling.Controllers" }
            );

            context.MapRoute(
                "PersonRelationships",
                "Profiling/Persons/{personId}/Relationships/{action}/{relationshipId}",
                new { controller = "PersonRelationships", action = "Index", personId = UrlParameter.Optional, relationshipId = UrlParameter.Optional },
                null,
                new string[] { "Profiling2.Web.Mvc.Areas.Profiling.Controllers" }
            );

            context.MapRoute(
                "AddPersonSource",
                "Profiling/Persons/{personId}/Sources/Add",
                new { controller = "PersonSources", action = "Add", personId = UrlParameter.Optional },
                null,
                new string[] { "Profiling2.Web.Mvc.Areas.Profiling.Controllers" }
            );

            context.MapRoute(
                "CreatePersonAlias",
                "Profiling/Persons/{personId}/Aliases/Create",
                new { controller = "PersonAliases", action = "Create", personId = UrlParameter.Optional },
                null,
                new string[] { "Profiling2.Web.Mvc.Areas.Profiling.Controllers" }
            );

            context.MapRoute(
                "AddActionTaken",
                "Profiling/Events/{eventId}/ActionsTaken/Add",
                new { controller = "ActionsTaken", action = "Add", eventId = UrlParameter.Optional },
                null,
                new string[] { "Profiling2.Web.Mvc.Areas.Profiling.Controllers" }
            );

            context.MapRoute(
                "AddOrgResponsibility",
                "Profiling/Events/{eventId}/OrgResponsibilities/Add",
                new { controller = "OrgResponsibilities", action = "Add", eventId = UrlParameter.Optional },
                null,
                new string[] { "Profiling2.Web.Mvc.Areas.Profiling.Controllers" }
            );

            context.MapRoute(
                "AddPersonResponsibilityForPerson",
                "Profiling/Persons/{personId}/PersonResponsibilities/AddForPerson",
                new { controller = "PersonResponsibilities", action = "AddForPerson", personId = UrlParameter.Optional },
                null,
                new string[] { "Profiling2.Web.Mvc.Areas.Profiling.Controllers" }
            );

            context.MapRoute(
                "AddPersonResponsibility",
                "Profiling/Events/{eventId}/PersonResponsibilities/Add",
                new { controller = "PersonResponsibilities", action = "Add", eventId = UrlParameter.Optional },
                null,
                new string[] { "Profiling2.Web.Mvc.Areas.Profiling.Controllers" }
            );

            context.MapRoute(
                "PersonResponsibility",
                "Profiling/Events/{eventId}/PersonResponsibilities/{personId}/{action}",
                new { controller = "PersonResponsibilities", action = "Index", eventId = UrlParameter.Optional, personId = UrlParameter.Optional },
                null,
                new string[] { "Profiling2.Web.Mvc.Areas.Profiling.Controllers" }
            );

            context.MapRoute(
                "AddEventSource",
                "Profiling/Events/{eventId}/Sources/Add",
                new { controller = "EventSources", action = "Add", eventId = UrlParameter.Optional },
                null,
                new string[] { "Profiling2.Web.Mvc.Areas.Profiling.Controllers" }
            );

            context.MapRoute(
                "EventSources",
                "Profiling/Events/{eventId}/Sources/{sourceId}/{action}",
                new { controller = "EventSources", action = "Index", eventId = UrlParameter.Optional, sourceId = UrlParameter.Optional },
                null,
                new string[] { "Profiling2.Web.Mvc.Areas.Profiling.Controllers" }
            );

            context.MapRoute(
                "AddPersonPhoto",
                "Profiling/Persons/{personId}/Photos/Add",
                new { controller = "PersonPhotos", action = "Add", personId = UrlParameter.Optional },
                null,
                new string[] { "Profiling2.Web.Mvc.Areas.Profiling.Controllers" }
            );

            context.MapRoute(
                "PersonPhotos",
                "Profiling/Persons/{personId}/Photo/{action}/{photoId}",
                new { controller = "PersonPhotos", action = "Get", photoId = UrlParameter.Optional, personId = UrlParameter.Optional },
                null,
                new string[] { "Profiling2.Web.Mvc.Areas.Profiling.Controllers" }
            );

            context.MapRoute(
                "SourceAttach",
                "Profiling/Sources/{sourceId}/Attach/{action}/{targetId}",
                new { controller = "SourceAttach", action = "Index", id = UrlParameter.Optional, sourceId = UrlParameter.Optional },
                null,
                new string[] { "Profiling2.Web.Mvc.Areas.Profiling.Controllers" }
            );

            context.MapRoute(
                "ImagePreviews",
                "Profiling/Sources/Preview/{id}/Images/{filename}",
                new { controller = "Sources", action = "PreviewImages", id = UrlParameter.Optional, filename = UrlParameter.Optional },
                null,
                new string[] { "Profiling2.Web.Mvc.Areas.Profiling.Controllers" }
            );

            context.MapRoute(
                "SourceTracking",
                "Profiling/Sources/{action}/{id}/{adminSourceSearchId}",
                new { controller = "Sources", action = "Index", id = UrlParameter.Optional, adminSourceSearchId = UrlParameter.Optional },
                null,
                new string[] { "Profiling2.Web.Mvc.Areas.Profiling.Controllers" }
            );

            context.MapRoute(
                "Profiling_default",
                "Profiling/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                null,
                new string[] { "Profiling2.Web.Mvc.Areas.Profiling.Controllers" }
            );
        }
    }
}
