using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    // Creates a copy of PRF_SP_Search_SearchForPerson stored procedure for use with Profiling2:
    // MilitaryID is stripped of certain characters in order to do pure-digit comparison.
    [Migration(201303221727)]
    public class SearchPersonNHibernate : Migration
    {
        public override void Down()
        {

        }

        public override void Up()
        {
            Execute.EmbeddedScript("201303221727_SearchPersonNHibernate.sql");
        }
    }
}
