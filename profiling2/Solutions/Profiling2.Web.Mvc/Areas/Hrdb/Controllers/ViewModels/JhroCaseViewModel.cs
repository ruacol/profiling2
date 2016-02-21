using System.ComponentModel.DataAnnotations;
using Profiling2.Domain.Prf.Sources;

namespace Profiling2.Web.Mvc.Areas.Hrdb.Controllers.ViewModels
{
    public class JhroCaseViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "The case code is required.")]
        public string CaseNumber { get; set; }

        public JhroCaseViewModel() { }

        public JhroCaseViewModel(JhroCase jhroCase)
        {
            if (jhroCase != null)
            {
                this.Id = jhroCase.Id;
                this.CaseNumber = jhroCase.CaseNumber;
            }
        }
    }
}