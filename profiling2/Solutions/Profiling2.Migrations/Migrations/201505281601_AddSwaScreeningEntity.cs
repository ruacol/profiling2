using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201505281601)]
    public class AddSwaScreeningEntity : Migration
    {
        public override void Down()
        {
            Delete.FromTable("SCR_ScreeningEntity").Row(new { ScreeningEntityName = "SWA" });
        }

        public override void Up()
        {
            Insert.IntoTable("SCR_ScreeningEntity").Row(new { ScreeningEntityName = "SWA" });
        }
    }
}
