using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201303110959)]
    public class FilterRequestEntities : Migration
    {
        public override void Down()
        {
            //throw new NotImplementedException();
        }

        public override void Up()
        {
            Execute.EmbeddedScript("201303110959_FilterRequestEntities.sql");
        }
    }
}
