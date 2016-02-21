using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201306041143)]
    public class AuditMoreTables : Migration
    {
        public override void Down()
        {
            Delete.Table("PRF_Role_AUD");
            Delete.Table("PRF_Rank_AUD");
        }

        public override void Up()
        {
            Create.Table("PRF_Role_AUD")
                .WithColumn("RoleID").AsInt32().NotNullable()
                .WithColumn("REV").AsInt32()
                .WithColumn("REVTYPE").AsInt16()
                .WithColumn("RoleName").AsString(255).Nullable()
                .WithColumn("RoleNameFr").AsString(255).Nullable()
                .WithColumn("Description").AsString(int.MaxValue).Nullable()
                .WithColumn("Archive").AsBoolean().Nullable()
                .WithColumn("Notes").AsString(int.MaxValue).Nullable();
            Create.PrimaryKey().OnTable("PRF_Role_AUD").Columns(new string[] { "RoleID", "REV" });

            Create.Table("PRF_Rank_AUD")
                .WithColumn("RankID").AsInt32().NotNullable()
                .WithColumn("REV").AsInt32()
                .WithColumn("REVTYPE").AsInt16()
                .WithColumn("RankName").AsString(255).Nullable()
                .WithColumn("Archive").AsBoolean().Nullable()
                .WithColumn("Notes").AsString(int.MaxValue).Nullable();
            Create.PrimaryKey().OnTable("PRF_Rank_AUD").Columns(new string[] { "RankID", "REV" });

            Execute.Sql(@"
                INSERT INTO PRF_Role_AUD (RoleID, REV, REVTYPE, RoleName, RoleNameFr, Description, Archive, Notes)
                    SELECT RoleID, 100, 0, RoleName, RoleNameFr, Description, Archive, Notes
                        FROM PRF_Role;

                INSERT INTO PRF_Role_AUD (RoleID, REV, REVTYPE, RoleName, Notes)
                    SELECT DISTINCT RoleID, 100, 2, CAST(RoleID AS varchar) + ' deleted', 'Automatically inserted to make up for deleted role'
                        FROM PRF_Career_AUD
                        WHERE RoleID NOT IN (SELECT RoleID FROM PRF_Role)

                INSERT INTO PRF_Rank_AUD (RankID, REV, REVTYPE, RankName, Archive, Notes)
                    SELECT RankID, 100, 0, RankName, Archive, Notes
                        FROM PRF_Rank;

                INSERT INTO PRF_Rank_AUD (RankID, REV, REVTYPE, RankName, Notes)
                    SELECT DISTINCT RankID, 100, 2, CAST(RankID AS varchar) + ' deleted', 'Automatically inserted to make up for deleted rank'
                        FROM PRF_Career_AUD
                        WHERE RankID NOT IN (SELECT RankID FROM PRF_Rank)
            ");
        }
    }
}
