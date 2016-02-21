using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;

namespace Profiling2.Infrastructure.NHibernateMaps.Conventions
{
    public class BinaryColumnLengthConvention : IPropertyConvention, IPropertyConventionAcceptance
    {
        public void Accept(IAcceptanceCriteria<IPropertyInspector> criteria)
        {
            criteria.Expect(x => x.Property.PropertyType == typeof(byte[]));
        }

        public void Apply(IPropertyInstance instance)
        {
            instance.Length(2147483647);
            instance.CustomSqlType("varbinary(MAX)");
        }
    }
}
