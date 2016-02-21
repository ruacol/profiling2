using System.Collections.Generic;

namespace Profiling2.Domain.Contracts.Search
{
    /// <summary>
    /// Implementing components must be manually registered in ComponentRegistrar class.
    /// </summary>
    /// <typeparam name="E"></typeparam>
    public interface ILuceneIndexer<E>
    {
        /// <summary>
        /// Add a single entity to Lucene index.  Commits IndexWriter after adding.
        /// </summary>
        /// <param name="entity"></param>
        void Add<T>(T entity);

        /// <summary>
        /// Add multiple entities to Lucene index.  Commits IndexWriter after adding.
        /// </summary>
        /// <param name="multiple"></param>
        void AddMultiple<T>(IEnumerable<T> multiple);

        /// <summary>
        /// Delete entire Lucene index.
        /// </summary>
        void DeleteIndex();

        /// <summary>
        /// Delete single entity from Lucene index.
        /// TODO do we need to catch OutOfMemoryException and close the writer, according to comment in IndexWriter.DeleteDocuments()?
        /// </summary>
        /// <param name="id"></param>
        void Delete<T>(int id);

        /// <summary>
        /// Delete and add an entity to the Lucene index (no true 'update' exists in Lucene).
        /// </summary>
        /// <param name="entity"></param>
        void Update<T>(T entity);
    }
}
