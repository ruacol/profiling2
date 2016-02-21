using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201301251202)]
    public class Violation : Migration
    {
        public override void Down()
        {
            if (Schema.Table("PRF_Violation").Exists())
                Delete.Table("PRF_Violation");
        }

        public override void Up()
        {
            if (!Schema.Table("PRF_Violation").Exists())
            {
                Create.Table("PRF_Violation")
                    .WithColumn("ViolationID").AsInt32().NotNullable().PrimaryKey().Identity()
                    .WithColumn("Name").AsString().NotNullable().Unique()
                    .WithColumn("Description").AsString(500).Nullable()
                    .WithColumn("ParentViolationID").AsInt32().Nullable();

                Create.ForeignKey()
                    .FromTable("PRF_Violation").ForeignColumn("ParentViolationID")
                    .ToTable("PRF_Violation").PrimaryColumn("ViolationID");
            }
        }
    }
}
