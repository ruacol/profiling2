using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201506021741)]
    public class AddViewPersonRelationshipsPermission : Migration
    {
        public override void Down()
        {
            Execute.Sql(@"
                DELETE FROM PRF_AdminRolePermission 
                WHERE AdminPermissionID IN 
                (SELECT AdminPermissionID FROM PRF_AdminPermission WHERE Name = 'CanViewPersonRelationships');
            ");
            Delete.FromTable("PRF_AdminPermission").Row(new { Name = "CanViewPersonRelationships" });
        }

        public override void Up()
        {
            Insert.IntoTable("PRF_AdminPermission").Row(new { Name = "CanViewPersonRelationships" });

            Execute.Sql(@"
                INSERT INTO PRF_AdminRolePermission SELECT 1, AdminPermissionID FROM PRF_AdminPermission WHERE Name = 'CanViewPersonRelationships';
            ");
        }
    }
}
