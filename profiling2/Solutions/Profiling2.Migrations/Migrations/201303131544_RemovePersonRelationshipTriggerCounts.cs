using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201303131544)]
    public class RemovePersonRelationshipTriggerCounts : Migration
    {
        public override void Down()
        {
            //throw new NotImplementedException();
        }

        public override void Up()
        {
            Execute.EmbeddedScript("201303131544_PersonRelationshipDeleteTrigger.sql");
            Execute.EmbeddedScript("201303131544_PersonRelationshipUpdateTrigger.sql");
        }
    }
}
