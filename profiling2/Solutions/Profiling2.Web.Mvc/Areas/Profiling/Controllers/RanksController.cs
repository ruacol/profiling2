using AutoMapper;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Careers;
using Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels;
using Profiling2.Web.Mvc.Controllers;
using SharpArch.NHibernate.Web.Mvc;
using SharpArch.Web.Mvc.JsonNet;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers
{
    public class RanksController : BaseController
    {
        protected readonly IOrganizationTasks orgTasks;

        public RanksController(IOrganizationTasks orgTasks)
        {
            this.orgTasks = orgTasks;
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchPersons)]
        public JsonNetResult Get()
        {
            string term = Request.QueryString["term"];
            if (string.IsNullOrEmpty(term))
            {
                IList<Rank> ranks = this.orgTasks.GetAllRanks();
                object[] objects = (from t in ranks
                                    select new { id = t.Id, text = t.ToString() }).ToArray();
                return JsonNet(objects);
            }
            else
            {
                IList<Rank> ranks = this.orgTasks.GetRanksByName(term.Trim());
                object[] objects = (from t in ranks
                                    select new { id = t.Id, text = t.ToString() }).ToArray();
                return JsonNet(objects);
            }
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchPersons)]
        public JsonNetResult Name(int id)
        {
            Rank rank = this.orgTasks.GetRank(id);
            if (rank != null)
            {
                return JsonNet(new
                {
                    Id = id,
                    Name = rank.ToString()
                });
            }
            else
                return JsonNet(string.Empty);
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchPersons)]
        public ActionResult Index()
        {
            return View(this.orgTasks.GetAllRanks());
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchPersons)]
        public ActionResult Army()
        {
            return View(this.orgTasks.GetAllRanks().Where(x => x.SortOrder > 0 && x.SortOrder < 100).ToList<Rank>());
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchPersons)]
        public ActionResult Navy()
        {
            return View(this.orgTasks.GetAllRanks().Where(x => x.SortOrder > 100 && x.SortOrder < 200).ToList<Rank>());
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchPersons)]
        public ActionResult Police()
        {
            return View(this.orgTasks.GetAllRanks().Where(x => x.SortOrder > 200 && x.SortOrder < 300).ToList<Rank>());
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchPersons)]
        public ActionResult Details(int id)
        {
            Rank rank = this.orgTasks.GetRank(id);
            if (rank != null)
                return View(rank);
            return new HttpNotFoundResult();
        }

        [PermissionAuthorize(AdminPermission.CanChangePersons)]
        public ActionResult Create()
        {
            RankViewModel vm = new RankViewModel();
            return View(vm);
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangePersons)]
        public ActionResult Create(RankViewModel vm)
        {
            if (this.orgTasks.GetRank(vm.RankName) != null)
                ModelState.AddModelError("RankName", "Rank name already exists.");
            if (ModelState.IsValid)
            {
                Rank rank = new Rank();
                rank = Mapper.Map(vm, rank);
                rank = this.orgTasks.SaveRank(rank);
                return RedirectToAction("Details", "Ranks", new { id = rank.Id });
            }
            return Create();
        }

        [PermissionAuthorize(AdminPermission.CanChangePersons)]
        public ActionResult CreateModal()
        {
            return Create();
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangePersons)]
        public JsonNetResult CreateModal(RankViewModel vm)
        {
            if (this.orgTasks.GetRank(vm.RankName) != null)
                ModelState.AddModelError("RankName", "Rank name already exists.");
            if (ModelState.IsValid)
            {
                Rank rank = new Rank();
                rank = Mapper.Map(vm, rank);
                rank = this.orgTasks.SaveRank(rank);
                return JsonNet(new
                {
                    Id = rank.Id,
                    Name = rank.RankName,
                    WasSuccessful = true
                });
            }
            return JsonNet(this.GetErrorsForJson());
        }

        [PermissionAuthorize(AdminPermission.CanChangePersons)]
        public ActionResult Edit(int id)
        {
            Rank rank = this.orgTasks.GetRank(id);
            if (rank != null)
                return View(Mapper.Map(rank, new RankViewModel()));
            return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangePersons)]
        public ActionResult Edit(RankViewModel vm)
        {
            Rank rank = this.orgTasks.GetRank(vm.Id);
            Rank newRank = this.orgTasks.GetRank(vm.RankName);
            if (rank != null && newRank != null && newRank.Id != rank.Id)
                ModelState.AddModelError("RankName", "Rank name already exists.");
            if (ModelState.IsValid)
            {
                rank = Mapper.Map(vm, rank);
                return RedirectToAction("Details", new { id = vm.Id });
            }
            return Edit(vm.Id);
        }

        [PermissionAuthorize(AdminPermission.CanChangePersons)]
        public ActionResult EditModal(int id)
        {
            return Edit(id);
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangePersons)]
        public JsonNetResult EditModal(RankViewModel vm)
        {
            Rank rank = this.orgTasks.GetRank(vm.Id);
            Rank newRank = this.orgTasks.GetRank(vm.RankName);
            if (rank != null && newRank != null && newRank.Id != rank.Id)
                ModelState.AddModelError("RankName", "Rank name already exists.");
            if (ModelState.IsValid)
            {
                rank = Mapper.Map(vm, rank);
                return JsonNet(string.Empty);
            }
            return JsonNet(this.GetErrorsForJson());
        }

        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangePersons)]
        public ActionResult Delete(int id)
        {
            Rank rank = this.orgTasks.GetRank(id);
            if (rank != null)
            {
                if (this.orgTasks.DeleteRank(rank))
                    return RedirectToAction("Index");
                else
                    return RedirectToAction("Details", new { id = id });
            }
            return new HttpNotFoundResult();
        }
    }
}