using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Profiling2.Domain.Contracts.Tasks.Sources;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Sources;
using Profiling2.Web.Mvc.Areas.Sources.Controllers.ViewModels;
using Profiling2.Web.Mvc.Controllers;
using SharpArch.NHibernate.Web.Mvc;
using SharpArch.Web.Mvc.JsonNet;

namespace Profiling2.Web.Mvc.Areas.Sources.Controllers
{
    public class AuthorsController : BaseController
    {
        protected readonly ISourcePermissionTasks sourcePermissionTasks;

        public AuthorsController(ISourcePermissionTasks sourcePermissionTasks)
        {
            this.sourcePermissionTasks = sourcePermissionTasks;
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchSources)]
        public JsonNetResult Name(int id)
        {
            SourceAuthor a = this.sourcePermissionTasks.GetSourceAuthor(id);
            if (a != null)
            {
                return JsonNet(new
                {
                    Id = id,
                    Name = a.Author
                });
            }
            else
                return JsonNet(string.Empty);
        }

        [PermissionAuthorize(AdminPermission.CanViewAndSearchSources)]
        public JsonNetResult Get()
        {
            string term = Request.QueryString["term"];
            IList<SourceAuthor> authors;

            if (string.IsNullOrEmpty(term))
                authors = this.sourcePermissionTasks.GetAllSourceAuthors();
            else
                authors = this.sourcePermissionTasks.SearchSourceAuthors(term);

            return JsonNet((from a in authors
                            select new { id = a.Id, text = a.Author }).ToList<object>());
        }

        [PermissionAuthorize(AdminPermission.CanUploadSources)]
        public ActionResult CreateModal()
        {
            SourceAuthorViewModel vm = new SourceAuthorViewModel();
            return View(vm);
        }

        [HttpPost]
        [Transaction]
        [PermissionAuthorize(AdminPermission.CanUploadSources)]
        public JsonNetResult CreateModal(SourceAuthorViewModel vm)
        {
            IList<SourceAuthor> existing = this.sourcePermissionTasks.GetSourceAuthor(vm.Author);
            if (existing != null && existing.Count > 0)
                ModelState.AddModelError("Author", "Author name already exists.");

            if (ModelState.IsValid)
            {
                SourceAuthor entity = new SourceAuthor();
                entity.Author = vm.Author;
                entity = this.sourcePermissionTasks.SaveSourceAuthor(entity);
                return JsonNet(new
                {
                    Id = entity.Id,
                    Name = entity.Author,
                    WasSuccessful = true
                });
            }
            else
            {
                return JsonNet(this.GetErrorsForJson());
            }
        }
    }
}