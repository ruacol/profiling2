using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201303151728)]
    public class AuditStuff : Migration
    {
        public override void Down()
        {
            Delete.Table("PRF_ActionTaken_AUD");
            Delete.Table("PRF_Career_AUD");
            Delete.Table("PRF_PersonAlias_AUD");
            Delete.Table("PRF_PersonRelationship_AUD");
            Delete.Table("PRF_PersonSource_AUD");
        }

        public override void Up()
        {
            Create.Table("PRF_ActionTaken_AUD")
                .WithColumn("ActionTakenID").AsInt32().NotNullable()
                .WithColumn("REV").AsInt32()
                .WithColumn("REVTYPE").AsInt16()
                .WithColumn("SubjectPersonID").AsInt32().Nullable()
                .WithColumn("ObjectPersonID").AsInt32().Nullable()
                .WithColumn("ActionTakenTypeID").AsInt32().Nullable()
                .WithColumn("EventID").AsInt32().Nullable()
                .WithColumn("DayOfStart").AsInt32().Nullable()
                .WithColumn("MonthOfStart").AsInt32().Nullable()
                .WithColumn("YearOfStart").AsInt32().Nullable()
                .WithColumn("DayOfEnd").AsInt32().Nullable()
                .WithColumn("MonthOfEnd").AsInt32().Nullable()
                .WithColumn("YearOfEnd").AsInt32().Nullable()
                .WithColumn("Commentary").AsString(int.MaxValue).Nullable()
                .WithColumn("Archive").AsBoolean().Nullable()
                .WithColumn("Notes").AsString(int.MaxValue).Nullable();
            Create.PrimaryKey().OnTable("PRF_ActionTaken_AUD").Columns(new string[] { "ActionTakenID", "REV" });

            Create.Table("PRF_Career_AUD")
                .WithColumn("CareerID").AsInt32().NotNullable()
                .WithColumn("REV").AsInt32()
                .WithColumn("REVTYPE").AsInt16()
                .WithColumn("PersonID").AsInt32().Nullable()
                .WithColumn("OrganizationID").AsInt32().Nullable()
                .WithColumn("LocationID").AsInt32().Nullable()
                .WithColumn("RankID").AsInt32().Nullable()
                .WithColumn("UnitID").AsInt32().Nullable()
                .WithColumn("RoleID").AsInt32().Nullable()
                .WithColumn("DayOfStart").AsInt32().Nullable()
                .WithColumn("MonthOfStart").AsInt32().Nullable()
                .WithColumn("YearOfStart").AsInt32().Nullable()
                .WithColumn("DayOfEnd").AsInt32().Nullable()
                .WithColumn("MonthOfEnd").AsInt32().Nullable()
                .WithColumn("YearOfEnd").AsInt32().Nullable()
                .WithColumn("Job").AsString(500).Nullable()
                .WithColumn("Commentary").AsString(int.MaxValue).Nullable()
                .WithColumn("Archive").AsBoolean().Nullable()
                .WithColumn("Notes").AsString(int.MaxValue).Nullable()
                .WithColumn("DayAsOf").AsInt32().Nullable()
                .WithColumn("MonthAsOf").AsInt32().Nullable()
                .WithColumn("YearAsOf").AsInt32().Nullable()
                .WithColumn("IsCurrentCareer").AsBoolean().Nullable();
            Create.PrimaryKey().OnTable("PRF_Career_AUD").Columns(new string[] { "CareerID", "REV" });

            Create.Table("PRF_PersonAlias_AUD")
                .WithColumn("PersonAliasID").AsInt32().NotNullable()
                .WithColumn("REV").AsInt32()
                .WithColumn("REVTYPE").AsInt16()
                .WithColumn("PersonID").AsInt32().Nullable()
                .WithColumn("FirstName").AsString(500).Nullable()
                .WithColumn("LastName").AsString(500).Nullable()
                .WithColumn("Archive").AsBoolean().Nullable()
                .WithColumn("Notes").AsString(int.MaxValue).Nullable();
            Create.PrimaryKey().OnTable("PRF_PersonAlias_AUD").Columns(new string[] { "PersonAliasID", "REV" });

            Create.Table("PRF_PersonRelationship_AUD")
                .WithColumn("PersonRelationshipID").AsInt32().NotNullable()
                .WithColumn("REV").AsInt32()
                .WithColumn("REVTYPE").AsInt16()
                .WithColumn("SubjectPersonID").AsInt32().Nullable()
                .WithColumn("ObjectPersonID").AsInt32().Nullable()
                .WithColumn("PersonRelationshipTypeID").AsInt32().Nullable()
                .WithColumn("EventID").AsInt32().Nullable()
                .WithColumn("DayOfStart").AsInt32().Nullable()
                .WithColumn("MonthOfStart").AsInt32().Nullable()
                .WithColumn("YearOfStart").AsInt32().Nullable()
                .WithColumn("DayOfEnd").AsInt32().Nullable()
                .WithColumn("MonthOfEnd").AsInt32().Nullable()
                .WithColumn("YearOfEnd").AsInt32().Nullable()
                .WithColumn("Commentary").AsString(int.MaxValue).Nullable()
                .WithColumn("Archive").AsBoolean().Nullable()
                .WithColumn("Notes").AsString(int.MaxValue).Nullable();
            Create.PrimaryKey().OnTable("PRF_PersonRelationship_AUD").Columns(new string[] { "PersonRelationshipID", "REV" });

            Create.Table("PRF_PersonSource_AUD")
                .WithColumn("PersonSourceID").AsInt32().NotNullable()
                .WithColumn("REV").AsInt32()
                .WithColumn("REVTYPE").AsInt16()
                .WithColumn("PersonID").AsInt32().Nullable()
                .WithColumn("SourceID").AsInt32().Nullable()
                .WithColumn("ReliabilityID").AsInt32().Nullable()
                .WithColumn("Commentary").AsString(int.MaxValue).Nullable()
                .WithColumn("Archive").AsBoolean().Nullable()
                .WithColumn("Notes").AsString(int.MaxValue).Nullable();
            Create.PrimaryKey().OnTable("PRF_PersonSource_AUD").Columns(new string[] { "PersonSourceID", "REV" });
        }
    }
}
