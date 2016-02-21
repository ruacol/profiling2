using System;
using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201505081556)]
    public class DeduplicateEventViolations : Migration
    {
        public override void Down()
        {
            Delete.UniqueConstraint("UQ_EventViolation").FromTable("PRF_EventViolation");
        }

        public override void Up()
        {
            Execute.Sql(@"
                DELETE FROM PRF_EventViolation
                WHERE EventViolationID IN
                (
	                SELECT EventViolationID FROM
	                (
		                SELECT MIN(EventViolationID) AS EventViolationID, EventID, ViolationID
                        FROM PRF_EventViolation
                        GROUP BY EventID, ViolationID
                        HAVING COUNT(EventID) > 1
                    ) AS x
                )
            ");

            Create.UniqueConstraint("UQ_EventViolation").OnTable("PRF_EventViolation").Columns(new string[] { "EventID", "ViolationID" });
        }
    }
}
