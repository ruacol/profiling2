using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201308261631)]
    public class RemoveLocationProvince : Migration
    {
        public override void Down()
        {
            Alter.Table("PRF_Location").AddColumn("Province").AsString(500).Nullable();
        }

        public override void Up()
        {
            Delete.Column("Province").FromTable("PRF_Location");
        }
    }
}
