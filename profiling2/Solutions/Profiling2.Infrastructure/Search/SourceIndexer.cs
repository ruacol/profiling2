using System.Collections.Generic;
using log4net;
using Lucene.Net.Documents;
using Profiling2.Domain.Contracts.Search;
using Profiling2.Domain.Prf.Sources;
using Profiling2.Infrastructure.Search.IndexWriters;
using TikaOnDotNet;

namespace Profiling2.Infrastructure.Search
{
    public class SourceIndexer : BaseIndexer, ILuceneIndexer<SourceIndexer>
    {
        protected readonly ILog log = LogManager.GetLogger(typeof(SourceIndexer));

        public SourceIndexer() 
        {
            this.indexWriter = SourceIndexWriterSingleton.Instance;
        }

        protected override void AddNoCommit<T>(T obj)
        {
            Source source = obj as Source;
            if (source != null && !source.Archive && this.indexWriter != null)
            {
                Document doc = new Document();
                doc.Add(new NumericField("Id", Field.Store.YES, true).SetIntValue(source.Id));
                if (!string.IsNullOrEmpty(source.SourceName))
                    doc.Add(new Field("SourceName", source.SourceName, Field.Store.YES, Field.Index.NOT_ANALYZED));
                if (!string.IsNullOrEmpty(source.SourcePath))
                {
                    // SourcePath includes filename, we index only the folder.
                    // Not using Path.GetDirectoryName since it has a char length limitation.
                    string path = source.SourcePath;
                    int i = path.LastIndexOf(System.IO.Path.DirectorySeparatorChar);
                    if (i > -1)
                        path = path.Substring(0, i);
                    doc.Add(new Field("SourcePath", path.ToLower(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                }
                if (source.FileDateTimeStamp.HasValue)
                    doc.Add(new NumericField("FileDateTimeStamp", Field.Store.YES, true).SetLongValue(source.FileDateTimeStamp.Value.Ticks));
                doc.Add(new Field("IsRestricted", source.IsRestricted ? "1" : "0", Field.Store.YES, Field.Index.NOT_ANALYZED_NO_NORMS));
                doc.Add(new Field("IsReadOnly", source.IsReadOnly ? "1" : "0", Field.Store.YES, Field.Index.NOT_ANALYZED_NO_NORMS));
                doc.Add(new Field("IsPublic", source.IsPublic ? "1" : "0", Field.Store.YES, Field.Index.NOT_ANALYZED_NO_NORMS));

                if (!string.IsNullOrEmpty(source.GetUploadedByHorsSession()))
                    doc.Add(new Field("UploadedBy", source.GetUploadedByHorsSession(), Field.Store.YES, Field.Index.NOT_ANALYZED_NO_NORMS));

                // multi-valued field
                IList<string> authors = source.GetAuthorsHorsSession();
                if (authors != null && authors.Count > 0)
                {
                    foreach (string author in authors)
                    {
                        doc.Add(new Field("Author", author, Field.Store.YES, Field.Index.ANALYZED_NO_NORMS));
                    }
                }
                //if (source.SourceAuthors != null && source.SourceAuthors.Any())
                //    foreach (string author in source.SourceAuthors.Select(x => x.Author))
                //        doc.Add(new Field("Author", author, Field.Store.YES, Field.Index.NOT_ANALYZED_NO_NORMS));

                IList<string> owners = source.GetOwnersHorsSession();
                if (owners != null && owners.Count > 0)
                {
                    foreach (string owner in owners)
                    {
                        doc.Add(new Field("Owner", owner, Field.Store.YES, Field.Index.ANALYZED_NO_NORMS));
                    }
                }
                //if (source.SourceOwningEntities != null && source.SourceOwningEntities.Any())
                //    foreach (string owner in source.SourceOwningEntities.Select(x => x.Name))
                //        doc.Add(new Field("Owner", owner, Field.Store.YES, Field.Index.NOT_ANALYZED_NO_NORMS));

                if (!string.IsNullOrEmpty(source.GetJhroCaseNnumberHorsSession()))
                {
                    doc.Add(new Field("JhroCaseNumber", source.GetJhroCaseNnumberHorsSession(), Field.Store.YES, Field.Index.NOT_ANALYZED_NO_NORMS));
                }
                else
                {
                    // trick to select all sources with case number
                    doc.Add(new Field("JhroCaseNumber", "0", Field.Store.YES, Field.Index.NOT_ANALYZED_NO_NORMS));
                }

                doc.Add(new NumericField("FileSize", Field.Store.YES, true).SetLongValue(source.GetFileSizeHorsSession()));

                if (source.FileData != null)
                {
                    TextExtractionResult extractionResult = new TextExtractor().Extract(source.FileData);
                    doc.Add(new Field("FileData", extractionResult.Text, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS));
                    this.indexWriter.AddDocument(doc);
                }
            }
        }
    }
}
