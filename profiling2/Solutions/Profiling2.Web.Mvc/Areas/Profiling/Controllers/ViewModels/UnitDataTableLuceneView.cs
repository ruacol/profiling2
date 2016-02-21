using System.Collections.Generic;
using System.Linq;
using Profiling2.Domain;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels
{
    public class UnitDataTableLuceneView
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public IList<string> Aliases { get; set; }
        public IList<string> ParentNames { get; set; }
        public IList<string> ChildNames { get; set; }
        public string BackgroundInformation { get; set; }
        public string Organization { get; set; }
        public string StartDateDisplay { get; set; }
        public string EndDateDisplay { get; set; }

        public UnitDataTableLuceneView() { }

        public UnitDataTableLuceneView(LuceneSearchResult result)
        {
            if (result.FieldValues.ContainsKey("Id"))
                foreach (string s in result.FieldValues["Id"])
                    this.Id = s;

            if (result.FieldValues.ContainsKey("Name"))
            {
                IList<string> names = result.FieldValues["Name"] as IList<string>;
                if (names != null && names.Any())
                {
                    // this assumes that the first is the original name
                    this.Name = names.First();
                    this.Aliases = names.Skip(1).ToList();
                }
            }

            if (result.FieldValues.ContainsKey("ParentNameChange"))
                this.ParentNames = result.FieldValues["ParentNameChange"] as IList<string>;

            if (result.FieldValues.ContainsKey("ChildNameChange"))
                this.ChildNames = result.FieldValues["ChildNameChange"] as IList<string>;
            
            if (result.FieldValues.ContainsKey("BackgroundInformation"))
                foreach (string s in result.FieldValues["BackgroundInformation"])
                    if (!string.IsNullOrEmpty(s))
                        this.BackgroundInformation = s.Replace("\r\n", "<br />").Replace("\n", "<br />");

            if (result.FieldValues.ContainsKey("Organization"))
                foreach (string s in result.FieldValues["Organization"])
                    this.Organization = s;

            if (result.FieldValues.ContainsKey("StartDateDisplay"))
                foreach (string s in result.FieldValues["StartDateDisplay"])
                    this.StartDateDisplay = s;

            if (result.FieldValues.ContainsKey("EndDateDisplay"))
                foreach (string s in result.FieldValues["EndDateDisplay"])
                    this.EndDateDisplay = s;
        }
    }
}