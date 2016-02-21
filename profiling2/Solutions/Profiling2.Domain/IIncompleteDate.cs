using System;

namespace Profiling2.Domain
{
    public interface IIncompleteDate
    {
        int DayOfStart { get; set; }
        int MonthOfStart { get; set; }
        int YearOfStart { get; set; }
        int DayOfEnd { get; set; }
        int MonthOfEnd { get; set; }
        int YearOfEnd { get; set; }
    }
}
