using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201505191643)]
    public class RemoveMimeTypeTable : Migration
    {
        public override void Down()
        {
            
        }

        public override void Up()
        {
            if (Schema.Table("PRF_AdminMimeType").Exists())
                Delete.Table("PRF_AdminMimeType");
        }
    }
}
