using Lucene.Net.Documents;
using System.Collections.Generic;

namespace Profiling2.Domain
{
    /// <summary>
    /// DTO combining Lucene search results into consumable format for the front end.
    /// </summary>
    public class LuceneSearchResult
    {
        /// <summary>
        /// Raw Lucene Document.
        /// </summary>
        protected Document Document { get; set; }

        /// <summary>
        /// Score for this Lucene Document as returned by search results.
        /// </summary>
        public float Score { get; set; }

        /// <summary>
        /// Convenience dictionary of fields returned in Lucene Document.
        /// </summary>
        public IDictionary<string, IList<string>> FieldValues { get; set; }

        /// <summary>
        /// Total number of hits as returned by search results.
        /// </summary>
        public int TotalHits { get; set; }

        /// <summary>
        /// String containing highlighted fragment for this search term (if highlighting was engaged during search).
        /// </summary>
        public string HighlightFragment { get; set; }

        public LuceneSearchResult(Document doc, float score, int totalHits) 
        {
            this.Init(doc, score, totalHits);
        }

        public LuceneSearchResult(Document doc, float score, int totalHits, string highlightFragment)
        {
            this.Init(doc, score, totalHits);
            this.HighlightFragment = highlightFragment;
        }

        protected void Init(Document doc, float score, int totalHits)
        {
            this.Document = doc;
            this.Score = score;
            this.FieldValues = new Dictionary<string, IList<string>>();

            // these Fields come out the order they are put in - but this is Lucene dependent
            foreach (Field field in doc.GetFields())
            {
                if (!this.FieldValues.ContainsKey(field.Name))
                    this.FieldValues[field.Name] = new List<string>();

                this.FieldValues[field.Name].Add(field.StringValue);
            }

            this.TotalHits = totalHits;
        }

        /// <summary>
        /// Return a plain object with the keys 'id' and 'text', passed to the Select2 JS plugin.
        /// </summary>
        /// <returns></returns>
        public object GetPersonIdAndNameForSelect2()
        {
            if (this.FieldValues != null)
            {
                return new
                {
                    id = this.FieldValues.ContainsKey("Id") && ((IList<string>)this.FieldValues["Id"]).Count > 0 ? this.FieldValues["Id"][0] : null,
                    text = this.FieldValues.ContainsKey("Name") && ((IList<string>)this.FieldValues["Name"]).Count > 0 ? this.FieldValues["Name"][0] : null
                };
            }
            return null;
        }

        public int GetPersonId()
        {
            var value = this.FieldValues.ContainsKey("Id") && ((IList<string>)this.FieldValues["Id"]).Count > 0 ? this.FieldValues["Id"][0] : null;
            int id;
            if (int.TryParse(value, out id))
                return id;
            return 0;
        }
    }
}
