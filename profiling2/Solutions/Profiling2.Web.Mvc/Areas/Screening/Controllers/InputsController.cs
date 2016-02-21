using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Mvc.JQuery.Datatables;
using Profiling2.Domain.Contracts.Export.Screening;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Contracts.Tasks.Screenings;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Scr;
using Profiling2.Domain.Scr.PersonEntity;
using Profiling2.Infrastructure.Security;
using Profiling2.Web.Mvc.Areas.Screening.Controllers.ViewModels;
using Profiling2.Web.Mvc.Controllers;
using SharpArch.NHibernate.Web.Mvc;
using SharpArch.Web.Mvc.JsonNet;

namespace Profiling2.Web.Mvc.Areas.Screening.Controllers
{
    [MultiRoleAuthorize(AdminRole.ScreeningRequestConditionalityParticipant)]
    public class InputsController : ScreeningBaseController
    {
        protected readonly IUserTasks userTasks;
        protected readonly IRequestTasks requestTasks;
        protected readonly IRequestPersonTasks requestPersonTasks;
        protected readonly IScreeningTasks screeningTasks;
        protected readonly IEmailTasks emailTasks;
        protected readonly IPdfExportRequestForConditionalityParticipantService pdfExportService;

        public InputsController(IUserTasks userTasks, 
            IRequestTasks requestTasks, 
            IRequestPersonTasks requestPersonTasks, 
            IScreeningTasks screeningTasks, 
            IEmailTasks emailTasks,
            IPdfExportRequestForConditionalityParticipantService pdfExportService)
        {
            this.userTasks = userTasks;
            this.requestTasks = requestTasks;
            this.requestPersonTasks = requestPersonTasks;
            this.screeningTasks = screeningTasks;
            this.emailTasks = emailTasks;
            this.pdfExportService = pdfExportService;
        }

        public ActionResult Index()
        {
            ViewBag.UserScreeningEntities = this.userTasks.GetAdminUser(User.Identity.Name).ScreeningEntities;
            ViewBag.ScreeningEntities = this.screeningTasks.GetScreeningEntities();
            ViewBag.NominatedRequestPersons = this.requestPersonTasks.GetNominatedRequestPersons();
            return View();
        }

        public DataTablesResult<RequestView> DataTables(DataTablesParam p)
        {
            AdminUser user = this.userTasks.GetAdminUser(User.Identity.Name);

            IQueryable<Request> q = this.requestTasks.GetRequestsRequiringResponse()
                .Where(x => user.ScreeningEntities.Any(y => !x.HasResponded(y.ScreeningEntityName)))
                .AsQueryable();

            return DataTablesResult.Create<RequestView>(q.Select(x => new RequestView(x)), p);
        }

        // The following is a list of all screening responses submitted by JHRO to the ODSRSG as of 7/17/2013 5:05:03 PM.
        public ActionResult All()
        {
            AdminUser user = this.userTasks.GetAdminUser(User.Identity.Name);
            ScreeningEntity se = user.GetScreeningEntity();
            if (se == null)
                return new HttpNotFoundResult();

            ViewBag.ScreeningEntity = se;
            return View((from srpe in this.screeningTasks.GetScreeningResponsesByEntity(se.ScreeningEntityName) select new ScreeningRequestPersonEntityDataTableView(srpe)).ToList());
        }

