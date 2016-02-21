using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201503271201)]
    public class AddFeedingSourceAttributes : Migration
    {
        public override void Down()
        {
            Delete.Column("IsPublic").FromTable("PRF_FeedingSource");
            Delete.Table("PRF_SourceAuthorFeedingSource");
        }

        public override void Up()
        {
            Alter.Table("PRF_FeedingSource").AddColumn("IsPublic").AsBoolean().WithDefaultValue(false).NotNullable();

            Create.Table("PRF_SourceAuthorFeedingSource")
                .WithColumn("SourceAuthorFeedingSourceID").AsInt32().PrimaryKey().Identity().NotNullable()
                .WithColumn("SourceAuthorID").AsInt32().NotNullable()
                .WithColumn("FeedingSourceID").AsInt32().NotNullable();

            Create.ForeignKey()
                .FromTable("PRF_SourceAuthorFeedingSource").ForeignColumn("SourceAuthorID")
                .ToTable("PRF_SourceAuthor").PrimaryColumn("SourceAuthorID");

            Create.ForeignKey()
                .FromTable("PRF_SourceAuthorFeedingSource").ForeignColumn("FeedingSourceID")
                .ToTable("PRF_FeedingSource").PrimaryColumn("FeedingSourceID");
        }
    }
}
