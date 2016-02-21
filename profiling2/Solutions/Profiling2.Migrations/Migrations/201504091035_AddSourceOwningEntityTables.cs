using System;
using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201504091035)]
    public class AddSourceOwningEntityTables : Migration
    {
        public override void Down()
        {
            Delete.ForeignKey()
                .FromTable("PRF_AdminUserAffiliation").ForeignColumn("SourceOwningEntityID")
                .ToTable("PRF_SourceOwningEntity").PrimaryColumn("SourceOwningEntityID");
            Delete.ForeignKey()
                .FromTable("PRF_AdminUserAffiliation").ForeignColumn("AdminUserID")
                .ToTable("PRF_AdminUser").PrimaryColumn("AdminUserID");
            Delete.Table("PRF_AdminUserAffiliation");

            Delete.ForeignKey()
                .FromTable("PRF_SourceOwner").ForeignColumn("SourceID")
                .ToTable("PRF_Source").PrimaryColumn("SourceID");
            Delete.ForeignKey()
                .FromTable("PRF_SourceOwner").ForeignColumn("SourceOwningEntityID")
                .ToTable("PRF_SourceOwningEntity").PrimaryColumn("SourceOwningEntityID");
            Delete.Table("PRF_SourceOwner");

            Delete.Table("PRF_SourceOwningEntity");
        }

        public override void Up()
        {
            Create.Table("PRF_SourceOwningEntity")
                .WithColumn("SourceOwningEntityID").AsInt32().PrimaryKey().Identity().NotNullable()
                .WithColumn("Name").AsString().NotNullable()
                .WithColumn("SourcePathPrefix").AsString().Nullable();

            Create.Table("PRF_AdminUserAffiliation")
                .WithColumn("AdminUserAffiliationID").AsInt32().PrimaryKey().Identity().NotNullable()
                .WithColumn("SourceOwningEntityID").AsInt32().NotNullable()
                .WithColumn("AdminUserID").AsInt32().NotNullable();

            Create.ForeignKey()
                .FromTable("PRF_AdminUserAffiliation").ForeignColumn("SourceOwningEntityID")
                .ToTable("PRF_SourceOwningEntity").PrimaryColumn("SourceOwningEntityID");

            Create.ForeignKey()
                .FromTable("PRF_AdminUserAffiliation").ForeignColumn("AdminUserID")
                .ToTable("PRF_AdminUser").PrimaryColumn("AdminUserID");

            Create.Table("PRF_SourceOwner")
                .WithColumn("SourceOwnerID").AsInt32().PrimaryKey().Identity().NotNullable()
                .WithColumn("SourceID").AsInt32().NotNullable()
                .WithColumn("SourceOwningEntityID").AsInt32().NotNullable();

            Create.ForeignKey()
                .FromTable("PRF_SourceOwner").ForeignColumn("SourceID")
                .ToTable("PRF_Source").PrimaryColumn("SourceID");

            Create.ForeignKey()
                .FromTable("PRF_SourceOwner").ForeignColumn("SourceOwningEntityID")
                .ToTable("PRF_SourceOwningEntity").PrimaryColumn("SourceOwningEntityID");
        }
    }
}
