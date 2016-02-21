using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201301251235)]
    public class EventViolation : Migration
    {
        public override void Down()
        {
            Delete.Table("PRF_EventViolation");
        }

        public override void Up()
        {
            Create.Table("PRF_EventViolation")
                .WithColumn("EventViolationID").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("EventID").AsInt32().NotNullable()
                .WithColumn("ViolationID").AsInt32().NotNullable();

            Create.ForeignKey()
                .FromTable("PRF_EventViolation").ForeignColumn("EventID")
                .ToTable("PRF_Event").PrimaryColumn("EventID");

            Create.ForeignKey()
                .FromTable("PRF_EventViolation").ForeignColumn("ViolationID")
                .ToTable("PRF_Violation").PrimaryColumn("ViolationID");
        }
    }
}
