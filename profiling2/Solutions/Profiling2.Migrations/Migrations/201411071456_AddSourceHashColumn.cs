using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201411071456)]
    public class AddSourceHashColumn : Migration
    {
        public override void Down()
        {
            Delete.Column("Hash").FromTable("PRF_Source");
        }

        public override void Up()
        {
            Alter.Table("PRF_Source").AddColumn("Hash").AsString().Nullable();
        }
    }
}
