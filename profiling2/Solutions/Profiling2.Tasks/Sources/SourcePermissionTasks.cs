using System.Collections.Generic;
using System.Linq;
using log4net;
using NHibernate;
using Profiling2.Domain.Contracts.Queries;
using Profiling2.Domain.Contracts.Queries.Search;
using Profiling2.Domain.Contracts.Tasks.Sources;
using Profiling2.Domain.Prf.Sources;
using SharpArch.NHibernate.Contracts.Repositories;

namespace Profiling2.Tasks.Sources
{
    public class SourcePermissionTasks : ISourcePermissionTasks
    {
        protected readonly static ILog log = LogManager.GetLogger(typeof(SourcePermissionTasks));
        protected readonly INHibernateRepository<SourceAuthor> sourceAuthorRepo;
        protected readonly INHibernateRepository<SourceOwningEntity> sourceOwningEntityRepo;
        protected readonly ISourceAuthorSearchQuery sourceAuthorSearchQuery;
        protected readonly ISourceQueries sourceQueries;

        public SourcePermissionTasks(INHibernateRepository<SourceAuthor> sourceAuthorRepo,
            INHibernateRepository<SourceOwningEntity> sourceOwningEntityRepo,
            ISourceAuthorSearchQuery sourceAuthorSearchQuery,
            ISourceQueries sourceQueries)
        {
            this.sourceAuthorRepo = sourceAuthorRepo;
            this.sourceOwningEntityRepo = sourceOwningEntityRepo;
            this.sourceAuthorSearchQuery = sourceAuthorSearchQuery;
            this.sourceQueries = sourceQueries;
        }

        public SourceAuthor GetSourceAuthor(int id)
        {
            return this.sourceAuthorRepo.Get(id);
        }

        public IList<SourceAuthor> GetSourceAuthor(string name)
        {
            IDictionary<string, object> criteria = new Dictionary<string, object>();
            criteria.Add("Author", name);
            return this.sourceAuthorRepo.FindAll(criteria);
        }

        public SourceAuthor SaveSourceAuthor(SourceAuthor author)
        {
            return this.sourceAuthorRepo.SaveOrUpdate(author);
        }

        public IList<SourceAuthor> GetAllSourceAuthors()
        {
            return this.sourceAuthorRepo.GetAll();
        }

        public IList<SourceAuthor> SearchSourceAuthors(string term)
        {
            return this.sourceAuthorSearchQuery.GetSourceAuthorsLike(term);
        }

        public SourceOwningEntity GetSourceOwningEntity(int id)
        {
            return this.sourceOwningEntityRepo.Get(id);
        }

        public IList<SourceOwningEntity> GetSourceOwningEntities(string name)
        {
            IDictionary<string, object> criteria = new Dictionary<string, object>();
            criteria.Add("Name", name);
            return this.sourceOwningEntityRepo.FindAll(criteria);
        }

        public IList<SourceOwningEntity> GetSourceOwningEntities(ISession session, string name)
        {
            return this.sourceQueries.GetSourceOwningEntities(session, name);
        }

        public IList<SourceOwningEntity> GetAllSourceOwningEntities()
        {
            return this.sourceOwningEntityRepo.GetAll();
        }

        public SourceOwningEntity SaveSourceOwningEntity(SourceOwningEntity soe)
        {
            return this.sourceOwningEntityRepo.SaveOrUpdate(soe);
        }

        public void DeleteSourceOwningEntity(SourceOwningEntity soe)
        {
            if (soe != null && !soe.AdminUsers.Any() && !soe.Sources.Any())
                this.sourceOwningEntityRepo.Delete(soe);
        }

        public IList<object> GetSourceOwningEntitiesJson(string term)
        {
            IList<SourceOwningEntity> entities;
            if (string.IsNullOrEmpty(term))
                entities = (from e in this.sourceOwningEntityRepo.GetAll() orderby e.Name select e).ToList<SourceOwningEntity>();
            else
                entities = (from e in this.sourceOwningEntityRepo.GetAll() orderby e.Name where e.Name.ToLower().Contains(term.ToLower()) select e).ToList<SourceOwningEntity>();

            IList<object> list = new List<object>();
            foreach (SourceOwningEntity e in entities)
                list.Add(new { id = e.Id, text = e.Name });
            return list;
        }

        public IList<object> GetSourceOwningEntitiesJson()
        {
            return this.GetSourceOwningEntitiesJson(string.Empty);
        }

        public string GetSourceOwningEntityPrefixPath(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                IList<SourceOwningEntity> list = this.GetSourceOwningEntities(name);
                if (list != null && list.Any())
                {
                    return list.First().SourcePathPrefix;
                }
            }
            return null;
        }
    }
}
