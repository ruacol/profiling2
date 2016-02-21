using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Contrib.Management;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Search.Similar;
using Lucene.Net.Search.Vectorhighlight;
using Profiling2.Domain;
using Profiling2.Domain.Contracts.Search;
using Profiling2.Infrastructure.Search.IndexWriters;

namespace Profiling2.Infrastructure.Search
{
    public class SourceSearcher : LuceneSearcher, ISourceSearcher
    {
        protected ILog log = LogManager.GetLogger(typeof(SourceSearcher));

        public SourceSearcher() { }

        public IList<LuceneSearchResult> GetSourcesLikeThis(int sourceId, int numResults)
        {
            IList<LuceneSearchResult> results = new List<LuceneSearchResult>();

            using (SearcherManager manager = new SearcherManager(SourceIndexWriterSingleton.Instance))
            {
                this.searcher = manager.Acquire().Searcher;

                Query query = NumericRangeQuery.NewIntRange("Id", sourceId, sourceId, true, true);

                this.topDocs = this.searcher.Search(query, null, 1);

                if (this.topDocs != null && this.topDocs.ScoreDocs != null && this.topDocs.ScoreDocs.Length > 0)
                {
                    // run second search using MoreLikeThis query
                    using (IndexReader reader = IndexReader.Open(SourceIndexWriterSingleton.Directory, true))
                    {
                        int maxDoc = reader.MaxDoc;

                        MoreLikeThis mlt = new MoreLikeThis(reader);
                        mlt.SetFieldNames(new string[] { "FileData" });
                        mlt.MinTermFreq = 1;
                        mlt.MinDocFreq = 1;

                        BooleanQuery bq = new BooleanQuery();
                        bq.Add(mlt.Like(this.topDocs.ScoreDocs[0].Doc), Occur.MUST);
                        bq.Add(query, Occur.MUST_NOT);
                        log.Info("More like this query: " + bq.ToString());

                        TopDocs similarDocs = this.searcher.Search(bq, numResults);

                        if (similarDocs.TotalHits > 0)
                            foreach (ScoreDoc scoreDoc in similarDocs.ScoreDocs)
                                results.Add(new LuceneSearchResult(this.searcher.Doc(scoreDoc.Doc), scoreDoc.Score, similarDocs.TotalHits));
                    }
                }
            }

            return results;
        }

        public int GetMaxSourceID()
        {
            using (SearcherManager manager = new SearcherManager(SourceIndexWriterSingleton.Instance))
            {
                this.searcher = manager.Acquire().Searcher;

                Query query = NumericRangeQuery.NewIntRange("Id", 1, int.MaxValue, true, true);

                this.topDocs = this.searcher.Search(query, null, 1, new Sort(new SortField("Id", SortField.INT, true)));

                if (this.topDocs != null && this.topDocs.ScoreDocs != null && this.topDocs.ScoreDocs.Length > 0)
                {
                    ScoreDoc scoreDoc = this.topDocs.ScoreDocs[0];
                    LuceneSearchResult result = new LuceneSearchResult(this.searcher.Doc(scoreDoc.Doc), scoreDoc.Score, this.topDocs.TotalHits);

                    int id = 0;
                    if (result.FieldValues != null && result.FieldValues["Id"] != null && result.FieldValues["Id"].Count > 0
                        && int.TryParse(result.FieldValues["Id"][0], out id))
                        return id;
                }
            }
            return 0;
        }

        public IList<LuceneSearchResult> Search(string term, string prefix, bool usePrefixQuery, DateTime? start, DateTime? end, int numResults, 
            bool canViewAndSearchAll, bool includeRestrictedSources, string uploadedByUserId, IList<string> owners, string sortField, bool descending)
        {
            using (SearcherManager manager = new SearcherManager(SourceIndexWriterSingleton.Instance))
            {
                this.searcher = manager.Acquire().Searcher;
                Query query = this.BuildQuery(term, prefix, usePrefixQuery, start, end, canViewAndSearchAll, includeRestrictedSources, uploadedByUserId, owners);
                this.PerformSearch(query, numResults, sortField, descending);
                return TransformTopDocs(query);
            }
        }

