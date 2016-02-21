using System.Collections.Generic;
using System.Web.Mvc;
using SharpArch.Web.Mvc.JsonNet;

namespace Profiling2.Web.Mvc.Controllers
{
    [Compress]
    [Authorize]
    public class BaseController : Controller
    {
        public const string SOURCE_ATTACHMENT_ENTITY = "SourceAttachmentEntity";

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

        public IDictionary<string, IList<string>> GetErrorsForJson()
        {
            IDictionary<string, IList<string>> errors = new Dictionary<string, IList<string>>();
            foreach (string key in ModelState.Keys)
            {
                IList<string> list = new List<string>();
                if (ModelState[key].Errors.Count > 0)
                {
                    foreach (ModelError me in ModelState[key].Errors)
                        list.Add(me.ErrorMessage);
                    errors[key] = list;
                }
            }
            return errors;
        }
    }
}
