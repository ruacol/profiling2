using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201506031015)]
    public class AddCanApproveEventPermission : Migration
    {
        public override void Down()
        {
            Execute.Sql(@"
                DELETE FROM PRF_AdminRolePermission 
                WHERE AdminPermissionID IN 
                (SELECT AdminPermissionID FROM PRF_AdminPermission WHERE Name = 'CanApproveEvents');
            ");
            Delete.FromTable("PRF_AdminPermission").Row(new { Name = "CanApproveEvents" });
        }

        public override void Up()
        {
            Insert.IntoTable("PRF_AdminPermission").Row(new { Name = "CanApproveEvents" });

            Execute.Sql(@"
                INSERT INTO PRF_AdminRolePermission SELECT 1, AdminPermissionID FROM PRF_AdminPermission WHERE Name = 'CanApproveEvents';
            ");
        }
    }
}
