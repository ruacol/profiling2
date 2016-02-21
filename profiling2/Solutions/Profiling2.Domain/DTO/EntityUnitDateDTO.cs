using Profiling2.Domain.Extensions;
using Profiling2.Domain.Prf.Careers;
using Profiling2.Domain.Prf.Responsibility;
using Profiling2.Domain.Prf.Units;

namespace Profiling2.Domain.DTO
{
    /// <summary>
    /// This DTO helps aggregate the units that have ever been present in a Location, using UnitLocation, Career and OrganizationResponsibility data.
    /// </summary>
    public class EntityUnitDateDTO : IAsOfDate, IIncompleteDate
    {
        public Unit Unit { get; set; }

        public int DayOfStart { get; set; }
        public int MonthOfStart { get; set; }
        public int YearOfStart { get; set; }
        public int DayOfEnd { get; set; }
        public int MonthOfEnd { get; set; }
        public int YearOfEnd { get; set; }

        public int DayAsOf { get; set; }
        public int MonthAsOf { get; set; }
        public int YearAsOf { get; set; }

        public string SourceTypeName { get; set; }

        public EntityUnitDateDTO() { }

        public EntityUnitDateDTO(UnitLocation ul)
        {
            if (ul != null)
            {
                this.Unit = ul.Unit;
                this.DayOfStart = ul.DayOfStart;
                this.MonthOfStart = ul.MonthOfStart;
                this.YearOfStart = ul.YearOfStart;
                this.DayOfEnd = ul.DayOfEnd;
                this.MonthOfEnd = ul.MonthOfEnd;
                this.YearOfEnd = ul.YearOfEnd;
                this.DayAsOf = ul.DayAsOf;
                this.MonthAsOf = ul.MonthAsOf;
                this.YearAsOf = ul.YearAsOf;
                this.SourceTypeName = typeof(UnitLocation).Name;
            }
        }

        public EntityUnitDateDTO(Career c)
        {
            if (c != null && c.Unit != null)
            {
                this.Unit = c.Unit;

                this.DayOfStart = c.DayOfStart;
                this.MonthOfStart = c.MonthOfStart;
                this.YearOfStart = c.YearOfStart;
                this.DayOfEnd = c.DayOfEnd;
                this.MonthOfEnd = c.MonthOfEnd;
                this.YearOfEnd = c.YearOfEnd;
                this.DayAsOf = c.DayAsOf;
                this.MonthAsOf = c.MonthAsOf;
                this.YearAsOf = c.YearAsOf;
                this.SourceTypeName = typeof(Career).Name;
            }
        }

        public EntityUnitDateDTO(OrganizationResponsibility or)
        {
            if (or != null && or.Unit != null)
            {
                this.Unit = or.Unit;

                this.DayOfStart = or.Event.DayOfStart;
                this.MonthOfStart = or.Event.MonthOfStart;
                this.YearOfStart = or.Event.YearOfStart;
                this.DayOfEnd = or.Event.DayOfEnd;
                this.MonthOfEnd = or.Event.MonthOfEnd;
                this.YearOfEnd = or.Event.YearOfEnd;
                this.SourceTypeName = typeof(OrganizationResponsibility).Name;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj.GetType() != typeof(EntityUnitDateDTO))
                return false;

            EntityUnitDateDTO other = (EntityUnitDateDTO)obj;
            if (this.Unit.Id == other.Unit.Id)
                if (this.GetStartDateTime() == other.GetStartDateTime() || this.GetStartDateTime() == other.GetAsOfDate() || this.GetAsOfDate() == other.GetStartDateTime())
                    if (this.GetEndDateTime() == other.GetEndDateTime())
                        if (this.SourceTypeName == other.SourceTypeName)
                            return true;

            return false;
        }

        public override int GetHashCode()
        {
            return this.Unit.GetHashCode() 
                + this.SourceTypeName.GetHashCode() 
                + this.GetEndDateTime().GetHashCode() 
                + (this.GetStartDateTime().HasValue ? this.GetStartDateTime().GetHashCode() : this.GetAsOfDate().GetHashCode());
        }
    }
}
