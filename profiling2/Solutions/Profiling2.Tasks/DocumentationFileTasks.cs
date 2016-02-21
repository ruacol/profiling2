using System.Collections.Generic;
using System.Linq;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Prf.Documentation;
using SharpArch.NHibernate.Contracts.Repositories;

namespace Profiling2.Tasks
{
    public class DocumentationFileTasks : IDocumentationFileTasks
    {
        protected readonly INHibernateRepository<DocumentationFile> fileRepo;
        protected readonly INHibernateRepository<DocumentationFileTag> tagRepo;

        public DocumentationFileTasks(INHibernateRepository<DocumentationFile> fileRepo,
            INHibernateRepository<DocumentationFileTag> tagRepo)
        {
            this.fileRepo = fileRepo;
            this.tagRepo = tagRepo;
        }

        // file related

        public IList<DocumentationFile> GetDocumentationFiles()
        {
            return this.fileRepo.GetAll().Where(x => !x.Archive).ToList();
        }

        public DocumentationFile GetDocumentationFile(int id)
        {
            return this.fileRepo.Get(id);
        }

        public DocumentationFile SaveDocumentationFile(DocumentationFile file)
        {
            return this.fileRepo.SaveOrUpdate(file);
        }

        public void DeleteDocumentationFile(DocumentationFile file)
        {
            this.fileRepo.Delete(file);
        }

        // tag related

        public IList<DocumentationFileTag> GetDocumentationFileTags()
        {
            return this.tagRepo.GetAll().ToList();
        }

        public DocumentationFileTag GetDocumentationFileTag(int id)
        {
            return this.tagRepo.Get(id);
        }

        public DocumentationFileTag SaveDocumentationFileTag(DocumentationFileTag tag)
        {
            return this.tagRepo.SaveOrUpdate(tag);
        }

        public void DeleteDocumentationFileTag(DocumentationFileTag tag)
        {
            this.tagRepo.Delete(tag);
        }
    }
}
