using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201501050951)]
    public class LengthenSourceLogColumn : Migration
    {
        public override void Down()
        {

        }

        public override void Up()
        {
            Alter.Column("LogSummary").OnTable("PRF_SourceLog").AsString(int.MaxValue).Nullable();
        }
    }
}
