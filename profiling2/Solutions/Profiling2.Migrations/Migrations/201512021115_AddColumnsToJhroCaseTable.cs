using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201512021115)]
    public class AddColumnsToJhroCaseTable : Migration
    {
        public override void Down()
        {
            if (Schema.Table("PRF_JhroCase_AUD").Exists())
            {
                Delete.Table("PRF_JhroCase_AUD");
            }

            if (Schema.Table("PRF_JhroCase").Column("EventID").Exists())
            {
                Delete.ForeignKey().FromTable("PRF_JhroCase").ForeignColumn("EventID").ToTable("PRF_Event").PrimaryColumn("EventID");

                Delete.Column("EventID").FromTable("PRF_JhroCase");
            }

            if (Schema.Table("PRF_JhroCase").Column("HrdbContentsSerialized").Exists())
            {
                Delete.Column("HrdbContentsSerialized").FromTable("PRF_JhroCase");
            }
        }

        public override void Up()
        {
            if (!Schema.Table("PRF_JhroCase").Column("EventID").Exists())
            {
                Alter.Table("PRF_JhroCase").AddColumn("EventID").AsInt32().Nullable();

                Create.ForeignKey().FromTable("PRF_JhroCase").ForeignColumn("EventID").ToTable("PRF_Event").PrimaryColumn("EventID");
            }

            if (!Schema.Table("PRF_JhroCase").Column("HrdbContentsSerialized").Exists())
            {
                Alter.Table("PRF_JhroCase").AddColumn("HrdbContentsSerialized").AsBinary(int.MaxValue).Nullable();
            }

            if (!Schema.Table("PRF_JhroCase_AUD").Exists())
            {
                Create.Table("PRF_JhroCase_AUD")
                    .WithColumn("JhroCaseID").AsInt32().NotNullable()
                    .WithColumn("REV").AsInt32()
                    .WithColumn("REVTYPE").AsInt16()
                    .WithColumn("CaseNumber").AsString(255).Nullable()
                    .WithColumn("EventID").AsInt32().Nullable();
                Create.PrimaryKey().OnTable("PRF_JhroCase_AUD").Columns(new string[] { "JhroCaseID", "REV" });
            }
        }
    }
}
