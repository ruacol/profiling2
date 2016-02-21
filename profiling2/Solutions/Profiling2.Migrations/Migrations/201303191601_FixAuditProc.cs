using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201303191601)]
    public class FixAuditProc : Migration
    {
        public override void Down()
        {
            //throw new NotImplementedException();
        }

        public override void Up()
        {
            Execute.EmbeddedScript("201303191601_FixAuditProc.sql");
        }
    }
}
