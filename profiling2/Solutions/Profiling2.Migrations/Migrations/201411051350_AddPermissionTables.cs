using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201411051350)]
    public class AddPermissionTables : Migration
    {
        public override void Down()
        {
            Delete.Column("Password").FromTable("PRF_AdminUser");
            Delete.Table("PRF_AdminRole");
            Delete.UniqueConstraint("UQ_AdminUserRole").FromTable("PRF_AdminUserRole");
            Delete.Table("PRF_AdminUserRole");
        }

        public override void Up()
        {
            Alter.Table("PRF_AdminUser").AddColumn("Password").AsString().Nullable();

            Create.Table("PRF_AdminRole")
                .WithColumn("AdminRoleID").AsInt32().PrimaryKey().Identity().NotNullable()
                .WithColumn("Name").AsString().NotNullable();

            Create.Table("PRF_AdminUserRole")
                .WithColumn("AdminUserRoleID").AsInt32().PrimaryKey().Identity().NotNullable()
                .WithColumn("AdminUserID").AsInt32().NotNullable()
                .WithColumn("AdminRoleID").AsInt32().NotNullable();
            Create.UniqueConstraint("UQ_AdminUserRole").OnTable("PRF_AdminUserRole").Columns(new string[] { "AdminUserID", "AdminRoleID" });
        }
    }
}
