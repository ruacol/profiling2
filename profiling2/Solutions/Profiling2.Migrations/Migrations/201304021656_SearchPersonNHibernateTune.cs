using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    // Cuts out some deprecated processing in the stored proc
    [Migration(201304021656)]
    public class SearchPersonNHibernateTune : Migration
    {
        public override void Down()
        {

        }

        public override void Up()
        {
            Execute.EmbeddedScript("201304021656_SearchPersonNHibernateTune.sql");
        }
    }
}
