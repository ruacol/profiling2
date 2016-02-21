using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201407170937)]
    public class AddUnitAlias : Migration
    {
        public override void Down()
        {
            Delete.ForeignKey().FromTable("PRF_UnitAlias").ForeignColumn("UnitID").ToTable("PRF_Unit").PrimaryColumn("UnitID");
            Delete.Table("PRF_UnitAlias");
            Delete.Table("PRF_UnitAlias_AUD");
        }

        public override void Up()
        {
            Create.Table("PRF_UnitAlias")
                .WithColumn("UnitAliasID").AsInt32().PrimaryKey().Identity().NotNullable()
                .WithColumn("UnitID").AsInt32().NotNullable()
                .WithColumn("UnitName").AsString(500).NotNullable()
                .WithColumn("Archive").AsBoolean().WithDefaultValue(false).NotNullable()
                .WithColumn("Notes").AsString(int.MaxValue).Nullable();

            Create.ForeignKey().FromTable("PRF_UnitAlias").ForeignColumn("UnitID").ToTable("PRF_Unit").PrimaryColumn("UnitID");

            Create.Table("PRF_UnitAlias_AUD")
                .WithColumn("UnitAliasID").AsInt32().NotNullable()
                .WithColumn("REV").AsInt32()
                .WithColumn("REVTYPE").AsInt16()
                .WithColumn("UnitID").AsInt32().Nullable()
                .WithColumn("UnitName").AsString(500).Nullable()
                .WithColumn("Archive").AsBoolean().Nullable()
                .WithColumn("Notes").AsString(int.MaxValue).Nullable();
            Create.PrimaryKey().OnTable("PRF_UnitAlias_AUD").Columns(new string[] { "UnitAliasID", "REV" });
        }
    }
}
