using System;
using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201302111847)]
    public class LengthenViolationKeywords : Migration
    {
        public override void Down()
        {
            Alter.Table("PRF_Violation").AlterColumn("Keywords").AsString().Nullable();
        }

        public override void Up()
        {
            Alter.Table("PRF_Violation").AlterColumn("Keywords").AsString(Int32.MaxValue).Nullable();
        }
    }
}
