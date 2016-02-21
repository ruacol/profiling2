using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201411041033)]
    public class AddDatesToUnitOperations : Migration
    {
        public override void Down()
        {
            Delete.Column("YearOfStart").FromTable("PRF_UnitOperation");
            Delete.Column("MonthOfStart").FromTable("PRF_UnitOperation");
            Delete.Column("DayOfStart").FromTable("PRF_UnitOperation");
            Delete.Column("YearOfEnd").FromTable("PRF_UnitOperation");
            Delete.Column("MonthOfEnd").FromTable("PRF_UnitOperation");
            Delete.Column("DayOfEnd").FromTable("PRF_UnitOperation");

            Delete.Column("YearOfStart").FromTable("PRF_UnitOperation_AUD");
            Delete.Column("MonthOfStart").FromTable("PRF_UnitOperation_AUD");
            Delete.Column("DayOfStart").FromTable("PRF_UnitOperation_AUD");
            Delete.Column("YearOfEnd").FromTable("PRF_UnitOperation_AUD");
            Delete.Column("MonthOfEnd").FromTable("PRF_UnitOperation_AUD");
            Delete.Column("DayOfEnd").FromTable("PRF_UnitOperation_AUD");
        }

        public override void Up()
        {
            Alter.Table("PRF_UnitOperation")
                .AddColumn("YearOfStart").AsInt32().Nullable()
                .AddColumn("MonthOfStart").AsInt32().Nullable()
                .AddColumn("DayOfStart").AsInt32().Nullable()
                .AddColumn("YearOfEnd").AsInt32().Nullable()
                .AddColumn("MonthOfEnd").AsInt32().Nullable()
                .AddColumn("DayOfEnd").AsInt32().Nullable();

            Alter.Table("PRF_UnitOperation_AUD")
                .AddColumn("YearOfStart").AsInt32().Nullable()
                .AddColumn("MonthOfStart").AsInt32().Nullable()
                .AddColumn("DayOfStart").AsInt32().Nullable()
                .AddColumn("YearOfEnd").AsInt32().Nullable()
                .AddColumn("MonthOfEnd").AsInt32().Nullable()
                .AddColumn("DayOfEnd").AsInt32().Nullable();
        }
    }
}
