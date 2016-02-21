using System.Collections.Generic;
using Profiling2.Domain.Prf.Documentation;

namespace Profiling2.Domain.Contracts.Tasks
{
    public interface IDocumentationFileTasks
    {
        // file related

        IList<DocumentationFile> GetDocumentationFiles();

        DocumentationFile GetDocumentationFile(int id);

        DocumentationFile SaveDocumentationFile(DocumentationFile file);

        void DeleteDocumentationFile(DocumentationFile file);

        // tag related

        IList<DocumentationFileTag> GetDocumentationFileTags();

        DocumentationFileTag GetDocumentationFileTag(int id);

        DocumentationFileTag SaveDocumentationFileTag(DocumentationFileTag tag);

        void DeleteDocumentationFileTag(DocumentationFileTag tag);
    }
}
