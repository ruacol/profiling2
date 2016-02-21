using System.Collections.Generic;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Prf.Sources
{
    /// <summary>
    /// This entity represents an organisation or section within the peacekeeping mission which may have contributed 
    /// sources to the database.  The attribute 'SourcePathPrefix' represents the share drive path where this entity's
    /// files may have been imported from.
    /// 
    /// User accounts may also be linked to this entity.  Together, these attributes, when saved in the Lucene index, 
    /// grant access to sources.  For example a file from the human rights office (JHRO) may be accessible by a user
    /// who is a member of JHRO.
    /// </summary>
    public class SourceOwningEntity : Entity
    {
        public virtual string Name { get; set; }
        public virtual string SourcePathPrefix { get; set; }

        public virtual IList<AdminUser> AdminUsers { get; set; }
        public virtual IList<Source> Sources { get; set; }

        public SourceOwningEntity()
        {
            this.AdminUsers = new List<AdminUser>();
            this.Sources = new List<Source>();
        }
    }
}
