using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Profiling2.Domain.Prf.Events;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels
{
    public class ViolationViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "A name is required.")]
        [StringLength(255, ErrorMessage = "Name must not be longer than 255 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "A set of keywords is required.")]
        public string Keywords { get; set; }

        [StringLength(500, ErrorMessage = "Description must not be longer than 500 characters.")]
        public string Description { get; set; }

        public int? ParentViolationID { get; set; }

        public bool ConditionalityInterest { get; set; }

        public SelectList ParentViolations { get; set; }

        public ViolationViewModel() { }

        public ViolationViewModel(Violation v)
        {
            this.Id = v.Id;
            this.Name = v.Name;
            this.Keywords = v.Keywords;
            this.Description = v.Description;
            this.ConditionalityInterest = v.ConditionalityInterest;
            if (v.ParentViolation != null)
                this.ParentViolationID = v.ParentViolation.Id;
        }

        public void PopulateDropDowns(IEnumerable<Violation> violations)
        {
            this.ParentViolations = new SelectList(violations, "Id", "Name", this.ParentViolationID);
        }
    }
}