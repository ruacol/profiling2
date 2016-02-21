using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lucene.Net.Index;
using Lucene.Net.Store;

namespace Profiling2.Infrastructure.Search.IndexWriters
{
    /// <summary>
    /// This singleton means the lazy IndexWriter is always open during the life of a web server worker...
    /// </summary>
    public sealed class EventIndexWriterSingleton
    {
        private static Directory directory = null;

        private static readonly Lazy<IndexWriter> lazy = new Lazy<IndexWriter>(() =>
                new IndexWriter(EventIndexWriterSingleton.Directory, new LowerCaseAnalyzer(), IndexWriter.MaxFieldLength.UNLIMITED)
            );

        public static IndexWriter Instance { get { return lazy.Value; } }

        private EventIndexWriterSingleton()
        {
        }

        public static Directory Directory
        {
            get
            {
                if (directory == null)
                {
                    string luceneEventIndexDirectory = System.IO.Path.Combine(ConfigurationManager.AppSettings["LuceneIndexDirectory"], ".\\LuceneEventIndex");
                    System.IO.Directory.CreateDirectory(luceneEventIndexDirectory);

                    directory = FSDirectory.Open(luceneEventIndexDirectory);
                }
                return directory;
            }
        }
    }
}
