using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201504131506)]
    public class AddFeedingSourceOwnerAttributes : Migration
    {
        public override void Down()
        {
            Delete.ForeignKey()
                .FromTable("PRF_FeedingSourceOwner").ForeignColumn("SourceOwningEntityID")
                .ToTable("PRF_SourceOwningEntity").PrimaryColumn("SourceOwningEntityID");
            Delete.ForeignKey()
                .FromTable("PRF_FeedingSourceOwner").ForeignColumn("FeedingSourceID")
                .ToTable("PRF_FeedingSource").PrimaryColumn("FeedingSourceID");
            Delete.Table("PRF_FeedingSourceOwner");
        }

        public override void Up()
        {
            Create.Table("PRF_FeedingSourceOwner")
                .WithColumn("FeedingSourceOwnerID").AsInt32().PrimaryKey().Identity().NotNullable()
                .WithColumn("SourceOwningEntityID").AsInt32().NotNullable()
                .WithColumn("FeedingSourceID").AsInt32().NotNullable();

            Create.ForeignKey()
                .FromTable("PRF_FeedingSourceOwner").ForeignColumn("SourceOwningEntityID")
                .ToTable("PRF_SourceOwningEntity").PrimaryColumn("SourceOwningEntityID");

            Create.ForeignKey()
                .FromTable("PRF_FeedingSourceOwner").ForeignColumn("FeedingSourceID")
                .ToTable("PRF_FeedingSource").PrimaryColumn("FeedingSourceID");
        }
    }
}
