using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201506030922)]
    public class AddEventApprovalTable : Migration
    {
        public override void Down()
        {
            Delete.ForeignKey().FromTable("PRF_EventApproval").ForeignColumn("EventID").ToTable("PRF_Event").PrimaryColumn("EventID");
            Delete.ForeignKey().FromTable("PRF_EventApproval").ForeignColumn("AdminUserID").ToTable("PRF_AdminUser").PrimaryColumn("AdminUserID");
            Delete.Table("PRF_EventApproval");
        }

        public override void Up()
        {
            Create.Table("PRF_EventApproval")
                .WithColumn("EventApprovalID").AsInt32().PrimaryKey().Identity().NotNullable()
                .WithColumn("EventID").AsInt32().NotNullable()
                .WithColumn("AdminUserID").AsInt32().Nullable()
                .WithColumn("ApprovalDateTime").AsDateTime().Nullable()
                .WithColumn("Notes").AsString(int.MaxValue).Nullable();

            Create.ForeignKey()
                .FromTable("PRF_EventApproval").ForeignColumn("EventID")
                .ToTable("PRF_Event").PrimaryColumn("EventID");

            Create.ForeignKey()
                .FromTable("PRF_EventApproval").ForeignColumn("AdminUserID")
                .ToTable("PRF_AdminUser").PrimaryColumn("AdminUserID");

            Execute.Sql(@"
                INSERT INTO PRF_EventApproval (EventID) SELECT EventID FROM PRF_Event;
            ");
        }
    }
}
