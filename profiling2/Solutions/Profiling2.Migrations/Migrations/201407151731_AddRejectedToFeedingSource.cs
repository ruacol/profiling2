using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201407151731)]
    public class AddRejectedToFeedingSource : Migration
    {
        public override void Down()
        {
            Delete.ForeignKey().FromTable("PRF_FeedingSource").ForeignColumn("RejectedByID").ToTable("PRF_AdminUser").PrimaryColumn("AdminUserID");
            Delete.Column("RejectedReason").FromTable("PRF_FeedingSource");
            Delete.Column("RejectedDate").FromTable("PRF_FeedingSource");
            Delete.Column("RejectedByID").FromTable("PRF_FeedingSource");
        }

        public override void Up()
        {
            Alter.Table("PRF_FeedingSource")
                .AddColumn("RejectedByID").AsInt32().Nullable()
                .AddColumn("RejectedDate").AsDateTime().Nullable()
                .AddColumn("RejectedReason").AsString(int.MaxValue).Nullable();

            Create.ForeignKey().FromTable("PRF_FeedingSource").ForeignColumn("RejectedByID").ToTable("PRF_AdminUser").PrimaryColumn("AdminUserID");
        }
    }
}
