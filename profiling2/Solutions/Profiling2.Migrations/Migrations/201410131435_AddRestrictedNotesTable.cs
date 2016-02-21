using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201410131435)]
    public class AddRestrictedNotesTable : Migration
    {
        public override void Down()
        {
            Delete.ForeignKey().FromTable("PRF_PersonRestrictedNote").ForeignColumn("PersonID").ToTable("PRF_Person").PrimaryColumn("PersonID");
            Delete.Table("PRF_PersonRestrictedNote_AUD");
            Delete.Table("PRF_PersonRestrictedNote");
        }

        public override void Up()
        {
            Create.Table("PRF_PersonRestrictedNote")
                .WithColumn("PersonRestrictedNoteID").AsInt32().PrimaryKey().Identity().NotNullable()
                .WithColumn("PersonID").AsInt32().NotNullable()
                .WithColumn("Note").AsString(int.MaxValue).NotNullable();

            Create.ForeignKey().FromTable("PRF_PersonRestrictedNote").ForeignColumn("PersonID").ToTable("PRF_Person").PrimaryColumn("PersonID");

            Create.Table("PRF_PersonRestrictedNote_AUD")
                .WithColumn("PersonRestrictedNoteID").AsInt32().NotNullable()
                .WithColumn("REV").AsInt32()
                .WithColumn("REVTYPE").AsInt16()
                .WithColumn("PersonID").AsInt32().Nullable()
                .WithColumn("Note").AsString(int.MaxValue).Nullable();
            Create.PrimaryKey().OnTable("PRF_PersonRestrictedNote_AUD").Columns(new string[] { "PersonRestrictedNoteID", "REV" });
        }
    }
}
