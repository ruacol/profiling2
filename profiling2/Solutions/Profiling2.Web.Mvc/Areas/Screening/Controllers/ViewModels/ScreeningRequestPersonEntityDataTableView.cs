using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Profiling2.Domain.Scr.PersonEntity;

namespace Profiling2.Web.Mvc.Areas.Screening.Controllers.ViewModels
{
    public class ScreeningRequestPersonEntityDataTableView
    {
        public DateTime? ResponseDate { get; set; }
        public int PersonId { get; set; }
        public string PersonName { get; set; }
        public string PersonIDNumber { get; set; }
        public string ScreeningResultName { get; set; }
        public string Reason { get; set; }
        public string Commentary { get; set; }
        public int RequestId { get; set; }
        public string RequestHeadline { get; set; }
        public DateTime LastModified { get; set; }
        public string User { get; set; }

        public ScreeningRequestPersonEntityDataTableView() { }

        public ScreeningRequestPersonEntityDataTableView(ScreeningRequestPersonEntity srpe)
        {
            if (srpe != null)
            {
                if (srpe.RequestPerson != null)
                {
                    if (srpe.RequestPerson.Person != null)
                    {
                        this.PersonId = srpe.RequestPerson.Person.Id;
                        this.PersonName = srpe.RequestPerson.Person.Name;
                        this.PersonIDNumber = srpe.RequestPerson.Person.MilitaryIDNumber;
                    }
                    if (srpe.RequestPerson.Request != null)
                    {
                        this.RequestId = srpe.RequestPerson.Request.Id;
                        this.RequestHeadline = srpe.RequestPerson.Request.Headline;
                        if (srpe.ScreeningEntity != null)
                            this.ResponseDate = srpe.RequestPerson.Request.ResponseDate(srpe.ScreeningEntity.ScreeningEntityName);
                    }
                }
                if (srpe.ScreeningResult != null)
                    this.ScreeningResultName = srpe.ScreeningResult.ScreeningResultName;
                this.Reason = srpe.Reason;
                this.Commentary = srpe.Commentary;
                if (srpe.MostRecentHistory != null)
                {
                    this.LastModified = srpe.MostRecentHistory.DateStatusReached;
                    this.User = srpe.MostRecentHistory.AdminUser.Headline;
                }
            }
        }
    }
}