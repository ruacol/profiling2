using System.Collections.Generic;
using log4net;
using Lucene.Net.Analysis;
using Lucene.Net.Contrib.Management;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Profiling2.Domain;
using Profiling2.Domain.Contracts.Search;
using Profiling2.Infrastructure.Search.IndexWriters;

namespace Profiling2.Infrastructure.Search
{
    public class ScreeningResponseSearcher : LuceneSearcher, IScreeningResponseSearcher
    {
        protected ILog log = LogManager.GetLogger(typeof(ScreeningResponseSearcher));

        public ScreeningResponseSearcher() { }

        public IList<LuceneSearchResult> Search(string term, string screeningEntityName, int numResults)
        {
            using (SearcherManager manager = new SearcherManager(ScreeningResponseIndexWriterSingleton.Instance))
            {
                this.searcher = manager.Acquire().Searcher;

                QueryParser parser = new MultiFieldQueryParser(Lucene.Net.Util.Version.LUCENE_30,
                    new string[] { "Reason", "Commentary" },
                    new SimpleAnalyzer());
                parser.DefaultOperator = QueryParser.Operator.AND;
                parser.AllowLeadingWildcard = true;

                try
                {
                    Query query = parser.Parse(term);

                    BooleanQuery bq = new BooleanQuery();
                    bq.Add(query, Occur.MUST);
                    bq.Add(new TermQuery(new Term("ScreeningEntityName", screeningEntityName)), Occur.MUST);

                    log.Debug("Search query: " + bq.ToString());

                    this.topDocs = this.searcher.Search(bq, numResults);
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
