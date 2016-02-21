using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201303011420)]
    public class AuditPerson : Migration
    {
        public override void Down()
        {
            Delete.Table("PRF_Person_AUD");
        }

        public override void Up()
        {
            Create.Table("PRF_Person_AUD")
                .WithColumn("PersonID").AsInt32().NotNullable()
                .WithColumn("REV").AsInt32()
                .WithColumn("REVTYPE").AsInt16()
                .WithColumn("LastName").AsString(500).Nullable()
                .WithColumn("FirstName").AsString(500).Nullable()
                .WithColumn("DayOfBirth").AsInt32().Nullable()
                .WithColumn("MonthOfBirth").AsInt32().Nullable()
                .WithColumn("YearOfBirth").AsInt32().Nullable()
                .WithColumn("BirthVillage").AsString(500).Nullable()
                .WithColumn("BirthRegionID").AsInt32().Nullable()
                .WithColumn("ApproximateBirthDate").AsString(255).Nullable()
                .WithColumn("EthnicityID").AsInt32().Nullable()
                .WithColumn("Height").AsString(255).Nullable()
                .WithColumn("Weight").AsString(255).Nullable()
                .WithColumn("BackgroundInformation").AsString(int.MaxValue).Nullable()
                .WithColumn("MilitaryIDNumber").AsString(255).Nullable()
                .WithColumn("Archive").AsBoolean().Nullable()
                .WithColumn("Notes").AsString(int.MaxValue).Nullable()
                .WithColumn("ProfileStatusID").AsInt32().Nullable()
                .WithColumn("IsRestrictedProfile").AsBoolean().Nullable();
            Create.PrimaryKey().OnTable("PRF_Person_AUD").Columns(new string[] { "PersonID", "REV" });
        }
    }
}
