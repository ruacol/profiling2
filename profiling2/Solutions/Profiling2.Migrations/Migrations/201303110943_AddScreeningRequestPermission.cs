using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201303110943)]
    public class AddScreeningRequestPermission : Migration
    {
        public override void Down()
        {
            //throw new NotImplementedException();
        }

        public override void Up()
        {
            Execute.EmbeddedScript("201303110943_SCR_SP_RequestGetForMyArchivedRequests.sql");
            Execute.EmbeddedScript("201303110943_SCR_SP_RequestGetForMyRequests.sql");
        }
    }
}
