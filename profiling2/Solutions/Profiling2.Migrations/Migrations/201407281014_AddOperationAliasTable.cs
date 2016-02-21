using System;
using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201407281014)]
    public class AddOperationAliasTable : Migration
    {
        public override void Down()
        {
            Delete.ForeignKey().FromTable("PRF_OperationAlias").ForeignColumn("OperationID").ToTable("PRF_Operation").PrimaryColumn("OperationID");
            Delete.Table("PRF_OperationAlias");
            Delete.Table("PRF_OperationAlias_AUD");
        }

        public override void Up()
        {
            Create.Table("PRF_OperationAlias")
                .WithColumn("OperationAliasID").AsInt32().PrimaryKey().Identity().NotNullable()
                .WithColumn("OperationID").AsInt32().NotNullable()
                .WithColumn("Name").AsString(500).NotNullable()
                .WithColumn("Archive").AsBoolean().WithDefaultValue(false).NotNullable()
                .WithColumn("Notes").AsString(int.MaxValue).Nullable();

            Create.ForeignKey().FromTable("PRF_OperationAlias").ForeignColumn("OperationID").ToTable("PRF_Operation").PrimaryColumn("OperationID");

            Create.Table("PRF_OperationAlias_AUD")
                .WithColumn("OperationAliasID").AsInt32().NotNullable()
                .WithColumn("REV").AsInt32()
                .WithColumn("REVTYPE").AsInt16()
                .WithColumn("OperationID").AsInt32().Nullable()
                .WithColumn("Name").AsString(500).Nullable()
                .WithColumn("Archive").AsBoolean().Nullable()
                .WithColumn("Notes").AsString(int.MaxValue).Nullable();
            Create.PrimaryKey().OnTable("PRF_OperationAlias_AUD").Columns(new string[] { "OperationAliasID", "REV" });
        }
    }
}
