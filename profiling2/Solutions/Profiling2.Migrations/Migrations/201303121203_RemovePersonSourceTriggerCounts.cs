﻿using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201303121203)]
    public class RemovePersonSourceTriggerCounts : Migration
    {
        public override void Down()
        {
            //throw new NotImplementedException();
        }

        public override void Up()
        {
            Execute.EmbeddedScript("201303121203_PersonSourceDeleteTrigger.sql");
            Execute.EmbeddedScript("201303121203_PersonSourceUpdateTrigger.sql");
        }
    }
}
