using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201303021729)]
    public class AuditUnit : Migration
    {
        public override void Down()
        {
            Delete.Table("PRF_Unit_AUD");
        }

        public override void Up()
        {
            Create.Table("PRF_Unit_AUD")
                .WithColumn("UnitID").AsInt32().NotNullable()
                .WithColumn("REV").AsInt32()
                .WithColumn("REVTYPE").AsInt16()
                .WithColumn("UnitName").AsString(500).Nullable()
                .WithColumn("BackgroundInformation").AsString(int.MaxValue).Nullable()
                .WithColumn("Archive").AsBoolean().NotNullable().Nullable()
                .WithColumn("Notes").AsString(int.MaxValue).Nullable();
            Create.PrimaryKey().OnTable("PRF_Unit_AUD").Columns(new string[] { "UnitID", "REV" });
        }
    }
}
