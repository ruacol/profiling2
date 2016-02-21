using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201512040915)]
    public class AddNominatedColumnToCareerTable : Migration
    {
        public override void Down()
        {
            if (Schema.Table("PRF_Career_AUD").Column("Nominated").Exists())
            {
                Delete.Column("Nominated").FromTable("PRF_Career_AUD");
            }
            if (Schema.Table("PRF_Career").Column("Nominated").Exists())
            {
                Delete.Column("Nominated").FromTable("PRF_Career");
            }
        }

        public override void Up()
        {
            if (!Schema.Table("PRF_Career").Column("Nominated").Exists())
            {
                Alter.Table("PRF_Career").AddColumn("Nominated").AsBoolean().WithDefaultValue(false).NotNullable();
            }
            if (!Schema.Table("PRF_Career_AUD").Column("Nominated").Exists())
            {
                Alter.Table("PRF_Career_AUD").AddColumn("Nominated").AsBoolean().WithDefaultValue(false).Nullable();
            }
        }
    }
}
