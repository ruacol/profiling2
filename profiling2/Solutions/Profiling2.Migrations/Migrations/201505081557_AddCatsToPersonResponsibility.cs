using System;
using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201505081557)]
    public class AddCatsToPersonResponsibility : Migration
    {
        public override void Down()
        {
            Delete.Table("PRF_PersonResponsibilityViolation_AUD");
            Delete.UniqueConstraint("UQ_PersonResponsibilityViolation").FromTable("PRF_PersonResponsibilityViolation");
            Delete.Table("PRF_PersonResponsibilityViolation");
        }

        public override void Up()
        {
            Create.Table("PRF_PersonResponsibilityViolation")
                .WithColumn("PersonResponsibilityViolationID").AsInt32().PrimaryKey().Identity().NotNullable()
                .WithColumn("PersonResponsibilityID").AsInt32().NotNullable()
                .WithColumn("ViolationID").AsInt32().NotNullable();

            Create.UniqueConstraint("UQ_PersonResponsibilityViolation")
                .OnTable("PRF_PersonResponsibilityViolation")
                .Columns(new string[] { "PersonResponsibilityID", "ViolationID" });

            Create.Table("PRF_PersonResponsibilityViolation_AUD")
                .WithColumn("PersonResponsibilityViolationID").AsInt32().Nullable()
                .WithColumn("REV").AsInt32()
                .WithColumn("REVTYPE").AsInt16()
                .WithColumn("PersonResponsibilityID").AsInt32().Nullable()
                .WithColumn("ViolationID").AsInt32().Nullable();
            //Create.PrimaryKey().OnTable("PRF_PersonViolationResponsibility_AUD").Columns(new string[] { "PersonViolationResponsibilityID", "REV" });

            Execute.Sql(@"
                INSERT INTO PRF_PersonResponsibilityViolation (PersonResponsibilityID, ViolationID)
                    SELECT pr.PersonResponsibilityID, ev.ViolationID
                        FROM PRF_PersonResponsibility pr, PRF_EventViolation ev
                        WHERE pr.EventID = ev.EventID;
            ");
        }
    }
}
