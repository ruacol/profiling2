using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Prf.Units
{
    public class UnitHierarchyType : Entity
    {
        public const string NAME_HIERARCHY = "Hierarchy";
        public const string NAME_CHANGED_NAME_TO = "ChangedNameTo";

        public virtual string UnitHierarchyTypeName { get; set; }
        public virtual bool Archive { get; set; }
        public virtual string Notes { get; set; }

        public override string ToString()
        {
            return this.UnitHierarchyTypeName;
        }
    }
}
