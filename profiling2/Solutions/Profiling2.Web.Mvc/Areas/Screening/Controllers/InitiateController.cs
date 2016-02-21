using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using AutoMapper;
using Mvc.JQuery.Datatables;
using Profiling2.Domain;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Contracts.Tasks.Screenings;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Domain.Prf.Units;
using Profiling2.Domain.Scr;
using Profiling2.Domain.Scr.Person;
using Profiling2.Domain.Scr.Proposed;
using Profiling2.Infrastructure.Security;
using Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels;
using Profiling2.Web.Mvc.Areas.Screening.Controllers.ViewModels;
using Profiling2.Web.Mvc.Controllers;
using SharpArch.NHibernate.Web.Mvc;
using SharpArch.Web.Mvc.JsonNet;

namespace Profiling2.Web.Mvc.Areas.Screening.Controllers
{
    [MultiRoleAuthorize(
        AdminRole.ScreeningRequestInitiator
    )]
    public class InitiateController : ScreeningBaseController
    {
        protected readonly IRequestTasks requestTasks;
        protected readonly IUserTasks userTasks;
        protected readonly IPersonTasks personTasks;
        protected readonly IRequestPersonTasks requestPersonTasks;
        protected readonly IEmailTasks emailTasks;
        protected readonly ILuceneTasks luceneTasks;
        protected readonly IOrganizationTasks orgTasks;

        public InitiateController(IRequestTasks requestTasks, 
            IUserTasks userTasks, 
            IPersonTasks personTasks, 
            IRequestPersonTasks requestPersonTasks, 
            IEmailTasks emailTasks,
            ILuceneTasks luceneTasks,
            IOrganizationTasks orgTasks)
        {
            this.requestTasks = requestTasks;
            this.userTasks = userTasks;
            this.personTasks = personTasks;
            this.requestPersonTasks = requestPersonTasks;
            this.emailTasks = emailTasks;
            this.luceneTasks = luceneTasks;
            this.orgTasks = orgTasks;
        }

        public ActionResult Index()
        {
            ViewBag.UserRequestEntities = this.userTasks.GetAdminUser(User.Identity.Name).RequestEntities;
            return View();
        }

        public DataTablesResult<RequestView> DataTables(DataTablesParam p)
        {
            AdminUser user = this.userTasks.GetAdminUser(User.Identity.Name);

            IQueryable<Request> q = this.requestTasks.GetRequestsQueryable()
                .Where(x => !x.Archive)
                .Where(x => !x.HasBeenSentForValidation)
                .Where(x => x.UserHasPermission(user) || User.IsInRole(AdminRole.ScreeningRequestValidator));

            return DataTablesResult.Create<RequestView>(q.Select(x => new RequestView(x)), p);
        }

        public ActionResult Create()
        {
            RequestViewModel vm = new RequestViewModel();
            vm.ReferenceNumber = "dummy";  // dummy value to pass model validation
            vm.PopulateDropDowns(this.requestTasks.GetRequestEntities(), this.requestTasks.GetRequestTypes(), this.requestTasks.GetRequestStatuses());
            return View(vm);
        }

        [HttpPost]
        [Transaction]
        public ActionResult Create(RequestViewModel vm)
        {
            if (ModelState.IsValid)
            {
                Request request = new Request();
                Mapper.Map(vm, request);
                request.RequestEntity = this.requestTasks.GetRequestEntity(vm.RequestEntityID);
                request.RequestType = this.requestTasks.GetRequestType(vm.RequestTypeID);
                request.ReferenceNumber = this.requestTasks.GetNextReferenceNumber();  // TODO concurrency situation
                request = this.requestTasks.SaveOrUpdateRequest(request);

                // update status
                this.requestTasks.SaveRequestHistory(request.Id, this.requestTasks.GetRequestStatus(RequestStatus.NAME_CREATED).Id, User.Identity.Name);

                return RedirectToAction("Submit", new { id = request.Id });
            }
            return Create();
        }

