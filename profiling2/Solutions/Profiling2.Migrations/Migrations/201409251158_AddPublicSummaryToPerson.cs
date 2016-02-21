using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201409251158)]
    public class AddPublicSummaryToPerson : Migration
    {
        public override void Down()
        {
            Delete.Column("PublicSummary").FromTable("PRF_Person");
            Delete.Column("PublicSummaryDate").FromTable("PRF_Person");

            Delete.Column("PublicSummary").FromTable("PRF_Person_AUD");
            Delete.Column("PublicSummaryDate").FromTable("PRF_Person_AUD");
        }

        public override void Up()
        {
            Alter.Table("PRF_Person")
                .AddColumn("PublicSummary").AsString(int.MaxValue).Nullable()
                .AddColumn("PublicSummaryDate").AsDateTime().Nullable();

            Alter.Table("PRF_Person_AUD")
                .AddColumn("PublicSummary").AsString(int.MaxValue).Nullable()
                .AddColumn("PublicSummaryDate").AsDateTime().Nullable();
        }
    }
}
