using System.Collections.Generic;
using Lucene.Net.Search;
using Profiling2.Domain;

namespace Profiling2.Infrastructure.Search
{
    /// <summary>
    /// Base class for searchers over various Lucene indexes.
    /// </summary>
    public abstract class LuceneSearcher
    {
        protected IndexSearcher searcher { get; set; }
        protected TopDocs topDocs { get; set; }

        public int TotalHits
        {
            get
            {
                if (this.topDocs != null)
                    return this.topDocs.TotalHits;
                return 0;
            }
        }

        protected IList<LuceneSearchResult> TransformTopDocs()
        {
            if (this.topDocs != null)
            {
                IList<LuceneSearchResult> results = new List<LuceneSearchResult>();
                
                foreach (ScoreDoc scoreDoc in this.topDocs.ScoreDocs)
                    results.Add(new LuceneSearchResult(this.searcher.Doc(scoreDoc.Doc), scoreDoc.Score, this.topDocs.TotalHits));

                return results;
            }
            return null;
        }

    }
}
