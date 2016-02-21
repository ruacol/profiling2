using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201307031532)]
    public class UnitLocationTable : Migration
    {
        public override void Down()
        {
            Alter.Table("PRF_UnitHierarchy").AddColumn("LocationID").AsInt32().Nullable();
            Create.ForeignKey("FK_PRF_UnitHierarchy_LocationID").FromTable("PRF_UnitHierarchy").ForeignColumn("LocationID").ToTable("PRF_Location").PrimaryColumn("LocationID");

            Delete.Table("PRF_UnitLocation");
        }

        public override void Up()
        {
            Create.Table("PRF_UnitLocation")
                .WithColumn("UnitLocationID").AsInt32().PrimaryKey().Identity().NotNullable()
                .WithColumn("UnitID").AsInt32().NotNullable()
                .WithColumn("LocationID").AsInt32().NotNullable()
                .WithColumn("YearOfStart").AsInt32().Nullable()
                .WithColumn("MonthOfStart").AsInt32().Nullable()
                .WithColumn("DayOfStart").AsInt32().Nullable()
                .WithColumn("YearOfEnd").AsInt32().Nullable()
                .WithColumn("MonthOfEnd").AsInt32().Nullable()
                .WithColumn("DayOfEnd").AsInt32().Nullable()
                .WithColumn("YearAsOf").AsInt32().Nullable()
                .WithColumn("MonthAsOf").AsInt32().Nullable()
                .WithColumn("DayAsOf").AsInt32().Nullable()
                .WithColumn("Commentary").AsString(int.MaxValue).Nullable()
                .WithColumn("Notes").AsString(int.MaxValue).Nullable()
                .WithColumn("Archive").AsBoolean().NotNullable().WithDefaultValue(false);
            Create.ForeignKey().FromTable("PRF_UnitLocation").ForeignColumn("UnitID").ToTable("PRF_Unit").PrimaryColumn("UnitID");
            Create.ForeignKey().FromTable("PRF_UnitLocation").ForeignColumn("LocationID").ToTable("PRF_Location").PrimaryColumn("LocationID");

            Execute.Sql(@"
                INSERT INTO PRF_UnitLocation (UnitID, LocationID, YearOfStart, MonthOfStart, DayOfStart, YearOfEnd, MonthOfEnd, DayOfEnd,
                    YearAsOf, MonthAsOf, DayAsOf, Commentary, Notes)
                SELECT UnitID, LocationID, YearOfStart, MonthOfStart, DayOfStart, YearOfEnd, MonthOfEnd, DayOfEnd,
                    YearAsOf, MonthAsOf, DayAsOf, Commentary, Notes
                FROM PRF_UnitHierarchy
                WHERE LocationID NOT IN (1488,1255)
            ");

            Execute.Sql(@"
                IF EXISTS (SELECT * FROM sys.all_objects WHERE name = 'FK_PRF_UnitHierarchy_LocationID')
                    ALTER TABLE PRF_UnitHierarchy DROP CONSTRAINT FK_PRF_UnitHierarchy_LocationID
            ");
            Delete.Column("LocationID").FromTable("PRF_UnitHierarchy");
        }
    }
}