        public ActionResult Respond(int id)
        {
            Request request = this.requestTasks.Get(id);
            if (request != null)
            {
                ViewBag.Request = request;
                ViewBag.User = this.userTasks.GetAdminUser(User.Identity.Name);
                ViewBag.HasProfileAccess = ((PrfPrincipal)User).HasPermission(AdminPermission.CanViewAndSearchPersons);

                // ensure we've persisted ScreeningRequestPersonEntity for every RequestPerson by this stage (its ID is used as param to 'Others' Input' link).
                this.screeningTasks.CreateScreeningRequestPersonEntitiesForRequest(request, ViewBag.User.GetScreeningEntity(), User.Identity.Name);
                RespondViewModel vm = new RespondViewModel(request, this.screeningTasks.GetScreeningResults(request.GetCreatedDate()), ViewBag.User.GetScreeningEntity());

                return View(vm);
            }
            return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        public ActionResult Respond(RespondViewModel vm)
        {
            if (ModelState.IsValid)
            {
                AdminUser user = this.userTasks.GetAdminUser(User.Identity.Name);

                // update responses
                foreach (ScreeningRequestPersonEntityViewModel rvm in vm.Responses.Values)
                {
                    ScreeningRequestPersonEntity srpe;
                    if (rvm.Id > 0)
                        srpe = this.screeningTasks.GetScreeningRequestPersonEntity(rvm.Id);
                    else
                    {
                        srpe = new ScreeningRequestPersonEntity();
                        srpe.ScreeningEntity = user.GetScreeningEntity();
                    }

                    if (srpe.Version > rvm.Version)
                    {
                        ModelState.AddModelError("somekey", "Data has changed since you loaded this page.  Please reload before saving again.  Try pressing the back button to recover your edits.");
                        return Respond(vm);
                    }

                    srpe.Reason = rvm.Reason;
                    srpe.Commentary = rvm.Commentary;
                    srpe.Version = rvm.Version;
                    srpe.RequestPerson = this.requestPersonTasks.GetRequestPerson(rvm.RequestPersonID);
                    srpe.ScreeningResult = this.screeningTasks.GetScreeningResult(rvm.ScreeningResultID);
                    this.screeningTasks.SaveOrUpdateScreeningRequestPersonEntity(srpe, User.Identity.Name, ScreeningStatus.UPDATED);
                }
                if (vm.SubmitResponse)
                {
                    ViewBag.SuccessMessage = "Screening request response complete.";

                    // save ScreeningRequestEntityResponse
                    Request request = this.requestTasks.Get(vm.Id);
                    this.screeningTasks.SetEntityResponse(request, user.GetScreeningEntity());

                    // update request history with new status
                    string newStatus = this.screeningTasks.HasAllResponses(request) ? RequestStatus.NAME_SENT_FOR_CONSOLIDATION : RequestStatus.NAME_SCREENING_IN_PROGRESS;
                    this.requestTasks.SaveRequestHistory(vm.Id, this.requestTasks.GetRequestStatus(newStatus).Id, User.Identity.Name);

                    // send email notification
                    this.emailTasks.SendRespondedToEmail(User.Identity.Name, vm.Id);

                    return RedirectToAction("Index");
                }
                else
                {
                    // need to start a new transaction in order to refresh srpe.Versions
                    return RedirectToAction("Respond", new { id = vm.Id });
                }
            }
            return Respond(vm.Id);
        }

        /// <summary>
        /// Update single response (i.e. single ScreeningRequestPersonEntity).
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        [HttpPost]
        [Transaction]
        public JsonNetResult RespondSingle(ScreeningRequestPersonEntityViewModel vm)
        {
            if (ModelState.IsValid)
            {
                AdminUser user = this.userTasks.GetAdminUser(User.Identity.Name);

                ScreeningRequestPersonEntity srpe;
                if (vm.Id > 0)
                    srpe = this.screeningTasks.GetScreeningRequestPersonEntity(vm.Id);
                else
                {
                    srpe = new ScreeningRequestPersonEntity();
                    srpe.ScreeningEntity = user.GetScreeningEntity();
                }

                if (srpe.Version > vm.Version)
                {
                    Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    string name = srpe.RequestPerson != null && srpe.RequestPerson.Person != null ? "for <strong>" + srpe.RequestPerson.Person.Name : string.Empty;
                    ModelState.AddModelError("Version", "Response " + name + " has changed since you loaded this page.  Try reloading before saving again.");
                    return JsonNet(this.GetErrorsForJson());
                }

                srpe.Reason = vm.Reason;
                srpe.Commentary = vm.Commentary;
                srpe.Version = vm.Version;
                srpe.RequestPerson = this.requestPersonTasks.GetRequestPerson(vm.RequestPersonID);
                srpe.ScreeningResult = this.screeningTasks.GetScreeningResult(vm.ScreeningResultID);
                this.screeningTasks.SaveOrUpdateScreeningRequestPersonEntity(srpe, User.Identity.Name, ScreeningStatus.UPDATED);

                // the page that calls this json action may need to initiate a reload, in order for the srpe.Version parameter to be up to date.
                return JsonNet("Save successful.");
            }
            Response.StatusCode = (int)HttpStatusCode.Forbidden;
            return JsonNet(this.GetErrorsForJson());
        }

        public void Export(int id)
        {
            Request request = this.requestTasks.Get(id);
            string sortByRankStr = Request.QueryString.Get("sortByRank");
            if (request != null)
            {
                AdminUser user = this.userTasks.GetAdminUser(User.Identity.Name);
                if (user != null)
                {
                    byte[] pdf = this.pdfExportService.GetExport(request, user.GetScreeningEntity(), Convert.ToBoolean(sortByRankStr));
                    if (pdf != null)
                    {
                        string fileName = user.GetScreeningEntity() != null 
                            ? request.ReferenceNumber + " response from " + user.GetScreeningEntity().ToString() + ".pdf"
                            : request.ReferenceNumber + ".pdf";
                        Response.ContentType = "application/pdf";
                        Response.AddHeader("Content-Disposition", "attachment; filename=\"" + fileName + "\"");
                        Response.OutputStream.Write(pdf, 0, pdf.Length);
                    }
                    else
                    {
                        Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        Response.StatusDescription = "Problem creating PDF.";
                    }
                }
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                Response.StatusDescription = "That request doesn't exist.";
            }
        }
    }
}