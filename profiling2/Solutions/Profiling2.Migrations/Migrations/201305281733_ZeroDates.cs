using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201305281733)]
    public class ZeroDates : Migration
    {
        public override void Down()
        {

        }

        public override void Up()
        {
            Execute.Sql(@"UPDATE PRF_Career SET DayOfStart = 0 WHERE DayOfStart IS NULL;
                UPDATE PRF_Career SET MonthOfStart = 0 WHERE MonthOfStart IS NULL;
                UPDATE PRF_Career SET YearOfStart = 0 WHERE YearOfStart IS NULL;
                UPDATE PRF_Career SET DayOfEnd = 0 WHERE DayOfEnd IS NULL;
                UPDATE PRF_Career SET MonthOfEnd = 0 WHERE MonthOfEnd IS NULL;
                UPDATE PRF_Career SET YearOfEnd = 0 WHERE YearOfEnd IS NULL;
                UPDATE PRF_Career SET DayAsOf = 0 WHERE DayAsOf IS NULL;
                UPDATE PRF_Career SET MonthAsOf = 0 WHERE MonthAsOf IS NULL;
                UPDATE PRF_Career SET YearAsOf = 0 WHERE YearAsOf IS NULL;

                UPDATE PRF_Event SET DayOfStart = 0 WHERE DayOfStart IS NULL;
                UPDATE PRF_Event SET MonthOfStart = 0 WHERE MonthOfStart IS NULL;
                UPDATE PRF_Event SET YearOfStart = 0 WHERE YearOfStart IS NULL;
                UPDATE PRF_Event SET DayOfEnd = 0 WHERE DayOfEnd IS NULL;
                UPDATE PRF_Event SET MonthOfEnd = 0 WHERE MonthOfEnd IS NULL;
                UPDATE PRF_Event SET YearOfEnd = 0 WHERE YearOfEnd IS NULL;

                UPDATE PRF_ActionTaken SET DayOfStart = 0 WHERE DayOfStart IS NULL;
                UPDATE PRF_ActionTaken SET MonthOfStart = 0 WHERE MonthOfStart IS NULL;
                UPDATE PRF_ActionTaken SET YearOfStart = 0 WHERE YearOfStart IS NULL;
                UPDATE PRF_ActionTaken SET DayOfEnd = 0 WHERE DayOfEnd IS NULL;
                UPDATE PRF_ActionTaken SET MonthOfEnd = 0 WHERE MonthOfEnd IS NULL;
                UPDATE PRF_ActionTaken SET YearOfEnd = 0 WHERE YearOfEnd IS NULL;

                UPDATE PRF_PersonRelationship SET DayOfStart = 0 WHERE DayOfStart IS NULL;
                UPDATE PRF_PersonRelationship SET MonthOfStart = 0 WHERE MonthOfStart IS NULL;
                UPDATE PRF_PersonRelationship SET YearOfStart = 0 WHERE YearOfStart IS NULL;
                UPDATE PRF_PersonRelationship SET DayOfEnd = 0 WHERE DayOfEnd IS NULL;
                UPDATE PRF_PersonRelationship SET MonthOfEnd = 0 WHERE MonthOfEnd IS NULL;
                UPDATE PRF_PersonRelationship SET YearOfEnd = 0 WHERE YearOfEnd IS NULL;
            ");
        }
    }
}
