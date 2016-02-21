using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Profiling2.Domain;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Contracts.Tasks.Screenings;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Scr;
using Profiling2.Domain.Scr.Person;
using Profiling2.Domain.Scr.PersonEntity;
using Profiling2.Domain.Scr.PersonFinalDecision;
using Profiling2.Domain.Scr.PersonRecommendation;
using Profiling2.Domain.Scr.Proposed;
using Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels;
using Profiling2.Web.Mvc.Areas.Screening.Controllers.ViewModels;
using Profiling2.Web.Mvc.Areas.Screening.Export;
using Profiling2.Web.Mvc.Controllers;
using SharpArch.NHibernate.Web.Mvc;
using SharpArch.Web.Mvc.JsonNet;

namespace Profiling2.Web.Mvc.Areas.Screening.Controllers
{
    public class RequestPersonController : ScreeningBaseController
    {
        protected readonly IRequestPersonTasks requestPersonTasks;
        protected readonly IScreeningTasks screeningTasks;
        protected readonly IUserTasks userTasks;

        public RequestPersonController(IRequestPersonTasks requestPersonTasks, IScreeningTasks screeningTasks, IUserTasks userTasks)
        {
            this.requestPersonTasks = requestPersonTasks;
            this.screeningTasks = screeningTasks;
            this.userTasks = userTasks;
        }

        public ActionResult RequestPersonModal(int id)
        {
            RequestPerson rp = this.requestPersonTasks.GetRequestPerson(id);
            if (rp != null)
            {
                RequestPersonViewModel vm = new RequestPersonViewModel(rp);
                return View(vm);
            }
            return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        public JsonNetResult RequestPersonModal(RequestPersonViewModel vm)
        {
            if (ModelState.IsValid)
            {
                RequestPerson rp = this.requestPersonTasks.GetRequestPerson(vm.Id);
                if (rp != null)
                {
                    rp.Notes = vm.Notes;
                    rp = this.requestPersonTasks.SaveRequestPerson(rp);
                    return JsonNet(new
                    {
                        WasSuccessful = true
                    });
                }
                else
                    ModelState.AddModelError("Id", "Request person does not exist.");
            }
            return JsonNet(this.GetErrorsForJson());
        }

        public ActionResult ProposedPersonModal(int id)
        {
            RequestProposedPerson rpp = this.requestPersonTasks.GetRequestProposedPerson(id);
            if (rpp != null)
            {
                ProposedPersonViewModel vm = new ProposedPersonViewModel(rpp);
                return View(vm);
            }
            return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        public JsonNetResult ProposedPersonModal(ProposedPersonViewModel vm)
        {
            if (ModelState.IsValid)
            {
                RequestProposedPerson rpp = this.requestPersonTasks.GetRequestProposedPerson(vm.Id);
                if (rpp != null)
                {
                    rpp.PersonName = vm.Name;
                    rpp.MilitaryIDNumber = vm.MilitaryIDNumber;
                    rpp.Notes = vm.Notes;
                    rpp = this.requestPersonTasks.SaveRequestProposedPerson(rpp);
                    return JsonNet(new
                    {
                        WasSuccessful = true
                    });
                }
                else
                    ModelState.AddModelError("Id", "Proposed person does not exist.");
            }
            return JsonNet(this.GetErrorsForJson());
        }

        public void ExportNominated()
        {
            IList<RequestPerson> rps = this.requestPersonTasks.GetNominatedRequestPersons();
            AdminUser user = this.userTasks.GetAdminUser(User.Identity.Name);
            if (rps != null && user != null)
            {
                ExcelCasesForDiscussion excel = new ExcelCasesForDiscussion(rps, this.screeningTasks.GetScreeningEntities().ToList(), user.GetScreeningEntity().ScreeningEntityName);
                if (excel != null)
                {
                    string fileName = DateTime.Now.ToString("yyyyMMdd", CultureInfo.InvariantCulture) + "_" + "CondMeetingExport.xls";
                    Response.ContentType = "application/vnd.ms-excel";
                    Response.AddHeader("Content-Disposition", "attachment; filename=\"" + fileName + "\"");
                    Response.OutputStream.Write(excel.PdfBytes, 0, excel.PdfBytes.Length);
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    Response.StatusDescription = "Problem creating spreadsheet.";
                }
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                Response.StatusDescription = "There are no nominated persons.";
            }
        }

        [MultiRoleAuthorize(
            AdminRole.ScreeningRequestConditionalityParticipant,
            AdminRole.ScreeningRequestConsolidator,
            AdminRole.ScreeningRequestFinalDecider
        )]
        public JsonNetResult Reasons()
        {
            string term = Request.QueryString["term"];
            if (!string.IsNullOrEmpty(term))
            {
                term = term.Trim();
                Regex regex = new Regex("^[0-9]+$");
                if (regex.IsMatch(term))
                {
                    int requestPersonId;
                    if (int.TryParse(term, out requestPersonId))
                    {
                        RequestPerson rp = this.requestPersonTasks.GetRequestPerson(requestPersonId);
                        if (rp != null && rp.Request != null)
                        {
                            AdminUser user = this.userTasks.GetAdminUser(User.Identity.Name);

                            // 2014-09-01 we used to check request.UserHasPermission(user) at this point, but actually there are no restrictions
                            // on access for any of the roles that have access to this action anyway, so - no check.
                            
                            IList<object> reasons = new List<object>();
                                
                            // conditionality participant inputs
                            foreach (ScreeningEntity se in this.screeningTasks.GetScreeningEntities())
                            {
                                ScreeningRequestPersonEntity srpe = rp.GetScreeningRequestPersonEntity(se.ScreeningEntityName);
                                if (srpe != null)
                                {
                                    reasons.Add(new
                                    {
                                        ScreeningEntity = srpe.ScreeningEntity.ToString(),
                                        Result = srpe.ScreeningResult.ToString(),
                                        Reason = srpe.Reason,
                                        Commentary = srpe.Commentary
                                    });
                                }
                            }

                            // consolidation phase commentary
                            ScreeningRequestPersonRecommendation srpr = rp.GetScreeningRequestPersonRecommendation();
                            if (srpr != null)
                            {
                                reasons.Add(new
                                {
                                    ScreeningEntity = "PWG",
                                    Result = srpr.ScreeningResult.ToString(),
                                    Commentary = srpr.Commentary
                                });
                            }

                            // final decision commentary
                            ScreeningRequestPersonFinalDecision srpfd = rp.GetScreeningRequestPersonFinalDecision();
                            if (srpfd != null)
                            {
                                reasons.Add(new
                                {
                                    ScreeningEntity = "SMG",
                                    Result = srpfd.ScreeningResult.ToString(),
                                    SupportStatus = srpfd.ScreeningSupportStatus.ToString(),
                                    Commentary = srpfd.Commentary
                                });
                            }

                            return JsonNet(reasons);
                        }
                    }
                }
            }
            return JsonNet(string.Empty);
        }
    }
}
