using System.Collections.Generic;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Prf.Documentation
{
    public class DocumentationFileTag : Entity
    {
        public virtual string Name { get; set; }
        public virtual AdminPermission AdminPermission { get; set; }

        public virtual IList<DocumentationFile> DocumentationFiles { get; set; }

        public DocumentationFileTag()
        {
            this.DocumentationFiles = new List<DocumentationFile>();
        }
    }
}
