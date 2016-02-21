using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201407081045)]
    public class AddFeedingSource : Migration
    {
        public override void Down()
        {
            Delete.UniqueConstraint("UQ_FeedingSourceName").FromTable("PRF_FeedingSource");
            Delete.ForeignKey().FromTable("PRF_FeedingSource").ForeignColumn("SourceID").ToTable("PRF_Source").PrimaryColumn("SourceID");
            Delete.ForeignKey().FromTable("PRF_FeedingSource").ForeignColumn("UploadedByID").ToTable("PRF_AdminUser").PrimaryColumn("AdminUserID");
            Delete.ForeignKey().FromTable("PRF_FeedingSource").ForeignColumn("ApprovedByID").ToTable("PRF_AdminUser").PrimaryColumn("AdminUserID");
            Delete.Table("PRF_FeedingSource");
        }

        public override void Up()
        {
            Create.Table("PRF_FeedingSource")
                .WithColumn("FeedingSourceID").AsInt32().PrimaryKey().Identity().NotNullable()
                .WithColumn("Name").AsString().NotNullable()
                .WithColumn("Restricted").AsBoolean().WithDefaultValue(false).NotNullable()
                .WithColumn("FileData").AsBinary(int.MaxValue).NotNullable()
                .WithColumn("FileModifiedDateTime").AsDateTime().NotNullable()
                .WithColumn("UploadedByID").AsInt32().NotNullable()
                .WithColumn("UploadDate").AsDateTime().NotNullable()
                .WithColumn("ApprovedByID").AsInt32().Nullable()
                .WithColumn("ApprovedDate").AsDateTime().Nullable()
                .WithColumn("SourceID").AsInt32().Nullable();

            Create.ForeignKey().FromTable("PRF_FeedingSource").ForeignColumn("UploadedByID").ToTable("PRF_AdminUser").PrimaryColumn("AdminUserID");
            Create.ForeignKey().FromTable("PRF_FeedingSource").ForeignColumn("ApprovedByID").ToTable("PRF_AdminUser").PrimaryColumn("AdminUserID");
            Create.ForeignKey().FromTable("PRF_FeedingSource").ForeignColumn("SourceID").ToTable("PRF_Source").PrimaryColumn("SourceID");
            Create.UniqueConstraint("UQ_FeedingSourceName").OnTable("PRF_FeedingSource").Column("Name");
        }
    }
}