        public IDictionary<IDictionary<string, string>, long> SearchGetFacets(string term, string prefix, bool usePrefixQuery, DateTime? start, DateTime? end, int numResults, 
            bool canViewAndSearchAll, bool includeRestrictedSources, string uploadedByUserId, IList<string> owners)
        {
            IDictionary<IDictionary<string, string>, long> facetCount = new Dictionary<IDictionary<string, string>, long>();

            try
            {
                string[] fieldNames = new string[] { 
                    //"IsRestricted", 
                    //"IsReadOnly", 
                    "UploadedBy" 
                    //"IsPublic" 
                };

                using (SimpleFacetedSearch sfs = new SimpleFacetedSearch(SourceIndexWriterSingleton.Instance.GetReader(), fieldNames))
                {
                    Query query = this.BuildQuery(term, prefix, usePrefixQuery, start, end, canViewAndSearchAll, includeRestrictedSources, uploadedByUserId, owners);
                    SimpleFacetedSearch.Hits hits = sfs.Search(query, numResults);

                    //long totalHits = hits.TotalHitCount;
                    foreach (SimpleFacetedSearch.HitsPerFacet hpg in hits.HitsPerFacet)
                    {
                        // HitsPerFacet.Name can be treated like an array whose values match the fieldNames array we fed into SimpleFacetedSearch() 
                        // constructor above.
                        IDictionary<string, string> key = new Dictionary<string, string>();
                        for (int i = 0; i < hpg.Name.Length; i++)
                        {
                            key.Add(fieldNames[i], hpg.Name[i]);
                        }
                        facetCount.Add(key, hpg.HitCount);
                    }
                }
            }
            catch (NullReferenceException e)
            {
                log.Error("SimpleFacetedSearch constructor fails when source index is empty: could this be the case?", e);
            }

            return facetCount;
        }

        protected Query BuildQuery(string term, string prefix, bool usePrefixQuery, DateTime? start, DateTime? end, 
            bool canViewAndSearchAll, bool includeRestrictedSources, string uploadedByUserId, IList<string> owners)
        {
            //QueryParser parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, "FileData", new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30));
            QueryParser parser = new MultiFieldQueryParser(Lucene.Net.Util.Version.LUCENE_30,
                        new string[] { "FileData", "Author", "Owner" },
                        new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30));

            BooleanQuery bq = new BooleanQuery();

            try
            {
                if (!string.IsNullOrEmpty(term))
                {
                    bq.Add(parser.Parse(term), Occur.MUST);
                }

                if (!string.IsNullOrEmpty(prefix))
                {
                    if (usePrefixQuery)
                    {
                        bq.Add(new PrefixQuery(new Term("SourcePath", prefix.ToLower())), Occur.MUST);
                    }
                    else
                    {
                        bq.Add(new TermQuery(new Term("SourcePath", prefix.ToLower())), Occur.MUST);
                    }
                }

                if (canViewAndSearchAll)
                {
                    if (!includeRestrictedSources)
                    {
                        bq.Add(new TermQuery(new Term("IsRestricted", "0")), Occur.MUST);
                    }
                }
                else
                {
                    // protected restricted sources
                    if (!includeRestrictedSources)
                        bq.Add(new TermQuery(new Term("IsRestricted", "0")), Occur.MUST);

                    BooleanQuery whitelist = new BooleanQuery();

                    // user can access sources they uploaded 
                    whitelist.Add(new TermQuery(new Term("UploadedBy", uploadedByUserId)), Occur.SHOULD);

                    // user can access sources they authored
                    whitelist.Add(new TermQuery(new Term("Author", uploadedByUserId)), Occur.SHOULD);

                    // user can access sources marked public
                    whitelist.Add(new TermQuery(new Term("IsPublic", "1")), Occur.SHOULD);

                    // user can access sources owned by entities they are affiliated with
                    foreach (string entity in owners)
                        whitelist.Add(new TermQuery(new Term("Owner", entity)), Occur.SHOULD);

                    bq.Add(whitelist, Occur.MUST);
                }

                // note sources with null FileDateTimeStamp will be filtered out
                if (start.HasValue || end.HasValue)
                {
                    long? min = null;
                    if (start.HasValue)
                        min = start.Value.Ticks;

                    long? max = null;
                    if (end.HasValue)
                        max = end.Value.Ticks;

                    bq.Add(NumericRangeQuery.NewLongRange("FileDateTimeStamp", min, max, true, true), Occur.MUST);
                }

                log.Debug("Search query: " + bq.ToString());
            }
            catch (ParseException e)
            {
                log.Error("Encountered problem parsing the search term: " + term, e);
            }

