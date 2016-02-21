using System;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels
{
    public class DateViewModel
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }

        public DateViewModel() { }

        public DateViewModel(DateTime startDate, DateTime endDate)
        {
            this.StartDate = string.Format("{0:yyyy-MM-dd}", startDate);
            this.EndDate = string.Format("{0:yyyy-MM-dd}", endDate);
        }

        public DateTime StartDateAsDate
        {
            get
            {
                return DateTime.Parse(this.StartDate);
            }
        }

        public DateTime EndDateAsDate
        {
            get
            {
                return DateTime.Parse(this.EndDate);
            }
        }
    }
}