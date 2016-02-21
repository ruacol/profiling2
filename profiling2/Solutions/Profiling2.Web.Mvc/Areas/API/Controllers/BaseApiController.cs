using SharpArch.Web.Mvc.JsonNet;
using System.Web.Mvc;

namespace Profiling2.Web.Mvc.Areas.API.Controllers
{
    [BasicAuthentication]
    public class BaseApiController : Controller
    {
        public JsonNetResult JsonNet(object data)
        {
            // Try to avoid IIS overwriting our custom error text with generic/unconfigured error text.
            // http://weblog.west-wind.com/posts/2009/Apr/29/IIS-7-Error-Pages-taking-over-500-Errors
            // http://stackoverflow.com/questions/22071211/when-performing-post-via-ajax-bad-request-is-returned-instead-of-the-json-resul
            Response.TrySkipIisCustomErrors = true;

            return new JsonNetResult
            {
                Data = data
            };
        }
    }
}