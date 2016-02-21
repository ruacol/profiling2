using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    // Add check for whether request persons still have a screening result of 'Pending' when submitting response. 
    [Migration(201306171034)]
    public class FixSPRequestRespond : Migration
    {
        public override void Down()
        {
            
        }

        public override void Up()
        {
            Execute.EmbeddedScript("201306171033_SP_RequestRespond.sql");
        }
    }
}
