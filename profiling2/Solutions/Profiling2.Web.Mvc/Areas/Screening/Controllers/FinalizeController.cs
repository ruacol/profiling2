using System.Collections.Generic;
using System.Web.Mvc;
using Profiling2.Domain;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Contracts.Tasks.Screenings;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Scr;
using Profiling2.Domain.Scr.PersonFinalDecision;
using Profiling2.Web.Mvc.Areas.Screening.Controllers.ViewModels;
using Profiling2.Web.Mvc.Controllers;
using SharpArch.NHibernate.Web.Mvc;

namespace Profiling2.Web.Mvc.Areas.Screening.Controllers
{
    [MultiRoleAuthorize(
        AdminRole.ScreeningRequestFinalDecider
    )]
    public class FinalizeController : ScreeningBaseController
    {
        protected readonly IRequestTasks requestTasks;
        protected readonly IScreeningTasks screeningTasks;
        protected readonly IRequestPersonTasks requestPersonTasks;
        protected readonly IEmailTasks emailTasks;

        public FinalizeController(IRequestTasks requestTasks, IScreeningTasks screeningTasks, IRequestPersonTasks requestPersonTasks, IEmailTasks emailTasks)
        {
            this.requestTasks = requestTasks;
            this.screeningTasks = screeningTasks;
            this.requestPersonTasks = requestPersonTasks;
            this.emailTasks = emailTasks;
        }

        public ActionResult Index()
        {
            IList<Request> requests = this.requestTasks.GetRequestsForFinalization();
            return View(requests);
        }

        public ActionResult RequestAction(int id)
        {
            Request request = this.requestTasks.Get(id);
            if (request != null)
            {
                ViewBag.Request = request;
                ViewBag.ScreeningEntities = ScreeningEntity.GetNames(request.GetCreatedDate());
                this.screeningTasks.CreateScreeningRequestPersonFinalDecisionsForRequest(request, User.Identity.Name);  // prepopulate results from recommendations
                FinalizeViewModel cvm = new FinalizeViewModel(request, 
                    this.screeningTasks.GetScreeningResults(request.GetCreatedDate()), 
                    this.screeningTasks.GetScreeningSupportStatuses());
                return View(cvm);
            }
            else
            {
                return new HttpNotFoundResult();
            }
        }

        [HttpPost]
        [Transaction]
        public ActionResult RequestAction(FinalizeViewModel vm)
        {
            if (ModelState.IsValid)
            {
                // update commentary
                foreach (FinalDecisionViewModel fdvm in vm.FinalDecisions.Values)
                {
                    ScreeningRequestPersonFinalDecision srpfd;
                    if (fdvm.Id > 0)
                        srpfd = this.screeningTasks.GetFinalDecision(fdvm.Id);
                    else
                        srpfd = new ScreeningRequestPersonFinalDecision();

                    if (srpfd.Version > fdvm.Version)
                    {
                        ModelState.AddModelError("somekey", "Data has changed since you loaded this page.  Please reload before saving again.  Try pressing the back button to recover your edits.");
                        return RequestAction(vm);
                    }

                    srpfd.Commentary = fdvm.Commentary;
                    srpfd.Version = fdvm.Version;
                    srpfd.RequestPerson = this.requestPersonTasks.GetRequestPerson(fdvm.RequestPersonID);
                    srpfd.ScreeningResult = this.screeningTasks.GetScreeningResult(fdvm.ScreeningResultID);
                    srpfd.ScreeningSupportStatus = this.screeningTasks.GetScreeningSupportStatus(fdvm.ScreeningSupportStatusID);
                    this.screeningTasks.SaveOrUpdateFinalDecision(srpfd, User.Identity.Name);
                }
                if (vm.Finalize)
                {
                    ViewBag.SuccessMessage = "Screening request complete.";

                    // update status
                    this.requestTasks.SaveRequestHistory(vm.Id, this.requestTasks.GetRequestStatus(RequestStatus.NAME_COMPLETED).Id, User.Identity.Name);

                    // send email notification
                    this.emailTasks.SendRequestCompletedEmail(User.Identity.Name, vm.Id);

                    return RedirectToAction("Index");
                }
                else
                {
                    // redirecting instead of calling RequestAction directly, as this starts a new transaction.
                    // this is needed for the Version column validation to work properly.
                    return RedirectToAction("RequestAction", new { id = vm.Id });
                }
            }
            return RequestAction(vm.Id);
        }
    }
}
