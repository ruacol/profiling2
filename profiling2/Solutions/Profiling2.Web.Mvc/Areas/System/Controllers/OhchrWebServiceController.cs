using System;
using System.Web.Mvc;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels;
using SharpArch.NHibernate.Web.Mvc;

namespace Profiling2.Web.Mvc.Areas.System.Controllers
{
    public class OhchrWebServiceController : SystemBaseController
    {
        protected readonly IOhchrWebServiceTasks ohchrWebServiceTasks;

        public OhchrWebServiceController(IOhchrWebServiceTasks ohchrWebServiceTasks)
        {
            this.ohchrWebServiceTasks = ohchrWebServiceTasks;
        }

        public ActionResult Index()
        {
            DateViewModel vm = new DateViewModel(DateTime.Now.Subtract(TimeSpan.FromDays(1)), DateTime.Now);
            return View(vm);
        }

        [HttpPost]
        [Transaction]
        public ActionResult Index(DateViewModel vm)
        {
            ViewBag.Result = this.ohchrWebServiceTasks.GetAndPersistHrdbCases(vm.StartDateAsDate, vm.EndDateAsDate, null);
            return View(vm);
        }

        public ActionResult LogColumns()
        {
            this.ohchrWebServiceTasks.LogColumns();
            return null;
        }
    }
}