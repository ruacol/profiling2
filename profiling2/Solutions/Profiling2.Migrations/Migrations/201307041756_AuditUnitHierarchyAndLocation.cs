using System;
using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201307041756)]
    public class AuditUnitHierarchyAndLocation : Migration
    {
        public override void Down()
        {
            Delete.Table("PRF_UnitHierarchy_AUD");
            Delete.Table("PRF_UnitLocation_AUD");
        }

        public override void Up()
        {
            Create.Table("PRF_UnitHierarchy_AUD")
                .WithColumn("UnitHierarchyID").AsInt32().NotNullable()
                .WithColumn("REV").AsInt32()
                .WithColumn("REVTYPE").AsInt16()
                .WithColumn("UnitID").AsInt32().Nullable()
                .WithColumn("ParentUnitID").AsInt32().Nullable()
                .WithColumn("DayOfStart").AsInt32().Nullable()
                .WithColumn("MonthOfStart").AsInt32().Nullable()
                .WithColumn("YearOfStart").AsInt32().Nullable()
                .WithColumn("DayOfEnd").AsInt32().Nullable()
                .WithColumn("MonthOfEnd").AsInt32().Nullable()
                .WithColumn("YearOfEnd").AsInt32().Nullable()
                .WithColumn("DayAsOf").AsInt32().Nullable()
                .WithColumn("MonthAsOf").AsInt32().Nullable()
                .WithColumn("YearAsOf").AsInt32().Nullable()
                .WithColumn("Commentary").AsString(int.MaxValue).Nullable()
                .WithColumn("Archive").AsBoolean().Nullable()
                .WithColumn("Notes").AsString(int.MaxValue).Nullable()
                .WithColumn("UnitHierarchyTypeID").AsInt32().Nullable();
            Create.PrimaryKey().OnTable("PRF_UnitHierarchy_AUD").Columns(new string[] { "UnitHierarchyID", "REV" });

            Create.Table("PRF_UnitLocation_AUD")
                .WithColumn("UnitLocationID").AsInt32().NotNullable()
                .WithColumn("REV").AsInt32()
                .WithColumn("REVTYPE").AsInt16()
                .WithColumn("UnitID").AsInt32().Nullable()
                .WithColumn("LocationID").AsInt32().Nullable()
                .WithColumn("DayOfStart").AsInt32().Nullable()
                .WithColumn("MonthOfStart").AsInt32().Nullable()
                .WithColumn("YearOfStart").AsInt32().Nullable()
                .WithColumn("DayOfEnd").AsInt32().Nullable()
                .WithColumn("MonthOfEnd").AsInt32().Nullable()
                .WithColumn("YearOfEnd").AsInt32().Nullable()
                .WithColumn("DayAsOf").AsInt32().Nullable()
                .WithColumn("MonthAsOf").AsInt32().Nullable()
                .WithColumn("YearAsOf").AsInt32().Nullable()
                .WithColumn("Commentary").AsString(int.MaxValue).Nullable()
                .WithColumn("Archive").AsBoolean().Nullable()
                .WithColumn("Notes").AsString(int.MaxValue).Nullable();
            Create.PrimaryKey().OnTable("PRF_UnitLocation_AUD").Columns(new string[] { "UnitLocationID", "REV" });


            Execute.Sql(@"
                INSERT INTO PRF_UnitHierarchy_AUD (UnitHierarchyID, REV, REVTYPE, UnitID, ParentUnitID, 
                    YearOfStart, MonthOfStart, DayOfStart, YearOfEnd, MonthOfEnd, DayOfEnd, YearAsOf, MonthAsOf, DayAsOf, 
                    Commentary, Archive, Notes, UnitHierarchyTypeID)
                    SELECT UnitHierarchyID, (SELECT MAX(REVINFOID)+1 FROM REVINFO), 0, UnitID, ParentUnitID, 
                        YearOfStart, MonthOfStart, DayOfStart, YearOfEnd, MonthOfEnd, DayOfEnd, YearAsOf, MonthAsOf, DayAsOf, 
                        Commentary, Archive, Notes, UnitHierarchyTypeID
                        FROM PRF_UnitHierarchy;

                INSERT INTO PRF_UnitLocation_AUD (UnitLocationID, REV, REVTYPE, UnitID, LocationID,
                    YearOfStart, MonthOfStart, DayOfStart, YearOfEnd, MonthOfEnd, DayOfEnd, YearAsOf, MonthAsOf, DayAsOf, 
                    Commentary, Archive, Notes)
                    SELECT UnitLocationID, (SELECT MAX(REVINFOID)+1 FROM REVINFO), 0, UnitID, LocationID, 
                        YearOfStart, MonthOfStart, DayOfStart, YearOfEnd, MonthOfEnd, DayOfEnd, YearAsOf, MonthAsOf, DayAsOf, 
                        Commentary, Archive, Notes
                        FROM PRF_UnitLocation;
            ");
        }
    }
}