        public ActionResult Submit(int id)
        {
            Request request = this.requestTasks.Get(id);
            if (request != null)
            {
                ViewBag.Request = request;

                RequestViewModel vm = new RequestViewModel(request);
                vm.RequestEntityID = request.RequestEntity.Id;
                vm.RequestTypeID = request.RequestType.Id;
                vm.PopulateDropDowns(this.requestTasks.GetRequestEntities(), this.requestTasks.GetRequestTypes(), this.requestTasks.GetRequestStatuses());
                return View(vm);
            }
            return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        public ActionResult Save(RequestViewModel vm)
        {
            if (ModelState.IsValid)
            {
                Request request = this.requestTasks.Get(vm.Id);
                if (request != null)
                {
                    Mapper.Map(vm, request);
                    request.RequestEntity = this.requestTasks.GetRequestEntity(vm.RequestEntityID);
                    request.RequestType = this.requestTasks.GetRequestType(vm.RequestTypeID);
                    if (!string.IsNullOrEmpty(vm.RespondBy))
                        request.RespondBy = DateTime.Parse(vm.RespondBy);
                    else
                        request.RespondBy = null;

                    // update status
                    this.requestTasks.SaveRequestHistory(request.Id, this.requestTasks.GetRequestStatus(RequestStatus.NAME_EDITED).Id, User.Identity.Name);

                    return RedirectToAction("Submit", "Initiate", new { area = "Screening", id = vm.Id });
                }
            }
            return Submit(vm.Id);
        }

        [HttpPost]
        [Transaction]
        public ActionResult Submit(RequestViewModel vm)
        {
            if (ModelState.IsValid)
            {
                Request request = this.requestTasks.Get(vm.Id);
                if (request != null)
                {
                    // don't 'send for validation' more than once
                    if (request.HasBeenSentForValidation)
                    {
                        ModelState.AddModelError("Status", "Request has already been sent for validation.");
                        return Submit(vm.Id);
                    }

                    Mapper.Map(vm, request);
                    request.RequestEntity = this.requestTasks.GetRequestEntity(vm.RequestEntityID);
                    request.RequestType = this.requestTasks.GetRequestType(vm.RequestTypeID);
                    if (!string.IsNullOrEmpty(vm.RespondBy))
                        request.RespondBy = DateTime.Parse(vm.RespondBy);
                    else
                        request.RespondBy = null;

                    // update status
                    this.requestTasks.SaveRequestHistory(request.Id, this.requestTasks.GetRequestStatus(RequestStatus.NAME_SENT_FOR_VALIDATION).Id, User.Identity.Name);

                    // send email
                    this.emailTasks.SendRequestSentForValidationEmail(User.Identity.Name, vm.Id);
                    if (request.ProposedPersons.Where(x => !x.Archive).Any())
                        this.emailTasks.SendPersonsProposedEmail(User.Identity.Name, vm.Id);

                    return RedirectToAction("Details", "Requests", new { area = "Screening", id = vm.Id });
                }
            }
            return Submit(vm.Id);
        }

        [Transaction]
        public JsonNetResult Delete(int id)
        {
            Request request = this.requestTasks.Get(id);
            if (request != null)
            {
                AdminUser user = this.userTasks.GetAdminUser(User.Identity.Name);
                if (request.UserHasPermission(user) || User.IsInRole(AdminRole.ScreeningRequestValidator))
                {
                    if (this.requestTasks.DeleteRequest(request))
                    {
                        this.requestTasks.SaveRequestHistory(id, this.requestTasks.GetRequestStatus(RequestStatus.NAME_DELETED).Id, User.Identity.Name);
                        return JsonNet("Request successfully deleted.");
                    }
                    else
                    {
                        Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        return JsonNet("Request can not be deleted after it has been submitted for validation.");
                    }
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    return JsonNet("User does not have permission for this request.");
                }
            }
            Response.StatusCode = (int)HttpStatusCode.NotFound;
            return JsonNet("Request does not exist.");
        }

        public ActionResult SearchPersonsModal()
        {
            return View();
        }

        public ActionResult SearchUnitsModal()
        {
            return View();
        }

        // same method in PersonsController, but exists here to allow ScreeningRequestInitiators and ScreeningRequestValidators to search persons
        // (these roles don't have access to PersonsController in themselves).
        public JsonNetResult PersonDataTables(DataTablesParam p)
        {
            // calculate total results to request from lucene search
            int numResults = (p.iDisplayStart >= 0 && p.iDisplayLength > 0) ? (p.iDisplayStart + 1) * p.iDisplayLength : 10;

            // conduct search - don't show restricted profiles outside of profiling team
            IList<LuceneSearchResult> results = this.luceneTasks.PersonSearch(p.sSearch, numResults,
                ((PrfPrincipal)User).HasPermission(AdminPermission.CanViewAndSearchRestrictedPersons));

            int iTotalRecords = 0;
            if (results != null && results.Count > 0)
                iTotalRecords = results.First().TotalHits;

            object[] aaData = results
                .Select(x => new PersonDataTableLuceneView(x))
                .Skip(p.iDisplayStart)
                .Take(p.iDisplayLength)
                .ToArray<PersonDataTableLuceneView>();

            return JsonNet(new DataTablesData
            {
                iTotalRecords = iTotalRecords,
                iTotalDisplayRecords = iTotalRecords,
                sEcho = p.sEcho,
                aaData = aaData.ToArray()
            });
        }

        [Transaction]
        public JsonNetResult AttachPerson(int requestId, int personId)
        {
            Request request = this.requestTasks.Get(requestId);
            Person person = this.personTasks.GetPerson(personId);
            if (request != null && person != null)
            {
                AdminUser user = this.userTasks.GetAdminUser(User.Identity.Name);
                if (request.UserHasPermission(user) || User.IsInRole(AdminRole.ScreeningRequestValidator))
                {
                    RequestPerson rp = this.requestPersonTasks.SaveRequestPerson(request, person);
                    this.requestPersonTasks.SaveRequestPersonHistory(rp, RequestPersonStatus.NAME_ADDED, user);
                    return JsonNet(string.Empty);
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    return JsonNet("User does not have permission for this request.");
                }
            }
            Response.StatusCode = (int)HttpStatusCode.NotFound;
            return JsonNet("Given Request or Person does not exist.");
        }

        [Transaction]
        public JsonNetResult RemovePerson(int requestId, int personId)
        {
            Request request = this.requestTasks.Get(requestId);
            Person person = this.personTasks.GetPerson(personId);
            if (request != null && person != null)
            {
                AdminUser user = this.userTasks.GetAdminUser(User.Identity.Name);
                if (request.UserHasPermission(user) || User.IsInRole(AdminRole.ScreeningRequestValidator))
                {
                    RequestPerson rp = this.requestPersonTasks.ArchiveRequestPerson(request, person);
                    this.requestPersonTasks.SaveRequestPersonHistory(rp, RequestPersonStatus.NAME_REMOVED, user);
                    return JsonNet(string.Empty);
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    return JsonNet("User does not have permission for this request.");
                }
            }
            Response.StatusCode = (int)HttpStatusCode.NotFound;
            return JsonNet("Given Request or Person does not exist.");
        }

        public DataTablesResult<RequestPersonDataTableView> AttachedPersonsDataTables(int id, DataTablesParam p)
        {
            Request request = this.requestTasks.Get(id);
            if (request != null)
            {
                IEnumerable<RequestPersonDataTableView> persons = request.Persons.Where(x => !x.Archive).Select(x => new RequestPersonDataTableView(x));
                persons = persons.Concat(request.ProposedPersons.Where(x => !x.Archive).Select(x => new RequestPersonDataTableView(x)));
                return DataTablesResult.Create(persons.AsQueryable(), p);
            }
            return null;
        }

        public ActionResult ProposePersonModal(int id)
        {
            ProposedPersonViewModel vm = new ProposedPersonViewModel()
            {
                RequestId = id
            };
            return View(vm);
        }

        [HttpPost]
        [Transaction]
        public JsonNetResult ProposePersonModal(ProposedPersonViewModel vm)
        {
            if (ModelState.IsValid)
            {
                Request request = this.requestTasks.Get(vm.RequestId);
                if (request != null)
                {
                    AdminUser user = this.userTasks.GetAdminUser(User.Identity.Name);
                    if (request.UserHasPermission(user) || User.IsInRole(AdminRole.ScreeningRequestValidator))
                    {
                        RequestProposedPerson rpp = new RequestProposedPerson();
                        rpp.PersonName = vm.Name;
                        rpp.MilitaryIDNumber = vm.MilitaryIDNumber;
                        rpp.Notes = vm.Notes;
                        rpp.Request = request;
                        rpp = this.requestPersonTasks.SaveRequestProposedPerson(rpp);
                        this.requestPersonTasks.SaveRequestProposedPersonHistory(rpp, RequestProposedPersonStatus.NAME_PROPOSED, user);
                        return JsonNet(new
                        {
                            WasSuccessful = true
                        });
                    }
                    else
                    {
                        Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        return JsonNet("User does not have permission for this request.");
                    }
                }
                else
                    ModelState.AddModelError("RequestId", "Request does not exist.");
            }
            return JsonNet(this.GetErrorsForJson());
        }

        [Transaction]
        public JsonNetResult RemoveProposedPerson(int requestId, int proposedPersonId)
        {
            Request request = this.requestTasks.Get(requestId);
            RequestProposedPerson rpp = this.requestPersonTasks.GetRequestProposedPerson(proposedPersonId);
            if (request != null && rpp != null)
            {
                AdminUser user = this.userTasks.GetAdminUser(User.Identity.Name);
                if (request.UserHasPermission(user) || User.IsInRole(AdminRole.ScreeningRequestValidator))
                {
                    rpp.Archive = true;
                    rpp = this.requestPersonTasks.SaveRequestProposedPerson(rpp);
                    this.requestPersonTasks.SaveRequestProposedPersonHistory(rpp, RequestProposedPersonStatus.NAME_WITHDRAWN, user);
                    return JsonNet(string.Empty);
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    return JsonNet("User does not have permission for this request.");
                }
            }
            Response.StatusCode = (int)HttpStatusCode.NotFound;
            return JsonNet("Given Request or Person does not exist.");
        }

        public JsonNetResult UnitDataTables(DataTablesParam p)
        {
            // calculate total results to request from lucene search
            int numResults = (p.iDisplayStart >= 0 && p.iDisplayLength > 0) ? (p.iDisplayStart + 1) * p.iDisplayLength : 10;

            // conduct search - don't show restricted profiles outside of profiling team
            IList<LuceneSearchResult> results = this.luceneTasks.UnitSearch(p.sSearch, numResults);

            int iTotalRecords = 0;
            if (results != null && results.Count > 0)
                iTotalRecords = results.First().TotalHits;

            object[] aaData = results
                .Select(x => new UnitDataTableLuceneView(x))
                .Skip(p.iDisplayStart)
                .Take(p.iDisplayLength)
                .ToArray<UnitDataTableLuceneView>();

            return JsonNet(new DataTablesData
            {
                iTotalRecords = iTotalRecords,
                iTotalDisplayRecords = iTotalRecords,
                sEcho = p.sEcho,
                aaData = aaData.ToArray()
            });
        }

        public DataTablesResult<UnitDataTableLuceneView> AttachedUnitsDataTables(int id, DataTablesParam p)
        {
            Request request = this.requestTasks.Get(id);
            if (request != null)
            {
                IEnumerable<UnitDataTableLuceneView> units = request.Units.Where(x => !x.Archive).Select(x => new UnitDataTableLuceneView() 
                { 
                    Id = x.Unit.Id.ToString(),
                    Name = x.Unit.UnitName,
                    BackgroundInformation = string.IsNullOrEmpty(x.Unit.BackgroundInformation) ? string.Empty : x.Unit.BackgroundInformation,
                    Organization = x.Unit.Organization != null ? x.Unit.Organization.OrgShortName : string.Empty
                });
                //persons = persons.Concat(request.ProposedPersons.Where(x => !x.Archive).Select(x => new RequestPersonDataTableView(x)));
                return DataTablesResult.Create(units.AsQueryable(), p);
            }
            return null;
        }

        [Transaction]
        public JsonNetResult AttachUnit(int requestId, int unitId)
        {
            Request request = this.requestTasks.Get(requestId);
            Unit unit = this.orgTasks.GetUnit(unitId);
            if (request != null && unit != null)
            {
                AdminUser user = this.userTasks.GetAdminUser(User.Identity.Name);
                if (request.UserHasPermission(user) || User.IsInRole(AdminRole.ScreeningRequestValidator))
                {
                    RequestUnit ru = this.requestPersonTasks.SaveRequestUnit(request, unit);
                    return JsonNet(string.Empty);
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    return JsonNet("User does not have permission for this request.");
                }
            }
            Response.StatusCode = (int)HttpStatusCode.NotFound;
            return JsonNet("Given Request or Unit does not exist.");
        }

        [Transaction]
        public JsonNetResult RemoveUnit(int requestId, int unitId)
        {
            Request request = this.requestTasks.Get(requestId);
            Unit unit = this.orgTasks.GetUnit(unitId);
            if (request != null && unit != null)
            {
                AdminUser user = this.userTasks.GetAdminUser(User.Identity.Name);
                if (request.UserHasPermission(user) || User.IsInRole(AdminRole.ScreeningRequestValidator))
                {
                    RequestUnit ru = this.requestPersonTasks.ArchiveRequestUnit(request, unit);
                    return JsonNet(string.Empty);
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    return JsonNet("User does not have permission for this request.");
                }
            }
            Response.StatusCode = (int)HttpStatusCode.NotFound;
            return JsonNet("Given Request or Unit does not exist.");
        }
    }
}