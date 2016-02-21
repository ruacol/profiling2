using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201302131648)]
    public class Envers : Migration
    {
        public override void Down()
        {
            Delete.Table("REVINFO");
            Delete.Table("SCR_Request_AUD");
            Delete.Table("PRF_Event_AUD");
            Delete.Table("PRF_Violation_AUD");
            Delete.Table("PRF_EventViolation_AUD");
        }

        public override void Up()
        {
            Create.Table("REVINFO")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("REV").AsInt32().NotNullable()
                .WithColumn("REVTSTMP").AsDateTime().Nullable()
                .WithColumn("UserName").AsString(450).Nullable();

            Create.Table("SCR_Request_AUD")
                .WithColumn("RequestID").AsInt32().NotNullable()
                .WithColumn("REV").AsInt32()
                .WithColumn("REVTYPE").AsInt16()
                .WithColumn("RequestEntityID").AsInt32().NotNullable()
                .WithColumn("RequestTypeID").AsInt32().NotNullable()
                .WithColumn("RequestName").AsString(255).NotNullable()
                .WithColumn("ReferenceNumber").AsString(20).NotNullable()
                .WithColumn("RespondBy").AsString(255).Nullable()
                .WithColumn("Notes").AsString(int.MaxValue).Nullable()
                .WithColumn("Archive").AsBoolean().NotNullable();
            Create.PrimaryKey().OnTable("SCR_Request_AUD").Columns(new string[] { "RequestID", "REV" });

            Create.Table("PRF_Event_AUD")
                .WithColumn("EventID").AsInt32().NotNullable()
                .WithColumn("REV").AsInt32()
                .WithColumn("REVTYPE").AsInt16()
                .WithColumn("EventName").AsString(500).NotNullable()
                .WithColumn("NarrativeEn").AsString(int.MaxValue).Nullable()
                .WithColumn("NarrativeFr").AsString(int.MaxValue).Nullable()
                .WithColumn("DayOfStart").AsInt32().NotNullable()
                .WithColumn("MonthOfStart").AsInt32().NotNullable()
                .WithColumn("YearOfStart").AsInt32().NotNullable()
                .WithColumn("DayOfEnd").AsInt32().Nullable()
                .WithColumn("MonthOfEnd").AsInt32().Nullable()
                .WithColumn("YearOfEnd").AsInt32().Nullable()
                .WithColumn("LocationID").AsInt32().NotNullable()
                .WithColumn("Archive").AsBoolean().NotNullable()
                .WithColumn("Notes").AsString(int.MaxValue).Nullable();
            Create.PrimaryKey().OnTable("PRF_Event_AUD").Columns(new string[] { "EventID", "REV" });

            Create.Table("PRF_Violation_AUD")
                .WithColumn("ViolationID").AsInt32().NotNullable()
                .WithColumn("REV").AsInt32()
                .WithColumn("REVTYPE").AsInt16()
                .WithColumn("Name").AsString(255).NotNullable()
                .WithColumn("Description").AsString(500).Nullable()
                .WithColumn("Keywords").AsString(int.MaxValue).Nullable()
                .WithColumn("ParentViolationID").AsInt32().Nullable();
            Create.PrimaryKey().OnTable("PRF_Violation_AUD").Columns(new string[] { "ViolationID", "REV" });

            Create.Table("PRF_EventViolation_AUD")
                .WithColumn("EventViolationID").AsInt32().Nullable()
                .WithColumn("REV").AsInt32()
                .WithColumn("REVTYPE").AsInt16()
                .WithColumn("EventID").AsInt32().Nullable()
                .WithColumn("ViolationID").AsInt32().Nullable();
            //Create.PrimaryKey().OnTable("PRF_EventViolation_AUD").Columns(new string[] { "EventViolationID", "REV" });
        }
    }
}
