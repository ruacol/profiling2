using System;
using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201507101641)]
    public class AddEventApprovalToAndFixEventMergeSP : Migration
    {
        public override void Down()
        {
            
        }

        public override void Up()
        {
            Execute.EmbeddedScript("201507101641_FixEventMergeSP.sql");
        }
    }
}
