using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201411060831)]
    public class AddEmailColumnToUser : Migration
    {
        public override void Down()
        {
            Delete.Column("Email").FromTable("PRF_AdminUser");
        }

        public override void Up()
        {
            Alter.Table("PRF_AdminUser").AddColumn("Email").AsString().Nullable();
        }
    }
}
