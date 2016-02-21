using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201308061636)]
    public class AddUnitChangedUnitHierarchyType : Migration
    {
        public override void Down()
        {
            
        }

        public override void Up()
        {
            Execute.Sql("INSERT INTO PRF_UnitHierarchyType (UnitHierarchyTypeName) VALUES ('ChangedNameTo')");
        }
    }
}
