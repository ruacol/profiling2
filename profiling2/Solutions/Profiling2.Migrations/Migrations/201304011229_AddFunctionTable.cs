using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201304011229)]
    public class AddFunctionTable : Migration
    {
        public override void Down()
        {
            Delete.ForeignKey("FK_PRF_Career_FunctionID_PRF_Function_FunctionID").OnTable("PRF_Career");

            Delete.Column("FunctionID").FromTable("PRF_Career");

            Delete.Table("PRF_Function");
        }

        public override void Up()
        {
            Create.Table("PRF_Function")
                .WithColumn("FunctionID").AsInt32().PrimaryKey().Identity().NotNullable()
                .WithColumn("FunctionName").AsString().NotNullable()
                .WithColumn("Aliases").AsString().Nullable()
                .WithColumn("Archive").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("Notes").AsString().Nullable();

            Alter.Table("PRF_Career").AddColumn("FunctionID").AsInt32().Nullable().ForeignKey("PRF_Function", "FunctionID");
        }
    }
}
