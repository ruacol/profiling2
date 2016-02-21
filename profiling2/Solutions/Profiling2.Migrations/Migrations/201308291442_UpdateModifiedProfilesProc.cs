using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201308291442)]
    public class UpdateModifiedProfilesProc : Migration
    {
        public override void Down()
        {

        }

        public override void Up()
        {
            Execute.EmbeddedScript("201308291442_UpdateModifiedProfilesProc.sql");
        }
    }
}
