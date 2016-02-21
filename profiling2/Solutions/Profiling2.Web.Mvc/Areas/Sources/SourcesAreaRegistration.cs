using System.Web.Mvc;

namespace Profiling2.Web.Mvc.Areas.Sources
{
    public class SourcesAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Sources";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "SourcesAllImagePreviews",
                "Sources/All/Images/{filename}",
                new { controller = "Explorer", action = "Images", filename = UrlParameter.Optional }
            );

            context.MapRoute(
                "SourcesExplorerImagePreviews",
                "Sources/Explorer/Images/{filename}",
                new { controller = "Explorer", action = "Images", filename = UrlParameter.Optional }
            );

            context.MapRoute(
                "SourcesExplorerBrowseImagePreviews",
                "Sources/Explorer/Browse/Images/{filename}",
                new { controller = "Explorer", action = "Images", filename = UrlParameter.Optional }
            );

            context.MapRoute(
                "SourcesExplorerMoreLikeThis",
                "Sources/Explorer/MoreLikeThis/{id}",
                new { controller = "Explorer", action = "MoreLikeThis" }
            );

            context.MapRoute(
                "SourcesExplorerPreview",
                "Sources/Explorer/Preview/{id}",
                new { controller = "Explorer", action = "Preview" }
            );

            context.MapRoute(
                "SourcesExplorerDownload",
                "Sources/Explorer/Download/{id}",
                new { controller = "Explorer", action = "Download" }
            );

            context.MapRoute(
                "SourcesExplorer",
                "Sources/Explorer/{action}/{code}",
                new { controller = "Explorer", action = "Index", code = UrlParameter.Optional }
            );

            context.MapRoute(
                "Sources_default",
                "Sources/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
