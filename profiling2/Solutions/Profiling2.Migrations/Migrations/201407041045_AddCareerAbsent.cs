using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201407041045)]
    public class AddCareerAbsent : Migration
    {
        public override void Down()
        {
            Delete.Column("Absent").FromTable("PRF_Career");

            Delete.Column("Absent").FromTable("PRF_Career_AUD");
        }

        public override void Up()
        {
            Alter.Table("PRF_Career").AddColumn("Absent").AsBoolean().NotNullable().WithDefaultValue(false);

            Alter.Table("PRF_Career_AUD").AddColumn("Absent").AsBoolean().Nullable();
        }
    }
}
