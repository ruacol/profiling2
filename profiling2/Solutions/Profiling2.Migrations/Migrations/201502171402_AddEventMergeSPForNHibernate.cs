using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201502171402)]
    public class AddEventMergeSPForNHibernate : Migration
    {
        public override void Down()
        {

        }

        public override void Up()
        {
            Execute.EmbeddedScript("201502171402_SP_EventMerge_NHibernate.sql");
        }
    }
}
