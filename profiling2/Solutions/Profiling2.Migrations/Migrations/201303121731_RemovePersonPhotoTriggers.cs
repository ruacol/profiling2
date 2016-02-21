using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201303121731)]
    public class RemovePersonPhotoTriggers : Migration
    {
        public override void Down()
        {
            //throw new NotImplementedException();
        }

        public override void Up()
        {
            Execute.EmbeddedScript("201303121731_PersonPhotoDeleteTrigger.sql");
            Execute.EmbeddedScript("201303121731_PersonPhotoUpdateTrigger.sql");
        }
    }
}
