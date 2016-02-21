using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using Profiling2.Domain.Scr;
using Profiling2.Domain.Scr.Attach;
using System;
using AutoMapper;

namespace Profiling2.Web.Mvc.Areas.Screening.Controllers.ViewModels
{
    // Used in forms as opposed to RequestView, which is used in DataTables.
    public class RequestViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "A name is required.")]
        [StringLength(255, ErrorMessage = "Name must not be longer than 255 characters.")]
        public string RequestName { get; set; }

        [Required(ErrorMessage = "A reference number is required.")]
        [StringLength(20, ErrorMessage = "Name must not be longer than 20 characters.")]
        public string ReferenceNumber { get; set; }

        // TODO validate date format
        public string RespondBy { get; set; }

        public bool RespondImmediately { get; set; }

        public string Notes { get; set; }

        public IList<RequestAttachment> RequestAttachments { get; set; }

        [Required(ErrorMessage = "A request entity is required.")]
        public int RequestEntityID { get; set; }

        [Required(ErrorMessage = "A request type is required.")]
        public int RequestTypeID { get; set; }

        public int RequestStatusID { get; set; }

        public IEnumerable<SelectListItem> RequestEntities { get; set; }

        public IEnumerable<SelectListItem> RequestTypes { get; set; }

        public IEnumerable<SelectListItem> RequestStatuses { get; set; }

        public RequestViewModel() { }

        public RequestViewModel(Request r) 
        {
            Mapper.Map(r, this);
            this.RespondBy = r.RespondBy.HasValue ? r.RespondBy.Value.ToString("yyyy-MM-dd") : string.Empty;
        }

        public void PopulateDropDowns(IEnumerable<RequestEntity> res, IEnumerable<RequestType> rts, IEnumerable<RequestStatus> rss)
        {
            this.RequestEntities = (from re in res
                                    where !(new string[] { RequestEntity.NAME_UNKNOWN, RequestEntity.NAME_UNDP }.Contains(re.RequestEntityName))
                                    select new SelectListItem { Text = re.ToString(), Value = re.Id.ToString() }).ToList();
            this.RequestTypes = (from rt in rts
                                 select new SelectListItem { Text = rt.ToString(), Value = rt.Id.ToString() }).ToList();
            this.RequestStatuses = (from rs in rss
                                    select new SelectListItem { Text = rs.ToString(), Value = rs.Id.ToString() }).ToList();
        }
    }
}