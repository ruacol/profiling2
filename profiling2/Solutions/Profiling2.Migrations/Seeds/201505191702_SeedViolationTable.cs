using FluentMigrator;

namespace Profiling2.Migrations.Seeds
{
    [Migration(201505191702)]
    public class SeedViolationTable : Migration
    {
        public override void Down()
        {
            
        }

        public override void Up()
        {
            Execute.EmbeddedScript("PRF_Violation_seed.sql");
        }
    }
}
