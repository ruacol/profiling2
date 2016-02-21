using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201310311020)]
    public class AddOriginalFileDataColumn : Migration
    {
        public override void Down()
        {
            Delete.Column("OriginalFileData").FromTable("PRF_Source");
        }

        public override void Up()
        {
            Alter.Table("PRF_Source").AddColumn("OriginalFileData").AsBinary(int.MaxValue).Nullable();
        }
    }
}
