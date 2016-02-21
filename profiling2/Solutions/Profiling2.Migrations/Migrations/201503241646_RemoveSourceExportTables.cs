using System;
using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201503241646)]
    public class RemoveSourceExportTables : Migration
    {
        public override void Down()
        {
            
        }

        public override void Up()
        {
            if (Schema.Table("PRF_SourceForExport").Exists())
            {
                Delete.ForeignKey().FromTable("PRF_SourceForExport").ForeignColumn("PersonID").ToTable("PRF_Person").PrimaryColumn("PersonID");
                Delete.ForeignKey().FromTable("PRF_SourceForExport").ForeignColumn("AssignedToID").ToTable("PRF_AdminUser").PrimaryColumn("AdminUserID");
                Delete.Table("PRF_SourceForExport");
            }
        }
    }
}
