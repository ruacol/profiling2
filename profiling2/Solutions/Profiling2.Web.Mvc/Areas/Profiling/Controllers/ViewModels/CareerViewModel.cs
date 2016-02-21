using AutoMapper;
using Profiling2.Domain;
using Profiling2.Domain.Prf.Careers;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Web.Mvc.Helpers;
using System.ComponentModel.DataAnnotations;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels
{
    public class CareerViewModel : PeriodBaseViewModel, IAsOfDate
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "A person is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "A valid person is required.")]
        public int? PersonId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "A valid organization is required.")]
        public int? OrganizationId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "A valid location is required.")]
        public int? LocationId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "A valid rank is required.")]
        public int? RankId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "A valid unit is required.")]
        public int? UnitId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "A valid role is required.")]
        public int? RoleId { get; set; }

        [StringLength(0, ErrorMessage = "Job field is deprecated - please delete and move the data to other standardised fields.")]
        public string Job { get; set; }

        public string Commentary { get; set; }
        public bool Archive { get; set; }
        public string Notes { get; set; }

        [Range(0, 31, ErrorMessage = "As Of Day must be between 0 and 31.")]
        public int DayAsOf { get; set; }

        [Range(0, 12, ErrorMessage = "As Of Month must be between 0 and 12.")]
        public int MonthAsOf { get; set; }

        [Range(0, 2030, ErrorMessage = "As Of Year must be between 0 and 2030.")]
        public int YearAsOf { get; set; }

        public bool IsCurrentCareer { get; set; }
        public bool Defected { get; set; }
        public bool Acting { get; set; }
        public bool Absent { get; set; }
        public bool Nominated { get; set; }

        // for display purposes only
        public string PersonName { get; set; }
        public string OrganizationShortName { get; set; }
        public string OrganizationLongName { get; set; }
        public string LocationName { get; set; }
        public string LocationFullName { get; set; }
        public string RankName { get; set; }
        public string RankNameFr { get; set; }
        public string RankDescription { get; set; }
        public string UnitName { get; set; }
        public string RoleName { get; set; }
        public string AsOfDate { get; set; }

        public string Function { get; set; }
        public string FunctionUnitSummary { get; set; }
        public string RankOrganizationLocationSummary { get; set; }

        public CareerViewModel() { }

        public CareerViewModel(Person p)
        {
            if (p != null)
            {
                this.PersonId = p.Id;
                this.PersonName = p.Name;
            }
        }

        public CareerViewModel(Career c)
        {
            if (c != null)
            {
                this.Id = c.Id;
                this.PersonId = c.Person.Id;
                this.PersonName = c.Person.Name;
                if (c.Organization != null)
                {
                    this.OrganizationId = c.Organization.Id;
                    this.OrganizationShortName = c.Organization.OrgShortName;
                    this.OrganizationLongName = c.Organization.OrgLongName;
                }
                if (c.Location != null)
                {
                    this.LocationId = c.Location.Id;
                    this.LocationName = c.Location.LocationName;
                    this.LocationFullName = c.Location.ToString();
                }
                if (c.Rank != null)
                {
                    this.RankId = c.Rank.Id;
                    this.RankName = c.Rank.RankName;
                    this.RankNameFr = c.Rank.RankNameFr;
                    this.RankDescription = c.Rank.Description;
                }
                if (c.Unit != null)
                {
                    this.UnitId = c.Unit.Id;
                    this.UnitName = c.Unit.UnitName;
                }
                if (c.Role != null)
                {
                    this.RoleId = c.Role.Id;
                    this.RoleName = c.Role.RoleName;
                }

                Mapper.Map<Career, CareerViewModel>(c, this);

                this.AsOfDate = new DateLabel(this.YearAsOf, this.MonthAsOf, this.DayAsOf, false).ToString();
            }
        }
    }
}