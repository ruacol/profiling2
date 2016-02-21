using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201306261153)]
    public class MoveOrganizationUnitHierarchyProcs : Migration
    {
        public override void Down()
        {

        }

        public override void Up()
        {
            Execute.EmbeddedScript("201306261153_GetUnitHierarchyProc.sql");
            Execute.EmbeddedScript("201306261153_GetUnitHierarchyRecordProc.sql");
            Execute.EmbeddedScript("201306261153_SearchForUnitProc.sql");
        }
    }
}
