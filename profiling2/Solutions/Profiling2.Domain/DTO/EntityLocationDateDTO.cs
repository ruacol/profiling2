using System;
using Profiling2.Domain.Extensions;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Careers;
using Profiling2.Domain.Prf.Events;
using Profiling2.Domain.Prf.Units;

namespace Profiling2.Domain.DTO
{
    /// <summary>
    /// This DTO helps aggregate the locations where a unit has been, using UnitLocation, Career and Event data.
    /// </summary>
    public class EntityLocationDateDTO
    {
        public Location Location { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? AsOfDate { get; set; }

        public string StartDateString { get; set; }
        public string EndDateString { get; set; }
        public string AsOfDateString { get; set; }

        public string SourceTypeName { get; set; }

        public EntityLocationDateDTO() { }

        public EntityLocationDateDTO(UnitLocation ul)
        {
            if (ul != null && ul.Location != null)
            {
                this.Location = ul.Location;
                this.SourceTypeName = typeof(UnitLocation).Name;
                if (ul.HasStartDate())
                {
                    this.StartDate = ul.GetStartDateTime();
                    this.StartDateString = ul.GetStartDateString();
                }
                if (ul.HasEndDate())
                {
                    this.EndDate = ul.GetEndDateTime();
                    this.EndDateString = ul.GetEndDateString();
                }
                if (ul.HasAsOfDate())
                {
                    this.AsOfDate = ul.GetAsOfDate();
                    this.AsOfDateString = ul.GetAsOfDateString();
                }
            }
        }

        public EntityLocationDateDTO(Career c)
        {
            if (c != null && c.Location != null)
            {
                this.Location = c.Location;
                this.SourceTypeName = typeof(Career).Name;
                if (c.HasStartDate())
                {
                    this.StartDate = c.GetStartDateTime();
                    this.StartDateString = c.GetStartDateString();
                }
                if (c.HasEndDate())
                {
                    this.EndDate = c.GetEndDateTime();
                    this.EndDateString = c.GetEndDateString();
                }
                if (c.HasAsOfDate())
                {
                    this.AsOfDate = c.GetAsOfDate();
                    this.AsOfDateString = c.GetAsOfDateString();
                }
            }
        }

        public EntityLocationDateDTO(Event e)
        {
            if (e != null && e.Location != null)
            {
                this.Location = e.Location;
                this.SourceTypeName = typeof(Event).Name;
                if (e.HasStartDate())
                {
                    this.StartDate = e.GetStartDateTime();
                    this.StartDateString = e.GetStartDateString();
                }
                if (e.HasEndDate())
                {
                    this.EndDate = e.GetEndDateTime();
                    this.EndDateString = e.GetEndDateString();
                }
            }
        }

        public DateTime GetSortDate()
        {
            if (this.StartDate.HasValue)
                return this.StartDate.Value;
            else if (this.AsOfDate.HasValue)
                return this.AsOfDate.Value;
            else if (this.EndDate.HasValue)
                return this.EndDate.Value;
            return new DateTime();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj.GetType() != typeof(EntityLocationDateDTO))
                return false;

            EntityLocationDateDTO other = (EntityLocationDateDTO)obj;
            if (this.Location.Id == other.Location.Id)
                if (this.StartDate == other.StartDate || this.StartDate == other.AsOfDate || this.AsOfDate == other.StartDate)
                    if (this.EndDate == other.EndDate)
                        if (this.SourceTypeName == other.SourceTypeName)
                            return true;

            return false;
        }

        public override int GetHashCode()
        {
            return this.Location.GetHashCode() 
                + this.SourceTypeName.GetHashCode() 
                + this.EndDate.GetHashCode() 
                + (this.StartDate.HasValue ? this.StartDate.GetHashCode() : this.AsOfDate.GetHashCode());
        }
    }
}
