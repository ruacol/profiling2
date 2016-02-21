using System;
using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201503251040)]
    public class AddSourceAttributes : Migration
    {
        public override void Down()
        {
            Delete.Column("IsPublic").FromTable("PRF_Source");
            Delete.Table("PRF_SourceAuthorSource");
            Delete.Table("PRF_SourceAuthor");
        }

        public override void Up()
        {
            Alter.Table("PRF_Source").AddColumn("IsPublic").AsBoolean().WithDefaultValue(false).NotNullable();

            Create.Table("PRF_SourceAuthor")
                .WithColumn("SourceAuthorID").AsInt32().PrimaryKey().Identity().NotNullable()
                .WithColumn("Author").AsString().NotNullable();

            Create.Table("PRF_SourceAuthorSource")
                .WithColumn("SourceAuthorSourceID").AsInt32().PrimaryKey().Identity().NotNullable()
                .WithColumn("SourceAuthorID").AsInt32().NotNullable()
                .WithColumn("SourceID").AsInt32().NotNullable();

            Create.ForeignKey()
                .FromTable("PRF_SourceAuthorSource").ForeignColumn("SourceAuthorID")
                .ToTable("PRF_SourceAuthor").PrimaryColumn("SourceAuthorID");

            Create.ForeignKey()
                .FromTable("PRF_SourceAuthorSource").ForeignColumn("SourceID")
                .ToTable("PRF_Source").PrimaryColumn("SourceID");
                
        }
    }
}
