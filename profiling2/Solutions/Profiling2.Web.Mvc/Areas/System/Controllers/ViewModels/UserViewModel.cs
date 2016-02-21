using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using Profiling2.Domain.Scr;

namespace Profiling2.Web.Mvc.Areas.System.Controllers.ViewModels
{
    public class UserViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "An ID is required for this user.")]
        [StringLength(450, ErrorMessage = "ID must not be longer than 450 characters.")]
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string AdminRoleIds { get; set; }
        public string SourceOwningEntityIds { get; set; }
        public int? RequestEntityID { get; set; }
        public int? ScreeningEntityID { get; set; }
        public bool HasIntranetServicesAccess { get; set; }

        public IEnumerable<SelectListItem> RequestEntitySelectItems { get; set; }
        public IEnumerable<SelectListItem> ScreeningEntitySelectItems { get; set; }

        public void PopulateDropDowns(IEnumerable<RequestEntity> res, IEnumerable<ScreeningEntity> ses)
        {
            this.RequestEntitySelectItems = (from re in res
                                             select new SelectListItem { Text = re.ToString(), Value = re.Id.ToString() }).ToList();
            this.ScreeningEntitySelectItems = (from se in ses
                                               select new SelectListItem { Text = se.ToString(), Value = se.Id.ToString() }).ToList();
        }
    }
}