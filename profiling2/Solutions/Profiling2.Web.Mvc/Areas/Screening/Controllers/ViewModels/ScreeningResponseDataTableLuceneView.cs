using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Profiling2.Domain;

namespace Profiling2.Web.Mvc.Areas.Screening.Controllers.ViewModels
{
    public class ScreeningResponseDataTableLuceneView
    {
        public string PersonId { get; set; }
        public string PersonName { get; set; }
        public string ScreeningEntityName { get; set; }
        public string LastColourCoding { get; set; }
        public string LastScreeningResult { get; set; }
        public DateTime LastScreeningDate { get; set; }
        public string Reason { get; set; }
        public string Commentary { get; set; }

        public ScreeningResponseDataTableLuceneView() { }

        public ScreeningResponseDataTableLuceneView(LuceneSearchResult result)
        {
            if (result.FieldValues.ContainsKey("PersonId"))
                foreach (string s in result.FieldValues["PersonId"])
                    this.PersonId = s;
            
            if (result.FieldValues.ContainsKey("PersonName"))
                foreach (string s in result.FieldValues["PersonName"])
                    this.PersonName = s;

            if (result.FieldValues.ContainsKey("ScreeningEntityName"))
                foreach (string s in result.FieldValues["ScreeningEntityName"])
                    this.ScreeningEntityName = s;

            if (result.FieldValues.ContainsKey("LastColourCoding"))
                foreach (string s in result.FieldValues["LastColourCoding"])
                    this.LastColourCoding = s;

            if (result.FieldValues.ContainsKey("LastScreeningResult"))
                foreach (string s in result.FieldValues["LastScreeningResult"])
                    this.LastScreeningResult = s;

            if (result.FieldValues.ContainsKey("LastScreeningDate"))
                foreach (string s in result.FieldValues["LastScreeningDate"])
                    this.LastScreeningDate = new DateTime(long.Parse(s));

            if (result.FieldValues.ContainsKey("Reason"))
                foreach (string s in result.FieldValues["Reason"])
                    this.Reason = s;

            if (result.FieldValues.ContainsKey("Commentary"))
                foreach (string s in result.FieldValues["Commentary"])
                    this.Commentary = s;
        }
    }
}