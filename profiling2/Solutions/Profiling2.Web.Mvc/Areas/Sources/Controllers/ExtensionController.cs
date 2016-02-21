using System.Net;
using System.Web.Mvc;
using Profiling2.Domain.Contracts.Tasks.Sources;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Sources;
using Profiling2.Infrastructure.Util;
using Profiling2.Web.Mvc.Controllers;
using SharpArch.NHibernate.Web.Mvc;
using SharpArch.Web.Mvc.JsonNet;
using TikaOnDotNet;

namespace Profiling2.Web.Mvc.Areas.Sources.Controllers
{
    [PermissionAuthorize(AdminPermission.CanAdministrate)]
    public class ExtensionController : BaseController
    {
        protected readonly ISourceTasks sourceTasks;
        protected readonly ISourceContentTasks sourceContentTasks;
        protected readonly ISourceManagementTasks sourceManagementTasks;

        public ExtensionController(ISourceTasks sourceTasks, 
            ISourceContentTasks sourceContentTasks, 
            ISourceManagementTasks sourceManagementTasks)
        {
            this.sourceTasks = sourceTasks;
            this.sourceContentTasks = sourceContentTasks;
            this.sourceManagementTasks = sourceManagementTasks;
        }

        public ActionResult Index()
        {
            return View(this.sourceManagementTasks.GetExtensionlessSources());
        }

        public JsonNetResult GuessContentType(int id)
        {
            string contentType;
            try
            {
                contentType = this.sourceContentTasks.GuessContentType(id);
            }
            catch (TextExtractionException e)
            {
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return JsonNet(e.Message);
            }
            return JsonNet(contentType);
        }

        [Transaction]
        public JsonNetResult SetFileExtension(int id)
        {
            Source source = this.sourceTasks.GetSource(id);
            if (source != null && string.IsNullOrEmpty(source.FileExtension))
            {
                string contentType;
                try
                {
                    contentType = this.sourceContentTasks.GuessContentType(id);
                }
                catch (TextExtractionException e)
                {
                    Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    return JsonNet(e.Message);
                }
                string ext = MIMEAssistant.GetFileExtension(contentType);
                if (!string.IsNullOrEmpty(ext))
                {
                    // required by MSSQL full-text indexing engine
                    source.FileExtension = ext;

                    // append new file extension, required for preview logic to work
                    source.SourceName = string.Join(".", new string[] { source.SourceName, ext });

                    // don't modify SourcePath, as this is currently used as a unique key to the file
                    // i.e. it may be reimported if changed
                    //source.SourcePath = string.Join(".", new string[] { source.SourcePath, ext });

                    this.sourceTasks.SaveSource(source);

                    Response.StatusCode = (int)HttpStatusCode.OK;
                    return JsonNet(null);
                }
            }

            Response.StatusCode = (int)HttpStatusCode.NotFound;
            return JsonNet(null);
        }
    }
}