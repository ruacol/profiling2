using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201305271039)]
    public class NullableDatesPersonRelationship : Migration
    {
        public override void Down()
        {
            
        }

        public override void Up()
        {
            Alter.Column("DayOfStart").OnTable("PRF_PersonRelationship").AsInt32().Nullable();
            Alter.Column("MonthOfStart").OnTable("PRF_PersonRelationship").AsInt32().Nullable();
            Alter.Column("YearOfStart").OnTable("PRF_PersonRelationship").AsInt32().Nullable();
            Alter.Column("DayOfEnd").OnTable("PRF_PersonRelationship").AsInt32().Nullable();
            Alter.Column("MonthOfEnd").OnTable("PRF_PersonRelationship").AsInt32().Nullable();
            Alter.Column("YearOfEnd").OnTable("PRF_PersonRelationship").AsInt32().Nullable();
        }
    }
}
