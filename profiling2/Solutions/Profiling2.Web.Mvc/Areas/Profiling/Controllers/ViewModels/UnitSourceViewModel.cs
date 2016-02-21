using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Profiling2.Domain.Prf.Sources;
using Profiling2.Domain.Prf.Units;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels
{
    public class UnitSourceViewModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "A source is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "A valid source is required.")]
        public int? SourceId { get; set; }

        [Required(ErrorMessage = "A unit is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "A valid unit is required.")]
        public int? UnitId { get; set; }

        [Required(ErrorMessage = "A reliability is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "A valid reliability is required.")]
        public int? ReliabilityId { get; set; }
        public string Commentary { get; set; }
        public string Notes { get; set; }

        public SelectList Reliabilities { get; set; }

        //for display purposes only
        public string UnitName { get; set; }
        public string SourceName { get; set; }
        public bool SourceArchive { get; set; }
        public bool SourceIsRestricted { get; set; }
        public string ReliabilityName { get; set; }

        public UnitSourceViewModel() { }

        public UnitSourceViewModel(Unit u)
        {
            if (u != null)
            {
                this.UnitId = u.Id;
                this.UnitName = u.UnitName;
            }
        }

        public UnitSourceViewModel(UnitSource us)
        {
            this.Id = us.Id;
            if (us.Unit != null)
            {
                this.UnitId = us.Unit.Id;
                this.UnitName = us.Unit.UnitName;
            }
            if (us.Reliability != null)
            {
                this.ReliabilityId = us.Reliability.Id;
                this.ReliabilityName = us.Reliability.ToString();
            }
            this.Commentary = us.Commentary;
            this.Notes = us.Notes;
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