using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201407080937)]
    public class AddUniqueIndexToObjectSourceTables : Migration
    {
        public override void Down()
        {
            Delete.UniqueConstraint("UQ_PersonSource").FromTable("PRF_PersonSource");
            Delete.UniqueConstraint("UQ_EventSource").FromTable("PRF_EventSource");
            Delete.UniqueConstraint("UQ_OrganizationSource").FromTable("PRF_OrganizationSource");
        }

        public override void Up()
        {
            // we assume here that any duplicates have already been removed via admin screens (which satisfies audit records in PRF_PersonSource at least)

            Create.UniqueConstraint("UQ_PersonSource").OnTable("PRF_PersonSource").Columns(new string[] { "PersonID", "SourceID" });
            Create.UniqueConstraint("UQ_EventSource").OnTable("PRF_EventSource").Columns(new string[] { "EventID", "SourceID" });
            Create.UniqueConstraint("UQ_OrganizationSource").OnTable("PRF_OrganizationSource").Columns(new string[] { "OrganizationID", "SourceID" });
        }
    }
}
