using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201303201159)]
    public class AuditPersonPhotos : Migration
    {
        public override void Down()
        {
            Delete.Table("PRF_PersonPhoto_AUD");
        }

        public override void Up()
        {
            Create.Table("PRF_PersonPhoto_AUD")
                .WithColumn("PersonPhotoID").AsInt32().NotNullable()
                .WithColumn("REV").AsInt32()
                .WithColumn("REVTYPE").AsInt16()
                .WithColumn("PersonID").AsInt32().Nullable()
                .WithColumn("PhotoID").AsInt32().Nullable()
                .WithColumn("Archive").AsBoolean().Nullable()
                .WithColumn("Notes").AsString(int.MaxValue).Nullable();
            Create.PrimaryKey().OnTable("PRF_PersonPhoto_AUD").Columns(new string[] { "PersonPhotoID", "REV" });
        }
    }
}
