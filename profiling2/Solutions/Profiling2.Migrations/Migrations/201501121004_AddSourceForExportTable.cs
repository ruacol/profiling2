using System;
using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201501121004)]
    public class AddSourceForExportTable : Migration
    {
        public override void Down()
        {
            Delete.ForeignKey().FromTable("PRF_SourceForExport").ForeignColumn("PersonID").ToTable("PRF_Person").PrimaryColumn("PersonID");
            Delete.ForeignKey().FromTable("PRF_SourceForExport").ForeignColumn("AssignedToID").ToTable("PRF_AdminUser").PrimaryColumn("AdminUserID");
            Delete.Table("PRF_SourceForExport");
        }

        public override void Up()
        {
            Create.Table("PRF_SourceForExport")
                .WithColumn("SourceForExportID").AsInt32().PrimaryKey().Identity().NotNullable()
                .WithColumn("SourceID").AsInt32().NotNullable()
                .WithColumn("PersonID").AsInt32().Nullable()
                .WithColumn("AssignedToID").AsInt32().Nullable()
                .WithColumn("Text").AsString(int.MaxValue).Nullable();

            Create.ForeignKey().FromTable("PRF_SourceForExport").ForeignColumn("PersonID").ToTable("PRF_Person").PrimaryColumn("PersonID");
            Create.ForeignKey().FromTable("PRF_SourceForExport").ForeignColumn("AssignedToID").ToTable("PRF_AdminUser").PrimaryColumn("AdminUserID");
        }
    }
}
