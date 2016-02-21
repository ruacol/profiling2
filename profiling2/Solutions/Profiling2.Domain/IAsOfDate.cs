
namespace Profiling2.Domain
{
    public interface IAsOfDate
    {
        int DayOfStart { get; set; }
        int MonthOfStart { get; set; }
        int YearOfStart { get; set; }
        int DayOfEnd { get; set; }
        int MonthOfEnd { get; set; }
        int YearOfEnd { get; set; }

        int DayAsOf { get; set; }
        int MonthAsOf { get; set; }
        int YearAsOf { get; set; }
    }
}
