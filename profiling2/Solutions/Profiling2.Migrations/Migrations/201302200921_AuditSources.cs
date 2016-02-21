using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201302200921)]
    public class AuditSources : Migration
    {

        public override void Down()
        {
            Delete.Table("PRF_Source_AUD");
        }

        public override void Up()
        {
            Create.Table("PRF_Source_AUD")
                .WithColumn("SourceID").AsInt32().NotNullable()
                .WithColumn("REV").AsInt32()
                .WithColumn("REVTYPE").AsInt16()
                .WithColumn("Archive").AsBoolean().NotNullable();
            Create.PrimaryKey().OnTable("PRF_Source_AUD").Columns(new string[] { "SourceID", "REV" });
        }
    }
}
