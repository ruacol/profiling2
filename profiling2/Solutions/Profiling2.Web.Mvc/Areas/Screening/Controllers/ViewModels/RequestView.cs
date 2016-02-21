using System;
using System.Linq;
using Profiling2.Domain;
using Profiling2.Domain.Scr;

namespace Profiling2.Web.Mvc.Areas.Screening.Controllers.ViewModels
{
    /// <summary>
    /// Used by DataTables.
    /// </summary>
    public class RequestView
    {
        public int Id { get; set; }
        public ReferenceNumber ReferenceNumber { get; set; }
        public string RequestName { get; set; }
        public string RequestEntity { get; set; }
        public string RequestType { get; set; }
        public RespondBy RespondBy { get; set; }
        public string CurrentStatus { get; set; }
        public DateTime CurrentStatusDate { get; set; }
        public int NumPersons { get; set; }
        public string Description { get; set; }

        public RequestView() { }

        public RequestView(Request r)
        {
            this.Id = r.Id;
            if (r.RequestEntity != null)
            {
                this.RequestEntity = r.RequestEntity.RequestEntityName;
            }
            if (r.RequestType != null)
            {
                this.RequestType = r.RequestType.RequestTypeName;
            }
            this.RequestName = r.RequestName;
            this.ReferenceNumber = new ReferenceNumber(r.ReferenceNumber);
            if (r.RespondImmediately)
                this.RespondBy = new RespondBy("Immediately");
            else
                this.RespondBy = new RespondBy(r.RespondBy.HasValue ? r.RespondBy.Value.ToString("yyyy-MM-dd") : string.Empty);
            this.CurrentStatus = r.CurrentStatus.ToString();
            this.CurrentStatusDate = r.CurrentStatusDate.Value;
            this.NumPersons = r.Persons.Where(x => !x.Archive).Count() + r.ProposedPersons.Where(x => !x.Archive).Count();

            this.Description = r.Notes;
        }
    }
}