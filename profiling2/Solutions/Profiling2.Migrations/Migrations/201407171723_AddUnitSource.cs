using System;
using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201407171723)]
    public class AddUnitSource : Migration
    {
        public override void Down()
        {
            Delete.Table("PRF_UnitSource_AUD");

            Delete.UniqueConstraint("UQ_UnitSource").FromTable("PRF_UnitSource");
            Delete.ForeignKey().FromTable("PRF_UnitSource").ForeignColumn("UnitID").ToTable("PRF_Unit").PrimaryColumn("UnitID");
            Delete.ForeignKey().FromTable("PRF_UnitSource").ForeignColumn("SourceID").ToTable("PRF_Source").PrimaryColumn("SourceID");
            Delete.Table("PRF_UnitSource");
        }

        public override void Up()
        {
            Create.Table("PRF_UnitSource")
                .WithColumn("UnitSourceID").AsInt32().PrimaryKey().Identity().NotNullable()
                .WithColumn("UnitID").AsInt32().NotNullable()
                .WithColumn("SourceID").AsInt32().NotNullable()
                .WithColumn("Commentary").AsString(int.MaxValue).Nullable()
                .WithColumn("ReliabilityID").AsInt32().NotNullable()
                .WithColumn("Archive").AsBoolean().WithDefaultValue(false).NotNullable()
                .WithColumn("Notes").AsString(int.MaxValue).Nullable();

            Create.ForeignKey().FromTable("PRF_UnitSource").ForeignColumn("UnitID").ToTable("PRF_Unit").PrimaryColumn("UnitID");
            Create.ForeignKey().FromTable("PRF_UnitSource").ForeignColumn("SourceID").ToTable("PRF_Source").PrimaryColumn("SourceID");
            Create.ForeignKey().FromTable("PRF_UnitSource").ForeignColumn("ReliabilityID").ToTable("PRF_Reliability").PrimaryColumn("ReliabilityID");
            Create.UniqueConstraint("UQ_UnitSource").OnTable("PRF_UnitSource").Columns(new string[] { "UnitID", "SourceID" });

            Create.Table("PRF_UnitSource_AUD")
                .WithColumn("UnitSourceID").AsInt32().NotNullable()
                .WithColumn("REV").AsInt32()
                .WithColumn("REVTYPE").AsInt16()
                .WithColumn("UnitID").AsInt32().Nullable()
                .WithColumn("SourceID").AsInt32().Nullable()
                .WithColumn("Commentary").AsString(int.MaxValue).Nullable()
                .WithColumn("ReliabilityID").AsInt32().Nullable()
                .WithColumn("Archive").AsBoolean().Nullable()
                .WithColumn("Notes").AsString(int.MaxValue).Nullable();
            Create.PrimaryKey().OnTable("PRF_UnitSource_AUD").Columns(new string[] { "UnitSourceID", "REV" });
        }
    }
}
