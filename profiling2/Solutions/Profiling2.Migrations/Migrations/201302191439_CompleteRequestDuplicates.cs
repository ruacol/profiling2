using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201302191439)]
    public class CompleteRequestDuplicates : Migration
    {
        public override void Down()
        {
            //throw new NotImplementedException();
        }

        public override void Up()
        {
            Execute.EmbeddedScript("201302191439_CompleteRequestDuplicates.sql");
        }
    }
}
