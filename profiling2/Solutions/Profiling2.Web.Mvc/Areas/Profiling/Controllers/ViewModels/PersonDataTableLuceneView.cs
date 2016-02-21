using System.Collections.Generic;
using System.Linq;
using Profiling2.Domain;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels
{
    public class PersonDataTableLuceneView
    {
        public string Id { get; set; }
        public IEnumerable<string> Name { get; set; }  // this assumes that the first is the original name
        public IEnumerable<string> MilitaryIDNumber { get; set; }
        public string Rank { get; set; }
        public string Function { get; set; }

        public PersonDataTableLuceneView() { }

        public PersonDataTableLuceneView(LuceneSearchResult result)
        {
            if (result.FieldValues.ContainsKey("Id"))
                foreach (string s in result.FieldValues["Id"])
                    this.Id = s;

            if (result.FieldValues.ContainsKey("Name"))
                this.Name = result.FieldValues["Name"] as IList<string>;
            
            if (result.FieldValues.ContainsKey("MilitaryIDNumber"))
                this.MilitaryIDNumber = result.FieldValues["MilitaryIDNumber"].ToList<string>();
            
            if (result.FieldValues.ContainsKey("Rank"))
                foreach (string s in result.FieldValues["Rank"])
                    this.Rank = s;
    
            if (result.FieldValues.ContainsKey("Function"))
                foreach (string s in result.FieldValues["Function"])
                    this.Function = s;
        }
    }
}