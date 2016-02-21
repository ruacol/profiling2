using System.IO;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Profiling2.Domain.Contracts.Tasks.Screenings;
using Profiling2.Domain.Scr;
using Profiling2.Domain.Scr.Attach;
using Profiling2.Infrastructure.Util;
using SharpArch.NHibernate.Web.Mvc;

namespace Profiling2.Web.Mvc.Areas.Screening.Controllers
{
    public class AttachmentsController : ScreeningBaseController
    {
        private readonly IRequestTasks requestTasks;
        private readonly IRequestAttachmentTasks requestAttachmentTasks;

        public AttachmentsController(IRequestTasks requestTasks, IRequestAttachmentTasks requestAttachmentTasks)
        {
            this.requestTasks = requestTasks;
            this.requestAttachmentTasks = requestAttachmentTasks;
        }

        public ActionResult Index(int requestId)
        {
            Request request = this.requestTasks.Get(requestId);
            if (request != null)
                return View(request);
            else
                return new HttpNotFoundResult();
        }

        [Transaction]
        public ActionResult Remove(int requestId, int id)
        {
            this.requestAttachmentTasks.ArchiveAttachment(requestId, id, User.Identity.Name);
            return RedirectToAction("Index", new { requestId = requestId });
        }

        [Transaction]
        public ActionResult Restore(int requestId, int id)
        {
            this.requestAttachmentTasks.RestoreAttachment(requestId, id, User.Identity.Name);
            return RedirectToAction("Index", new { requestId = requestId });
        }

        public ActionResult Create(int requestId)
        {
            Request request = this.requestTasks.Get(requestId);
            if (request != null)
                return View(request);
            else
                return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        public ActionResult Create(int requestId, HttpPostedFileBase file)
        {
            Request request = this.requestTasks.Get(requestId);
            if (request != null)
            {
                if (file != null && file.ContentLength > 0)
                {
                    this.requestAttachmentTasks.AddAttachment(requestId, Path.GetFileName(file.FileName), file.InputStream, User.Identity.Name);
                }
                return RedirectToAction("Index", new { requestId = requestId });
            }
            else
                return new HttpNotFoundResult();
        }

        public void Download(int requestId, int id)
        {
            Attachment att = this.requestAttachmentTasks.GetAttachment(id);
            if (att != null && att.FileData != null)
            {
                string contentType = MIMEAssistant.GetMIMEType(att.FileName);
                Response.ContentType = contentType;
                Response.AddHeader("Content-Disposition", "attachment; filename=\"" + att.FileName + "\"");
                Response.OutputStream.Write(att.FileData, 0, att.FileData.Length);
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                Response.StatusDescription = "That attachment doesn't exist.";
            }
        }
    }
}