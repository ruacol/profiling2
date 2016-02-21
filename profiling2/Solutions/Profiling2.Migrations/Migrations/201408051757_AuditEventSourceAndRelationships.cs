using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201408051757)]
    public class AuditEventSourceAndRelationships : Migration
    {
        public override void Down()
        {
            Delete.Table("PRF_EventSource_AUD");
            Delete.Table("PRF_EventRelationship_AUD");
        }

        public override void Up()
        {
            Create.Table("PRF_EventSource_AUD")
                .WithColumn("EventSourceID").AsInt32().NotNullable()
                .WithColumn("REV").AsInt32()
                .WithColumn("REVTYPE").AsInt16()
                .WithColumn("EventID").AsInt32().Nullable()
                .WithColumn("SourceID").AsInt32().Nullable()
                .WithColumn("Commentary").AsString(int.MaxValue).Nullable()
                .WithColumn("ReliabilityID").AsInt32().Nullable()
                .WithColumn("Archive").AsBoolean().Nullable()
                .WithColumn("Notes").AsString(int.MaxValue).Nullable();
            Create.PrimaryKey().OnTable("PRF_EventSource_AUD").Columns(new string[] { "EventSourceID", "REV" });

            Create.Table("PRF_EventRelationship_AUD")
                .WithColumn("EventRelationshipID").AsInt32().NotNullable()
                .WithColumn("REV").AsInt32()
                .WithColumn("REVTYPE").AsInt16()
                .WithColumn("SubjectEventID").AsInt32().Nullable()
                .WithColumn("ObjectEventID").AsInt32().Nullable()
                .WithColumn("EventRelationshipTypeID").AsInt32().Nullable()
                .WithColumn("DayOfStart").AsInt32().Nullable()
                .WithColumn("MonthOfStart").AsInt32().Nullable()
                .WithColumn("YearOfStart").AsInt32().Nullable()
                .WithColumn("DayOfEnd").AsInt32().Nullable()
                .WithColumn("MonthOfEnd").AsInt32().Nullable()
                .WithColumn("YearOfEnd").AsInt32().Nullable()
                .WithColumn("Archive").AsBoolean().Nullable()
                .WithColumn("Notes").AsString(int.MaxValue).Nullable();
            Create.PrimaryKey().OnTable("PRF_EventRelationship_AUD").Columns(new string[] { "EventRelationshipID", "REV" });

            Execute.Sql(@"
                INSERT INTO PRF_EventSource_AUD (EventSourceID, REV, REVTYPE, EventID, SourceID, Commentary, ReliabilityID, Archive, Notes)
                    SELECT EventSourceID, 100, 0, EventID, SourceID, Commentary, ReliabilityID, Archive, Notes
                    FROM PRF_EventSource;

                INSERT INTO PRF_EventRelationship_AUD (EventRelationshipID, REV, REVTYPE, SubjectEventID, ObjectEventID, 
                        EventRelationshipTypeID, DayOfStart, MonthOfStart, YearOfStart, DayOfEnd, MonthOfEnd, YearOfEnd, Archive, Notes)
                    SELECT EventRelationshipID, 100, 0, SubjectEventID, ObjectEventID, 
                        EventRelationshipTypeID, DayOfStart, MonthOfStart, YearOfStart, DayOfEnd, MonthOfEnd, YearOfEnd, Archive, Notes
                    FROM PRF_EventRelationship;
            ");
        }
    }
}
