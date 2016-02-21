using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using Profiling2.Domain;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Contracts.Tasks.Screenings;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Scr;
using Profiling2.Domain.Scr.Person;
using Profiling2.Domain.Scr.PersonRecommendation;
using Profiling2.Web.Mvc.Areas.Screening.Controllers.ViewModels;
using Profiling2.Web.Mvc.Controllers;
using SharpArch.NHibernate.Web.Mvc;
using SharpArch.Web.Mvc.JsonNet;

namespace Profiling2.Web.Mvc.Areas.Screening.Controllers
{
    [MultiRoleAuthorize(
        AdminRole.ScreeningRequestConsolidator
    )]
    public class ConsolidateController : ScreeningBaseController
    {
        private readonly IRequestTasks requestTasks;
        private readonly IRequestPersonTasks requestPersonTasks;
        private readonly IScreeningTasks screeningTasks;
        private readonly IEmailTasks emailTasks;
        private readonly IUserTasks userTasks;

        public ConsolidateController(IRequestTasks requestTasks, IRequestPersonTasks requestPersonTasks, IScreeningTasks screeningTasks, IEmailTasks emailTasks, IUserTasks userTasks)
        {
            this.requestTasks = requestTasks;
            this.requestPersonTasks = requestPersonTasks;
            this.screeningTasks = screeningTasks;
            this.emailTasks = emailTasks;
            this.userTasks = userTasks;
        }

        public ActionResult Index()
        {
            IList<Request> requests = this.requestTasks.GetRequestsForConsolidation();
            ViewBag.ScreeningEntities = this.screeningTasks.GetScreeningEntities();
            ViewBag.NominatedRequestPersons = this.requestPersonTasks.GetNominatedRequestPersons();
            return View(requests);
        }

        public ActionResult RequestAction(int id)
        {
            Request request = this.requestTasks.Get(id);
            if (request != null)
            {
                ViewBag.Request = request;
                ViewBag.ScreeningEntities = ScreeningEntity.GetNames(request.GetCreatedDate());
                ConsolidateViewModel cvm = new ConsolidateViewModel(request, this.screeningTasks.GetScreeningResults(request.GetCreatedDate()));
                return View(cvm);
            }
            else
            {
                return new HttpNotFoundResult();
            }
        }

        [HttpPost]
        [Transaction]
        public ActionResult RequestAction(ConsolidateViewModel cvm)
        {
            if (ModelState.IsValid)
            {
                // update commentary
                foreach (RecommendationViewModel rvm in cvm.Recommendations.Values)
                {
                    ScreeningRequestPersonRecommendation srpr;
                    if (rvm.Id > 0)
                        srpr = this.screeningTasks.GetRecommendation(rvm.Id);
                    else
                        srpr = new ScreeningRequestPersonRecommendation();

                    if (srpr.Version > rvm.Version)
                    {
                        ModelState.AddModelError("somekey", "Data has changed since you loaded this page.  Please reload before saving again.  Try pressing the back button to recover your edits.");
                        return RequestAction(cvm);
                    }

                    srpr.Commentary = rvm.Commentary;
                    srpr.Version = rvm.Version;
                    srpr.RequestPerson = this.requestPersonTasks.GetRequestPerson(rvm.RequestPersonID);
                    srpr.ScreeningResult = this.screeningTasks.GetScreeningResult(rvm.ScreeningResultID);
                    this.screeningTasks.SaveOrUpdateRecommendation(srpr, User.Identity.Name);
                }
                if (cvm.SendForFinalDecision)
                {
                    ViewBag.SuccessMessage = "Screening request sent for final decision.";

                    // update status
                    this.requestTasks.SaveRequestHistory(cvm.Id, this.requestTasks.GetRequestStatus(RequestStatus.NAME_SENT_FOR_FINAL_DECISION).Id, User.Identity.Name);

                    // send email notification
                    this.emailTasks.SendRequestSentForFinalDecisionEmail(User.Identity.Name, cvm.Id);

                    return RedirectToAction("Index");
                }
                else
                    //ViewBag.SuccessMessage = "Successfully saved commentary.  Click 'Send for Final Decision' when complete.";
                    return RedirectToAction("RequestAction", new { id = cvm.Id });  // need to finish transaction in order to load Recommendation.Id
            }
            return RequestAction(cvm.Id);
        }

        public JsonNetResult RequestProfile(int id)
        {
            RequestPerson rp = this.requestPersonTasks.GetRequestPerson(id);
            if (rp != null)
            {
                this.emailTasks.SendProfileRequestEmail(User.Identity.Name, rp.Person);
                return JsonNet(string.Empty);
            }
            return JsonNet("No request person exists with that ID.");
        }

        public JsonNetResult Nominate(int id)
        {
            RequestPerson rp = this.requestPersonTasks.GetRequestPerson(id);
            if (rp != null)
            {
                AdminUser user = this.userTasks.GetAdminUser(User.Identity.Name);
                if (this.requestPersonTasks.NominateRequestPerson(rp, user))
                    return JsonNet(string.Empty);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return JsonNet("Problem saving nomination for person.");
            }
            return JsonNet("No request person exists with that ID.");
        }

        public JsonNetResult WithdrawNomination(int id)
        {
            RequestPerson rp = this.requestPersonTasks.GetRequestPerson(id);
            if (rp != null)
            {
                AdminUser user = this.userTasks.GetAdminUser(User.Identity.Name);
                if (this.requestPersonTasks.WithdrawRequestPersonNomination(rp, user))
                    return JsonNet(string.Empty);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return JsonNet("Problem withdrawing nomination for person.");
            }
            return JsonNet("No request person exists with that ID.");
        }
    }
}