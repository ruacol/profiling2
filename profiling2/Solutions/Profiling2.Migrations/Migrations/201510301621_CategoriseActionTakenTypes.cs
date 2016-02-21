using System;
using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201510301621)]
    public class CategoriseActionTakenTypes : Migration
    {
        public override void Down()
        {
            if (Schema.Table("PRF_ActionTakenType").Column("IsRemedial").Exists())
                Delete.Column("IsRemedial").FromTable("PRF_ActionTakenType");
            if (Schema.Table("PRF_ActionTakenType").Column("IsDisciplinary").Exists())
                Delete.Column("IsDisciplinary").FromTable("PRF_ActionTakenType");
        }

        public override void Up()
        {
            if (!Schema.Table("PRF_ActionTakenType").Column("IsRemedial").Exists())
                Alter.Table("PRF_ActionTakenType").AddColumn("IsRemedial").AsBoolean().WithDefaultValue(false).NotNullable();
            if (!Schema.Table("PRF_ActionTakenType").Column("IsDisciplinary").Exists())
                Alter.Table("PRF_ActionTakenType").AddColumn("IsDisciplinary").AsBoolean().WithDefaultValue(false).NotNullable();
        }
    }
}
