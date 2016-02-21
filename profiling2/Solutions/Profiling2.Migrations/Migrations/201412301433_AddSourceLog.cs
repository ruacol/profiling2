using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201412301433)]
    public class AddSourceLog : Migration
    {
        public override void Down()
        {
            Delete.Table("PRF_SourceLog");
        }

        public override void Up()
        {
            Create.Table("PRF_SourceLog")
                .WithColumn("SourceLogID").AsInt32().PrimaryKey().Identity().NotNullable()
                .WithColumn("SourceID").AsInt32().NotNullable()
                .WithColumn("LogSummary").AsString().Nullable()
                .WithColumn("Log").AsString(int.MaxValue).Nullable()
                .WithColumn("DateTime").AsDateTime().Nullable();
        }
    }
}
