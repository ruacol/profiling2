using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201311121416)]
    public class CreateJhroDbTables : Migration
    {
        public override void Down()
        {
            Delete.ForeignKey().FromTable("PRF_Source").ForeignColumn("JhroCaseID").ToTable("PRF_JhroCase").PrimaryColumn("JhroCaseID");
            Delete.Column("JhroCaseID").FromTable("PRF_Source");
            Delete.Table("PRF_SourceRelationship");
            Delete.Table("PRF_JhroCase");
        }

        public override void Up()
        {
            Create.Table("PRF_JhroCase")
                .WithColumn("JhroCaseID").AsInt32().PrimaryKey().Identity().NotNullable()
                .WithColumn("CaseNumber").AsString().NotNullable().Unique();

            Create.Table("PRF_SourceRelationship")
                .WithColumn("SourceRelationshipID").AsInt32().PrimaryKey().Identity().NotNullable()
                .WithColumn("ParentSourcePath").AsString().Nullable()
                .WithColumn("ParentSourceID").AsInt32().Nullable()
                .WithColumn("ChildSourceID").AsInt32().NotNullable();

            Create.ForeignKey().FromTable("PRF_SourceRelationship").ForeignColumn("ParentSourceID").ToTable("PRF_Source").PrimaryColumn("SourceID");
            Create.ForeignKey().FromTable("PRF_SourceRelationship").ForeignColumn("ChildSourceID").ToTable("PRF_Source").PrimaryColumn("SourceID");

            Alter.Table("PRF_Source").AddColumn("JhroCaseID").AsInt32().Nullable();
            Create.ForeignKey().FromTable("PRF_Source").ForeignColumn("JhroCaseID").ToTable("PRF_JhroCase").PrimaryColumn("JhroCaseID");
        }
    }
}
