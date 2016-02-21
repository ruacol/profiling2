using Profiling2.Domain.Scr.Person;
using Profiling2.Domain.Scr.Proposed;
using System.Collections.Generic;
using System.Linq;

namespace Profiling2.Web.Mvc.Areas.Screening.Controllers.ViewModels
{
    public class RequestPersonDataTableView
    {
        public int PersonId { get; set; }
        public string Name { get; set; }
        public string IdNumber { get; set; }
        public string Notes { get; set; }
        public string Function { get; set; }
        public string CurrentRank { get; set; }
        public string LatestScreeningSupportStatus { get; set; }
        public int RequestPersonId { get; set; }
        public int RequestProposedPersonId { get; set; }

        public RequestPersonDataTableView() { }

        public RequestPersonDataTableView(object obj)
        {
            if (obj != null)
            {
                // We surround text values with span tags as workaround for Mvc.Jquery.Datatables not sending these values when processed by
                // DataTablesResult.Create - DataTables expects an array of certain size.

                if (obj is RequestPerson)
                {
                    RequestPerson rp = (RequestPerson)obj;
                    if (rp.Person != null)
                    {
                        this.PersonId = rp.Person.Id;
                        this.Name = "<span>" + rp.Person.Name + "</span>";
                        this.IdNumber = "<span>" + rp.Person.MilitaryIDNumber + "</span>";
                        this.Notes = "<span>" + rp.Notes + "</span>";
                        if (rp.Person.GetCurrentCareers() != null)
                        {
                            IList<string> careers = rp.Person.GetCurrentCareers().Select(x => x.FunctionUnitSummary).ToList<string>();
                            this.Function = string.Join("<br />", careers);
                        }
                        this.CurrentRank = "<span>" + rp.Person.CurrentRank + "</span>";
                        this.LatestScreeningSupportStatus = "<span>" + rp.Person.LatestScreeningSupportStatus + "</span>";
                        this.RequestPersonId = rp.Id;
                    }
                }
                else if (obj is RequestProposedPerson)
                {
                    RequestProposedPerson rpp = (RequestProposedPerson)obj;
                    this.Name = "<span>" + rpp.PersonName + "</span>";
                    this.IdNumber = "<span>" + rpp.MilitaryIDNumber + "</span>";
                    this.Notes = "<span>" + rpp.Notes + "</span>";
                    this.Function = "<span></span>";
                    this.CurrentRank = "<span></span>";
                    this.LatestScreeningSupportStatus = "<span></span>";
                    this.RequestProposedPersonId = rpp.Id;
                }
            }
        }
    }
}