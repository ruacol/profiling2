using System;
using Profiling2.Domain;

namespace Profiling2.Web.Mvc.Areas.Screening.Controllers.ViewModels
{
    public class RequestDataTableLuceneView
    {
        public string Id { get; set; }
        public string ReferenceNumber { get; set; }
        public string RequestName { get; set; }
        public string RequestEntity { get; set; }
        public string RequestType { get; set; }
        public string RespondBy { get; set; }
        public string CurrentStatus { get; set; }
        public string CurrentStatusDate { get; set; }
        public string Persons { get; set; }
        public string Description { get; set; }

        public RequestDataTableLuceneView(LuceneSearchResult result)
        {
            if (result.FieldValues.ContainsKey("Id"))
                foreach (string s in result.FieldValues["Id"])
                    this.Id = s;

            if (result.FieldValues.ContainsKey("ReferenceNumber"))
                foreach (string s in result.FieldValues["ReferenceNumber"])
                    this.ReferenceNumber = s;

            if (result.FieldValues.ContainsKey("RequestName"))
                foreach (string s in result.FieldValues["RequestName"])
                    this.RequestName = s;

            if (result.FieldValues.ContainsKey("RequestEntity"))
                foreach (string s in result.FieldValues["RequestEntity"])
                    this.RequestEntity = s;

            if (result.FieldValues.ContainsKey("RequestType"))
                foreach (string s in result.FieldValues["RequestType"])
                    this.RequestType = s;

            if (result.FieldValues.ContainsKey("RespondByDateDisplay"))
                foreach (string s in result.FieldValues["RespondByDateDisplay"])
                    this.RespondBy = s;

            if (result.FieldValues.ContainsKey("RespondImmediately"))
                foreach (string s in result.FieldValues["RespondImmediately"])
                    if (string.Equals("1", s))
                        this.RespondBy = "Immediately";

            if (result.FieldValues.ContainsKey("CurrentStatus"))
                foreach (string s in result.FieldValues["CurrentStatus"])
                    this.CurrentStatus = s;

            if (result.FieldValues.ContainsKey("CurrentStatusDateDisplay"))
                foreach (string s in result.FieldValues["CurrentStatusDateDisplay"])
                    this.CurrentStatusDate = s;

            if (result.FieldValues.ContainsKey("Persons"))
                foreach (string s in result.FieldValues["Persons"])
                    this.Persons = s;

            if (result.FieldValues.ContainsKey("Notes"))
                foreach (string s in result.FieldValues["Notes"])
                    this.Description = s;
        }
    }
}