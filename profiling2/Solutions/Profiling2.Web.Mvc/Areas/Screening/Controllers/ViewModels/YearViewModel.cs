using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Profiling2.Web.Mvc.Areas.Screening.Controllers.ViewModels
{
    public class YearViewModel
    {
        public int Year { get; set; }

        public SelectList Years { get; set; }

        public YearViewModel() { }

        public void PopulateDropDowns(int year)
        {
            IList<int> years = new List<int>();
            for (int i = 0; i < 10; i++)
                years.Add(year - i);

            this.Years = new SelectList(years.Select(x => new { Text = x, Value = x }), "Value", "Text", this.Year);
        }
    }
}