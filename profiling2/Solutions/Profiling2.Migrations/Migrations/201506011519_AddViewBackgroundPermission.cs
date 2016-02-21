using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201506011519)]
    public class AddViewBackgroundPermission : Migration
    {
        public override void Down()
        {
            Execute.Sql(@"
                DELETE FROM PRF_AdminRolePermission 
                WHERE AdminPermissionID IN 
                (SELECT AdminPermissionID FROM PRF_AdminPermission WHERE Name = 'CanViewBackgroundInformation');
            ");
            Delete.FromTable("PRF_AdminPermission").Row(new { Name = "CanViewBackgroundInformation" });
        }

        public override void Up()
        {
            Insert.IntoTable("PRF_AdminPermission").Row(new { Name = "CanViewBackgroundInformation" });

            Execute.Sql(@"
                INSERT INTO PRF_AdminRolePermission SELECT 1, AdminPermissionID FROM PRF_AdminPermission WHERE Name = 'CanViewBackgroundInformation';
            ");
        }
    }
}
