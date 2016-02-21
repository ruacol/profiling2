using System;
using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201407280939)]
    public class AddOperationTable : Migration
    {
        public override void Down()
        {
            Delete.UniqueConstraint("UQ_OperationName").FromTable("PRF_Operation");
            Delete.Table("PRF_Operation");
            Delete.Table("PRF_Operation_AUD");
        }

        public override void Up()
        {
            Create.Table("PRF_Operation")
                .WithColumn("OperationID").AsInt32().PrimaryKey().Identity().NotNullable()
                .WithColumn("Name").AsString(500).NotNullable()
                .WithColumn("Objective").AsString(int.MaxValue).Nullable()
                .WithColumn("YearOfStart").AsInt32().Nullable()
                .WithColumn("MonthOfStart").AsInt32().Nullable()
                .WithColumn("DayOfStart").AsInt32().Nullable()
                .WithColumn("YearOfEnd").AsInt32().Nullable()
                .WithColumn("MonthOfEnd").AsInt32().Nullable()
                .WithColumn("DayOfEnd").AsInt32().Nullable()
                .WithColumn("Archive").AsBoolean().WithDefaultValue(false).NotNullable()
                .WithColumn("Notes").AsString(int.MaxValue).Nullable();

            Create.UniqueConstraint("UQ_OperationName").OnTable("PRF_Operation").Column("Name");

            Create.Table("PRF_Operation_AUD")
                .WithColumn("OperationID").AsInt32().NotNullable()
                .WithColumn("REV").AsInt32()
                .WithColumn("REVTYPE").AsInt16()
                .WithColumn("Name").AsString(500).Nullable()
                .WithColumn("Objective").AsString(int.MaxValue).Nullable()
                .WithColumn("YearOfStart").AsInt32().Nullable()
                .WithColumn("MonthOfStart").AsInt32().Nullable()
                .WithColumn("DayOfStart").AsInt32().Nullable()
                .WithColumn("YearOfEnd").AsInt32().Nullable()
                .WithColumn("MonthOfEnd").AsInt32().Nullable()
                .WithColumn("DayOfEnd").AsInt32().Nullable()
                .WithColumn("Archive").AsBoolean().Nullable()
                .WithColumn("Notes").AsString(int.MaxValue).Nullable();
            Create.PrimaryKey().OnTable("PRF_Operation_AUD").Columns(new string[] { "OperationID", "REV" });
        }
    }
}
