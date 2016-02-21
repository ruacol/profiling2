using System;
using Profiling2.Domain.Prf;

namespace Profiling2.Domain.Scr
{
    // Useful object for combining various *History objects and sorting based on their common date field.
    public class HistoryViewModel
    {
        public string Status { get; set; }
        public DateTime Date { get; set; }
        public AdminUser User { get; set; }
        public Type Type { get; set; }
        public string Notes { get; set; }

        public HistoryViewModel() { }
    }
}
