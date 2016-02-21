using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201406061551)]
    public class RemoveProvinceFromEventSearchSP : Migration
    {
        public override void Down()
        {
            
        }

        public override void Up()
        {
            Execute.EmbeddedScript("201406061551_SP_Search_SearchForEvent.sql");
        }
    }
}
