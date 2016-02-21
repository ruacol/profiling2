using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Domain.Scr;
using System.ComponentModel.DataAnnotations;
using Profiling2.Domain.Prf;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels
{
    public class ActiveScreeningViewModel
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public int? RequestId { get; set; }
        [Required]
        public string DateActivelyScreened { get; set; }
        public int? ScreenedById { get; set; }
        public string Notes { get; set; }

        public SelectList Requests { get; set; }
        public SelectList Users { get; set; }

        // display purposes
        public string PersonName { get; set; }
        public string RequestHeadline { get; set; }
        public string ScreenedByName { get; set; }

        public ActiveScreeningViewModel() { }

        public ActiveScreeningViewModel(Person p)
        {
            if (p != null)
            {
                this.PersonId = p.Id;
                this.PersonName = p.Name;
            }
        }

        public ActiveScreeningViewModel(ActiveScreening a)
        {
            this.Id = a.Id;
            if (a.Person != null)
            {
                this.PersonId = a.Person.Id;
                this.PersonName = a.Person.Name;
            }
            if (a.Request != null)
            {
                this.RequestId = a.Request.Id;
                this.RequestHeadline = a.Request.Headline;
            }
            this.DateActivelyScreened = string.Format("{0:yyyy-MM-dd}", a.DateActivelyScreened);
            if (a.ScreenedBy != null)
            {
                this.ScreenedById = a.ScreenedBy.Id;
                this.ScreenedByName = a.ScreenedBy.ToString();
            }
            this.Notes = a.Notes;
        }

        public void PopulateDropDowns(IEnumerable<Request> requests, IEnumerable<AdminUser> users)
        {
            this.Requests = new SelectList(requests, "Id", "Headline", this.RequestId);
            this.Users = new SelectList(users, "Id", "Headline", this.ScreenedById);
        }
    }
}