using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using log4net;
using Profiling2.Domain;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Contracts.Tasks.Sources;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Scr;
using Profiling2.Infrastructure.Security;
using Profiling2.Web.Mvc.Areas.System.Controllers.ViewModels;

namespace Profiling2.Web.Mvc.Areas.System.Controllers
{
    public class LuceneController : SystemBaseController
    {
        protected static ILog log = LogManager.GetLogger(typeof(LuceneController));
        protected readonly ILuceneTasks luceneTasks;
        protected readonly IUserTasks userTasks;
        protected readonly ISourcePermissionTasks sourcePermissionTasks;
        protected readonly IBackgroundTasks backgroundTasks;

        public LuceneController(ILuceneTasks luceneTasks, 
            IUserTasks userTasks,
            ISourcePermissionTasks sourcePermissionTasks,
            IBackgroundTasks backgroundTasks)
        {
            this.luceneTasks = luceneTasks;
            this.userTasks = userTasks;
            this.sourcePermissionTasks = sourcePermissionTasks;
            this.backgroundTasks = backgroundTasks;
        }

        public ActionResult Index()
        {
            ViewBag.SourceOwningEntities = this.sourcePermissionTasks.GetAllSourceOwningEntities();
            return View();
        }

        public ActionResult CreatePersonIndexes()
        {
            this.backgroundTasks.CreatePersonIndex();
            return null;
        }

        public ActionResult CreateUnitIndexes()
        {
            this.backgroundTasks.CreateUnitIndex();
            return null;
        }

        public ActionResult CreateScreeningResponseIndexes()
        {
            this.backgroundTasks.CreateScreeningResponseIndex();
            return null;
        }

        /// <summary>
        /// This is a long-running action - be careful not to call at a time when IIS will recycle its workers!  Check IIS configuration.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public ActionResult CreateSourceIndexes(string code)
        {
            this.backgroundTasks.UpdateSourceIndex();
            return null;
        }

        public ActionResult CreateEventIndexes()
        {
            this.backgroundTasks.CreateEventIndex();
            return null;
        }

        public ActionResult CreateRequestIndexes()
        {
            this.backgroundTasks.CreateRequestIndex();
            return null;
        }

        public ActionResult DeletePersonIndexes()
        {
            this.luceneTasks.DeletePersonIndexes();
            return null;
        }

        public ActionResult DeleteUnitIndexes()
        {
            this.luceneTasks.DeleteUnitIndexes();
            return null;
        }

        public ActionResult DeleteScreeningResponseIndexes()
        {
            this.luceneTasks.DeleteScreeningResponseIndexes();
            return null;
        }

        public ActionResult DeleteSourceIndexes()
        {
            this.luceneTasks.DeleteSourceIndexes();
            return null;
        }

        public ActionResult DeleteEventIndexes()
        {
            this.luceneTasks.DeleteEventIndexes();
            return null;
        }

        public ActionResult DeleteRequestIndexes()
        {
            this.luceneTasks.DeleteRequestIndexes();
            return null;
        }

        public ActionResult PersonSearch()
        {
            SearchViewModel svm = new SearchViewModel();
            return View(svm);
        }

        [HttpPost]
        public ActionResult PersonSearch(SearchViewModel vm)
        {
            if (ModelState.IsValid)
            {
                IList<LuceneSearchResult> results = this.luceneTasks.PersonSearch(vm.Term, 50, true);
                ViewData["results"] = results;
                return View(vm);
            }
            return PersonSearch();
        }

        public ActionResult UnitSearch()
        {
            SearchViewModel svm = new SearchViewModel();
            return View(svm);
        }

        [HttpPost]
        public ActionResult UnitSearch(SearchViewModel vm)
        {
            if (ModelState.IsValid)
            {
                IList<LuceneSearchResult> results = this.luceneTasks.UnitSearch(vm.Term, 50);
                ViewData["results"] = results;
                return View(vm);
            }
            return UnitSearch();
        }

        public ActionResult ScreeningResponseSearch()
        {
            SearchViewModel svm = new SearchViewModel();
            return View(svm);
        }

        [HttpPost]
        public ActionResult ScreeningResponseSearch(SearchViewModel vm)
        {
            if (ModelState.IsValid)
            {
                AdminUser user = this.userTasks.GetAdminUser(User.Identity.Name);
                if (user != null)
                {
                    ScreeningEntity entity = user.GetScreeningEntity();
                    if (entity != null)
                    {
                        IList<LuceneSearchResult> results = this.luceneTasks.ScreeningResponseSearch(vm.Term, entity.ScreeningEntityName, 50);
                        ViewData["results"] = results;
                        return View(vm);
                    }
                    else
                        ModelState.AddModelError("ScreeningEntity", "User is not a member of any screening entity.");
                }
                else
                    ModelState.AddModelError("User", "User does not exist.");
            }
            return ScreeningResponseSearch();
        }

        public ActionResult SourceSearch()
        {
            SearchViewModel svm = new SearchViewModel();
            return View(svm);
        }

        [HttpPost]
        public ActionResult SourceSearch(SearchViewModel vm)
        {
            if (ModelState.IsValid)
            {
                if (((PrfPrincipal)User).HasPermission(AdminPermission.CanViewAndSearchAllSources))
                {
                    AdminUser user = this.userTasks.GetAdminUser(User.Identity.Name);
                    IList<string> affiliations = user.Affiliations.Select(x => x.Name).ToList();

                    ViewData["results"] = this.luceneTasks.SourceSearch(vm.Term, null, 50,
                        ((PrfPrincipal)User).HasPermission(AdminPermission.CanViewAndSearchAllSources),
                        ((PrfPrincipal)User).HasPermission(AdminPermission.CanViewAndSearchRestrictedSources),
                        User.Identity.Name, affiliations, null, false);
                    ViewData["facets"] = this.luceneTasks.SourceSearchFacets(vm.Term, null, null, null, 50,
                        ((PrfPrincipal)User).HasPermission(AdminPermission.CanViewAndSearchAllSources),
                        ((PrfPrincipal)User).HasPermission(AdminPermission.CanViewAndSearchRestrictedSources),
                        User.Identity.Name, affiliations);
                }
                else
                {
                    throw new NotImplementedException();
                }

                return View(vm);
            }
            return SourceSearch();
        }

        public ActionResult SourcesLike(int id)
        {
            IList<LuceneSearchResult> results = this.luceneTasks.GetSourcesLikeThis(id, 10);
            ViewData["results"] = results;
            return View();
        }

        public ActionResult EventSearch()
        {
            SearchViewModel svm = new SearchViewModel();
            return View(svm);
        }

        [HttpPost]
        public ActionResult EventSearch(SearchViewModel vm)
        {
            if (ModelState.IsValid)
            {
                IList<LuceneSearchResult> results = this.luceneTasks.EventSearch(vm.Term, 50, null, false);
                ViewData["results"] = results;
                return View(vm);
            }
            return EventSearch();
        }

        public ActionResult RequestSearch()
        {
            SearchViewModel svm = new SearchViewModel();
            return View(svm);
        }

        [HttpPost]
        public ActionResult RequestSearch(SearchViewModel vm)
        {
            if (ModelState.IsValid)
            {
                IList<LuceneSearchResult> results = this.luceneTasks.RequestSearch(vm.Term, 50, null, null, false);
                ViewData["results"] = results;
                return View(vm);
            }
            return RequestSearch();
        }
    }
}