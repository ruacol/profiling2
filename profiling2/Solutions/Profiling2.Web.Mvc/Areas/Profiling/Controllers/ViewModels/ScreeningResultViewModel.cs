using System;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels
{
    public class ScreeningResultViewModel
    {
        public string Name { get; set; }
        public string Result { get; set; }
        public string Reason { get; set; }
        public string Commentary { get; set; }
        public DateTime Date { get; set; }
    }
}