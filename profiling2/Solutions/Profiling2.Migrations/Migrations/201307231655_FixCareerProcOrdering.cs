using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201307231655)]
    public class FixCareerProcOrdering : Migration
    {
        public override void Down()
        {
            
        }

        public override void Up()
        {
            Execute.EmbeddedScript("201305151739_GetPersonProfile_CareerInformation.sql");
            Execute.EmbeddedScript("201305151740_GetPersonProfile_CurrentPosition.sql");
        }
    }
}
