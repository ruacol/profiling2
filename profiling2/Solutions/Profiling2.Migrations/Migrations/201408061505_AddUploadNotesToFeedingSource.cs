using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201408061505)]
    public class AddUploadNotesToFeedingSource : Migration
    {
        public override void Down()
        {
            Delete.Column("UploadNotes").FromTable("PRF_FeedingSource");
        }

        public override void Up()
        {
            Alter.Table("PRF_FeedingSource").AddColumn("UploadNotes").AsString(int.MaxValue).Nullable();
        }
    }
}
