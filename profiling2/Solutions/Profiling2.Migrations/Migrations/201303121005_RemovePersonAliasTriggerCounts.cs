using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201303121005)]
    public class RemovePersonAliasTriggerCounts : Migration
    {
        public override void Down()
        {
            //throw new NotImplementedException();
        }

        public override void Up()
        {
            Execute.EmbeddedScript("201303121005_PersonAliasDeleteTrigger.sql");
            Execute.EmbeddedScript("201303121005_PersonAliasUpdateTrigger.sql");
        }
    }
}
