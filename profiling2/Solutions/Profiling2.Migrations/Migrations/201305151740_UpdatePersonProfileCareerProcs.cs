using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    // modifies person export to be compatible with Career table adjustments
    [Migration(201305151740)]
    public class UpdatePersonProfileCareerProcs : Migration
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
