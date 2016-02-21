using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201307081457)]
    public class ModifiedProfilesProc : Migration
    {
        public override void Down()
        {

        }

        public override void Up()
        {
            Execute.EmbeddedScript("201307081457_ModifiedProfilesProc.sql");
        }
    }
}
