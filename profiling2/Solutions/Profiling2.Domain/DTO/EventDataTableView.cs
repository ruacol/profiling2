using System.Collections.Generic;

namespace Profiling2.Domain.Prf.DTO
{
    public class EventDataTableView
    {
        public int Id { get; set; }
        public string JhroCaseNumber { get; set; }
        public string Violations { get; set; }
        public string StartDateDisplay { get; set; }
        public string EndDateDisplay { get; set; }
        public string Location { get; set; }

        public EventDataTableView() { }

        public EventDataTableView(LuceneSearchResult result)
        {
            if (result != null)
            {
                if (result.FieldValues.ContainsKey("Id"))
                {
                    foreach (string s in result.FieldValues["Id"])
                    {
                        int i;
                        if (int.TryParse(s, out i))
                            this.Id = i;
                    }
                }

                if (result.FieldValues.ContainsKey("JhroCaseNumber"))
                {
                    IList<string> codes = result.FieldValues["JhroCaseNumber"] as IList<string>;
                    if (codes != null)
                        this.JhroCaseNumber = string.Join("; ", codes);
                }

                if (result.FieldValues.ContainsKey("Violation"))
                {
                    IList<string> violations = result.FieldValues["Violation"] as IList<string>;
                    if (violations != null)
                        this.Violations = string.Join("<br />", violations);
                }

                if (result.FieldValues.ContainsKey("StartDateDisplay"))
                    foreach (string s in result.FieldValues["StartDateDisplay"])
                        this.StartDateDisplay = s;

                if (result.FieldValues.ContainsKey("EndDateDisplay"))
                    foreach (string s in result.FieldValues["EndDateDisplay"])
                        this.EndDateDisplay = s;

                if (result.FieldValues.ContainsKey("Location"))
                    foreach (string s in result.FieldValues["Location"])
                        this.Location = s;
            }
        }
    }
}