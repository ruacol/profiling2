using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201512171223)]
    public class AddJhroCaseToEventMergeSP : Migration
    {
        public override void Down()
        {

        }

        public override void Up()
        {
            Execute.EmbeddedScript("201507101641_FixEventMergeSP.sql");
        }
    }
}
