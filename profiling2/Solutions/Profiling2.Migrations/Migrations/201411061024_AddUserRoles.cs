using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201411061024)]
    public class AddUserRoles : Migration
    {
        public override void Down()
        {
            Execute.Sql(@"DELETE FROM PRF_AdminRole");
        }

        public override void Up()
        {
            Execute.Sql(@"
                INSERT INTO PRF_AdminRole (Name) VALUES ('ProfilingInternational');
                INSERT INTO PRF_AdminRole (Name) VALUES ('ProfilingNational');
                INSERT INTO PRF_AdminRole (Name) VALUES ('ProfilingAdmin');
                INSERT INTO PRF_AdminRole (Name) VALUES ('ProfilingLimitedPersonEdit');
                INSERT INTO PRF_AdminRole (Name) VALUES ('ProfilingReadOnlyPersonView');
                INSERT INTO PRF_AdminRole (Name) VALUES ('ScreeningRequestInitiator');
                INSERT INTO PRF_AdminRole (Name) VALUES ('ScreeningRequestConsolidator');
                INSERT INTO PRF_AdminRole (Name) VALUES ('ScreeningRequestConditionalityParticipant');
                INSERT INTO PRF_AdminRole (Name) VALUES ('ScreeningRequestAggregator');
                INSERT INTO PRF_AdminRole (Name) VALUES ('ScreeningRequestFinalDecider');
            ");
        }
    }
}
