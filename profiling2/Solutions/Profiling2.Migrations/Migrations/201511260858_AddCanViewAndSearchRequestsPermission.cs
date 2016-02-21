using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201511260858)]
    public class AddCanViewAndSearchRequestsPermission : Migration
    {
        public override void Down()
        {
            Execute.Sql(@"
                DELETE FROM PRF_AdminRolePermission 
                WHERE AdminPermissionID IN (SELECT AdminPermissionID FROM PRF_AdminPermission WHERE Name = 'CanViewAndSearchRequests')

                UPDATE t SET t.AdminPermissionID = p2.AdminPermissionID
                    FROM PRF_DocumentationFileTag t, PRF_AdminPermission p1, PRF_AdminPermission p2
                    WHERE t.AdminPermissionID = p1.AdminPermissionID
                    AND p1.Name = 'CanViewAndSearchRequests'
                    AND p2.Name = 'CanPerformScreeningInput';
            ");
            Delete.FromTable("PRF_AdminPermission").Row(new { Name = "CanViewAndSearchRequests" });
        }

        public override void Up()
        {
            Insert.IntoTable("PRF_AdminPermission").Row(new { Name = "CanViewAndSearchRequests" });

            Execute.Sql(@"
                UPDATE t SET t.AdminPermissionID = p2.AdminPermissionID
                    FROM PRF_DocumentationFileTag t, PRF_AdminPermission p1, PRF_AdminPermission p2
                    WHERE t.AdminPermissionID = p1.AdminPermissionID
                    AND p1.Name = 'CanPerformScreeningInput'
                    AND p2.Name = 'CanViewAndSearchRequests';

                INSERT INTO PRF_AdminRolePermission SELECT 6, AdminPermissionID FROM PRF_AdminPermission WHERE Name = 'CanViewAndSearchRequests';
                INSERT INTO PRF_AdminRolePermission SELECT 7, AdminPermissionID FROM PRF_AdminPermission WHERE Name = 'CanViewAndSearchRequests';
                INSERT INTO PRF_AdminRolePermission SELECT 8, AdminPermissionID FROM PRF_AdminPermission WHERE Name = 'CanViewAndSearchRequests';
                INSERT INTO PRF_AdminRolePermission SELECT 9, AdminPermissionID FROM PRF_AdminPermission WHERE Name = 'CanViewAndSearchRequests';
                INSERT INTO PRF_AdminRolePermission SELECT 10, AdminPermissionID FROM PRF_AdminPermission WHERE Name = 'CanViewAndSearchRequests';
            ");
        }
    }
}
