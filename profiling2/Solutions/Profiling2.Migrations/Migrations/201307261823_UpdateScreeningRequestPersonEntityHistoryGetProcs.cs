using System;
using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    // Remove AdminUser.Archive = 0 condition in these procs; as historical views we want to be able to 
    // see the actions of users even if they are no longer active in the system.
    [Migration(201307261823)]
    public class UpdateScreeningRequestPersonEntityHistoryGetProcs : Migration
    {
        public override void Down()
        {
            
        }

        public override void Up()
        {
            Execute.EmbeddedScript("201307261820_SP_ScreeningRequestPersonEntityHistoryGetOther.sql");
            Execute.EmbeddedScript("201307261822_SP_ScreeningRequestPersonEntityHistoryGet.sql");
        }
    }
}
