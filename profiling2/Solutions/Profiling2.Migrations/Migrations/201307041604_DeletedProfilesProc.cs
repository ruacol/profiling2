using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201307041604)]
    public class DeletedProfilesProc : Migration
    {
        public override void Down()
        {

        }

        public override void Up()
        {
            Execute.EmbeddedScript("201307041604_DeletedProfilesProc.sql");
        }
    }
}
