using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201409101412)]
    public class AddSourceIndexLog : Migration
    {
        public override void Down()
        {
            Delete.Table("PRF_SourceIndexLog");
        }

        public override void Up()
        {
            Create.Table("PRF_SourceIndexLog")
                .WithColumn("SourceIndexLogID").AsInt32().PrimaryKey().Identity().NotNullable()
                .WithColumn("SourceID").AsInt32().NotNullable()
                .WithColumn("LogSummary").AsString().Nullable()
                .WithColumn("Log").AsString(int.MaxValue).Nullable()
                .WithColumn("DateTime").AsDateTime().Nullable();
        }
    }
}
