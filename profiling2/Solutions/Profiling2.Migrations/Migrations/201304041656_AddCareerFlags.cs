using System;
using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201304041656)]
    public class AddCareerFlags : Migration
    {
        public override void Down()
        {
            Delete.Column("Defected").FromTable("PRF_Career");
            Delete.Column("Acting").FromTable("PRF_Career");

            Delete.Column("FunctionID").FromTable("PRF_Career_AUD");
            Delete.Column("Defected").FromTable("PRF_Career_AUD");
            Delete.Column("Acting").FromTable("PRF_Career_AUD");
        }

        public override void Up()
        {
            Alter.Table("PRF_Career").AddColumn("Defected").AsBoolean().NotNullable().WithDefaultValue(false);
            Alter.Table("PRF_Career").AddColumn("Acting").AsBoolean().NotNullable().WithDefaultValue(false);

            Alter.Table("PRF_Career_AUD").AddColumn("FunctionID").AsInt32().Nullable();
            Alter.Table("PRF_Career_AUD").AddColumn("Defected").AsBoolean().Nullable();
            Alter.Table("PRF_Career_AUD").AddColumn("Acting").AsBoolean().Nullable();
        }
    }
}
