using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using Lucene.Net.Contrib.Management;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Profiling2.Domain;
using Profiling2.Domain.Contracts.Search;
using Profiling2.Infrastructure.Search.IndexWriters;

namespace Profiling2.Infrastructure.Search
{
    public class PersonSearcher : LuceneSearcher, IPersonSearcher
    {
        protected ILog log = LogManager.GetLogger(typeof(PersonSearcher));

        public PersonSearcher() { }

        // See PersonIndexer.cs for details on how fields were originally indexed.
        public IList<LuceneSearchResult> Search(string term, int numResults, bool includeRestrictedProfiles)
        {
            using (SearcherManager manager = new SearcherManager(PersonIndexWriterSingleton.Instance))
            {
                this.searcher = manager.Acquire().Searcher;

                try
                {
                    Query query = CreateQueryFromTerm(term);
                    if (query == null)
                        return new List<LuceneSearchResult>();

                    // filter on Person.IsRestrictedProfile since restricted profiles are not viewable in screening
                    if (!includeRestrictedProfiles)
                    {
                        BooleanQuery booleanQuery = new BooleanQuery();
                        booleanQuery.Add(query, Occur.MUST);
                        booleanQuery.Add(new TermQuery(new Term("IsRestrictedProfile", "0")), Occur.MUST);
                        query = booleanQuery;
                    }

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

        protected Query CreateQueryFromTerm(string term)
        {
            Query query = null;

            if (!string.IsNullOrEmpty(term))
            {
                // Single and ranged numeric value search on Id field
                if (term.Trim().StartsWith("Id"))
                {
                    QueryParser parser = new NumericQueryParser(Lucene.Net.Util.Version.LUCENE_30, "Id", new PersonAnalyzer());
                    query = parser.Parse(term);
                }
                else if (term.Trim().StartsWith("IsRestrictedProfile"))
                {
                    QueryParser parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, "IsRestrictedProfile", new PersonAnalyzer());
                    query = parser.Parse(term);
                }
                else
                {
                    BooleanQuery topQuery = new BooleanQuery();

                    // manually construct a wildcard query for each search term for more results
                    foreach (string termToken in term.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        BooleanQuery termTokenQuery = new BooleanQuery();
                        foreach (string field in new string[] { "Name", "MilitaryIDNumber", "Rank", "Function" })
                        {
                            string txt = termToken.ToLower();

                            // perform filtering of non-alphanumeric chars for MilitaryIDNumber field (this would also
                            // be done via PersonAnalyzer, if we were using a QueryParser to construct the Query).
                            if (string.Equals("MilitaryIDNumber", field))
                            {
                                txt = new string(txt.ToCharArray()
                                    .Where(x => char.IsLetterOrDigit(x))
                                    .ToArray());
                            }

                            // default similarity of 0.5 = max edit distance of 2
                            if (!string.Equals("MilitaryIDNumber", field))
                            {
                                termTokenQuery.Add(new FuzzyQuery(new Term(field, txt)), Occur.SHOULD);
                            }

                            txt = "*" + txt + "*";

                            termTokenQuery.Add(new WildcardQuery(new Term(field, txt)), Occur.SHOULD);
                        }

                        topQuery.Add(termTokenQuery, Occur.SHOULD);
                    }

                    query = topQuery;
                }

                log.Debug("Search query: " + query.ToString());
            }

            return query;
        }
    }
}
