using System;
using System.Web.Mvc;
using Profiling2.Domain;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Contracts.Tasks.Screenings;
using Profiling2.Domain.Prf.Organizations;
using Profiling2.Domain.Scr;
using Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels;
using Profiling2.Web.Mvc.Areas.Screening.Controllers.ViewModels;
using Profiling2.Web.Mvc.Controllers;
using StackExchange.Profiling;
using System.Collections.Generic;
using Profiling2.Domain.Prf;

namespace Profiling2.Web.Mvc.Areas.Screening.Controllers
{
    public class HomeController : ScreeningBaseController
    {
        private readonly IScreeningTasks screeningTasks;
        private readonly IRequestTasks requestTasks;
        private readonly IScreeningStatisticTasks screeningStatisticTasks;
        private readonly IOrganizationTasks orgTasks;

        public HomeController(IScreeningTasks screeningTasks, 
            IRequestTasks requestTasks, 
            IScreeningStatisticTasks screeningStatisticTasks,
            IOrganizationTasks orgTasks)
        {
            this.screeningTasks = screeningTasks;
            this.requestTasks = requestTasks;
            this.screeningStatisticTasks = screeningStatisticTasks;
            this.orgTasks = orgTasks;
        }

        public ActionResult Index()
        {
            var count = this.screeningStatisticTasks.GetFinalDecisionCountByMonth();
            return View(count);
        }

        [MultiRoleAuthorize(
            AdminRole.ProfilingAdmin,
            AdminRole.ScreeningRequestConsolidator,
            AdminRole.ScreeningRequestFinalDecider
        )]
        public ActionResult Counts()
        {
            DateViewModel vm = new DateViewModel(DateTime.Now.Subtract(TimeSpan.FromDays(30)), DateTime.Now);
            return View(vm);
        }

        [HttpPost]
        [MultiRoleAuthorize(
            AdminRole.ProfilingAdmin,
            AdminRole.ScreeningRequestConsolidator,
            AdminRole.ScreeningRequestFinalDecider
        )]
        public ActionResult Counts(DateViewModel vm)
        {
            var profiler = MiniProfiler.Current;

            using (profiler.Step("CountsByRequestEntity"))
                ViewBag.CountsByRequestEntity = this.screeningStatisticTasks.GetFinalDecisionCountByRequestEntity(vm.StartDateAsDate, vm.EndDateAsDate);

            using (profiler.Step("CountsByIndividualRequestEntity"))
                ViewBag.CountsByIndividualRequestEntity = this.screeningStatisticTasks.GetFinalDecisionIndividualCountByRequestEntity(vm.StartDateAsDate, vm.EndDateAsDate);
            
            using (profiler.Step("CountsByResult"))
                ViewBag.CountsByResult = this.screeningStatisticTasks.GetFinalDecisionCountByResult(vm.StartDateAsDate, vm.EndDateAsDate);

            using (profiler.Step("CountsByIndividualResult"))
                ViewBag.CountsByIndividualResult = this.screeningStatisticTasks.GetFinalDecisionIndividualCountByResult(vm.StartDateAsDate, vm.EndDateAsDate);

            using (profiler.Step("CountsByRequestType"))
                ViewBag.CountsByRequestType = this.requestTasks.GetRequestCountByType(vm.StartDateAsDate, vm.EndDateAsDate);

            return View(vm);
        }

        [PermissionAuthorize(AdminPermission.CanAdministrate)]
        public ActionResult ScreeningEntityCounts()
        {
            DateViewModel vm = new DateViewModel(DateTime.Now.Subtract(TimeSpan.FromDays(30)), DateTime.Now);
            return View(vm);
        }

        [HttpPost]
        [PermissionAuthorize(AdminPermission.CanAdministrate)]
        public ActionResult ScreeningEntityCounts(DateViewModel vm)
        {
            var profiler = MiniProfiler.Current;

            using (profiler.Step("ScreeningEntityStatistics"))
                ViewBag.ScreeningEntityStatistics = this.screeningStatisticTasks.GetScreeningEntityStatistics(vm.StartDateAsDate, vm.EndDateAsDate);

            return View(vm);
        }

        [PermissionAuthorize(AdminPermission.CanAdministrate)]
        public ActionResult FinalDecisions()
        {
            QueryFinalDecisionViewModel vm = new QueryFinalDecisionViewModel();
            vm.PopulateDropDowns(this.screeningTasks.GetScreeningResults());
            return View(vm);
        }

        [HttpPost]
        [PermissionAuthorize(AdminPermission.CanAdministrate)]
        public ActionResult FinalDecisions(QueryFinalDecisionViewModel vm)
        {
            ScreeningResult sr = this.screeningTasks.GetScreeningResult(vm.ScreeningResultId);
            if (sr != null)
            {
                if (vm.OrganizationId.HasValue)
                {
                    Organization org = this.orgTasks.GetOrganization(vm.OrganizationId.Value);
                    ViewData["people"] = this.screeningTasks.GetFinalDecisions(sr.ScreeningResultName, org);
                }
                else
                {
                    ViewData["people"] = this.screeningTasks.GetFinalDecisions(sr.ScreeningResultName);
                }
            }

            vm.PopulateDropDowns(this.screeningTasks.GetScreeningResults());
            return View(vm);
        }

        [MultiRoleAuthorize(
            AdminRole.ProfilingAdmin,
            AdminRole.ScreeningRequestConsolidator,
            AdminRole.ScreeningRequestFinalDecider
        )]
        public ActionResult Charts()
        {
            YearViewModel vm = new YearViewModel();
            vm.PopulateDropDowns(DateTime.Now.Year);
            return View(vm);
        }

        [HttpPost]
        [MultiRoleAuthorize(
            AdminRole.ProfilingAdmin,
            AdminRole.ScreeningRequestConsolidator,
            AdminRole.ScreeningRequestFinalDecider
        )]
        public ActionResult Charts(YearViewModel vm)
        {
            var profiler = MiniProfiler.Current;

            using (profiler.Step("CountsByRequestEntity"))
                ViewBag.CountsByRequestEntity = this.screeningStatisticTasks.GetFinalDecisionCountByRequestEntity(vm.Year);

            using (profiler.Step("CountsByResult"))
                ViewBag.CountsByResult = this.screeningStatisticTasks.GetFinalDecisionCountByResult(vm.Year);

            vm.PopulateDropDowns(DateTime.Now.Year);
            return View(vm);
        }
    }
}
