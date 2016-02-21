using System;
using System.Configuration;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.Store;

namespace Profiling2.Infrastructure.Search.IndexWriters
{
    /// <summary>
    /// This singleton means the lazy IndexWriter is always open during the life of a web server worker...
    /// </summary>
    public sealed class SourceIndexWriterSingleton
    {
        private static Directory directory = null;

        private static readonly Lazy<IndexWriter> lazy = new Lazy<IndexWriter>(() =>
                new IndexWriter(SourceIndexWriterSingleton.Directory, new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30), IndexWriter.MaxFieldLength.UNLIMITED)
            );

        public static IndexWriter Instance { get { return lazy.Value; } }

        private SourceIndexWriterSingleton()
        {
        }

        public static Directory Directory
        {
            get
            {
                if (directory == null)
                {
                    string luceneIndexDirectory = System.IO.Path.Combine(ConfigurationManager.AppSettings["LuceneIndexDirectory"], ".\\LuceneSourceIndex");
                    System.IO.Directory.CreateDirectory(luceneIndexDirectory);

                    directory = FSDirectory.Open(luceneIndexDirectory);
                }
                return directory;
            }
        }
    }
}
