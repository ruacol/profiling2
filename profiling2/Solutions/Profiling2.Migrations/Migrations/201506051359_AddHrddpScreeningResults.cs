using System;
using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201506051359)]
    public class AddHrddpScreeningResults : Migration
    {
        public override void Down()
        {
            Delete.FromTable("SCR_ScreeningResult").Row(new { ScreeningResultName = "Background checked" });
            Delete.FromTable("SCR_ScreeningResult").Row(new { ScreeningResultName = "Risk assessed" }); 
        }

        public override void Up()
        {
            Insert.IntoTable("SCR_ScreeningResult").Row(new { ScreeningResultName = "Background checked" });
            Insert.IntoTable("SCR_ScreeningResult").Row(new { ScreeningResultName = "Risk assessed" });
        }
    }
}
