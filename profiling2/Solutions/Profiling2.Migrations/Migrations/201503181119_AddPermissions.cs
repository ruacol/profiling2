using FluentMigrator;
using System;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201503181119)]
    public class AddPermissions : Migration
    {
        public override void Down()
        {
            Execute.Sql(@"
                DELETE FROM PRF_AdminRolePermission;
                DELETE FROM PRF_AdminPermission;
            ");
        }

        public override void Up()
        {
            Execute.Sql(@"
                INSERT INTO PRF_AdminPermission (Name) VALUES ('CanViewAndSearchPersons');
                INSERT INTO PRF_AdminPermission (Name) VALUES ('CanChangePersons');
                INSERT INTO PRF_AdminPermission (Name) VALUES ('CanChangePersonPublicSummaries');
                INSERT INTO PRF_AdminPermission (Name) VALUES ('CanChangePersonNotes');
                INSERT INTO PRF_AdminPermission (Name) VALUES ('CanViewAndChangePersonRestrictedNotes');
                INSERT INTO PRF_AdminPermission (Name) VALUES ('CanChangePersonBackground');
                INSERT INTO PRF_AdminPermission (Name) VALUES ('CanDeletePersons');
                INSERT INTO PRF_AdminPermission (Name) VALUES ('CanExportPersons');
                INSERT INTO PRF_AdminPermission (Name) VALUES ('CanViewPersonReports');
                INSERT INTO PRF_AdminPermission (Name) VALUES ('CanViewAndSearchRestrictedPersons');
                INSERT INTO PRF_AdminPermission (Name) VALUES ('CanViewPersonResponsibilities');
                INSERT INTO PRF_AdminPermission (Name) VALUES ('CanChangePersonResponsibilities');
                INSERT INTO PRF_AdminPermission (Name) VALUES ('CanViewAndSearchEvents');
                INSERT INTO PRF_AdminPermission (Name) VALUES ('CanChangeEvents');
                INSERT INTO PRF_AdminPermission (Name) VALUES ('CanViewAndSearchUnits');
                INSERT INTO PRF_AdminPermission (Name) VALUES ('CanChangeUnits');
                INSERT INTO PRF_AdminPermission (Name) VALUES ('CanViewAndSearchSources');
                INSERT INTO PRF_AdminPermission (Name) VALUES ('CanUploadSources');
                INSERT INTO PRF_AdminPermission (Name) VALUES ('CanChangeSources');
                INSERT INTO PRF_AdminPermission (Name) VALUES ('CanApproveAndRejectSources');
                INSERT INTO PRF_AdminPermission (Name) VALUES ('CanViewAndSearchRestrictedSources');
                INSERT INTO PRF_AdminPermission (Name) VALUES ('CanPerformScreeningInput');
                INSERT INTO PRF_AdminPermission (Name) VALUES ('CanAdministrate');

                INSERT INTO PRF_AdminRolePermission SELECT 1, AdminPermissionID FROM PRF_AdminPermission WHERE Name = 'CanViewAndSearchPersons';
                INSERT INTO PRF_AdminRolePermission SELECT 1, AdminPermissionID FROM PRF_AdminPermission WHERE Name = 'CanChangePersons';
                INSERT INTO PRF_AdminRolePermission SELECT 1, AdminPermissionID FROM PRF_AdminPermission WHERE Name = 'CanChangePersonPublicSummaries';
                INSERT INTO PRF_AdminRolePermission SELECT 1, AdminPermissionID FROM PRF_AdminPermission WHERE Name = 'CanChangePersonNotes';
                INSERT INTO PRF_AdminRolePermission SELECT 1, AdminPermissionID FROM PRF_AdminPermission WHERE Name = 'CanViewAndChangePersonRestrictedNotes';
                INSERT INTO PRF_AdminRolePermission SELECT 1, AdminPermissionID FROM PRF_AdminPermission WHERE Name = 'CanChangePersonBackground';
                INSERT INTO PRF_AdminRolePermission SELECT 1, AdminPermissionID FROM PRF_AdminPermission WHERE Name = 'CanDeletePersons';
                INSERT INTO PRF_AdminRolePermission SELECT 1, AdminPermissionID FROM PRF_AdminPermission WHERE Name = 'CanExportPersons';
                INSERT INTO PRF_AdminRolePermission SELECT 1, AdminPermissionID FROM PRF_AdminPermission WHERE Name = 'CanViewPersonReports';
                INSERT INTO PRF_AdminRolePermission SELECT 1, AdminPermissionID FROM PRF_AdminPermission WHERE Name = 'CanViewAndSearchRestrictedPersons';
                INSERT INTO PRF_AdminRolePermission SELECT 1, AdminPermissionID FROM PRF_AdminPermission WHERE Name = 'CanViewPersonResponsibilities';
                INSERT INTO PRF_AdminRolePermission SELECT 1, AdminPermissionID FROM PRF_AdminPermission WHERE Name = 'CanChangePersonResponsibilities';
                INSERT INTO PRF_AdminRolePermission SELECT 1, AdminPermissionID FROM PRF_AdminPermission WHERE Name = 'CanViewAndSearchEvents';
                INSERT INTO PRF_AdminRolePermission SELECT 1, AdminPermissionID FROM PRF_AdminPermission WHERE Name = 'CanChangeEvents';
                INSERT INTO PRF_AdminRolePermission SELECT 1, AdminPermissionID FROM PRF_AdminPermission WHERE Name = 'CanViewAndSearchUnits';
                INSERT INTO PRF_AdminRolePermission SELECT 1, AdminPermissionID FROM PRF_AdminPermission WHERE Name = 'CanChangeUnits';
                INSERT INTO PRF_AdminRolePermission SELECT 1, AdminPermissionID FROM PRF_AdminPermission WHERE Name = 'CanViewAndSearchSources';
                INSERT INTO PRF_AdminRolePermission SELECT 1, AdminPermissionID FROM PRF_AdminPermission WHERE Name = 'CanUploadSources';
                INSERT INTO PRF_AdminRolePermission SELECT 1, AdminPermissionID FROM PRF_AdminPermission WHERE Name = 'CanChangeSources';
                INSERT INTO PRF_AdminRolePermission SELECT 1, AdminPermissionID FROM PRF_AdminPermission WHERE Name = 'CanApproveAndRejectSources';
                INSERT INTO PRF_AdminRolePermission SELECT 1, AdminPermissionID FROM PRF_AdminPermission WHERE Name = 'CanViewAndSearchRestrictedSources';
                INSERT INTO PRF_AdminRolePermission SELECT 3, AdminPermissionID FROM PRF_AdminPermission WHERE Name = 'CanAdministrate';
            ");
        }
    }
}
