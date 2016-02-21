using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201508041613)]
    public class AddRestrictedNotesToPersonMergeSP : Migration
    {
        public override void Down()
        {
            
        }

        public override void Up()
        {
            Execute.EmbeddedScript("201508041613_SP_PersonMerge_NHibernate.sql");
        }
    }
}
