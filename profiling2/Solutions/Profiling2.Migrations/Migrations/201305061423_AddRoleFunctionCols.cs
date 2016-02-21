using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201305061423)]
    public class AddRoleFunctionCols : Migration
    {
        public override void Down()
        {
            Delete.Column("RoleNameFr").FromTable("PRF_Role");
            Delete.Column("Description").FromTable("PRF_Role");

            Delete.Column("FunctionNameFr").FromTable("PRF_Function");
            Delete.Column("Description").FromTable("PRF_Function");
        }

        public override void Up()
        {
            Alter.Table("PRF_Role").AddColumn("RoleNameFr").AsString().Nullable();
            Alter.Table("PRF_Role").AddColumn("Description").AsString(int.MaxValue).Nullable();

            Alter.Table("PRF_Function").AddColumn("FunctionNameFr").AsString().Nullable();
            Alter.Table("PRF_Function").AddColumn("Description").AsString().Nullable();
        }
    }
}
