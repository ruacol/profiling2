using System.Collections.Generic;
using System.Linq;
using log4net;
using Lucene.Net.Contrib.Management;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Profiling2.Domain;
using Profiling2.Domain.Contracts.Search;
using Profiling2.Infrastructure.Search.IndexWriters;

namespace Profiling2.Infrastructure.Search
{
    public class UnitSearcher : LuceneSearcher, IUnitSearcher
    {
        protected ILog log = LogManager.GetLogger(typeof(UnitSearcher));

        public UnitSearcher() { }

        // See UnitIndexer.cs for details on how fields were originally indexed.
        public IList<LuceneSearchResult> Search(string term, int numResults)
        {
            using (SearcherManager manager = new SearcherManager(UnitIndexWriterSingleton.Instance))
            {
                this.searcher = manager.Acquire().Searcher;

                QueryParser parser;
                if (!string.IsNullOrEmpty(term) && term.Trim().StartsWith("Id"))
                {
                    // Single and ranged numeric value search on Id field
                    parser = new NumericQueryParser(Lucene.Net.Util.Version.LUCENE_30, "Id", new PersonAnalyzer());
                }
                else
                {
                    // General search across text fields
                    parser = new MultiFieldQueryParser(Lucene.Net.Util.Version.LUCENE_30,
                        new string[] { "Name", "ParentNameChange", "ChildNameChange", "BackgroundInformation", "Organization" },
                        new PersonAnalyzer());

                    // We maintain OR as default for maximum results
                    parser.DefaultOperator = QueryParser.Operator.OR;

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
                    Query query = parser.Parse(term);
                    log.Debug("Search query: " + query.ToString());

                    this.topDocs = this.searcher.Search(query, numResults);
                    return TransformTopDocs();
                }
                catch (ParseException e)
                {
                    log.Error("Encountered problem parsing the search term: " + term, e);
                    return new List<LuceneSearchResult>();
                }
            }
        }
    }
}
