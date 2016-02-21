using System;
using Profiling2.Domain;

namespace Profiling2.Web.Mvc.Areas.Sources.Controllers.ViewModels
{
    public class SourcePathDataTableLuceneView
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public bool IsRestricted { get; set; }
        public DateTime? FileDateTimeStamp { get; set; }
        public long FileSize { get; set; }
        public string JhroCaseNumber { get; set; }

        public SourcePathDataTableLuceneView() { }

        public SourcePathDataTableLuceneView(LuceneSearchResult result)
        {
            if (result.FieldValues.ContainsKey("Id"))
                foreach (string s in result.FieldValues["Id"])
                    this.Id = s;

            if (result.FieldValues.ContainsKey("SourceName"))
                foreach (string s in result.FieldValues["SourceName"])
                    this.Name = s;
            
            if (result.FieldValues.ContainsKey("SourcePath"))
                foreach (string s in result.FieldValues["SourcePath"])
                    this.Path = s;

            if (result.FieldValues.ContainsKey("IsRestricted"))
                foreach (string s in result.FieldValues["IsRestricted"])
                    this.IsRestricted = string.Equals(s, "1");

            if (result.FieldValues.ContainsKey("FileDateTimeStamp"))
                foreach (string s in result.FieldValues["FileDateTimeStamp"])
                    this.FileDateTimeStamp = new DateTime(long.Parse(s));

            if (result.FieldValues.ContainsKey("FileSize"))
                foreach (string s in result.FieldValues["FileSize"])
                    this.FileSize = long.Parse(s);

            if (result.FieldValues.ContainsKey("JhroCaseNumber"))
                foreach (string s in result.FieldValues["JhroCaseNumber"])
                    this.JhroCaseNumber = s;
        }
    }
}