using System;
using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201511241118)]
    public class AddDocumentationFileEntities : Migration
    {
        public override void Down()
        {
            if (Schema.Table("PRF_DocumentationFile").Exists())
            {
                Delete.ForeignKey().FromTable("PRF_DocumentationFile").ForeignColumn("UploadedByID")
                    .ToTable("PRF_AdminUser").PrimaryColumn("AdminUserID");
                Delete.ForeignKey().FromTable("PRF_DocumentationFile").ForeignColumn("DocumentationFileTagID")
                    .ToTable("PRF_DocumentationFileTag").PrimaryColumn("DocumentationFileTagID");

                Delete.Table("PRF_DocumentationFile");
            }

            if (Schema.Table("PRF_DocumentationFileTag").Exists())
            {
                Delete.ForeignKey().FromTable("PRF_DocumentationFileTag").ForeignColumn("AdminPermissionID").ToTable("PRF_AdminPermission").PrimaryColumn("AdminPermissionID");

                Delete.Table("PRF_DocumentationFileTag");
            }
        }

        public override void Up()
        {
            if (!Schema.Table("PRF_DocumentationFileTag").Exists())
            {
                Create.Table("PRF_DocumentationFileTag")
                    .WithColumn("DocumentationFileTagID").AsInt32().PrimaryKey().Identity().NotNullable()
                    .WithColumn("Name").AsString(int.MaxValue).NotNullable()
                    .WithColumn("AdminPermissionID").AsInt32().NotNullable();

                Create.ForeignKey().FromTable("PRF_DocumentationFileTag").ForeignColumn("AdminPermissionID").ToTable("PRF_AdminPermission").PrimaryColumn("AdminPermissionID");

                Execute.Sql(@"
                    INSERT INTO PRF_DocumentationFileTag SELECT 'Administration', AdminPermissionID FROM PRF_AdminPermission WHERE Name = 'CanAdministrate';
                    INSERT INTO PRF_DocumentationFileTag SELECT 'Profiling', AdminPermissionID FROM PRF_AdminPermission WHERE Name = 'CanViewAndSearchPersons';
                    INSERT INTO PRF_DocumentationFileTag SELECT 'Screening', AdminPermissionID FROM PRF_AdminPermission WHERE Name = 'CanPerformScreeningInput';
                ");
            }

            if (!Schema.Table("PRF_DocumentationFile").Exists())
            {
                Create.Table("PRF_DocumentationFile")
                    .WithColumn("DocumentationFileID").AsInt32().PrimaryKey().Identity().NotNullable()
                    .WithColumn("FileName").AsString(int.MaxValue).NotNullable()
                    .WithColumn("FileData").AsBinary(int.MaxValue).NotNullable()
                    .WithColumn("Title").AsString(int.MaxValue).NotNullable()
                    .WithColumn("Description").AsString(int.MaxValue).NotNullable()
                    .WithColumn("LastModifiedDate").AsDateTime().NotNullable()
                    .WithColumn("UploadedDate").AsDateTime().NotNullable()
                    .WithColumn("UploadedByID").AsInt32().NotNullable()
                    .WithColumn("DocumentationFileTagID").AsInt32().NotNullable()
                    .WithColumn("Archive").AsBoolean().WithDefaultValue(false).NotNullable();

                Create.ForeignKey().FromTable("PRF_DocumentationFile").ForeignColumn("UploadedByID")
                    .ToTable("PRF_AdminUser").PrimaryColumn("AdminUserID");
                Create.ForeignKey().FromTable("PRF_DocumentationFile").ForeignColumn("DocumentationFileTagID")
                    .ToTable("PRF_DocumentationFileTag").PrimaryColumn("DocumentationFileTagID");
            }
        }
    }
}
