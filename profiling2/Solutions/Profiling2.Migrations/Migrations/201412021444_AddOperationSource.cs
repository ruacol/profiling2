using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201412021444)]
    public class AddOperationSource : Migration
    {
        public override void Down()
        {
            Delete.Table("PRF_OperationSource_AUD");

            Delete.UniqueConstraint("UQ_OperationSource").FromTable("PRF_OperationSource");
            Delete.ForeignKey().FromTable("PRF_OperationSource").ForeignColumn("OperationID").ToTable("PRF_Operation").PrimaryColumn("OperationID");
            Delete.ForeignKey().FromTable("PRF_OperationSource").ForeignColumn("SourceID").ToTable("PRF_Source").PrimaryColumn("SourceID");
            Delete.Table("PRF_OperationSource");
        }

        public override void Up()
        {
            Create.Table("PRF_OperationSource")
                .WithColumn("OperationSourceID").AsInt32().PrimaryKey().Identity().NotNullable()
                .WithColumn("OperationID").AsInt32().NotNullable()
                .WithColumn("SourceID").AsInt32().NotNullable()
                .WithColumn("Commentary").AsString(int.MaxValue).Nullable()
                .WithColumn("ReliabilityID").AsInt32().NotNullable()
                .WithColumn("Archive").AsBoolean().WithDefaultValue(false).NotNullable()
                .WithColumn("Notes").AsString(int.MaxValue).Nullable();

            Create.ForeignKey().FromTable("PRF_OperationSource").ForeignColumn("OperationID").ToTable("PRF_Operation").PrimaryColumn("OperationID");
            Create.ForeignKey().FromTable("PRF_OperationSource").ForeignColumn("SourceID").ToTable("PRF_Source").PrimaryColumn("SourceID");
            Create.ForeignKey().FromTable("PRF_OperationSource").ForeignColumn("ReliabilityID").ToTable("PRF_Reliability").PrimaryColumn("ReliabilityID");
            Create.UniqueConstraint("UQ_OperationSource").OnTable("PRF_OperationSource").Columns(new string[] { "OperationID", "SourceID" });

            Create.Table("PRF_OperationSource_AUD")
                .WithColumn("OperationSourceID").AsInt32().NotNullable()
                .WithColumn("REV").AsInt32()
                .WithColumn("REVTYPE").AsInt16()
                .WithColumn("OperationID").AsInt32().Nullable()
                .WithColumn("SourceID").AsInt32().Nullable()
                .WithColumn("Commentary").AsString(int.MaxValue).Nullable()
                .WithColumn("ReliabilityID").AsInt32().Nullable()
                .WithColumn("Archive").AsBoolean().Nullable()
                .WithColumn("Notes").AsString(int.MaxValue).Nullable();
            Create.PrimaryKey().OnTable("PRF_OperationSource_AUD").Columns(new string[] { "OperationSourceID", "REV" });
        }
    }
}
