using System;
using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201503251533)]
    public class AddAllSourcePermission : Migration
    {
        public override void Down()
        {
            
        }

        public override void Up()
        {
            Execute.Sql(@"
                INSERT INTO PRF_AdminPermission (Name) VALUES ('CanViewAndSearchAllSources');

                INSERT INTO PRF_AdminRolePermission SELECT 1, AdminPermissionID FROM PRF_AdminPermission WHERE Name = 'CanViewAndSearchAllSources';
            ");
        }
    }
}
