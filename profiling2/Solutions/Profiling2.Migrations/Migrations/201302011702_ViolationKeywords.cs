using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201302011702)]
    public class ViolationKeywords : Migration
    {
        public override void Down()
        {
            Delete.Column("Keywords").FromTable("PRF_Violation");
        }

        public override void Up()
        {
            Alter.Table("PRF_Violation").AddColumn("Keywords").AsString().Nullable();
        }
    }
}
