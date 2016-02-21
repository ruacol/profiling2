using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201309180853)]
    public class UpdateRequestStatusNames : Migration
    {
        public override void Down()
        {
            Execute.Sql(@"
                UPDATE SCR_RequestStatus SET RequestStatusName = 'Sent to ODSRSG-RoL for validation' WHERE RequestStatusID = 2;
                UPDATE SCR_RequestStatus SET RequestStatusName = 'Sent to Conditionality Participants for screening' WHERE RequestStatusID = 3;
                UPDATE SCR_RequestStatus SET RequestStatusName = 'Sent to ODSRSG-RoL for consolidation of screening inputs' WHERE RequestStatusID = 4;
                UPDATE SCR_RequestStatus SET RequestStatusName = 'Sent to SMG for final decision' WHERE RequestStatusID = 12;
            ");
        }

        public override void Up()
        {
            Execute.Sql(@"
                UPDATE SCR_RequestStatus SET RequestStatusName = 'Sent for validation' WHERE RequestStatusID = 2;
                UPDATE SCR_RequestStatus SET RequestStatusName = 'Sent for screening' WHERE RequestStatusID = 3;
                UPDATE SCR_RequestStatus SET RequestStatusName = 'Sent for consolidation' WHERE RequestStatusID = 4;
                UPDATE SCR_RequestStatus SET RequestStatusName = 'Sent for final decision' WHERE RequestStatusID = 12;
            ");
        }
    }
}
