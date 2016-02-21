using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201412031648)]
    public class ChangeHashColumnToBinary : Migration
    {
        public override void Down()
        {

        }

        public override void Up()
        {
            Alter.Table("PRF_Source").AddColumn("HashBinary").AsBinary().Nullable();

            Execute.Sql("UPDATE [PRF_Source] SET HashBinary = CAST(Hash AS varbinary(max)) WHERE Hash IS NOT NULL");

            Delete.Column("Hash").FromTable("PRF_Source");

            Rename.Column("HashBinary").OnTable("PRF_Source").To("Hash");
        }
    }
}
