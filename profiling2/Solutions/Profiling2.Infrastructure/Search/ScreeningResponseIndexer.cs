using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Profiling2.Domain.Contracts.Search;
using Profiling2.Domain.Scr.PersonEntity;
using Profiling2.Infrastructure.Search.IndexWriters;

namespace Profiling2.Infrastructure.Search
{
    public class ScreeningResponseIndexer : BaseIndexer, IScreeningResponseIndexer
    {
        public ScreeningResponseIndexer() 
        {
            this.indexWriter = ScreeningResponseIndexWriterSingleton.Instance;
        }

        protected override void AddNoCommit<T>(T obj)
        {
            ScreeningRequestPersonEntity response = obj as ScreeningRequestPersonEntity;
            if (response != null && !response.Archive && this.indexWriter != null)
            {
                Document doc = new Document();

                if (response.RequestPerson != null && response.RequestPerson.Person != null)
                {
                    doc.Add(new NumericField("PersonId", Field.Store.YES, true).SetIntValue(response.RequestPerson.Person.Id));
                    doc.Add(new Field("PersonName", response.RequestPerson.Person.Name, Field.Store.YES, Field.Index.NO));
                    doc.Add(new Field("LastScreeningResult", response.RequestPerson.Person.LatestScreeningSupportStatus, Field.Store.YES, Field.Index.NO));
                    if (response.RequestPerson.Person.LatestScreeningFinalDecisionDate.HasValue)
                        doc.Add(new NumericField("LastScreeningDate", Field.Store.YES, false)
                            .SetLongValue(response.RequestPerson.Person.LatestScreeningFinalDecisionDate.Value.Ticks));
                }

                if (response.ScreeningResult != null)
                    doc.Add(new Field("LastColourCoding", response.ScreeningResult.ScreeningResultName, Field.Store.YES, Field.Index.NO));

                if (!string.IsNullOrEmpty(response.Reason))
                    doc.Add(new Field("Reason", response.Reason, Field.Store.YES, Field.Index.ANALYZED));

                if (!string.IsNullOrEmpty(response.Commentary))
                    doc.Add(new Field("Commentary", response.Commentary, Field.Store.YES, Field.Index.ANALYZED));

                if (response.ScreeningEntity != null)
                {
                    doc.Add(new Field("ScreeningEntityName", response.ScreeningEntity.ScreeningEntityName, Field.Store.YES, Field.Index.NOT_ANALYZED));
                }

                this.indexWriter.AddDocument(doc);
            }
        }

        public void DeleteResponse(int personId, string screeningEntityName)
        {
            if (this.indexWriter != null && personId > 0)
            {
                BooleanQuery bq = new BooleanQuery();
                bq.Add(NumericRangeQuery.NewIntRange("PersonId", personId, personId, true, true), Occur.MUST);
                bq.Add(new TermQuery(new Term("ScreeningEntityName", screeningEntityName)), Occur.MUST);
                this.indexWriter.DeleteDocuments(bq);
            }
        }

        public void UpdateResponse(ScreeningRequestPersonEntity response)
        {
            if (this.indexWriter != null && response != null)
            {
                this.DeleteResponse(response.RequestPerson.Person.Id, response.ScreeningEntity.ScreeningEntityName);
                this.Add<ScreeningRequestPersonEntity>(response);
            }
        }
    }
}
