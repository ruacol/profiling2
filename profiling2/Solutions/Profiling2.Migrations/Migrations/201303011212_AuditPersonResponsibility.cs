using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201303011212)]
    public class AuditPersonResponsibility : Migration
    {
        public override void Down()
        {
            Delete.Table("PRF_PersonResponsibility_AUD");
        }

        public override void Up()
        {
            Create.Table("PRF_PersonResponsibility_AUD")
                .WithColumn("PersonResponsibilityID").AsInt32().NotNullable()
                .WithColumn("REV").AsInt32()
                .WithColumn("REVTYPE").AsInt16()
                .WithColumn("PersonID").AsInt32().Nullable()
                .WithColumn("EventID").AsInt32().Nullable()
                .WithColumn("PersonResponsibilityTypeID").AsInt32().Nullable()
                .WithColumn("Commentary").AsString(int.MaxValue).Nullable()
                .WithColumn("Archive").AsBoolean().NotNullable().Nullable()
                .WithColumn("Notes").AsString(int.MaxValue).Nullable();
            Create.PrimaryKey().OnTable("PRF_PersonResponsibility_AUD").Columns(new string[] { "PersonResponsibilityID", "REV" });
        }
    }
}
