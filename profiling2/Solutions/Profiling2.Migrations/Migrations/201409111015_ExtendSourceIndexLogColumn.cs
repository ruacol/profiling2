using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201409111015)]
    public class ExtendSourceIndexLogColumn : Migration
    {
        public override void Down()
        {
            
        }

        public override void Up()
        {
            Alter.Column("LogSummary").OnTable("PRF_SourceIndexLog").AsString(int.MaxValue).Nullable();
        }
    }
}
