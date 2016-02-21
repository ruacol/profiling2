using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201305131559)]
    public class NullableDatePeriods : Migration
    {
        public override void Down()
        {
            // need to delete indexes before making notnullable
            //Alter.Column("DayOfStart").OnTable("PRF_ActionTaken").AsInt32().NotNullable();
            //Alter.Column("MonthOfStart").OnTable("PRF_ActionTaken").AsInt32().NotNullable();
            //Alter.Column("YearOfStart").OnTable("PRF_ActionTaken").AsInt32().NotNullable();
            //Alter.Column("DayOfEnd").OnTable("PRF_ActionTaken").AsInt32().NotNullable();
            //Alter.Column("MonthOfEnd").OnTable("PRF_ActionTaken").AsInt32().NotNullable();
            //Alter.Column("YearOfEnd").OnTable("PRF_ActionTaken").AsInt32().NotNullable();

            //Alter.Column("DayOfStart").OnTable("PRF_Career").AsInt32().NotNullable();
            //Alter.Column("MonthOfStart").OnTable("PRF_Career").AsInt32().NotNullable();
            //Alter.Column("YearOfStart").OnTable("PRF_Career").AsInt32().NotNullable();
            //Alter.Column("DayOfEnd").OnTable("PRF_Career").AsInt32().NotNullable();
            //Alter.Column("MonthOfEnd").OnTable("PRF_Career").AsInt32().NotNullable();
            //Alter.Column("YearOfEnd").OnTable("PRF_Career").AsInt32().NotNullable();
            //Alter.Column("DayAsOf").OnTable("PRF_Career").AsInt32().NotNullable();
            //Alter.Column("MonthAsOf").OnTable("PRF_Career").AsInt32().NotNullable();
            //Alter.Column("YearAsOf").OnTable("PRF_Career").AsInt32().NotNullable();

            //Alter.Column("DayOfStart").OnTable("PRF_Event").AsInt32().NotNullable();
            //Alter.Column("MonthOfStart").OnTable("PRF_Event").AsInt32().NotNullable();
            //Alter.Column("YearOfStart").OnTable("PRF_Event").AsInt32().NotNullable();

            //Alter.Column("DayOfStart").OnTable("PRF_Event_AUD").AsInt32().NotNullable();
            //Alter.Column("MonthOfStart").OnTable("PRF_Event_AUD").AsInt32().NotNullable();
            //Alter.Column("YearOfStart").OnTable("PRF_Event_AUD").AsInt32().NotNullable();
        }

        public override void Up()
        {
            Alter.Column("DayOfStart").OnTable("PRF_ActionTaken").AsInt32().Nullable();
            Alter.Column("MonthOfStart").OnTable("PRF_ActionTaken").AsInt32().Nullable();
            Alter.Column("YearOfStart").OnTable("PRF_ActionTaken").AsInt32().Nullable();
            Alter.Column("DayOfEnd").OnTable("PRF_ActionTaken").AsInt32().Nullable();
            Alter.Column("MonthOfEnd").OnTable("PRF_ActionTaken").AsInt32().Nullable();
            Alter.Column("YearOfEnd").OnTable("PRF_ActionTaken").AsInt32().Nullable();

            Alter.Column("DayOfStart").OnTable("PRF_Career").AsInt32().Nullable();
            Alter.Column("MonthOfStart").OnTable("PRF_Career").AsInt32().Nullable();
            Alter.Column("YearOfStart").OnTable("PRF_Career").AsInt32().Nullable();
            Alter.Column("DayOfEnd").OnTable("PRF_Career").AsInt32().Nullable();
            Alter.Column("MonthOfEnd").OnTable("PRF_Career").AsInt32().Nullable();
            Alter.Column("YearOfEnd").OnTable("PRF_Career").AsInt32().Nullable();
            Alter.Column("DayAsOf").OnTable("PRF_Career").AsInt32().Nullable();
            Alter.Column("MonthAsOf").OnTable("PRF_Career").AsInt32().Nullable();
            Alter.Column("YearAsOf").OnTable("PRF_Career").AsInt32().Nullable();

            Execute.Sql(@"UPDATE PRF_Career SET DayOfStart = NULL WHERE DayOfStart = 0;
                UPDATE PRF_Career SET MonthOfStart = NULL WHERE MonthOfStart = 0;
                UPDATE PRF_Career SET YearOfStart = NULL WHERE YearOfStart = 0;
                UPDATE PRF_Career SET DayOfEnd = NULL WHERE DayOfEnd = 0;
                UPDATE PRF_Career SET MonthOfEnd = NULL WHERE MonthOfEnd = 0;
                UPDATE PRF_Career SET YearOfEnd = NULL WHERE YearOfEnd = 0;
                UPDATE PRF_Career SET DayAsOf = NULL WHERE DayAsOf = 0;
                UPDATE PRF_Career SET MonthAsOf = NULL WHERE MonthAsOf = 0;
                UPDATE PRF_Career SET YearAsOf = NULL WHERE YearAsOf = 0;
            ");

            Alter.Column("DayOfStart").OnTable("PRF_Event").AsInt32().Nullable();
            Alter.Column("MonthOfStart").OnTable("PRF_Event").AsInt32().Nullable();
            Alter.Column("YearOfStart").OnTable("PRF_Event").AsInt32().Nullable();

            Alter.Column("DayOfStart").OnTable("PRF_Event_AUD").AsInt32().Nullable();
            Alter.Column("MonthOfStart").OnTable("PRF_Event_AUD").AsInt32().Nullable();
            Alter.Column("YearOfStart").OnTable("PRF_Event_AUD").AsInt32().Nullable();
        }
    }
}
