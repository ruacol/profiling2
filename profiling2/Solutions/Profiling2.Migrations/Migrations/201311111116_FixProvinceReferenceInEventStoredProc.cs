using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201311111116)]
    public class FixProvinceReferenceInEventStoredProc : Migration
    {
        public override void Down()
        {
            
        }

        public override void Up()
        {
            Execute.EmbeddedScript("201311111116_SP_GetEventProfile_EventInformation.sql");
        }
    }
}
