using AutoMapper;
using Mvc.JQuery.Datatables;
using Profiling2.Domain;
using Profiling2.Domain.Contracts.Export.Screening;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Contracts.Tasks.Screenings;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Careers;
using Profiling2.Domain.Scr;
using Profiling2.Domain.Scr.Person;
using Profiling2.Domain.Scr.PersonEntity;
using Profiling2.Web.Mvc.Areas.Screening.Controllers.ViewModels;
using Profiling2.Web.Mvc.Controllers;
using SharpArch.NHibernate.Web.Mvc;
using SharpArch.Web.Mvc.JsonNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Profiling2.Infrastructure.Security;

namespace Profiling2.Web.Mvc.Areas.Screening.Controllers
{
    [MultiRoleAuthorize(
        AdminRole.ProfilingAdmin,
        AdminRole.ScreeningRequestInitiator,
        AdminRole.ScreeningRequestValidator,
        AdminRole.ScreeningRequestConditionalityParticipant,
        AdminRole.ScreeningRequestConsolidator,
        AdminRole.ScreeningRequestFinalDecider
    )]
    public class RequestsController : ScreeningBaseController
    {
        protected readonly IRequestTasks requestTasks;
        protected readonly IUserTasks userTasks;
        protected readonly IRequestPersonTasks requestPersonTasks;
        protected readonly IPdfExportRequestForInitiatorService pdfExportService;
        protected readonly IScreeningTasks screeningTasks;
        protected readonly ILuceneTasks luceneTasks;

        public RequestsController(IRequestTasks requestTasks, 
            IUserTasks userTasks, 
            IRequestPersonTasks requestPersonTasks, 
            IPdfExportRequestForInitiatorService pdfExportService,
            IScreeningTasks screeningTasks,
            ILuceneTasks luceneTasks)
        {
            this.requestTasks = requestTasks;
            this.userTasks = userTasks;
            this.requestPersonTasks = requestPersonTasks;
            this.pdfExportService = pdfExportService;
            this.screeningTasks = screeningTasks;
            this.luceneTasks = luceneTasks;
        }

        //
        // GET: /Requests/
        
        public ActionResult Index()
        {
            return View();
        }

        public DataTablesResult<RequestView> DataTables(DataTablesParam p)
        {
            IQueryable<Request> q = this.requestTasks.GetRequestsQueryable().Where(x => !x.Archive);

            AdminUser user = this.userTasks.GetAdminUser(User.Identity.Name);
            // a user with only the initiator role and none of the other screening roles can only view selected requests
            if (User.IsInRole(AdminRole.ScreeningRequestInitiator)
                && !User.IsInRole(AdminRole.ScreeningRequestValidator)
                && !((PrfPrincipal)User).HasPermission(AdminPermission.CanPerformScreeningInput)
                && !User.IsInRole(AdminRole.ScreeningRequestConsolidator)
                && !User.IsInRole(AdminRole.ScreeningRequestFinalDecider)
                && user.ScreeningEntities.Count == 0)  // being a member of a screening entity allows you to view any request
            {
                q = q.Where(x => x.UserHasPermission(user));
            }

            DataTablesResult<RequestView> result = DataTablesResult.Create(q.Select(x => new RequestView(x)), p, rv => new
                {
                    // custom transform for date field - default is to transform datetimes from UTC to local time, but we store
                    // dates in local time (this could be rethought).
                    CurrentStatusDate = rv.CurrentStatusDate.ToString()
                });
            return result;
        }

        public JsonNetResult DataTablesLucene(DataTablesParam p)
        {
            // calculate total results to request from lucene search
            int numResults = (p.iDisplayStart >= 0 && p.iDisplayLength > 0) ? (p.iDisplayStart + 1) * p.iDisplayLength : 10;

            // figure out sort column - tied to frontend table columns.  assuming one column for now.
            string sortField = null;
            if (p.iSortCol != null && p.iSortingCols > 0)
            {
                switch (p.iSortCol.First())
                {
                    case 0:
                        sortField = "ReferenceNumberSortable"; break;
                    case 1:
                        sortField = "RequestName"; break;
                    case 2:
                        sortField = "RequestEntity"; break;
                    case 3:
                        sortField = "RequestType"; break;
                    case 4:
                        sortField = "CurrentStatus"; break;
                    case 5:
                        sortField = "CurrentStatusDate"; break;
                    case 6:
                        sortField = "Persons"; break;
                }
            }

            // figure out sort direction
            bool descending = true;
            if (p.sSortDir != null && string.Equals(p.sSortDir.First(), "asc"))
                descending = false;

            IList<LuceneSearchResult> results;

            // a user with only the initiator role and none of the other screening roles can only view selected requests
            AdminUser user = this.userTasks.GetAdminUser(User.Identity.Name);
            if (User.IsInRole(AdminRole.ScreeningRequestInitiator)
                && !User.IsInRole(AdminRole.ScreeningRequestValidator)
                && !((PrfPrincipal)User).HasPermission(AdminPermission.CanPerformScreeningInput)
                && !User.IsInRole(AdminRole.ScreeningRequestConsolidator)
                && !User.IsInRole(AdminRole.ScreeningRequestFinalDecider)
                && user.ScreeningEntities.Count == 0)  // being a member of a screening entity allows you to view any request
            {
                results = this.luceneTasks.RequestSearch(p.sSearch, numResults, user, sortField, descending);
            }
            else
            {
                results = this.luceneTasks.RequestSearch(p.sSearch, numResults, null, sortField, descending);
            }

            int iTotalRecords = 0;
            if (results != null && results.Count > 0)
                iTotalRecords = results.First().TotalHits;

            object[] aaData = results
                .Select(x => new RequestDataTableLuceneView(x))
                .Skip(p.iDisplayStart)
                .Take(p.iDisplayLength)
                .ToArray<RequestDataTableLuceneView>();

            return JsonNet(new DataTablesData
            {
                iTotalRecords = iTotalRecords,
                iTotalDisplayRecords = iTotalRecords,
                sEcho = p.sEcho,
                aaData = aaData.ToArray()
            });
        }

