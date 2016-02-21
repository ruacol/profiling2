using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Prf;
using Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels;
using Profiling2.Web.Mvc.Controllers;
using SharpArch.NHibernate.Web.Mvc;
using SharpArch.Web.Mvc.JsonNet;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers
{
    public class TagsController : BaseController
    {
        protected readonly IEventTasks eventTasks;

        public TagsController(IEventTasks eventTasks)
        {
            this.eventTasks = eventTasks;
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchEvents)]
        public ActionResult Index()
        {
            return View(this.eventTasks.GetAllTags());
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchEvents)]
        public ActionResult Details(int id)
        {
            Tag tag = this.eventTasks.GetTag(id);
            if (tag != null)
            {
                return View(tag);
            }
            return new HttpNotFoundResult();
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchEvents)]
        public JsonNetResult Get()
        {
            string term = Request.QueryString["term"];
            if (string.IsNullOrEmpty(term))
            {
                IList<Tag> tags = this.eventTasks.GetAllTags();
                object[] objects = (from t in tags
                                    select new { id = t.Id, text = t.ToString() }).ToArray();
                return JsonNet(objects);
            }
            else
            {
                term = term.Trim();
                IList<Tag> tags = this.eventTasks.SearchTags(term.Trim());
                object[] objects = (from t in tags
                                    select new { id = t.Id, text = t.ToString() }).ToArray();
                return JsonNet(objects);
            }
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchEvents)]
        public JsonNetResult Name(int id)
        {
            Tag tag = this.eventTasks.GetTag(id);
            if (tag != null)
            {
                return JsonNet(new
                {
                    Id = id,
                    Name = tag.ToString()
                });
            }
            else
                return JsonNet(string.Empty);
        }

        [PermissionAuthorize(AdminPermission.CanChangeEvents)]
        public ActionResult Create()
        {
            TagViewModel vm = new TagViewModel();
            return View(vm);
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangeEvents)]
        public ActionResult Create(TagViewModel vm)
        {
            if (this.eventTasks.GetTag(vm.TagName) != null)
                ModelState.AddModelError("TagName", "Tag name already exists.");

            if (ModelState.IsValid)
            {
                Tag tag = new Tag()
                {
                    TagName = vm.TagName,
                    Notes = vm.Notes,
                    Archive = vm.Archive
                };
                tag = this.eventTasks.SaveTag(tag);
                return RedirectToAction("Details", "Tags", new { id = tag.Id });
            }
            return Create();
        }

        [PermissionAuthorize(AdminPermission.CanChangeEvents)]
        public ActionResult Edit(int id)
        {
            Tag tag = this.eventTasks.GetTag(id);
            if (tag != null)
                return View(new TagViewModel(tag));
            return new HttpNotFoundResult();
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangeEvents)]
        public ActionResult Edit(TagViewModel vm)
        {
            Tag tag = this.eventTasks.GetTag(vm.Id);
            Tag newTag = this.eventTasks.GetTag(vm.TagName);
            if (tag != null && newTag != null && newTag.Id != tag.Id)
                ModelState.AddModelError("TagName", "Tag name already exists.");

            if (ModelState.IsValid)
            {
                tag.TagName = vm.TagName;
                tag.Notes = vm.Notes;
                tag.Archive = vm.Archive;
                return RedirectToAction("Details", new { id = vm.Id });
            }
            return Edit(vm.Id);
        }

        [Transaction]
        [PermissionAuthorize(AdminPermission.CanChangeEvents)]
        public ActionResult Delete(int id)
        {
            Tag tag = this.eventTasks.GetTag(id);
            if (tag != null)
            {
                this.eventTasks.DeleteTag(tag);
                return RedirectToAction("Index");
            }
            return new HttpNotFoundResult();
        }
    }
}