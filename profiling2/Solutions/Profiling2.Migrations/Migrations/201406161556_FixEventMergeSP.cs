using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201406161556)]
    public class FixEventMergeSP : Migration
    {
        public override void Down()
        {
            
        }

        public override void Up()
        {
            Execute.EmbeddedScript("201406161556_SP_EventMerge.sql");
        }
    }
}
