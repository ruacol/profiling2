using Profiling2.Domain.Prf.Units;
using Profiling2.Web.Mvc.Helpers;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels
{
    public class OperationDataTableView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Objective { get; set; }
        public string StartDate { get; set; }

        public OperationDataTableView() { }

        public OperationDataTableView(Operation o)
        {
            if (o != null)
            {
                this.Id = o.Id;
                this.Name = o.ToString();
                this.Objective = o.Objective;
                this.StartDate = new DateLabel(o.YearOfStart, o.MonthOfStart, o.DayOfStart, false).ToString();
            }
        }
    }
}