using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201412231709)]
    public class AddViolationConditionalityInterest : Migration
    {
        public override void Down()
        {
            Delete.Column("ConditionalityInterest").FromTable("PRF_Violation_AUD");
            Delete.Column("ConditionalityInterest").FromTable("PRF_Violation");
        }

        public override void Up()
        {
            Alter.Table("PRF_Violation").AddColumn("ConditionalityInterest").AsBoolean().WithDefaultValue(true).NotNullable();
            Alter.Table("PRF_Violation_AUD").AddColumn("ConditionalityInterest").AsBoolean().WithDefaultValue(true).NotNullable();

            Execute.Sql(@"
                UPDATE PRF_Violation SET ConditionalityInterest = 0
                WHERE ViolationID NOT IN (71, 60, 3, 52, 53, 44, 34, 49, 8, 5, 43, 37, 36, 33, 67, 35, 4, 47, 1, 57, 68, 41, 59, 79, 58, 70,
                12, 56, 69, 78, 7, 40, 48, 81, 45)
            ");
        }
    }
}
