using FluentMigrator;

namespace Profiling2.Migrations
{
    [Migration(201306251237)]
    public class MoveUnitHierarchyOrganizationAudit : Migration
    {
        public override void Down()
        {
            Delete.Column("OrganizationID").FromTable("PRF_Unit_AUD");
        }

        public override void Up()
        {
            Create.Column("OrganizationID").OnTable("PRF_Unit_AUD").AsInt32().Nullable();
        }
    }
}
