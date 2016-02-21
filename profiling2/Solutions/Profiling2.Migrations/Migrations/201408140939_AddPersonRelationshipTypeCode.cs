using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201408140939)]
    public class AddPersonRelationshipTypeCode : Migration
    {
        public override void Down()
        {
            Delete.Index("UQ_PersonRelationshipTypeCode").OnTable("PRF_PersonRelationshipType").OnColumn("Code");
            Delete.Column("Code").FromTable("PRF_PersonRelationshipType");
            Delete.Column("Code").FromTable("PRF_PersonRelationshipType_AUD");
        }

        public override void Up()
        {
            Alter.Table("PRF_PersonRelationshipType").AddColumn("Code").AsString().NotNullable().WithDefaultValue(string.Empty);
            Alter.Table("PRF_PersonRelationshipType_AUD").AddColumn("Code").AsString().Nullable();

            Execute.Sql(@"
                UPDATE PRF_PersonRelationshipType SET Code = REPLACE(UPPER(PersonRelationshipTypeName), ' ', '_');
            ");

            Create.Index("UQ_PersonRelationshipTypeCode").OnTable("PRF_PersonRelationshipType").OnColumn("Code").Unique();
        }
    }
}
