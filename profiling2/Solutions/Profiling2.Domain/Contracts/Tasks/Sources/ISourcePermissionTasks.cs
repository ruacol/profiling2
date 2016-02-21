using System.Collections.Generic;
using NHibernate;
using Profiling2.Domain.Prf.Sources;

namespace Profiling2.Domain.Contracts.Tasks.Sources
{
    public interface ISourcePermissionTasks
    {
        SourceAuthor GetSourceAuthor(int id);

        IList<SourceAuthor> GetSourceAuthor(string name);

        SourceAuthor SaveSourceAuthor(SourceAuthor author);

        IList<SourceAuthor> GetAllSourceAuthors();

        IList<SourceAuthor> SearchSourceAuthors(string term);

        SourceOwningEntity GetSourceOwningEntity(int id);

        IList<SourceOwningEntity> GetSourceOwningEntities(string name);

        IList<SourceOwningEntity> GetSourceOwningEntities(ISession session, string name);

        IList<SourceOwningEntity> GetAllSourceOwningEntities();

        SourceOwningEntity SaveSourceOwningEntity(SourceOwningEntity soe);

        void DeleteSourceOwningEntity(SourceOwningEntity soe);

        IList<object> GetSourceOwningEntitiesJson(string term);

        IList<object> GetSourceOwningEntitiesJson();

        string GetSourceOwningEntityPrefixPath(string name);
    }
}
