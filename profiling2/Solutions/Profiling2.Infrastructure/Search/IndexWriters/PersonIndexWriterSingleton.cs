using System;
using System.Configuration;
using Lucene.Net.Index;
using Lucene.Net.Store;

namespace Profiling2.Infrastructure.Search.IndexWriters
{
    /// <summary>
    /// This singleton means the lazy IndexWriter is always open during the life of a web server worker...
    /// </summary>
    public sealed class PersonIndexWriterSingleton
    {
        private static Directory directory = null;

        private static Lazy<IndexWriter> lazy = new Lazy<IndexWriter>(() => 
                new IndexWriter(PersonIndexWriterSingleton.Directory, new PersonAnalyzer(), IndexWriter.MaxFieldLength.UNLIMITED)
            );
    
        public static IndexWriter Instance { get { return lazy.Value; } }

        private PersonIndexWriterSingleton() { }

        public static Directory Directory
        {
            get
            {
                if (directory == null)
                {
                    string lucenePersonIndexDirectory = System.IO.Path.Combine(ConfigurationManager.AppSettings["LuceneIndexDirectory"], ".\\LucenePersonIndex");
                    System.IO.Directory.CreateDirectory(lucenePersonIndexDirectory);
                    
                    directory = FSDirectory.Open(lucenePersonIndexDirectory);
                }
                return directory;
            }
        }
    }
}
