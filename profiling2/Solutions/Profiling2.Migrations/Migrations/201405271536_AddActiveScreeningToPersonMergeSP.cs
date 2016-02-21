using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201405271536)]
    public class AddActiveScreeningToPersonMergeSP : Migration
    {
        public override void Down()
        {
            
        }

        public override void Up()
        {
            Execute.EmbeddedScript("201405271536_SP_PersonMerge.sql");
        }
    }
}
