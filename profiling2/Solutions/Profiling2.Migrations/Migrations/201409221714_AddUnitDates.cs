using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201409221714)]
    public class AddUnitDates : Migration
    {
        public override void Down()
        {
            Delete.Column("YearOfStart").FromTable("PRF_Unit");
            Delete.Column("MonthOfStart").FromTable("PRF_Unit");
            Delete.Column("DayOfStart").FromTable("PRF_Unit");
            Delete.Column("YearOfEnd").FromTable("PRF_Unit");
            Delete.Column("MonthOfEnd").FromTable("PRF_Unit");
            Delete.Column("DayOfEnd").FromTable("PRF_Unit");

            Delete.Column("YearOfStart").FromTable("PRF_Unit_AUD");
            Delete.Column("MonthOfStart").FromTable("PRF_Unit_AUD");
            Delete.Column("DayOfStart").FromTable("PRF_Unit_AUD");
            Delete.Column("YearOfEnd").FromTable("PRF_Unit_AUD");
            Delete.Column("MonthOfEnd").FromTable("PRF_Unit_AUD");
            Delete.Column("DayOfEnd").FromTable("PRF_Unit_AUD");
        }

        public override void Up()
        {
            Alter.Table("PRF_Unit")
                .AddColumn("YearOfStart").AsInt32().Nullable()
                .AddColumn("MonthOfStart").AsInt32().Nullable()
                .AddColumn("DayOfStart").AsInt32().Nullable()
                .AddColumn("YearOfEnd").AsInt32().Nullable()
                .AddColumn("MonthOfEnd").AsInt32().Nullable()
                .AddColumn("DayOfEnd").AsInt32().Nullable();

            Alter.Table("PRF_Unit_AUD")
                .AddColumn("YearOfStart").AsInt32().Nullable()
                .AddColumn("MonthOfStart").AsInt32().Nullable()
                .AddColumn("DayOfStart").AsInt32().Nullable()
                .AddColumn("YearOfEnd").AsInt32().Nullable()
                .AddColumn("MonthOfEnd").AsInt32().Nullable()
                .AddColumn("DayOfEnd").AsInt32().Nullable();
        }
    }
}
