using System.Collections.Generic;
using Lucene.Net.Index;
using Lucene.Net.Search;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Infrastructure.Search
{
    public abstract class BaseIndexer
    {
        protected IndexWriter indexWriter { get; set; }

        public void DeleteIndex()
        {
            if (this.indexWriter != null)
            {
                this.indexWriter.DeleteAll();

                this.indexWriter.Commit();
            }
        }

        protected abstract void AddNoCommit<T>(T obj);

        public void Add<T>(T obj)
        {
            if (obj != null)
            {
                IList<T> list = new List<T>();
                list.Add(obj);
                this.AddMultiple<T>(list);
            }
        }

        public void AddMultiple<T>(IEnumerable<T> multiple)
        {
            if (multiple != null && this.indexWriter != null)
            {
                foreach (T obj in multiple)
                    this.AddNoCommit(obj);

                this.indexWriter.Commit();
            }
        }

        public void Delete<T>(int entityId)
        {
            if (this.indexWriter != null && entityId > 0)
            {
                this.indexWriter.DeleteDocuments(NumericRangeQuery.NewIntRange("Id", entityId, entityId, true, true));

                this.indexWriter.Commit();
            }
        }

        public void Update<T>(T obj)
        {
            if (this.indexWriter != null && obj != null)
            {
                Entity entity = obj as Entity;
                if (entity != null)
                {
                    this.Delete<T>(entity.Id);
                    this.Add<T>(obj);
                }
            }
        }
    }
}