        //
        // GET: /Requests/Details/5

        public ActionResult Details(int id)
        {
            Request request = this.requestTasks.Get(id);

            if (request != null)
            {
                IDictionary<int, IList<Career>> personCareers = new Dictionary<int, IList<Career>>();
                foreach (RequestPerson rp in request.Persons.Where(x => !x.Archive))
                    personCareers.Add(rp.Person.Id, this.requestPersonTasks.GetHistoricalCurrentCareers(rp, false));
                ViewBag.PersonCareers = personCareers;

                ViewBag.Histories = this.requestTasks.GetRequestHistory(id);
                ViewBag.User = this.userTasks.GetAdminUser(User.Identity.Name);
                return View(request);
            }
            return new HttpNotFoundResult();
        }
        
        //
        // GET: /Requests/Edit/5

        [PermissionAuthorize(AdminPermission.CanAdministrate)]
        public ActionResult Edit(int id)
        {
            Request request = this.requestTasks.Get(id);
            if (request != null)
            {
                // populate existing data
                RequestViewModel rvm = new RequestViewModel(request);

                // populate dropdown values
                rvm.PopulateDropDowns(this.requestTasks.GetRequestEntities(), this.requestTasks.GetRequestTypes(), this.requestTasks.GetRequestStatuses());
                rvm.RequestEntityID = request.RequestEntity.Id;
                rvm.RequestTypeID = request.RequestType.Id;
                rvm.RequestStatusID = request.CurrentStatus.Id;

                return View(rvm);
            }
            else
                return new HttpNotFoundResult();
        }

        //
        // POST: /Requests/Edit/5

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanAdministrate)]
        public ActionResult Edit(RequestViewModel rvm)
        {
            if (ModelState.IsValid)
            {
                Request request = this.requestTasks.Get(rvm.Id);
                Mapper.Map<RequestViewModel, Request>(rvm, request);
                request.RequestEntity = this.requestTasks.GetRequestEntity(rvm.RequestEntityID);
                request.RequestType = this.requestTasks.GetRequestType(rvm.RequestTypeID);
                if (!string.IsNullOrEmpty(rvm.RespondBy))
                    request.RespondBy = DateTime.Parse(rvm.RespondBy);
                else
                    request.RespondBy = null;

                this.requestTasks.SaveRequestHistory(request.Id, rvm.RequestStatusID, User.Identity.Name);

                if (rvm.RequestStatusID == this.requestTasks.GetRequestStatus(RequestStatus.NAME_DELETED).Id)
                    request.Archive = true;

                this.requestTasks.SaveOrUpdateRequest(request);
                ViewBag.SuccessMessage = "Screening request successfully edited.";
            }
            return Edit(rvm.Id);
        }

        public void Export(int id)
        {
            Request request = this.requestTasks.Get(id);
            string sortByRankStr = Request.QueryString.Get("sortByRank");
            if (request != null)
            {
                // For some reason this OO-version doesn't produce a PDF that Adobe will open.
                //RequestPdf pdf = new RequestPdf(request);
                // Original Profiling1 code using Aspose.Pdf.Generator namespace
                byte[] pdf = this.pdfExportService.GetExport(request, Convert.ToBoolean(sortByRankStr));
                if (pdf != null)
                {
                    string fileName = request.ReferenceNumber + ".pdf";
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
            else
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                Response.StatusDescription = "That request doesn't exist.";
            }
        }

        [Transaction]
        [PermissionAuthorize(AdminPermission.CanAdministrate)]
        public ActionResult UndoResponse(int id, string screeningEntityName)
        {
            Request request = this.requestTasks.Get(id);
            if (request != null)
            {
                foreach (ScreeningRequestEntityResponse response in request.ScreeningRequestEntityResponses
                    .Where(x => string.Equals(screeningEntityName, x.ScreeningEntity.ScreeningEntityName)))
                    this.screeningTasks.UndoEntityResponse(response);

                // update request history with new status
                this.requestTasks.SaveRequestHistory(id, this.requestTasks.GetRequestStatus(RequestStatus.NAME_SCREENING_IN_PROGRESS).Id, User.Identity.Name);

                // TODO is there a need for a notification here?

                return RedirectToAction("Details", new { id = id });
            }
            else
                return new HttpNotFoundResult();
        }
    }
}
