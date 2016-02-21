using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201303021727)]
    public class AuditOrganizationResponsibility : Migration
    {
        public override void Down()
        {
            Delete.Table("PRF_OrganizationResponsibility_AUD");
        }

        public override void Up()
        {
            Create.Table("PRF_OrganizationResponsibility_AUD")
                .WithColumn("OrganizationResponsibilityID").AsInt32().NotNullable()
                .WithColumn("REV").AsInt32()
                .WithColumn("REVTYPE").AsInt16()
                .WithColumn("OrganizationID").AsInt32().Nullable()
                .WithColumn("EventID").AsInt32().Nullable()
                .WithColumn("OrganizationResponsibilityTypeID").AsInt32().Nullable()
                .WithColumn("Commentary").AsString(int.MaxValue).Nullable()
                .WithColumn("Archive").AsBoolean().NotNullable().Nullable()
                .WithColumn("Notes").AsString(int.MaxValue).Nullable()
                .WithColumn("UnitID").AsInt32().Nullable();
            Create.PrimaryKey().OnTable("PRF_OrganizationResponsibility_AUD").Columns(new string[] { "OrganizationResponsibilityID", "REV" });
        }
    }
}
