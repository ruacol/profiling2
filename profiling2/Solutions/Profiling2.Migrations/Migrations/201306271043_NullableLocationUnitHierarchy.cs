using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201306271043)]
    public class NullableLocationUnitHierarchy : Migration
    {
        public override void Down()
        {
            
        }

        public override void Up()
        {
            Alter.Column("LocationID").OnTable("PRF_UnitHierarchy").AsInt32().Nullable();
        }
    }
}
