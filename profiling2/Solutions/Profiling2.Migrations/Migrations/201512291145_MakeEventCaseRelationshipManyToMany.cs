using System;
using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201512291145)]
    public class MakeEventCaseRelationshipManyToMany : Migration
    {
        public override void Down()
        {
            if (!Schema.Table("PRF_JhroCase").Column("EventID").Exists())
            {
                Alter.Table("PRF_JhroCase").AddColumn("EventID").AsInt32().Nullable();
                Create.ForeignKey().FromTable("PRF_JhroCase").ForeignColumn("EventID").ToTable("PRF_Event").PrimaryColumn("EventID");
            }

            Execute.EmbeddedScript("201507101641_FixEventMergeSP.sql");

            Execute.Sql(@"
                UPDATE jc
                SET jc.EventID = ejc.EventID
                FROM PRF_JhroCase jc, PRF_EventJhroCase ejc
                WHERE jc.JhroCaseID = ejc.JhroCaseID
            ");

            if (Schema.Table("PRF_EventJhroCase_AUD").Exists())
            {
                Delete.Table("PRF_EventJhroCase_AUD");
            }

            if (Schema.Table("PRF_EventJhroCase").Exists())
            {
                Delete.ForeignKey()
                    .FromTable("PRF_EventJhroCase").ForeignColumn("EventID")
                    .ToTable("PRF_Event").PrimaryColumn("EventID");
                Delete.ForeignKey()
                    .FromTable("PRF_EventJhroCase").ForeignColumn("JhroCaseID")
                    .ToTable("PRF_JhroCase").PrimaryColumn("JhroCaseID");
                Delete.Table("PRF_EventJhroCase");
            }
        }

        public override void Up()
        {
            // create new many-to-many schema
            if (!Schema.Table("PRF_EventJhroCase").Exists())
            {
                Create.Table("PRF_EventJhroCase")
                    .WithColumn("EventJhroCaseID").AsInt32().NotNullable().PrimaryKey().Identity()
                    .WithColumn("EventID").AsInt32().NotNullable()
                    .WithColumn("JhroCaseID").AsInt32().NotNullable();

                Create.ForeignKey()
                    .FromTable("PRF_EventJhroCase").ForeignColumn("EventID")
                    .ToTable("PRF_Event").PrimaryColumn("EventID");

                Create.ForeignKey()
                    .FromTable("PRF_EventJhroCase").ForeignColumn("JhroCaseID")
                    .ToTable("PRF_JhroCase").PrimaryColumn("JhroCaseID");
            }

            if (!Schema.Table("PRF_EventJhroCase_AUD").Exists())
            {
                Create.Table("PRF_EventJhroCase_AUD")
                    .WithColumn("EventJhroCaseID").AsInt32().Nullable()
                    .WithColumn("REV").AsInt32()
                    .WithColumn("REVTYPE").AsInt16()
                    .WithColumn("EventID").AsInt32().Nullable()
                    .WithColumn("JhroCaseID").AsInt32().Nullable();
            }

            // migrate data to new table
            Execute.Sql(@"
                INSERT INTO PRF_EventJhroCase SELECT EventID, JhroCaseID FROM PRF_JhroCase WHERE EventID IS NOT NULL;
            ");

            // update event merge SP
            Execute.EmbeddedScript("201512291215_UpdateEventMergeSP.sql");

            // undo old schema
            if (Schema.Table("PRF_JhroCase").Column("EventID").Exists())
            {
                Delete.ForeignKey().FromTable("PRF_JhroCase").ForeignColumn("EventID").ToTable("PRF_Event").PrimaryColumn("EventID");
                Delete.Column("EventID").FromTable("PRF_JhroCase");
            }

            //if (Schema.Table("PRF_JhroCase_AUD").Column("EventID").Exists())
            //{
            //    Delete.Column("EventID").FromTable("PRF_JhroCase_AUD");
            //}
        }
    }
}
