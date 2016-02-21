using FluentMigrator;

namespace Profiling2.Migrations.Seeds
{
    [Migration(201301231052)]
    public class SCR_seeds : Migration
    {
        public override void Down()
        {

        }

        public override void Up()
        {
            Execute.EmbeddedScript("201301231052_SCR_seeds.sql");
        }
    }
}
