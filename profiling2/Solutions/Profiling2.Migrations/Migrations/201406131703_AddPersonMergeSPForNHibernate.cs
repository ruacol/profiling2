using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201406131703)]
    public class AddPersonMergeSPForNHibernate : Migration
    {
        public override void Down()
        {
            
        }

        public override void Up()
        {
            Execute.EmbeddedScript("201406131703_SP_PersonMerge_NHibernate.sql");
        }
    }
}
