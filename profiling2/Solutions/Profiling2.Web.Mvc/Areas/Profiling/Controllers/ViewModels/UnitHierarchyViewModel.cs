using AutoMapper;
using Profiling2.Domain;
using Profiling2.Domain.Extensions;
using Profiling2.Domain.Prf.Units;
using Profiling2.Web.Mvc.Helpers;
using Profiling2.Web.Mvc.Validators;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels
{
    [UnitHierarchyPreventLoop]
    public class UnitHierarchyViewModel : PeriodBaseViewModel, IAsOfDate
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "A child unit is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "A valid child unit is required.")]
        public int UnitId { get; set; }

        [Required(ErrorMessage = "A parent unit is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "A valid parent unit is required.")]
        public int ParentUnitId { get; set; }

        [Required(ErrorMessage = "A hierarchy type is required.")]
        public int UnitHierarchyTypeId { get; set; }

        [Range(0, 31, ErrorMessage = "As Of Day must be between 0 and 31.")]
        public int DayAsOf { get; set; }

        [Range(0, 12, ErrorMessage = "As Of Month must be between 0 and 12.")]
        public int MonthAsOf { get; set; }

        [Range(0, 2030, ErrorMessage = "As Of Year must be between 0 and 2030.")]
        public int YearAsOf { get; set; }

        public string Commentary { get; set; }
        public string Notes { get; set; }

        public IList<UnitHierarchyViewModel> Children { get; set; }

        // for display purposes only
        public string UnitName { get; set; }
        public string ParentUnitName { get; set; }
        public string UnitHierarchyTypeName { get; set; }
        public string AsOfDate { get; set; }
        public int NumCareers { get; set; }
        public IList<string> Commanders { get; set; }
        public IList<string> DeputyCommanders { get; set; }

        public SelectList UnitHierarchyTypeList { get; set; }

        public UnitHierarchyViewModel()
        {
            this.Children = new List<UnitHierarchyViewModel>();
            this.Commanders = new List<string>();
            this.DeputyCommanders = new List<string>();
        }

        public UnitHierarchyViewModel(UnitHierarchy uh)
        {
            if (uh != null)
            {
                this.Children = new List<UnitHierarchyViewModel>();
                this.Commanders = new List<string>();
                this.DeputyCommanders = new List<string>();

                this.Id = uh.Id;
                if (uh.Unit != null)
                {
                    this.UnitId = uh.Unit.Id;
                    this.UnitName = uh.Unit.UnitName;
                    this.NumCareers = uh.Unit.Careers.Count();

                    foreach (UnitHierarchy child in uh.Unit.UnitHierarchyChildren)
                        this.Children.Add(new UnitHierarchyViewModel(child));

                    this.Commanders = uh.Unit.GetCommanders(false).Select(x => x.Person.Name).Distinct().ToList();
                    this.DeputyCommanders = uh.Unit.GetDeputyCommanders(false).Select(x => x.Person.Name).Distinct().ToList();
                }
                if (uh.ParentUnit != null)
                {
                    this.ParentUnitId = uh.ParentUnit.Id;
                    this.ParentUnitName = uh.ParentUnit.UnitName;
                }
                if (uh.UnitHierarchyType != null)
                {
                    this.UnitHierarchyTypeId = uh.UnitHierarchyType.Id;
                    this.UnitHierarchyTypeName = uh.UnitHierarchyType.UnitHierarchyTypeName;
                }

                Mapper.Map<UnitHierarchy, UnitHierarchyViewModel>(uh, this);

                this.AsOfDate = new DateLabel(this.YearAsOf, this.MonthAsOf, this.DayAsOf, false).ToString();
            }
        }

        public void PopulateDropDowns(IEnumerable<UnitHierarchyType> types)
        {
            this.PopulateDropDowns(types, UnitHierarchyType.NAME_HIERARCHY);
        }

        public void PopulateDropDowns(IEnumerable<UnitHierarchyType> types, string defaultTypeName)
        {
            if (this.UnitHierarchyTypeId <= 0)
            {
                this.UnitHierarchyTypeId = (from uht in types
                                            where string.Equals(uht.UnitHierarchyTypeName, defaultTypeName)
                                            select uht).First().Id;
            }
            this.UnitHierarchyTypeList = new SelectList(types, "Id", "UnitHierarchyTypeName", this.UnitHierarchyTypeId);
        }

        public string DateSummary
        {
            get
            {
                if (this.HasStartDate())
                {
                    string date = "From " + this.StartDate;
                    if (this.HasEndDate())
                        date += " until " + this.EndDate;
                    return date;
                }
                else if (this.HasEndDate())
                {
                    if (this.HasAsOfDate())
                        return "As of " + this.AsOfDate + " until " + this.EndDate;
                    else
                        return "Until " + this.EndDate;
                }
                else if (this.HasAsOfDate())
                {
                    return "As of " + this.AsOfDate;
                }
                return string.Empty;
            }
        }
    }
}