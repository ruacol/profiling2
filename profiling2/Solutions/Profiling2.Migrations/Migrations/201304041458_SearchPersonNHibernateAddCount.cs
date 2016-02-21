using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201304041458)]
    public class SearchPersonNHibernateAddCount : Migration
    {
        public override void Down()
        {

        }

        public override void Up()
        {
            Execute.EmbeddedScript("201304041458_SearchPersonNHibernate.sql");
        }
    }
}
