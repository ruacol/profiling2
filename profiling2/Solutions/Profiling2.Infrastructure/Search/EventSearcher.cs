using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using Lucene.Net.Analysis;
using Lucene.Net.Contrib.Management;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Profiling2.Domain;
using Profiling2.Domain.Contracts.Search;
using Profiling2.Infrastructure.Search.IndexWriters;

namespace Profiling2.Infrastructure.Search
{
    public class EventSearcher : LuceneSearcher, IEventSearcher
    {
        protected ILog log = LogManager.GetLogger(typeof(EventSearcher));

        public EventSearcher() { }

        public IList<LuceneSearchResult> Search(string term, int numResults, string sortField, bool descending)
        {
            return this.Search(term, null, null, numResults, sortField, descending);
        }

        public IList<LuceneSearchResult> Search(string term, DateTime? start, DateTime? end, int numResults, string sortField, bool descending)
        {
            using (SearcherManager manager = new SearcherManager(EventIndexWriterSingleton.Instance))
            {
                this.searcher = manager.Acquire().Searcher;

                QueryParser parser;
                if (!string.IsNullOrEmpty(term) && term.Trim().StartsWith("Id"))
                {
                    // Single and ranged numeric value search on Id field
                    parser = new NumericQueryParser(Lucene.Net.Util.Version.LUCENE_30, "Id", new KeywordAnalyzer());
                }
                else
                {
                    // General search across text fields
                    parser = new MultiFieldQueryParser(Lucene.Net.Util.Version.LUCENE_30,
                        new string[] { "Name", "JhroCaseNumber", "Violation", "NarrativeEn", "NarrativeFr", "Location", "Notes", "StartDateDisplay", "EndDateDisplay" },
                        new LowerCaseAnalyzer());

                    parser.DefaultOperator = QueryParser.Operator.AND;

                    if (!string.IsNullOrEmpty(term))
                    {
                        if (!term.Contains(':'))
                        {
                            // Edit user's search string and add wildcards.
                            term = string.Join(" ", term.Split(new string[] { " " }, System.StringSplitOptions.RemoveEmptyEntries)
                                .Select(x => "*" + x + "*")
                                .ToArray()
                            );
                        }
                    }
                }
                parser.AllowLeadingWildcard = true;

                try
                {
                    Query query = null;
                    if (!string.IsNullOrEmpty(term))
                        query = parser.Parse(term);

                    if (start.HasValue || end.HasValue)
                    {
                        long? min = null;
                        if (start.HasValue)
                            min = start.Value.Ticks;

                        long? max = null;
                        if (end.HasValue)
                            max = end.Value.Ticks;

                        BooleanQuery bq = new BooleanQuery();
                        if (query != null)
                            bq.Add(query, Occur.MUST);

                        BooleanQuery bq2 = new BooleanQuery();
                        bq2.Add(NumericRangeQuery.NewLongRange("StartDateSearch", min, max, true, true), Occur.SHOULD);
                        bq2.Add(NumericRangeQuery.NewLongRange("EndDateSearch", min, max, true, true), Occur.SHOULD);
                        bq.Add(bq2, Occur.MUST);

                        query = bq;
                    }

                    log.Debug("Search query: " + query.ToString());

                    this.PerformSearch(query, numResults, sortField, descending);
                    return TransformTopDocs();
                }
                catch (ParseException e)
                {
                    log.Error("Encountered problem parsing the search term: " + term, e);
                    return new List<LuceneSearchResult>();
                }
            }
        }

        protected void PerformSearch(Query query, int numResults, string sortField, bool descending)
        {
            if (!string.IsNullOrEmpty(sortField) 
                && new string[] { "StartDateDisplay", "StartDateSearch", "EndDateDisplay", "EndDateSearch", "Location", "Id", "JhroCaseNumber" }.Contains(sortField))
            {
                int sortFieldType = SortField.STRING;
                switch (sortField)
                {
                    case "StartDateDisplay":
                    case "EndDateDisplay":
                    case "Location":
                    case "JhroCaseNumber":
                        sortFieldType = SortField.STRING; break;
                    case "Id":
                        sortFieldType = SortField.INT; break;
                    case "StartDateSearch":
                    case "EndDateSearch":
                        sortFieldType = SortField.LONG; break;
                }
                Sort sort = new Sort(new SortField(sortField, sortFieldType, descending));
                this.topDocs = this.searcher.Search(query, null, numResults, sort);
            }
            else
            {
                this.topDocs = this.searcher.Search(query, null, numResults);
            }
        }
    }
}
