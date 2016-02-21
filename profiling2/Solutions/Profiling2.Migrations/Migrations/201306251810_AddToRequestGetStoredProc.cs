using System;
using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201306251810)]
    public class AddToRequestGetStoredProc : Migration
    {
        public override void Down()
        {
            
        }

        public override void Up()
        {
            Execute.EmbeddedScript("201306251809_SP_RequestGet.sql");
        }
    }
}
