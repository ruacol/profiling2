using AutoMapper;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Web.Mvc.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels
{
    public class PersonViewModel
    {
        public int Id { get; set; }

        [RequirePersonName]
        [StringLength(500, ErrorMessage = "Last name must not be longer than 500 characters.")]
        public string LastName { get; set; }

        [RequirePersonName]
        [StringLength(500, ErrorMessage = "First name must not be longer than 500 characters.")]
        public string FirstName { get; set; }

        [Range(0, 31, ErrorMessage = "Birth date day must be between 0 and 31.")]
        public int DayOfBirth { get; set; }

        [Range(0, 12, ErrorMessage = "Birth date month must be between 0 and 12.")]
        public int MonthOfBirth { get; set; }

        [Range(0, 2030, ErrorMessage = "Birth date year must be between 0 and 2030.")]
        public int YearOfBirth { get; set; }

        [StringLength(500, ErrorMessage = "Place of birth must not be longer than 500 characters.")]
        public string BirthVillage { get; set; }

        public int? BirthRegionId { get; set; }

        [StringLength(255, ErrorMessage = "Approximate birth date must not be longer than 255 characters.")]
        [ApproximateBirthDateAttribute]
        public string ApproximateBirthDate { get; set; }

        public int? EthnicityId { get; set; }

        [StringLength(255, ErrorMessage = "Height must not be longer than 255 characters.")]
        public string Height { get; set; }

        [StringLength(255, ErrorMessage = "Weight must not be longer than 255 characters.")]
        public string Weight { get; set; }

        public string BackgroundInformation { get; set; }

        public string PublicSummary { get; set; }

        [StringLength(255, ErrorMessage = "ID number must not be longer than 255 characters.")]
        [IsRecognisedID]  // doesn't allow for incomplete ID numbers
        public string MilitaryIDNumber { get; set; }

        public bool Archive { get; set; }
        public string Notes { get; set; }
        public int ProfileStatusId { get; set; }
        public bool IsRestrictedProfile { get; set; }

        // for display purposes
        public string Name { get; set; }
        public string DateOfBirth { get; set; }
        public string PlaceOfBirth { get; set; }
        public string EthnicityName { get; set; }
        public string ProfileStatusName { get; set; }
        public DateTime ProfileLastModified { get; set; }
        public DateTime? PublicSummaryDate { get; set; }

        public SelectList Regions { get; set; }
        public SelectList Ethnicities { get; set; }
        public SelectList ProfileStatuses { get; set; }

        public void PopulateDropDowns(IEnumerable<Region> regions, IEnumerable<Ethnicity> ethnicities, IEnumerable<ProfileStatus> profileStatuses)
        {
            this.Regions = new SelectList(regions, "Id", "RegionName", this.BirthRegionId);
            this.Ethnicities = new SelectList(ethnicities, "Id", "EthnicityName", this.EthnicityId);
            this.ProfileStatuses = new SelectList(profileStatuses, "Id", "ProfileStatusName", this.ProfileStatusId);
        }

        public PersonViewModel() { }

        public PersonViewModel(Person person)
        {
            Mapper.Map<Person, PersonViewModel>(person, this);
            this.EthnicityName = (person.Ethnicity != null ? person.Ethnicity.ToString() : string.Empty);
            this.ProfileStatusName = person.ProfileStatus.ToString();
        }
    }
}