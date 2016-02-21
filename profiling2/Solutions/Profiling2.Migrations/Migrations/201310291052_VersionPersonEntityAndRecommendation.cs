using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201310291052)]
    public class VersionPersonEntityAndRecommendation : Migration
    {
        public override void Down()
        {
            Delete.Column("Version").FromTable("SCR_ScreeningRequestPersonEntity");
            Delete.Column("Version").FromTable("SCR_ScreeningRequestPersonRecommendation");
        }

        public override void Up()
        {
            Alter.Table("SCR_ScreeningRequestPersonEntity").AddColumn("Version").AsInt32().NotNullable().WithDefaultValue(0);
            Alter.Table("SCR_ScreeningRequestPersonRecommendation").AddColumn("Version").AsInt32().NotNullable().WithDefaultValue(0);
        }
    }
}
