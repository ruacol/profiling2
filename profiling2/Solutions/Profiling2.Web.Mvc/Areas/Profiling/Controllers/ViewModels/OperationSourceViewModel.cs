using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Profiling2.Domain.Prf.Sources;
using Profiling2.Domain.Prf.Units;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels
{
    public class OperationSourceViewModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "A source is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "A valid source is required.")]
        public int? SourceId { get; set; }

        [Required(ErrorMessage = "An operation is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "A valid operation is required.")]
        public int? OperationId { get; set; }

        [Required(ErrorMessage = "A reliability is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "A valid reliability is required.")]
        public int? ReliabilityId { get; set; }
        public string Commentary { get; set; }
        public string Notes { get; set; }

        public SelectList Reliabilities { get; set; }

        //for display purposes only
        public string OperationName { get; set; }
        public string SourceName { get; set; }
        public bool SourceArchive { get; set; }
        public bool SourceIsRestricted { get; set; }
        public string ReliabilityName { get; set; }

        public OperationSourceViewModel() { }

        public OperationSourceViewModel(Operation o)
        {
            if (o != null)
            {
                this.OperationId = o.Id;
                this.OperationName = o.Name;
            }
        }

        public OperationSourceViewModel(OperationSource os)
        {
            this.Id = os.Id;
            if (os.Operation != null)
            {
                this.OperationId = os.Operation.Id;
                this.OperationName = os.Operation.Name;
            }
            if (os.Reliability != null)
            {
                this.ReliabilityId = os.Reliability.Id;
                this.ReliabilityName = os.Reliability.ToString();
            }
            this.Commentary = os.Commentary;
            this.Notes = os.Notes;
        }

        public void PopulateDropDowns(IEnumerable<Reliability> reliabilities)
        {
            this.Reliabilities = new SelectList(reliabilities, "Id", "ReliabilityName", this.ReliabilityId);
        }

        public void PopulateSource(SourceDTO s)
        {
            if (s != null)
            {
                this.SourceId = s.SourceID;
                this.SourceName = s.SourceName;
                this.SourceArchive = s.Archive;
                this.SourceIsRestricted = s.IsRestricted;
            }
        }
    }
}