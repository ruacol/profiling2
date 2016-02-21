﻿using System;
using System.Configuration;
using Lucene.Net.Index;
using Lucene.Net.Store;

namespace Profiling2.Infrastructure.Search.IndexWriters
{
    /// <summary>
    /// This singleton means the lazy IndexWriter is always open during the life of a web server worker...
    /// </summary>
    public sealed class SourcePathIndexWriterSingleton
    {
        private static Directory directory = null;

        private static readonly Lazy<IndexWriter> lazy = new Lazy<IndexWriter>(() =>
                new IndexWriter(SourcePathIndexWriterSingleton.Directory, new LowerCaseAnalyzer(), IndexWriter.MaxFieldLength.UNLIMITED)
            );

        public static IndexWriter Instance { get { return lazy.Value; } }

        private SourcePathIndexWriterSingleton()
        {
        }

        public static Directory Directory
        {
            get
            {
                if (directory == null)
                {
                    string luceneIndexDirectory = System.IO.Path.Combine(ConfigurationManager.AppSettings["LuceneIndexDirectory"], ".\\LuceneSourcePathIndex");
                    System.IO.Directory.CreateDirectory(luceneIndexDirectory);

                    directory = FSDirectory.Open(luceneIndexDirectory);
                }
                return directory;
            }
        }
    }
}
