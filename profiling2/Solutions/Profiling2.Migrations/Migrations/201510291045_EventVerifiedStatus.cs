using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201510291045)]
    public class EventVerifiedStatus : Migration
    {
        public override void Down()
        {
            if (Schema.Table("PRF_EventVerifiedStatus").Exists())
            {
                if (Schema.Table("PRF_Event").Column("EventVerifiedStatusID").Exists())
                {
                    Delete.ForeignKey().FromTable("PRF_Event").ForeignColumn("EventVerifiedStatusID").ToTable("PRF_EventVerifiedStatus").PrimaryColumn("EventVerifiedStatusID");
                    Delete.Column("EventVerifiedStatusID").FromTable("PRF_Event");

                    if (Schema.Table("PRF_Event_AUD").Column("EventVerifiedStatusID").Exists())
                    {
                        Delete.Column("EventVerifiedStatusID").FromTable("PRF_Event_AUD");
                    }
                }

                Delete.Table("PRF_EventVerifiedStatus");
            }
        }

        public override void Up()
        {
            if (!Schema.Table("PRF_EventVerifiedStatus").Exists())
            {
                Create.Table("PRF_EventVerifiedStatus")
                    .WithColumn("EventVerifiedStatusID").AsInt32().PrimaryKey().Identity().NotNullable()
                    .WithColumn("EventVerifiedStatusName").AsString(int.MaxValue).NotNullable()
                    .WithColumn("Notes").AsString(int.MaxValue).Nullable()
                    .WithColumn("Archive").AsBoolean().WithDefaultValue(false).NotNullable();

                if (!Schema.Table("PRF_Event").Column("EventVerifiedStatusID").Exists())
                {
                    Alter.Table("PRF_Event").AddColumn("EventVerifiedStatusID").AsInt32().Nullable();
                    Create.ForeignKey().FromTable("PRF_Event").ForeignColumn("EventVerifiedStatusID").ToTable("PRF_EventVerifiedStatus").PrimaryColumn("EventVerifiedStatusID");

                    if (!Schema.Table("PRF_Event_AUD").Column("EventVerifiedStatusID").Exists())
                    {
                        Alter.Table("PRF_Event_AUD").AddColumn("EventVerifiedStatusID").AsInt32().Nullable();
                    }
                }

                Insert.IntoTable("PRF_EventVerifiedStatus").Row(new { EventVerifiedStatusName = "JHRO-verified" });
                Insert.IntoTable("PRF_EventVerifiedStatus").Row(new { EventVerifiedStatusName = "not JHRO-verified" });
                Insert.IntoTable("PRF_EventVerifiedStatus").Row(new { EventVerifiedStatusName = "allegation" });
            }
        }
    }
}
