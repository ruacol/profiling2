using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201303191818)]
    public class FixREVINFOPrimaryKey : Migration
    {
        public override void Down()
        {
            Rename.Column("REVINFOID").OnTable("REVINFO").To("Id");
        }

        public override void Up()
        {
            Rename.Column("Id").OnTable("REVINFO").To("REVINFOID");
        }
    }
}
