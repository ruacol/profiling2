using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    // Creates a copy of PRF_SP_Search_SearchForPersonCount stored procedure for use with NHibernate:
    // NHibernate requires that stored procs return a result set, whereas the existing stored proc
    // returns with a RETURN call, incompatible with NHibernate.
    [Migration(201303201111)]
    public class DuplicateSearchPersonCountProc : Migration
    {
        public override void Down()
        {
            
        }

        public override void Up()
        {
            Execute.EmbeddedScript("201303201111_SearchPersonCountNHibernate.sql");
        }
    }
}
