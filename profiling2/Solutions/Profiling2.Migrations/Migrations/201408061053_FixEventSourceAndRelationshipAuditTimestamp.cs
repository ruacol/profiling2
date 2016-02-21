using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201408061053)]
    public class FixEventSourceAndRelationshipAuditTimestamp : Migration
    {
        public override void Down()
        {
            
        }

        public override void Up()
        {/* this only need to run once
            Execute.Sql(@"
                INSERT INTO REVINFO (REVTSTMP, REV, UserName) VALUES (GETDATE(), 0, 'GEN-01328');

                UPDATE PRF_EventSource_AUD SET REV = 
                (
                    SELECT TOP 1 REVINFOID FROM REVINFO ORDER BY REVINFOID DESC
                )
                WHERE REV = 100;

                UPDATE PRF_EventRelationship_AUD SET REV = 
                (
                    SELECT TOP 1 REVINFOID FROM REVINFO ORDER BY REVINFOID DESC
                )
                WHERE REV = 100;
            ");
          */
        }
    }
}