            return bq;
        }

        public IList<LuceneSearchResult> AllSourcesWithCaseNumbers(string term, int numResults, bool includeRestrictedSources, string sortField, bool descending)
        {
            using (SearcherManager manager = new SearcherManager(SourceIndexWriterSingleton.Instance))
            {
                this.searcher = manager.Acquire().Searcher;

                BooleanQuery booleanQuery = new BooleanQuery();
                if (string.IsNullOrEmpty(term))
                    booleanQuery.Add(new MatchAllDocsQuery(), Occur.MUST);
                else
                    booleanQuery.Add(new WildcardQuery(new Term("JhroCaseNumber", "*" + term + "*")), Occur.MUST);

                // value '0' set by SourceIndexer
                booleanQuery.Add(new TermQuery(new Term("JhroCaseNumber", "0")), Occur.MUST_NOT);

                if (!includeRestrictedSources)
                {
                    booleanQuery.Add(new TermQuery(new Term("IsRestricted", "0")), Occur.MUST);
                }

                log.Debug("Search query: " + booleanQuery.ToString());

                this.PerformSearch(booleanQuery, numResults, sortField, descending);
                return this.TransformTopDocs();
            }
        }

        protected void PerformSearch(Query query, int numResults, string sortField, bool descending)
        {
            if (!string.IsNullOrEmpty(sortField)
                && new string[] { "FileDateTimeStamp", "SourceName", "SourcePath", "IsRestricted", "JhroCaseNumber", "FileDateTimeStamp", "FileSize", "Id" }.Contains(sortField))
            {
                int sortFieldType = SortField.STRING;
                switch (sortField)
                {
                    case "SourceName":
                    case "SourcePath":
                    case "IsRestricted":
                    case "JhroCaseNumber":
                        sortFieldType = SortField.STRING; break;
                    case "FileDateTimeStamp":
                    case "FileSize":
                    case "Id":
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

        /// <summary>
        /// Runs Lucene Highlighter module over search results and original Lucene Query.
        /// </summary>
        /// <param name="query"></param>
        /// <returns>List of results transformed into LuceneSearchResult DTOs.</returns>
        protected IList<LuceneSearchResult> TransformTopDocs(Query query)
        {
            if (this.topDocs != null)
            {
                IList<LuceneSearchResult> results = new List<LuceneSearchResult>();

                FastVectorHighlighter highlighter = new FastVectorHighlighter(true, true,
                    new SimpleFragListBuilder(), new ScoreOrderFragmentsBuilder(
                        BaseFragmentsBuilder.COLORED_PRE_TAGS,
                        BaseFragmentsBuilder.COLORED_POST_TAGS));
                FieldQuery fieldQuery = highlighter.GetFieldQuery(query);

                foreach (ScoreDoc scoreDoc in this.topDocs.ScoreDocs)
                {
                    string snippet = highlighter.GetBestFragment(fieldQuery, this.searcher.IndexReader, scoreDoc.Doc, "FileData", 200);
                    results.Add(new LuceneSearchResult(this.searcher.Doc(scoreDoc.Doc), scoreDoc.Score, this.topDocs.TotalHits, snippet));
                }

                return results;
            }
            return null;
        }
    }
}
