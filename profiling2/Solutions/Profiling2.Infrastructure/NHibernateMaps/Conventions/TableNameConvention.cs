namespace Profiling2.Infrastructure.NHibernateMaps.Conventions
{
    #region Using Directives

    using FluentNHibernate.Conventions;

    #endregion

    public class TableNameConvention : IClassConvention
    {
        public void Apply(FluentNHibernate.Conventions.Instances.IClassInstance instance)
        {
            //instance.Table(Inflector.Net.Inflector.Pluralize(instance.EntityType.Name));
            //instance.Table("PRF_" + instance.EntityType.Name);

            // e.g. Profiling2.Domain.Scr.Proposed.RequestProposedPerson
            string[] namespaces = instance.EntityType.Namespace.Split('.');

            if (namespaces != null && namespaces.Length > 2)
            {
                // produces 'Scr_RequestProposedPerson'.
                instance.Table(namespaces[2] + "_" + instance.EntityType.Name);
            }
            else
                instance.Table(instance.EntityType.Name);
        }
    }
}