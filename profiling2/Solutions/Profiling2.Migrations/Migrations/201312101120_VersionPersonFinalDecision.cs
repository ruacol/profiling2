using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201312101120)]
    public class VersionPersonFinalDecision : Migration
    {
        public override void Down()
        {
            Delete.Column("Version").FromTable("SCR_ScreeningRequestPersonFinalDecision");
        }

        public override void Up()
        {
            Alter.Table("SCR_ScreeningRequestPersonFinalDecision").AddColumn("Version").AsInt32().NotNullable().WithDefaultValue(0);
        }
    }
}
