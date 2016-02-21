using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Prf.Sources
{
    /// <summary>
    /// Links two Sources.  Mainly useful for tracking JHRO cases exported from LotusNotes in a hierarchical tree structure.
    /// </summary>
    public class SourceRelationship : Entity
    {
        public virtual string ParentSourcePath { get; set; }
        public virtual Source ParentSource { get; set; }
        public virtual Source ChildSource { get; set; }

        public SourceRelationship() { }
    }
}
