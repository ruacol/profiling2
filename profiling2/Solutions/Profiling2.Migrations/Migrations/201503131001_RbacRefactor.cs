using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201503131001)]
    public class RbacRefactor : Migration
    {
        public override void Down()
        {
            Delete.ForeignKey().FromTable("PRF_AdminRolePermission").ForeignColumn("AdminRoleID").ToTable("PRF_AdminRole").PrimaryColumn("AdminRoleID");
            Delete.ForeignKey().FromTable("PRF_AdminRolePermission").ForeignColumn("AdminPermissionID").ToTable("PRF_AdminPermission").PrimaryColumn("AdminPermissionID");
            Delete.UniqueConstraint("UQ_AdminRolePermission").FromTable("PRF_AdminRolePermission");
            Delete.Table("PRF_AdminRolePermission");
            Delete.Table("PRF_AdminPermission");
        }

        public override void Up()
        {
            Create.Table("PRF_AdminPermission")
                .WithColumn("AdminPermissionID").AsInt32().PrimaryKey().Identity().NotNullable()
                .WithColumn("Name").AsString().NotNullable();

            Create.Table("PRF_AdminRolePermission")
                .WithColumn("AdminRolePermissionID").AsInt32().PrimaryKey().Identity().NotNullable()
                .WithColumn("AdminRoleID").AsInt32().NotNullable()
                .WithColumn("AdminPermissionID").AsInt32().NotNullable();
            Create.UniqueConstraint("UQ_AdminRolePermission").OnTable("PRF_AdminRolePermission").Columns(new string[] { "AdminRoleID", "AdminPermissionID" });
            Create.ForeignKey().FromTable("PRF_AdminRolePermission").ForeignColumn("AdminRoleID").ToTable("PRF_AdminRole").PrimaryColumn("AdminRoleID");
            Create.ForeignKey().FromTable("PRF_AdminRolePermission").ForeignColumn("AdminPermissionID").ToTable("PRF_AdminPermission").PrimaryColumn("AdminPermissionID");
        }
    }
}
