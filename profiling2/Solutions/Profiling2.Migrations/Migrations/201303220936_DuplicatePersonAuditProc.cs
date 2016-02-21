using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201303220936)]
    public class DuplicatePersonAuditProc : Migration
    {
        public override void Down()
        {
            //throw new NotImplementedException();
        }

        public override void Up()
        {
            Execute.EmbeddedScript("201303220936_DuplicatePersonAuditProc.sql");
        }
    }
}
