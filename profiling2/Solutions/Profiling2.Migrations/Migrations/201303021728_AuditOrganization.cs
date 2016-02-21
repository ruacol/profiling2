using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201303021728)]
    public class AuditOrganization : Migration
    {
        public override void Down()
        {
            Delete.Table("PRF_Organization_AUD");
        }

        public override void Up()
        {
            Create.Table("PRF_Organization_AUD")
                .WithColumn("OrganizationID").AsInt32().NotNullable()
                .WithColumn("REV").AsInt32()
                .WithColumn("REVTYPE").AsInt16()
                .WithColumn("OrgShortName").AsString(255).Nullable()
                .WithColumn("OrgLongName").AsString(500).Nullable()
                .WithColumn("DayOfStart").AsInt32().Nullable()
                .WithColumn("MonthOfStart").AsInt32().Nullable()
                .WithColumn("YearOfStart").AsInt32().Nullable()
                .WithColumn("DayOfEnd").AsInt32().Nullable()
                .WithColumn("MonthOfEnd").AsInt32().Nullable()
                .WithColumn("YearOfEnd").AsInt32().Nullable()
                .WithColumn("Archive").AsBoolean().NotNullable().Nullable()
                .WithColumn("Notes").AsString(int.MaxValue).Nullable();
            Create.PrimaryKey().OnTable("PRF_Organization_AUD").Columns(new string[] { "OrganizationID", "REV" });
        }
    }
}
