using System;
using System.Collections.Generic;
using System.Web.Mvc;
using AutoMapper;
using Profiling2.Domain;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Contracts.Tasks.Screenings;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Scr;
using Profiling2.Web.Mvc.Areas.Screening.Controllers.ViewModels;
using Profiling2.Web.Mvc.Controllers;
using SharpArch.NHibernate.Web.Mvc;

namespace Profiling2.Web.Mvc.Areas.Screening.Controllers
{
    [MultiRoleAuthorize(
        AdminRole.ScreeningRequestValidator
    )]
    public class ValidateController : ScreeningBaseController
    {
        protected readonly IRequestTasks requestTasks;
        protected readonly IUserTasks userTasks;
        protected readonly IRequestPersonTasks requestPersonTasks;
        protected readonly IEmailTasks emailTasks;

        public ValidateController(IRequestTasks requestTasks, IUserTasks userTasks, IRequestPersonTasks requestPersonTasks, IEmailTasks emailTasks)
        {
            this.requestTasks = requestTasks;
            this.userTasks = userTasks;
            this.requestPersonTasks = requestPersonTasks;
            this.emailTasks = emailTasks;
        }

        public ActionResult Index()
        {
            IList<Request> requests = this.requestTasks.GetRequestsForValidation();
            return View(requests);
        }

        public ActionResult RequestAction(int id)
        {
            Request request = this.requestTasks.Get(id);
            if (request != null)
            {
                ViewBag.Request = request;
                ValidateViewModel vm = new ValidateViewModel(request) { ForwardRequest = false };
                vm.PopulateDropDowns(this.requestTasks.GetRequestEntities(), this.requestTasks.GetRequestTypes(), this.requestTasks.GetRequestStatuses());
                return View(vm);
            }
            else
            {
                return new HttpNotFoundResult();
            }
        }

        [HttpPost]
        [Transaction]
        public ActionResult RequestAction(ValidateViewModel vm)
        {
            if (ModelState.IsValid)
            {
                Request request = this.requestTasks.Get(vm.Id);
                if (request != null)
                {
                    // only forward requests that are in the 'sent for validation' stage of the workflow
                    if ((request.HasBeenSentForValidation && !request.HasBeenForwardedForScreening)  // allows for 'Edited' status
                        || request.IsSentForValidation)
                    {
                        // update Request attributes
                        Mapper.Map<RequestViewModel, Request>(vm, request);
                        request.RequestEntity = this.requestTasks.GetRequestEntity(vm.RequestEntityID);
                        request.RequestType = this.requestTasks.GetRequestType(vm.RequestTypeID);
                        if (!string.IsNullOrEmpty(vm.RespondBy))
                            request.RespondBy = DateTime.Parse(vm.RespondBy);
                        else
                            request.RespondBy = null;
                        this.requestTasks.SaveOrUpdateRequest(request);

                        if (vm.RejectRequest)
                        {
                            // set rejected status
                            this.requestTasks.SaveRequestHistory(vm.Id, this.requestTasks.GetRequestStatus(RequestStatus.NAME_REJECTED).Id, User.Identity.Name, vm.RejectReason);

                            // send email notification
                            this.emailTasks.SendRequestRejectedEmail(User.Identity.Name, vm.Id, vm.RejectReason);

                            return RedirectToAction("Index");
                        }

                        if (vm.ForwardRequest)
                        {
                            // can't forward request that contains any proposed persons
                            if (!request.HasProposedPersons)
                            {
                                // update status
                                this.requestTasks.SaveRequestHistory(vm.Id, this.requestTasks.GetRequestStatus(RequestStatus.NAME_SENT_FOR_SCREENING).Id, User.Identity.Name);

                                // send email notification
                                this.emailTasks.SendRequestForwardedToConditionalityParticipantsEmail(User.Identity.Name, vm.Id);

                                return RedirectToAction("Index");
                            }
                            else
                            {
                                ModelState.AddModelError("Request", "Request containing proposed persons cannot be forwarded until all proposed persons have been replaced by profiles existing in the database.");
                                return RequestAction(vm.Id);
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("Request", "Request is not in the right status to be validated.");
                        return RequestAction(vm.Id);
                    }
                }
                else
                {
                    return new HttpNotFoundResult();
                }
            }
            return RequestAction(vm.Id);
        }
    }
}
