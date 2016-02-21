using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201301231524)]
    public class CreatedTimestamps : Migration
    {

        public override void Down()
        {
            foreach (string table in new string[] { "PRF_Person", "PRF_Organization", "PRF_Event", "PRF_Career", "PRF_PersonResponsibility" })
                Delete.Column("Created").FromTable(table);
        }

        public override void Up()
        {
            foreach (string table in new string[] { "PRF_Person", "PRF_Organization", "PRF_Event", "PRF_Career", "PRF_PersonResponsibility" })
                Alter.Table(table).AddColumn("Created").AsDateTime().Nullable();
        }
    }
}
