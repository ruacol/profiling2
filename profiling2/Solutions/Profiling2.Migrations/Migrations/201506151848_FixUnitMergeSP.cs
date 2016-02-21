using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201506151848)]
    public class FixUnitMergeSP : Migration
    {
        public override void Down()
        {
            
        }

        public override void Up()
        {
            Execute.EmbeddedScript("201506151848_FixUnitMergeSP.sql");
        }
    }
}
