using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201510301152)]
    public class AddPersonResponsibilityType : Migration
    {
        public override void Down()
        {
            Delete.FromTable("PRF_PersonResponsibilityType").Row(new { PersonResponsibilityTypeName = "Indirect command" });
        }

        public override void Up()
        {
            Insert.IntoTable("PRF_PersonResponsibilityType").Row(new { PersonResponsibilityTypeName = "Indirect command" });
        }
    }
}
