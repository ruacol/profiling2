using System;
using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201306051648)]
    public class RemoveFunctionTable : Migration
    {
        public override void Down()
        {
            
        }

        public override void Up()
        {
            Delete.ForeignKey().FromTable("PRF_Career").ForeignColumn("FunctionID").ToTable("PRF_Function").PrimaryColumn("FunctionID");
            Delete.Column("FunctionID").FromTable("PRF_Career");
            Delete.Column("FunctionID").FromTable("PRF_Career_AUD");
            Delete.Table("PRF_Function");
        }
    }
}
