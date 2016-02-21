using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201303151649)]
    public class RemoveActionTakenTriggerCounts : Migration
    {
        public override void Down()
        {
            //throw new NotImplementedException();
        }

        public override void Up()
        {
            Execute.EmbeddedScript("201303151649_ActionTakenDeleteTrigger.sql");
            Execute.EmbeddedScript("201303151649_ActionTakenUpdateTrigger.sql");
        }
    }
}
