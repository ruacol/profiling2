using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201303141502)]
    public class RemoveCareerTriggerCounts : Migration
    {
        public override void Down()
        {
            //throw new NotImplementedException();
        }

        public override void Up()
        {
            Execute.EmbeddedScript("201303141502_CareerInsertTrigger.sql");
            Execute.EmbeddedScript("201303141502_CareerDeleteTrigger.sql");
            Execute.EmbeddedScript("201303141502_CareerUpdateTrigger.sql");
        }
    }
}
