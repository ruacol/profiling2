using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201305101539)]
    public class DenullLocation : Migration
    {
        public override void Down()
        {
            Alter.Column("Province").OnTable("PRF_Location").AsString(500).NotNullable();
            Alter.Column("Territory").OnTable("PRF_Location").AsString(500).NotNullable();
            Alter.Column("Town").OnTable("PRF_Location").AsString(500).NotNullable();
        }

        public override void Up()
        {
            Alter.Column("Province").OnTable("PRF_Location").AsString(500).Nullable();
            Alter.Column("Territory").OnTable("PRF_Location").AsString(500).Nullable();
            Alter.Column("Town").OnTable("PRF_Location").AsString(500).Nullable();
        }
    }
}
