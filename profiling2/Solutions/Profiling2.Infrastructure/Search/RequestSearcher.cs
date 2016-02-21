using System.Collections.Generic;
using System.Linq;
using log4net;
using Lucene.Net.Analysis;
using Lucene.Net.Contrib.Management;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Profiling2.Domain;
using Profiling2.Domain.Contracts.Search;
using Profiling2.Domain.Prf;
using Profiling2.Infrastructure.Search.IndexWriters;

namespace Profiling2.Infrastructure.Search
{
    public class RequestSearcher : LuceneSearcher, IRequestSearcher
    {
        protected ILog log = LogManager.GetLogger(typeof(RequestSearcher));

        public RequestSearcher() { }

        public IList<LuceneSearchResult> Search(string term, int numResults, AdminUser user, string sortField, bool descending)
        {
            using (SearcherManager manager = new SearcherManager(RequestIndexWriterSingleton.Instance))
            {
                this.searcher = manager.Acquire().Searcher;

                Query query = null;

                if (string.IsNullOrEmpty(term))
                {
                    query = new MatchAllDocsQuery();
                }
                else
                {
                    QueryParser parser;
                    if (term.Trim().StartsWith("Id"))
                    {
                        // Single and ranged numeric value search on Id field
                        parser = new NumericQueryParser(Lucene.Net.Util.Version.LUCENE_30, "Id", new KeywordAnalyzer());
                    }
                    else
                    {
                        // General search across text fields
                        parser = new MultiFieldQueryParser(Lucene.Net.Util.Version.LUCENE_30,
                            new string[] { "ReferenceNumber", "RequestName", "RequestEntity", "RequestType", "CurrentStatus" },
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
                        query = parser.Parse(term);
                    }
                    catch (ParseException e)
                    {
                        log.Error("Encountered problem parsing the search term: " + term, e);
                        return new List<LuceneSearchResult>();
                    }
                }

                // when given a user, assumes results must be filtered
                if (user != null && user.GetRequestEntity() != null)
                {
                    BooleanQuery bq = new BooleanQuery();
                    bq.Add(query, Occur.MUST);

                    BooleanQuery bq2 = new BooleanQuery();
                    bq2.Add(new TermQuery(new Term("RequestEntity", user.GetRequestEntity().RequestEntityName)), Occur.SHOULD);
                    bq2.Add(new TermQuery(new Term("CreatorRequestEntity", user.GetRequestEntity().RequestEntityName)), Occur.SHOULD);
                    bq2.Add(new TermQuery(new Term("Creator", user.UserID)), Occur.SHOULD);

                    bq.Add(bq2, Occur.MUST);
                    query = bq;
                }

                log.Debug("Search query: " + query.ToString());

                this.PerformSearch(query, numResults, sortField, descending);
                return TransformTopDocs();
            }
        }

        protected void PerformSearch(Query query, int numResults, string sortField, bool descending)
        {
            if (!string.IsNullOrEmpty(sortField)
                && new string[] { "ReferenceNumberSortable", "RequestName", "RequestEntity", "RequestType", "CurrentStatus", "CurrentStatusDate", "RespondByDate", "Persons", "Id" }.Contains(sortField))
            {
                int sortFieldType = SortField.STRING;
                switch (sortField)
                {
                    case "ReferenceNumberSortable":
                    case "RequestName":
                    case "RequestEntity":
                    case "RequestType":
                    case "CurrentStatus":
                        sortFieldType = SortField.STRING; break;
                    case "Id":
                    case "Persons":
                        sortFieldType = SortField.INT; break;
                    case "CurrentStatusDate":
                    case "RespondByDate":
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
