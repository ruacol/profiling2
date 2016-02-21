using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Xml;
using AutoMapper;
using log4net;
using Mvc.JQuery.Datatables;
using Profiling2.Domain;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Contracts.Tasks.Sources;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.DTO;
using Profiling2.Domain.Prf.Events;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Domain.Prf.Sources;
using Profiling2.Domain.Prf.Suggestions;
using Profiling2.Infrastructure.Security;
using Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels;
using Profiling2.Web.Mvc.Controllers;
using SharpArch.NHibernate.Web.Mvc;
using SharpArch.Web.Mvc.JsonNet;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers
{
    public class EventsController : BaseController
    {
        protected readonly ILog log = LogManager.GetLogger(typeof(EventsController));

        protected readonly IEventTasks eventTasks;
        protected readonly ILocationTasks locationTasks;
        protected readonly IPersonTasks personTasks;
        protected readonly IUserTasks userTasks;
        protected readonly ISuggestionTasks suggestionTasks;
        protected readonly IAuditTasks auditTasks;
        protected readonly ILuceneTasks luceneTasks;
        protected readonly ISourceTasks sourceTasks;
        protected readonly IEventMatchingTasks eventMatchingTasks;

        public EventsController(IEventTasks eventTasks, 
            ILocationTasks locationTasks, 
            IPersonTasks personTasks, 
            IUserTasks userTasks, 
            ISuggestionTasks suggestionTasks,
            IAuditTasks auditTasks,
            ILuceneTasks luceneTasks,
            ISourceTasks sourceTasks,
            IEventMatchingTasks eventMatchingTasks)
        {
            this.eventTasks = eventTasks;
            this.locationTasks = locationTasks;
            this.personTasks = personTasks;
            this.userTasks = userTasks;
            this.suggestionTasks = suggestionTasks;
            this.auditTasks = auditTasks;
            this.luceneTasks = luceneTasks;
            this.sourceTasks = sourceTasks;
            this.eventMatchingTasks = eventMatchingTasks;
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchEvents)]
        public ActionResult Index()
        {
            return View();
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchEvents)]
        public JsonNetResult DataTables(DataTablesParam p)
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
                        sortField = "Id"; break;
                    case 1:
                        sortField = "JhroCaseNumber"; break;
                    case 3:
                        sortField = "StartDateSearch"; break;
                    case 4:
                        sortField = "EndDateSearch"; break;
                    case 5:
                        sortField = "Location"; break;
                }
            }

            // figure out sort direction
            bool descending = true;
            if (p.sSortDir != null && string.Equals(p.sSortDir.First(), "asc"))
                descending = false;

            // parse date filter inputs
            DateTime s, e;
            DateTime? start = null, end = null;
            if (DateTime.TryParse(Request.QueryString["start-date"], out s))
                start = s;
            if (DateTime.TryParse(Request.QueryString["end-date"], out e))
                end = e;

            // run search
            IList<LuceneSearchResult> results = null;
            if (start.HasValue || end.HasValue)
                results = this.luceneTasks.EventSearch(p.sSearch, start, end, numResults, sortField, descending);
            else
                results = this.luceneTasks.EventSearch(p.sSearch, numResults, sortField, descending);

            int iTotalRecords = 0;
            if (results != null && results.Count > 0)
                iTotalRecords = results.First().TotalHits;

            object[] aaData = results
                .Select(x => new EventDataTableView(x))
                .Skip(p.iDisplayStart)
                .Take(p.iDisplayLength)
                .ToArray<EventDataTableView>();

            return JsonNet(new DataTablesData
            {
                iTotalRecords = iTotalRecords,
                iTotalDisplayRecords = iTotalRecords,
                sEcho = p.sEcho,
                aaData = aaData.ToArray()
            });
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchEvents)]
        public JsonNetResult Get()
        {
            string term = Request.QueryString["term"];
            if (!string.IsNullOrEmpty(term))
            {
                term = term.Trim();
                Regex regex = new Regex("^[0-9]+$");
                if (regex.IsMatch(term))
                {
                    int eventId;
                    if (int.TryParse(term, out eventId))
                    {
                        Event e = this.eventTasks.GetEvent(eventId);
                        if (e != null)
                        {
                            return JsonNet(new object[] {
                                new
                                {
                                    id = e.Id,
                                    text = e.Headline,
                                    narrativeEn = e.NarrativeEn,
                                    narrativeFr = e.NarrativeFr
                                }
                            });
                        }
                    }
                }
                else
                {
                    IList<Event> events = this.eventTasks.GetEvents(term);
                    object[] objects = (from i in events
                                        select new { id = i.Id, text = i.Headline }).ToArray();
                    return JsonNet(objects);
                }
            }
            return JsonNet(string.Empty);
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchEvents)]
        public JsonNetResult Name(int id)
        {
            Event e = this.eventTasks.GetEvent(id);
            if (e != null)
            {
                return JsonNet(new
                {
                    Id = id,
                    Name = e.Headline
                });
            }
            else
                return JsonNet(string.Empty);
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchEvents)]
        public JsonNetResult Json(int id)
        {
            Event e = this.eventTasks.GetEvent(id);
            if (e != null)
            {
                IDictionary<string, object> jsonObj = new Dictionary<string, object>();

                jsonObj.Add("Event", e.ToShortJSON());

                jsonObj.Add("Categories", from v in e.Violations
                                          select new { Id = v.Id, Name = v.Name });

                jsonObj.Add("EventSources", from es in e.EventSources
                                            where !es.Archive
                                            select new { Id = es.Id, SourceId = es.Source.Id });

                jsonObj.Add("OrganizationResponsibilities", from or in e.OrganizationResponsibilities
                                                             where !or.Archive
                                                             select new { Id = or.Id, Organization = or.Organization.ToString() });

                jsonObj.Add("PersonResponsibilities", from pr in e.PersonResponsibilities
                                                      where !pr.Archive
                                                      select new { Id = pr.Id, Person = pr.Person.Name });

                jsonObj.Add("EventRelationships", from er in e.GetEventRelationships()
                                                  where !er.Archive
                                                  select new 
                                                  { 
                                                      Id = er.Id, 
                                                      RelationshipTypeName = er.EventRelationshipType == null ? string.Empty : er.EventRelationshipType.ToString()
                                                  });

                jsonObj.Add("ActionsTaken", from at in e.ActionTakens
                                           where !at.Archive
                                           select new { Id = at.Id, ActionTakenTypeName = at.ActionTakenType == null ? string.Empty : at.ActionTakenType.ToString() });

                return JsonNet(jsonObj);
            }

            Response.StatusCode = (int)HttpStatusCode.NotFound;
            return JsonNet(string.Empty);
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchEvents)]
        public JsonNetResult RedFlags(int id)
        {
            Event ev = this.eventTasks.GetEvent(id);
            if (ev != null)
                return JsonNet(ev.GetToBeConfirmedDTOs());

            Response.StatusCode = (int)HttpStatusCode.NotFound;
            return JsonNet(string.Empty);
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchEvents)]
        public ActionResult Details(int id)
        {
            Event e = this.eventTasks.GetEvent(id);
            if (e != null)
                return View(e);
            else
                return new HttpNotFoundResult();
        }

        [PermissionAuthorize(AdminPermission.CanChangeEvents)]
        public ActionResult Create()
        {
            EventViewModel evm = new EventViewModel();
            evm.PopulateDropDowns(this.eventTasks.GetAllEventVerifiedStatuses());
            return View(evm);
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangeEvents)]
        public JsonNetResult Create(EventViewModel evm)
        {
            if (ModelState.IsValid)
            {
                Event e = new Event();
                Mapper.Map<EventViewModel, Event>(evm, e);
                if (!string.IsNullOrEmpty(evm.ViolationIds))
                {
                    string[] ids = evm.ViolationIds.Split(',');
                    foreach (string id in ids)
                    {
                        int result;
                        if (int.TryParse(id, out result))
                        {
                            Violation v = this.eventTasks.GetViolation(result);
                            if (v != null)
                                e.Violations.Add(v);
                        }
                    }
                }
                e.Location = this.locationTasks.GetLocation(evm.LocationId.Value);
                e.EventVerifiedStatus = evm.EventVerifiedStatusId.HasValue ? this.eventTasks.GetEventVerifiedStatus(evm.EventVerifiedStatusId.Value) : null;
                if (!string.IsNullOrEmpty(evm.TagIds))
                {
                    string[] ids = evm.TagIds.Split(',');
                    foreach (string id in ids)
                    {
                        int result;
                        if (int.TryParse(id, out result))
                        {
                            Tag t = this.eventTasks.GetTag(result);
                            if (t != null)
                                e.Tags.Add(t);
                        }
                    }
                }
                if (!string.IsNullOrEmpty(evm.JhroCaseIds))
                {
                    string[] ids = evm.JhroCaseIds.Split(',');
                    foreach (string id in ids)
                    {
                        int result;
                        if (int.TryParse(id, out result))
                        {
                            JhroCase jc = this.sourceTasks.GetJhroCase(result);
                            if (jc != null)
                                e.AddJhroCase(jc);
                        }
                    }
                }
                e = this.eventTasks.SaveEvent(e);

                return JsonNet(new
                {
                    Id = e.Id,
                    Name = e.Headline,
                    WasSuccessful = true
                });
            }
            else
            {
                return JsonNet(this.GetErrorsForJson());
            }
        }

        [PermissionAuthorize(AdminPermission.CanChangeEvents)]
        public ActionResult Edit(int id)
        {
            Event e = this.eventTasks.GetEvent(id);
            if (e != null)
            {
                EventViewModel evm = new EventViewModel();
                Mapper.Map<Event, EventViewModel>(e, evm);
                evm.ViolationIds = string.Join(",", (from v in e.Violations select v.Id).ToList<int>());
                evm.LocationId = e.Location.Id;
                evm.LocationText = e.Location.ToString();
                evm.JhroCaseIds = string.Join(",", e.JhroCases.Select(x => x.Id.ToString()));
                evm.TagIds = string.Join(",", (from t in e.Tags select t.Id).ToList<int>());
                evm.PopulateDropDowns(this.eventTasks.GetAllEventVerifiedStatuses());
                return View(evm);
            }
            else
                return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangeEvents)]
        public JsonNetResult Edit(EventViewModel evm)
        {
            if (ModelState.IsValid)
            {
                Event e = this.eventTasks.GetEvent(evm.Id);
                if (e != null)
                {
                    Mapper.Map<EventViewModel, Event>(evm, e);
                    if (!string.IsNullOrEmpty(evm.ViolationIds))
                    {
                        string[] ids = evm.ViolationIds.Split(',');
                        e.Violations.Clear();
                        foreach (string id in ids)
                        {
                            int result;
                            if (int.TryParse(id, out result))
                            {
                                Violation v = this.eventTasks.GetViolation(result);
                                if (v != null)
                                    e.Violations.Add(v);
                            }
                        }
                    }
                    e.Location = this.locationTasks.GetLocation(evm.LocationId.Value);
                    e.EventVerifiedStatus = evm.EventVerifiedStatusId.HasValue ? this.eventTasks.GetEventVerifiedStatus(evm.EventVerifiedStatusId.Value) : null;
                    e.Tags.Clear();
                    if (!string.IsNullOrEmpty(evm.TagIds))
                    {
                        string[] ids = evm.TagIds.Split(',');
                        foreach (string id in ids)
                        {
                            int result;
                            if (int.TryParse(id, out result))
                            {
                                Tag t = this.eventTasks.GetTag(result);
                                if (t != null)
                                    e.Tags.Add(t);
                            }
                        }
                    }
                    e.JhroCases.Clear();
                    if (!string.IsNullOrEmpty(evm.JhroCaseIds))
                    {
                        string[] ids = evm.JhroCaseIds.Split(',');
                        foreach (string id in ids)
                        {
                            int result;
                            if (int.TryParse(id, out result))
                            {
                                JhroCase jc = this.sourceTasks.GetJhroCase(result);
                                if (jc != null)
                                    e.AddJhroCase(jc);
                            }
                        }
                    }
                    e = this.eventTasks.SaveEvent(e);

                    return JsonNet(string.Empty);
                }
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return JsonNet("Event not found.");
            }
            else
            {
                return JsonNet(this.GetErrorsForJson());
            }
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchEvents)]
        public JsonNetResult Violations(int id)
        {
            if (!string.IsNullOrEmpty(Request.QueryString["term"]))
                return JsonNet(this.eventTasks.GetViolationsJson(Request.QueryString["term"], id));
            else
                return JsonNet(this.eventTasks.GetViolationsJson(id));
        }

        [PermissionAuthorize(AdminPermission.CanChangeEvents)]
        public ActionResult Automap()
        {
            IList<Event> events = this.eventTasks.GetAllEvents().Where(x => !string.IsNullOrEmpty(x.EventName) && !x.Violations.Any()).ToList();
            IDictionary<Event, IDictionary<Violation, int>> matches = new Dictionary<Event, IDictionary<Violation, int>>();

            foreach (Event e in events)
                matches.Add(e, this.eventTasks.ScoreViolations(e.EventName, new string[] { ",", " and ", "/", "\\", ";" }));

            return View(matches);
        }

        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangeEvents)]
        public ActionResult AutomapSave()
        {
            IList<Event> events = this.eventTasks.GetAllEvents();//.AsQueryable<Event>().Take<Event>(50).ToList<Event>();
            IDictionary<Event, IDictionary<Violation, int>> matches = new Dictionary<Event, IDictionary<Violation, int>>();

            foreach (Event e in events)
                matches.Add(e, this.eventTasks.ScoreViolations(e.EventName, new string[] { ",", " and ", "/", "\\", ";" }));

            // save
            foreach (Event e in events)
            {
                IDictionary<Violation, int> scores = matches[e];
                foreach (KeyValuePair<Violation, int> kvp in scores)
                    if (kvp.Value <= 2)
                        if (!e.Violations.Contains(kvp.Key))
                            e.Violations.Add(kvp.Key);
            }

            return RedirectToAction("Index", "Violations");
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchEvents)]
        public ActionResult SuggestModal()
        {
            ViewBag.Features = this.suggestionTasks.GetAdminSuggestionFeaturePersonResponsibilities();
            return View();
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchEvents)]
        public JsonNetResult Suggest(int id, ICollection<int> enabledIds)
        {
            //int iTotalRecords = this.suggestionTasks.GetSuggestionTotal(id);
            //IList<SuggestionEventForPersonDTO> suggestions = this.suggestionTasks.GetSuggestionResults(p.iDisplayStart, p.iDisplayLength, id);
            //object[] aaData = (from s in suggestions select s).ToArray<SuggestionEventForPersonDTO>();

            //return JsonNet(new DataTablesData
            //{
            //    iTotalRecords = iTotalRecords,
            //    iTotalDisplayRecords = iTotalRecords,
            //    sEcho = p.sEcho,
            //    aaData = aaData.ToArray()
            //});

            Person person = this.personTasks.GetPerson(id);
            if (person != null)
            {
                IList<SuggestionEventForPersonDTO> suggestions = this.suggestionTasks.GetSuggestionsRefactored(person, enabledIds.ToArray());
                return JsonNet(new DataTablesData
                {
                    iTotalRecords = suggestions.Count,
                    iTotalDisplayRecords = suggestions.Count,
                    aaData = suggestions.ToArray<SuggestionEventForPersonDTO>()
                });
            }
            Response.StatusCode = (int)HttpStatusCode.NotFound;
            return JsonNet(null);
        }

        [HttpPost, ValidateInput(false)]  // vm.SuggestionFeatures is XML
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanViewAndSearchEvents)]
        public JsonNetResult SuggestSave(AdminSuggestionPersonResponsibilityViewModel vm)
        {
            if (ModelState.IsValid)
            {
                AdminSuggestionPersonResponsibility aspr = new AdminSuggestionPersonResponsibility();
                aspr.DecisionDateTime = DateTime.Now;
                aspr.DecisionAdminUser = this.userTasks.GetAdminUser(User.Identity.Name);
                aspr.Person = this.personTasks.GetPerson(vm.PersonId.Value);
                aspr.Event = this.eventTasks.GetEvent(vm.EventId.Value);
                XmlDocument suggestionFeatures = new XmlDocument();
                suggestionFeatures.LoadXml(vm.SuggestionFeatures);
                aspr.SuggestionFeatures = suggestionFeatures;
                aspr.Notes = vm.Notes;
                aspr.IsAccepted = vm.IsAccepted;
                this.suggestionTasks.SaveSuggestionPersonResponsibility(aspr);
                return JsonNet(string.Empty);
            }
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return JsonNet("Invalid input.");
        }

        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangeEvents)]
        public JsonNetResult Delete(int id)
        {
            Event e = this.eventTasks.GetEvent(id);
            if (e != null)
            {
                if (this.eventTasks.DeleteEvent(e))
                    return JsonNet("Event successfully deleted.");
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    return JsonNet("Could not delete event: please ensure all attached responsibilities, sources etc. are deleted.");
                }
            }
            Response.StatusCode = (int)HttpStatusCode.NotFound;
            return JsonNet("Event doesn't exist");
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchEvents)]
        public ActionResult FeatureProbabilityCalcs()
        {
            return View(this.suggestionTasks.CountSuggestedFeatures());
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchEvents)]
        public ActionResult NewSuggest(int id)
        {
            Person p = this.personTasks.GetPerson(id);
            if (p != null)
                return View(this.suggestionTasks.GetSuggestionsRefactored(p, this.suggestionTasks
                    .GetAdminSuggestionFeaturePersonResponsibilities().Values
                    .Where(x => new string[]
                        {
                            AdminSuggestionFeaturePersonResponsibility.RESPONSIBLE_FOR_RELATED_EVENT,
                            AdminSuggestionFeaturePersonResponsibility.LAST_NAME_APPEARS,
                            AdminSuggestionFeaturePersonResponsibility.FIRST_NAME_APPEARS,
                            AdminSuggestionFeaturePersonResponsibility.ALIAS_APPEARS,
                            AdminSuggestionFeaturePersonResponsibility.COMMON_SOURCE,
                            AdminSuggestionFeaturePersonResponsibility.CAREER_IN_LOCATION,
                            AdminSuggestionFeaturePersonResponsibility.CAREER_IN_ORG_RESPONSIBLE,
                            AdminSuggestionFeaturePersonResponsibility.CAREER_IN_UNIT_RESPONSIBLE,
                            AdminSuggestionFeaturePersonResponsibility.RESPONSIBILITY_IN_LOCATION
                        }.Contains(x.Code))
                    .Select(x => x.Id)
                    .ToList()));
            return new HttpNotFoundResult();
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchEvents)]
        public ActionResult Audit(int id)
        {
            Event e = this.eventTasks.GetEvent(id);
            if (e != null)
            {
                ViewBag.Event = e;
                ViewBag.OldAuditTrail = this.auditTasks.GetEventOldAuditTrail(id);
                ViewBag.AuditTrail = this.auditTasks.GetEventAuditTrail(id);
                ViewBag.Users = this.userTasks.GetAllAdminUsers();
                return View();
            }
            return new HttpNotFoundResult();
        }

        [PermissionAuthorize(AdminPermission.CanChangeEvents)]
        public ActionResult Merge()
        {
            return View();
        }

        [HttpPost]
        [PermissionAuthorize(AdminPermission.CanChangeEvents)]
        public JsonNetResult Merge(MergeViewModel vm)
        {
            if (ModelState.IsValid)
            {
                AdminUser user = this.userTasks.GetAdminUser(User.Identity.Name);

                if (user != null)
                {
                    if (this.eventTasks.MergeEvents(vm.ToKeepId, vm.ToDeleteId, user.UserID, ((PrfPrincipal)User).HasPermission(AdminPermission.CanViewPersonResponsibilities)) == 1)
                    {
                        return JsonNet(null);
                    }
                    else
                    {
                        Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        return JsonNet(new { merge = "There was a database error merging the events." });
                    }
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return JsonNet(new { user = "There was a problem identifying the logged-in user." });
                }
            }
            Response.StatusCode = (int)HttpStatusCode.NotFound;
            return JsonNet(GetErrorsForJson());
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchEvents)]
        public void Export(int id)
        {
            Event e = this.eventTasks.GetEvent(id);
            if (e != null)
            {
                AdminUser user = this.userTasks.GetAdminUser(User.Identity.Name);
                if (user != null)
                {
                    byte[] doc = this.eventTasks.ExportDocument(e, this.auditTasks.GetEventLastModified(e.Id),
                        user, Request.UserHostName, Request.UserHostAddress, Request.UserAgent);
                    if (doc != null)
                    {
                        string fileName = !string.IsNullOrEmpty(e.Headline)
                            ? "Event " + e.Headline.Replace('/', '.') + ".doc"
                            : "Event ID " + e.Id + ".doc";
                        Response.ContentType = "application/msword";
                        Response.AddHeader("Content-Disposition", "attachment; filename=\"" + fileName + "\"");
                        Response.OutputStream.Write(doc, 0, doc.Length);
                    }
                    else
                    {
                        Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        Response.StatusDescription = "Problem creating document.";
                    }
                }
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                Response.StatusDescription = "That event doesn't exist.";
            }
        }

        [Transaction]
        [PermissionAuthorize(AdminPermission.CanApproveEvents)]
        public JsonNetResult Approve(int id)
        {
            Event e = this.eventTasks.GetEvent(id);
            if (e != null)
            {
                AdminUser user = this.userTasks.GetAdminUser(User.Identity.Name);
                this.eventTasks.ApproveEvent(e, user);
                return JsonNet("Event approved.");
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return JsonNet("Event doesn't exist");
            }
        }

        [PermissionAuthorize(AdminPermission.CanApproveEvents)]
        public ActionResult Approvals()
        {
            return View(this.eventTasks.GetUnapprovedEvents());
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchEvents)]
        public ActionResult WithCases()
        {
            return View(this.eventTasks.GetAllEvents()
                .Where(x => x.EventSources.Where(y => y.HasCaseCode()).Any())
                .ToList());
        }

        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangeEvents)]
        public ActionResult LinkCases()
        {
            foreach (Event e in this.eventTasks.GetAllEvents().Where(x => x.EventSources.Where(y => y.HasCaseCode()).Any()))
            {
                foreach (string code in e.GetCaseCodesInEventSources())
                {
                    JhroCase jc = this.sourceTasks.GetJhroCase(code);
                    if (jc != null)
                    {
                        e.AddJhroCase(jc);
                        this.eventTasks.SaveEvent(e);
                        log.Info("Linking EventID=" + e.Id.ToString() + " with JhroCase.CaseNumber=" + jc.CaseNumber);
                    }
                }
            }

            return RedirectToAction("WithCases");
        }

        public JsonNetResult FindSimilarEvents(int id)
        {
            Event e = this.eventTasks.GetEvent(id);
            if (e != null)
            {
                    // ...find potential matching Events to link to this Event
                return JsonNet(new
                {
                    Events = this.eventMatchingTasks.FindSimilarEvents(e)
                });
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                Response.StatusDescription = "That event doesn't exist.";
                return JsonNet(Response.StatusDescription);
            }
        }
    }
}