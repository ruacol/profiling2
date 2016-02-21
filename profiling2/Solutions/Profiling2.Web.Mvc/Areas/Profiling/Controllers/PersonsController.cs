using AutoMapper;
using CsvHelper;
using log4net;
using Microsoft.Practices.ServiceLocation;
using Mvc.JQuery.Datatables;
using Profiling2.Domain;
using Profiling2.Domain.Contracts.Queries;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Contracts.Tasks.Sources;
using Profiling2.Domain.Extensions;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Domain.Scr;
using Profiling2.Domain.Scr.Person;
using Profiling2.Infrastructure.Security;
using Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels;
using Profiling2.Web.Mvc.Areas.System.Controllers.ViewModels;
using Profiling2.Web.Mvc.Controllers;
using SharpArch.NHibernate.Web.Mvc;
using SharpArch.Web.Mvc.JsonNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Profiling2.Domain.Prf.Actions;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers
{
    public class PersonsController : BaseController
    {
        protected readonly ILog log = LogManager.GetLogger(typeof(PersonsController));
        protected readonly IPersonTasks personTasks;
        protected readonly ILocationTasks locationTasks;
        protected readonly IWantedTasks wantedTasks;
        protected readonly IAuditTasks auditTasks;
        protected readonly IUserTasks userTasks;
        protected readonly ILuceneTasks luceneTasks;
        protected readonly ISourceTasks sourceTasks;

        public PersonsController(IPersonTasks personTasks, 
            ILocationTasks locationTasks, 
            IWantedTasks wantedTasks, 
            IAuditTasks auditTasks, 
            IUserTasks userTasks, 
            ILuceneTasks luceneTasks, 
            ISourceTasks sourceTasks)
        {
            this.personTasks = personTasks;
            this.locationTasks = locationTasks;
            this.wantedTasks = wantedTasks;
            this.auditTasks = auditTasks;
            this.userTasks = userTasks;
            this.luceneTasks = luceneTasks;
            this.sourceTasks = sourceTasks;
        }

        [PermissionAuthorize(AdminPermission.CanViewPersonReports)]
        public ActionResult Audit(int id)
        {
            Person person = this.personTasks.GetPerson(id);
            if (person != null)
            {
                ViewBag.Person = person;
                ViewBag.OldAuditTrail = this.auditTasks.GetPersonOldAuditTrail(id);
                ViewBag.AuditTrail = this.auditTasks.GetPersonAuditTrail(id);
                ViewBag.Users = this.userTasks.GetAllAdminUsers();
                return View();
            }
            return new HttpNotFoundResult();
        }


        [PermissionAuthorize(AdminPermission.CanViewPersonReports)]
        public ActionResult Deleted()
        {
            ViewBag.OldAuditTrail = this.auditTasks.GetOldDeletedProfiles();
            ViewBag.AuditTrail = this.auditTasks.GetDeletedProfiles();
            ViewBag.Users = this.userTasks.GetAllAdminUsers();
            return View();
        }

        [PermissionAuthorize(AdminPermission.CanViewPersonReports)]
        public ActionResult Changed()
        {
            DateViewModel vm = new DateViewModel(DateTime.Now.Subtract(TimeSpan.FromDays(30)), DateTime.Now);
            return View(vm);
        }

        [HttpPost]
        [PermissionAuthorize(AdminPermission.CanViewPersonReports)]
        public ActionResult Changed(DateViewModel vm)
        {
            ViewBag.CreatedProfiles = this.auditTasks.GetCreatedProfiles(vm.StartDateAsDate, vm.EndDateAsDate);
            ViewBag.ModifiedProfiles = this.auditTasks.GetModifiedProfiles(vm.StartDateAsDate, vm.EndDateAsDate);
            ViewBag.Users = this.userTasks.GetAllAdminUsers();
            return View(vm);
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchPersons)]
        public ActionResult Index()
        {
            return View();
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchPersons)]
        public JsonNetResult Get()
        {
            string term = Request.QueryString["term"];
            if (!string.IsNullOrEmpty(term))
            {
                term = term.Trim();
                Regex regex = new Regex("^[0-9]+$");
                if (regex.IsMatch(term))
                {
                    int personId;
                    if (int.TryParse(term, out personId))
                    {
                        Person p = this.personTasks.GetPerson(personId);
                        if (p != null)
                        {
                            if (p.IsRestrictedProfile && !((PrfPrincipal)User).HasPermission(AdminPermission.CanViewAndSearchRestrictedPersons))
                            {
                                return JsonNet(string.Empty);
                            }

                            return JsonNet(new object[] 
                            {
                                new
                                {
                                    id = p.Id,
                                    text = p.Name
                                }
                            });
                        }
                    }
                }
                else
                {
                    // conduct search - don't show restricted profiles outside of profiling team
                    IList<LuceneSearchResult> results = this.luceneTasks.PersonSearch(term, 50,
                        ((PrfPrincipal)User).HasPermission(AdminPermission.CanViewAndSearchRestrictedPersons));
                    object[] objects = results.Select(x => x.GetPersonIdAndNameForSelect2()).ToArray();
                    return JsonNet(objects);
                }
            }
            return JsonNet(string.Empty);
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchPersons)]
        public JsonNetResult Name(int id)
        {
            Person p = this.personTasks.GetPerson(id);
            if (p != null)
            {
                return JsonNet(new
                {
                    Id = id,
                    Name = p.Name
                });
            }
            else
                return JsonNet(string.Empty);
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchPersons)]
        public JsonNetResult DataTables(DataTablesParam p)
        {
            if (p.sSearch != null && p.sSearch.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length > 4)
            {
                Response.StatusCode = (int)HttpStatusCode.NotImplemented;
                return JsonNet("Search must not contain more than 4 terms.");
            }
            else
            {
                IList<SearchForPersonDTO> persons = this.personTasks.GetPersonDataTablesPaginated(p.iDisplayStart, p.iDisplayLength, p.sSearch,
                    p.iSortingCols, p.iSortCol, p.sSortDir, User.Identity.Name, ((PrfPrincipal)User).HasPermission(AdminPermission.CanViewAndSearchRestrictedPersons));
                int iTotalRecords = 0;
                if (persons != null && persons.Count > 0)
                    iTotalRecords = persons.First().TotalRecords;
                object[] aaData = (from person in persons select new PersonDataTableView(person)).ToArray<PersonDataTableView>();

                return JsonNet(new DataTablesData
                {
                    iTotalRecords = iTotalRecords,
                    iTotalDisplayRecords = iTotalRecords,
                    sEcho = p.sEcho,
                    aaData = aaData.ToArray()
                });
            }
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchPersons)]
        public JsonNetResult DataTablesLucene(DataTablesParam p)
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

        [PermissionAuthorize(AdminPermission.CanViewAndSearchPersons)]
        public ActionResult Details(int id)
        {
            Person person = this.personTasks.GetPerson(id);
            if (person != null)
            {
                if (person.IsRestrictedProfile && !((PrfPrincipal)User).HasPermission(AdminPermission.CanViewAndSearchRestrictedPersons))
                    return new HttpUnauthorizedResult();

                return View(person);
            }
            else
                return new HttpNotFoundResult();
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchPersons)]
        public JsonNetResult Json(int id)
        {
            Person person = this.personTasks.GetPerson(id);
            if (person != null)
            {
                if (person.IsRestrictedProfile && !((PrfPrincipal)User).HasPermission(AdminPermission.CanViewAndSearchRestrictedPersons))
                {
                    Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    return JsonNet(string.Empty);
                }

                IDictionary<string, object> jsonObj = new Dictionary<string, object>();

                PersonViewModel vm = new PersonViewModel(person);
                if (!((PrfPrincipal)User).HasPermission(AdminPermission.CanViewBackgroundInformation))
                {
                    vm.BackgroundInformation = string.Empty;
                }
                jsonObj.Add("Person", vm);

                jsonObj.Add("Aliases", person.PersonAliases.Where(x => !x.Archive).Select(x => new PersonAliasViewModel(x)));

                jsonObj.Add("Photos", person.PersonPhotos.Where(x => !x.Archive).Select(x => new PersonPhotoViewModel(x)));

                if (((PrfPrincipal)User).HasPermission(AdminPermission.CanViewAndSearchSources))
                {
                    jsonObj.Add("PersonSources", this.personTasks.GetPersonSourceDTOs(id).Select(x => x.SetPersonName(person.Name)));

                    jsonObj.Add("ActiveScreenings", person.ActiveScreenings.Select(x => new ActiveScreeningViewModel(x)));
                }

                return JsonNet(jsonObj);
            }

            Response.StatusCode = (int)HttpStatusCode.NotFound;
            return JsonNet(string.Empty);
        }

        [PermissionAuthorize(AdminPermission.CanViewPersonReports)]
        public JsonNetResult RedFlags(int id)
        {
            Person person = this.personTasks.GetPerson(id);
            if (person != null)
                return JsonNet(person.GetToBeConfirmedDTOs());

            Response.StatusCode = (int)HttpStatusCode.NotFound;
            return JsonNet(string.Empty);
        }

        [PermissionAuthorize(AdminPermission.CanChangePersons)]
        public ActionResult Create()
        {
            PersonViewModel vm = new PersonViewModel();
            vm.PopulateDropDowns(this.locationTasks.GetAllRegions(), this.personTasks.GetEthnicities(), this.personTasks.GetAllProfileStatuses());
            return View(vm);
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangePersons)]
        public JsonNetResult Create(PersonViewModel vm)
        {
            if (ModelState.IsValid)
            {
                Person person = new Person();
                Mapper.Map<PersonViewModel, Person>(vm, person);
                if (vm.BirthRegionId.HasValue)
                    person.BirthRegion = this.locationTasks.GetRegion(vm.BirthRegionId.Value);
                if (vm.EthnicityId.HasValue)
                    person.Ethnicity = this.personTasks.GetEthnicity(vm.EthnicityId.Value);
                person.ProfileStatus = this.personTasks.GetProfileStatus(vm.ProfileStatusId);
                person.ProfileLastModified = DateTime.Now;
                person = this.personTasks.SavePerson(person);
                return JsonNet(new
                {
                    Id = person.Id,
                    Name = person.Name,
                    WasSuccessful = true
                });
            }
            else
            {
                return JsonNet(this.GetErrorsForJson());
            }
        }

        [PermissionAuthorize(AdminPermission.CanChangePersons)]
        public ActionResult QuickCreate()
        {
            PersonViewModel vm = new PersonViewModel();
            return View(vm);
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangePersons)]
        public JsonNetResult QuickCreate(PersonViewModel vm)
        {
            if (ModelState.IsValid)
            {
                Person person = new Person();
                Mapper.Map<PersonViewModel, Person>(vm, person);
                person.ProfileStatus = this.personTasks.GetProfileStatus(ProfileStatus.ROUGH_OUTLINE);
                person.ProfileLastModified = DateTime.Now;
                person = this.personTasks.SavePerson(person);
                return JsonNet(new
                {
                    Id = person.Id,
                    Name = person.Name,
                    WasSuccessful = true
                });
            }
            else
            {
                return JsonNet(this.GetErrorsForJson());
            }
        }

        [PermissionAuthorize(AdminPermission.CanChangePersons)]
        public ActionResult Edit(int id)
        {
            Person person = this.personTasks.GetPerson(id);
            if (person != null)
            {
                PersonViewModel vm = new PersonViewModel(person);
                vm.PopulateDropDowns(this.locationTasks.GetAllRegions(), this.personTasks.GetEthnicities(), this.personTasks.GetAllProfileStatuses());
                return View(vm);
            }
            else
                return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangePersons)]
        public JsonNetResult Edit(PersonViewModel vm)
        {
            if (ModelState.IsValid)
            {
                Person person = this.personTasks.GetPerson(vm.Id);
                if (person != null)
                {
                    Mapper.Map<PersonViewModel, Person>(vm, person);
                    if (vm.BirthRegionId.HasValue)
                        person.BirthRegion = this.locationTasks.GetRegion(vm.BirthRegionId.Value);
                    if (vm.EthnicityId.HasValue)
                        person.Ethnicity = this.personTasks.GetEthnicity(vm.EthnicityId.Value);
                    else
                        person.Ethnicity = null;
                    person.ProfileStatus = this.personTasks.GetProfileStatus(vm.ProfileStatusId);
                    person.ProfileLastModified = DateTime.Now;
                    person = this.personTasks.SavePerson(person);
                    return JsonNet(string.Empty);
                }
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return JsonNet("Person not found.");
            }
            else
            {
                return JsonNet(this.GetErrorsForJson());
            }
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchPersons)]
        public JsonNetResult Relationships(int id)
        {
            Person person = this.personTasks.GetPerson(id);
            if (person != null)
            {
                return JsonNet(new
                {
                    PersonRelationshipsAsSubject = person.PersonRelationshipAsSubject
                        .Where(x => !x.Archive)
                        .Select(x => new PersonRelationshipViewModel(x)),
                    PersonRelationshipsAsObject = person.PersonRelationshipAsObject
                        .Where(x => !x.Archive)
                        .Select(x => new PersonRelationshipViewModel(x)),
                    PersonsWithSameEthnicity = this.personTasks.GetPersonsWithSameEthnicity(person, ((PrfPrincipal)User).HasPermission(AdminPermission.CanViewAndSearchRestrictedPersons))
                        .Select(x => new
                        {
                            Id = x.Id,
                            MilitaryIDNumber = x.MilitaryIDNumber,
                            EthnicityName = x.Ethnicity.EthnicityName,
                            Name = x.Name,
                            FunctionUnitSummary = x.CurrentCareer == null ? string.Empty : x.CurrentCareer.FunctionUnitSummary,
                            UnitId = new CareerViewModel(x.CurrentCareer).UnitId,
                            UnitName = new CareerViewModel(x.CurrentCareer).UnitName,
                            Rank = x.CurrentRank
                        })
                });
            }

            Response.StatusCode = (int)HttpStatusCode.NotFound;
            return JsonNet(string.Empty);
        }

        [PermissionAuthorize(AdminPermission.CanViewPersonResponsibilities)]
        public JsonNetResult Responsibilities(int id)
        {
            Person person = this.personTasks.GetPerson(id);
            if (person != null)
            {
                return JsonNet(new
                {
                    Responsibilities = person.PersonResponsibilities.Where(x => !x.Archive).Select(x => x.ToJSON()),
                    Events = person.PersonResponsibilities.Where(x => !x.Archive).Select(x => x.Event.ToJSON(
                        x.Event.EventSources.Select(y => this.sourceTasks.GetSourceDTO(y.Source.Id)).ToList(),
                        person.Id
                    )),
                    Person = new
                        {
                            Id = person.Id,
                            Name = person.Name
                        },
                    PersonResponsibilityTypes = ServiceLocator.Current.GetInstance<IResponsibilityTasks>().GetPersonResponsibilityTypes().Select(x => x.ToJSON()),
                    VerifiedStatuses = ServiceLocator.Current.GetInstance<IEventTasks>().GetAllEventVerifiedStatuses().Select(x => x.ToJSON())
                });
            }

            Response.StatusCode = (int)HttpStatusCode.NotFound;
            return JsonNet(string.Empty);
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchPersons)]
        public JsonNetResult Careers(int id)
        {
            Person person = this.personTasks.GetPerson(id);
            if (person != null)
            {
                return JsonNet(new
                {
                    Careers = person.Careers.Where(x => !x.Archive).Select(x => new CareerViewModel(x))
                });
            }

            Response.StatusCode = (int)HttpStatusCode.NotFound;
            return JsonNet(string.Empty);
        }

        [PermissionAuthorize(AdminPermission.CanViewPersonReports)]
        public ActionResult Incomplete()
        {
            IList<Person> persons = this.personTasks.GetPersonsIncomplete();
            return View(persons);
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchPersons)]
        public JsonNetResult Screening(int id)
        {
            Person person = this.personTasks.GetPerson(id);
            if (person != null)
            {
                IList<RequestPersonViewModel> vms = new List<RequestPersonViewModel>();
                foreach (RequestPerson rp in person.RequestPersons
                    .Where(x => !x.Archive)
                    .Where(x => !new string[] { RequestStatus.NAME_REJECTED, RequestStatus.NAME_DELETED }.Contains(x.Request.CurrentStatus.RequestStatusName)))
                    vms.Add(new RequestPersonViewModel(rp));
                return JsonNet(new
                {
                    ScreeningInformation = vms
                });
            }

            Response.StatusCode = (int)HttpStatusCode.NotFound;
            return JsonNet(string.Empty);
        }

        [PermissionAuthorize(AdminPermission.CanViewPersonReports)]
        public ActionResult Wanted()
        {
            return View(this.personTasks.GetPersonsWanted());
        }

        [PermissionAuthorize(AdminPermission.CanViewPersonReports)]
        public ActionResult WantedCommanders()
        {
            return View(this.wantedTasks.GetWantedCommanders());
        }

        [Transaction]
        [PermissionAuthorize(AdminPermission.CanDeletePersons)]
        public JsonNetResult Delete(int id)
        {
            Person person = this.personTasks.GetPerson(id);
            if (person != null)
            {
                if (this.personTasks.DeletePerson(person))
                    return JsonNet("Profile successfully deleted.");
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    return JsonNet("Person could not be deleted.");
                }
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return JsonNet("Person does not exist.");
            }
        }

        // Helper page that displays ID numbers that don't pass validation
        [PermissionAuthorize(AdminPermission.CanViewAndSearchPersons)]
        public ActionResult UnrecognisedIds()
        {
            return View(this.personTasks.GetPersonsWithUnmatchedMilitaryID());
        }

        [PermissionAuthorize(AdminPermission.CanDeletePersons)]
        public ActionResult Merge()
        {
            return View();
        }

        /// <summary>
        /// Action to be called via ajax, front-end for stored procedure which does the actual heavy lifting.
        /// 
        /// No need for Transaction attribute since stored proc creates its own transaction.
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        [HttpPost]
        [PermissionAuthorize(AdminPermission.CanDeletePersons)]
        public JsonNetResult Merge(MergeViewModel vm)
        {
            if (ModelState.IsValid)
            {
                AdminUser user = this.userTasks.GetAdminUser(User.Identity.Name);

                if (user != null)
                {
                    if (this.personTasks.MergePersons(vm.ToKeepId, vm.ToDeleteId, user.UserID, ((PrfPrincipal)User).HasPermission(AdminPermission.CanViewPersonResponsibilities)) == 1)
                    {
                        return JsonNet(null);
                    }
                    else
                    {
                        Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        return JsonNet(new { merge = "There was a database error merging the persons." });
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

        [PermissionAuthorize(AdminPermission.CanChangePersonPublicSummaries)]
        public ActionResult EditPublicSummaryModal(int id)
        {
            Person person = this.personTasks.GetPerson(id);
            if (person != null)
            {
                PersonViewModel vm = new PersonViewModel(person);
                vm.Name = person.Name;
                return View(vm);
            }
            else
                return new HttpUnauthorizedResult();
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangePersonPublicSummaries)]
        public JsonNetResult EditPublicSummaryModal(PersonViewModel vm)
        {
            // don't check model state as we're only interested in the PublicSummary field
            Person person = this.personTasks.GetPerson(vm.Id);
            if (person != null)
            {
                person.PublicSummary = vm.PublicSummary;
                person.PublicSummaryDate = DateTime.Now;
                this.personTasks.SavePerson(person);

                return JsonNet(new
                {
                    Id = person.Id,
                    Name = person.Name,
                    WasSuccessful = true
                });
            }
            else
            {
                ModelState.AddModelError("Id", "Person doesn't exist.");
                return JsonNet(this.GetErrorsForJson());
            }
        }

        [PermissionAuthorize(AdminPermission.CanChangePersonNotes)]
        public ActionResult EditNotesModal(int id)
        {
            Person person = this.personTasks.GetPerson(id);
            if (person != null)
            {
                PersonViewModel vm = new PersonViewModel(person);
                vm.Name = person.Name;
                return View(vm);
            }
            else
                return new HttpUnauthorizedResult();
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangePersonNotes)]
        public JsonNetResult EditNotesModal(PersonViewModel vm)
        {
            // don't check model state as we're only interested in the Notes field
            Person person = this.personTasks.GetPerson(vm.Id);
            if (person != null)
            {
                person.Notes = vm.Notes;
                this.personTasks.SavePerson(person);

                return JsonNet(new
                {
                    Id = person.Id,
                    Name = person.Name,
                    WasSuccessful = true
                });
            }
            else
            {
                ModelState.AddModelError("Id", "Person doesn't exist.");
                return JsonNet(this.GetErrorsForJson());
            }
        }

        [PermissionAuthorize(AdminPermission.CanChangePersonBackground)]
        public ActionResult EditBackgroundInformationModal(int id)
        {
            Person person = this.personTasks.GetPerson(id);
            if (person != null)
            {
                PersonViewModel vm = new PersonViewModel(person);
                vm.Name = person.Name;
                return View(vm);
            }
            else
                return new HttpUnauthorizedResult();
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangePersonBackground)]
        public JsonNetResult EditBackgroundInformationModal(PersonViewModel vm)
        {
            // don't check model state as we're only interested in the BackgroundInformation field
            Person person = this.personTasks.GetPerson(vm.Id);
            if (person != null)
            {
                person.BackgroundInformation = vm.BackgroundInformation;
                this.personTasks.SavePerson(person);

                return JsonNet(new
                {
                    Id = person.Id,
                    Name = person.Name,
                    WasSuccessful = true
                });
            }
            else
            {
                ModelState.AddModelError("Id", "Person doesn't exist.");
                return JsonNet(this.GetErrorsForJson());
            }
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchPersons)]
        public ActionResult MatchPersons()
        {
            return View(new SearchViewModel());
        }

        [HttpPost]
        [PermissionAuthorize(AdminPermission.CanViewAndSearchPersons)]
        public ActionResult MatchPersons(HttpPostedFileBase file)
        {
            if (file != null && file.InputStream != null)
            {
                IList<SearchForPersonDTO> dtos = new List<SearchForPersonDTO>();

                try
                {
                    var reader = new CsvReader(new StreamReader(file.InputStream));

                    while (reader.Read())
                    {
                        SearchForPersonDTO dto = new SearchForPersonDTO();
                        dto.MilitaryIDNumber = reader.GetField<string>(0);
                        dto.FirstName = reader.GetField<string>(1);
                        dto.LastName = reader.GetField<string>(2);
                        dtos.Add(dto);
                    }
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("file", e.Message);
                }

                ViewData["results"] = dtos;
                return View();
            }

            return View();
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchPersons)]
        public JsonNetResult MatchPerson(SearchForPersonDTO vm)
        {
            if (vm != null)
            {
                return JsonNet(this.personTasks.MatchPerson(vm).Select(x => new PersonViewModel(x)));
            }
            return null;
        }

        [PermissionAuthorize(AdminPermission.CanExportPersons)]
        public void Export(int id)
        {
            Person person = this.personTasks.GetPerson(id);
            string includeBackground = Request.QueryString.Get("includeBackground");
            if (person != null)
            {
                AdminUser user = this.userTasks.GetAdminUser(User.Identity.Name);
                if (user != null)
                {
                    byte[] doc = this.personTasks.ExportDocument(person, Convert.ToBoolean(includeBackground), user, 
                        Request.UserHostName, Request.UserHostAddress, Request.UserAgent);
                    if (doc != null)
                    {
                        string fileName = !string.IsNullOrEmpty(person.Name)
                            ? "Profile " + person.Name + ".doc"
                            : "Profile Person ID " + person.Id + ".doc";
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
                Response.StatusDescription = "That person doesn't exist.";
            }
        }

        /// <summary>
        /// TODO permissions check if we include actions taken/events/HRVs
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [PermissionAuthorize(AdminPermission.CanViewAndSearchPersons)]
        public ActionResult Timeline(int id)
        {
            Person person = this.personTasks.GetPerson(id);
            if (person != null)
                return View(person);
            else
                return new HttpNotFoundResult();
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchPersons)]
        public JsonNetResult TimelineJson(int id)
        {
            Person person = this.personTasks.GetPerson(id);
            if (person != null)
            {
                return JsonNet(new
                {
                    events = person.GetCareerTimelineSlideObjects(false)
                        .Concat(person.PersonResponsibilities.Where(x => !x.Archive && (x.Event.HasStartDate() || x.Event.HasEndDate())).Select(x => x.GetTimelineSlideObject()))
                        .Concat(person.PersonResponsibilities.Where(x => !x.Archive).Select(x => x.Event.ActionTakens).Aggregate(new List<ActionTaken>(), (x, y) => x.Concat(y).ToList()).Where(x => x.HasStartDate() || x.HasEndDate()).Select(x => x.GetTimelineSlideObject()))
                        .Concat(person.GetPersonRelationships().Where(x => !x.Archive && (x.HasStartDate() || x.HasEndDate())).Select(x => x.GetTimelineSlideObject(person)))
                });
            }
            else
            {
                ModelState.AddModelError("Id", "Person doesn't exist.");
                return JsonNet(this.GetErrorsForJson());
            }
        }
    }
}