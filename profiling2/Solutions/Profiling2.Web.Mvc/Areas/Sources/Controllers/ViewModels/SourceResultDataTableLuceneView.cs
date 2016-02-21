using System;
using System.Collections.Generic;
using Profiling2.Domain;

namespace Profiling2.Web.Mvc.Areas.Sources.Controllers.ViewModels
{
    public class SourceResultDataTableLuceneView
    {
        public string Id { get; set; }
        public string SourceName { get; set; }
        public string SourcePath { get; set; }
        public bool IsRestricted { get; set; }
        public bool IsReadOnly { get; set; }
        public bool IsPublic { get; set; }
        public string UploadedBy { get; set; }
        public DateTime? FileDateTimeStamp { get; set; }
        public string HighlightFragment { get; set; }
        public string Author { get; set; }
        public float Score { get; set; }

        public SourceResultDataTableLuceneView() { }

        public SourceResultDataTableLuceneView(LuceneSearchResult result)
        {
            if (result.FieldValues.ContainsKey("Id"))
                foreach (string s in result.FieldValues["Id"])
                    this.Id = s;

            if (result.FieldValues.ContainsKey("SourceName"))
                foreach (string s in result.FieldValues["SourceName"])
                    this.SourceName = s;
            
            if (result.FieldValues.ContainsKey("SourcePath"))
                foreach (string s in result.FieldValues["SourcePath"])
                    this.SourcePath = s;

            if (result.FieldValues.ContainsKey("IsRestricted"))
                foreach (string s in result.FieldValues["IsRestricted"])
                    this.IsRestricted = string.Equals(s, "1");

            if (result.FieldValues.ContainsKey("IsReadOnly"))
                foreach (string s in result.FieldValues["IsReadOnly"])
                    this.IsReadOnly = string.Equals(s, "1");

            if (result.FieldValues.ContainsKey("IsPublic"))
                foreach (string s in result.FieldValues["IsPublic"])
                    this.IsReadOnly = string.Equals(s, "1");

            if (result.FieldValues.ContainsKey("UploadedBy"))
                foreach (string s in result.FieldValues["UploadedBy"])
                    this.UploadedBy = s;

            if (result.FieldValues.ContainsKey("FileDateTimeStamp"))
                foreach (string s in result.FieldValues["FileDateTimeStamp"])
                    this.FileDateTimeStamp = new DateTime(long.Parse(s));

            this.HighlightFragment = result.HighlightFragment;

            if (result.FieldValues.ContainsKey("Author"))
            {
                IList<string> authors = result.FieldValues["Author"] as IList<string>;
                if (authors != null)
                {
                    this.Author = string.Join("<br />", authors);
                }
            }

            this.Score = result.Score;
        }
    }
}