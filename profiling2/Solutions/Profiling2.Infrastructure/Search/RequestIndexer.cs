using System.Linq;
using log4net;
using Lucene.Net.Documents;
using Profiling2.Domain.Contracts.Search;
using Profiling2.Domain.Scr;
using Profiling2.Infrastructure.Search.IndexWriters;

namespace Profiling2.Infrastructure.Search
{
    public class RequestIndexer : BaseIndexer, ILuceneIndexer<RequestIndexer>
    {
        protected readonly ILog log = LogManager.GetLogger(typeof(RequestIndexer));

        public RequestIndexer() 
        {
            this.indexWriter = RequestIndexWriterSingleton.Instance;
        }

        protected override void AddNoCommit<T>(T obj)
        {
            Request request = obj as Request;
            if (request != null && !request.Archive && this.indexWriter != null)
            {
                Document doc = new Document();

                doc.Add(new NumericField("Id", Field.Store.YES, true).SetIntValue(request.Id));

                if (!string.IsNullOrEmpty(request.ReferenceNumber))
                {
                    doc.Add(new Field("ReferenceNumber", request.ReferenceNumber, Field.Store.YES, Field.Index.NOT_ANALYZED));
                    doc.Add(new Field("ReferenceNumberSortable", request.GetSortableReferenceNumber(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                }

                if (!string.IsNullOrEmpty(request.RequestName))
                    doc.Add(new Field("RequestName", request.RequestName, Field.Store.YES, Field.Index.ANALYZED));

                if (request.RequestEntity != null)
                    doc.Add(new Field("RequestEntity", request.RequestEntity.RequestEntityName, Field.Store.YES, Field.Index.ANALYZED));

                if (request.RequestType != null)
                    doc.Add(new Field("RequestType", request.RequestType.RequestTypeName, Field.Store.YES, Field.Index.ANALYZED));

                if (request.CurrentStatus != null)
                    doc.Add(new Field("CurrentStatus", request.CurrentStatus.RequestStatusName, Field.Store.YES, Field.Index.ANALYZED));

                if (request.CurrentStatusDate.HasValue)
                {
                    doc.Add(new NumericField("CurrentStatusDate", Field.Store.YES, true).SetLongValue(request.CurrentStatusDate.Value.Ticks));
                    doc.Add(new Field("CurrentStatusDateDisplay", string.Format("{0:yyyy-MM-dd HH:mm:ss}", request.CurrentStatusDate.Value), Field.Store.YES, Field.Index.NOT_ANALYZED));
                }

                if (request.RespondBy.HasValue)
                {
                    doc.Add(new NumericField("RespondByDate", Field.Store.YES, true).SetLongValue(request.RespondBy.Value.Ticks));
                    doc.Add(new Field("RespondByDateDisplay", string.Format("{0:yyyy-MM-dd HH:mm:ss}", request.RespondBy.Value), Field.Store.YES, Field.Index.NOT_ANALYZED));
                }

                doc.Add(new Field("RespondImmediately", request.RespondImmediately ? "1" : "0", Field.Store.YES, Field.Index.NO));

                // for display - indexed for sorting purposes
                if (request.Persons != null && request.ProposedPersons != null)
                    doc.Add(new NumericField("Persons", Field.Store.YES, true)
                        .SetIntValue(request.Persons.Where(x => !x.Archive).Count() + request.ProposedPersons.Where(x => !x.Archive).Count()));

                if (!string.IsNullOrEmpty(request.Notes))
                    doc.Add(new Field("Notes", request.Notes, Field.Store.YES, Field.Index.NO));

                // for permission checking purposes for initiators
                if (request.Creator != null)
                {
                    doc.Add(new Field("Creator", request.Creator.UserID, Field.Store.YES, Field.Index.NOT_ANALYZED));
                    if (request.Creator.GetRequestEntity() != null)
                        doc.Add(new Field("CreatorRequestEntity", request.Creator.GetRequestEntity().RequestEntityName, Field.Store.YES, Field.Index.NOT_ANALYZED));
                }

                this.indexWriter.AddDocument(doc);
            }
        }
    }
}
