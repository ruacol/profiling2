using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201409011617)]
    public class AddSourceReadOnly : Migration
    {
        public override void Down()
        {
            Delete.Column("IsReadOnly").FromTable("PRF_Source");
            Delete.Column("IsReadOnly").FromTable("PRF_FeedingSource");
        }

        public override void Up()
        {
            Alter.Table("PRF_Source").AddColumn("IsReadOnly").AsBoolean().WithDefaultValue(false).NotNullable();
            Alter.Table("PRF_FeedingSource").AddColumn("IsReadOnly").AsBoolean().WithDefaultValue(false).NotNullable();
        }
    }
}
