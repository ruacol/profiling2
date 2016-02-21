using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201412020906)]
    public class SetNoCountOn : Migration
    {
        public override void Down()
        {

        }

        public override void Up()
        {
            Execute.EmbeddedScript("201412020906_TR_PopulateSourceHashSetNoCountOn.sql");
        }
    }
}
