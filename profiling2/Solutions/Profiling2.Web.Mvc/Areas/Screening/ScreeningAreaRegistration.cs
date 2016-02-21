using System.Web.Mvc;

namespace Profiling2.Web.Mvc.Areas.Screening
{
    public class ScreeningAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Screening";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Initiate request remove unit",
                "Screening/Initiate/{requestId}/RemoveUnit/{unitId}",
                new { controller = "Initiate", action = "RemoveUnit", requestId = UrlParameter.Optional, unitId = UrlParameter.Optional },
                null,
                new string[] { "Profiling2.Web.Mvc.Areas.Screening.Controllers" }
            );

            context.MapRoute(
                "Initiate request attach unit",
                "Screening/Initiate/{requestId}/AttachUnit/{unitId}",
                new { controller = "Initiate", action = "AttachUnit", requestId = UrlParameter.Optional, unitId = UrlParameter.Optional },
                null,
                new string[] { "Profiling2.Web.Mvc.Areas.Screening.Controllers" }
            );

            context.MapRoute(
                "Undo request response",
                "Screening/Requests/UndoResponse/{id}/{screeningEntityName}",
                new { controller = "Requests", action = "UndoResponse", id = UrlParameter.Optional },
                null,
                new string[] { "Profiling2.Web.Mvc.Areas.Screening.Controllers" }
            );

            context.MapRoute(
                "Validate Request action",
                "Screening/Validate/Request/{id}",
                new { controller = "Validate", action = "RequestAction", id = UrlParameter.Optional },
                null,
                new string[] { "Profiling2.Web.Mvc.Areas.Screening.Controllers" }
            );

            context.MapRoute(
                "Finalize Request action",
                "Screening/Finalize/Request/{id}",
                new { controller = "Finalize", action = "RequestAction", id = UrlParameter.Optional },
                null,
                new string[] { "Profiling2.Web.Mvc.Areas.Screening.Controllers" }
            );

            context.MapRoute(
                "Initiate request remove proposed person",
                "Screening/Initiate/{requestId}/RemoveProposedPerson/{proposedPersonId}",
                new { controller = "Initiate", action = "RemoveProposedPerson", requestId = UrlParameter.Optional, proposedPersonId = UrlParameter.Optional },
                null,
                new string[] { "Profiling2.Web.Mvc.Areas.Screening.Controllers" }
            );

            context.MapRoute(
                "Initiate request remove person",
                "Screening/Initiate/{requestId}/RemovePerson/{personId}",
                new { controller = "Initiate", action = "RemovePerson", requestId = UrlParameter.Optional, personId = UrlParameter.Optional },
                null,
                new string[] { "Profiling2.Web.Mvc.Areas.Screening.Controllers" }
            );

            context.MapRoute(
                "Initiate request attach person",
                "Screening/Initiate/{requestId}/AttachPerson/{personId}",
                new { controller = "Initiate", action = "AttachPerson", requestId = UrlParameter.Optional, personId = UrlParameter.Optional },
                null,
                new string[] { "Profiling2.Web.Mvc.Areas.Screening.Controllers" }
            );

            context.MapRoute(
                "Consolidate Request action",
                "Screening/Consolidate/Request/{id}",
                new { controller = "Consolidate", action = "RequestAction", id = UrlParameter.Optional },
                null,
                new string[] { "Profiling2.Web.Mvc.Areas.Screening.Controllers" }
            );

            context.MapRoute(
                "RequestAttachments",
                "Screening/Requests/{requestId}/Attachments/{action}/{id}",
                new { controller = "Attachments", action = "Index", id = UrlParameter.Optional },
                null,
                new string[] { "Profiling2.Web.Mvc.Areas.Screening.Controllers" }
            );

            context.MapRoute(
                "Screening_default",
                "Screening/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                null,
                new string[] { "Profiling2.Web.Mvc.Areas.Screening.Controllers" }
            );
        }
    }
}
