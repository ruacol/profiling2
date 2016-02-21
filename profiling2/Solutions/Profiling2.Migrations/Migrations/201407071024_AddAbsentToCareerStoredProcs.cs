using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201407071024)]
    public class AddAbsentToCareerStoredProcs : Migration
    {
        public override void Down()
        {
            
        }

        public override void Up()
        {
            Execute.EmbeddedScript("201407071024_SP_UpdatePersonCareerInformation.sql");
            Execute.EmbeddedScript("201407071024_SP_UpdatePersonCurrentPosition.sql");
        }
    }
}
