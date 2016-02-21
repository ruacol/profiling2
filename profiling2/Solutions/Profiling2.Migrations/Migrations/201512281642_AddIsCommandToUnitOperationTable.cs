using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201512281642)]
    public class AddIsCommandToUnitOperationTable : Migration
    {
        public override void Down()
        {
            if (Schema.Table("PRF_UnitOperation").Column("IsCommandUnit").Exists())
                Delete.Column("IsCommandUnit").FromTable("PRF_UnitOperation");

            if (Schema.Table("PRF_UnitOperation_AUD").Column("IsCommandUnit").Exists())
                Delete.Column("IsCommandUnit").FromTable("PRF_UnitOperation_AUD");
        }

        public override void Up()
        {
            if (!Schema.Table("PRF_UnitOperation").Column("IsCommandUnit").Exists())
                Alter.Table("PRF_UnitOperation").AddColumn("IsCommandUnit").AsBoolean().NotNullable().WithDefaultValue(false);

            if (!Schema.Table("PRF_UnitOperation_AUD").Column("IsCommandUnit").Exists())
                Alter.Table("PRF_UnitOperation_AUD").AddColumn("IsCommandUnit").AsBoolean().Nullable();
        }
    }
}
