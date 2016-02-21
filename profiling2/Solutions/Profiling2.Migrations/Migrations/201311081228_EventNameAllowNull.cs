using System;
using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201311081228)]
    public class EventNameAllowNull : Migration
    {
        public override void Down()
        {
            //Delete.Index("_dta_index_PRF_Event_11_907150277__K1_2_12").OnTable("PRF_Event");
        }

        public override void Up()
        {
            Alter.Column("EventName").OnTable("PRF_Event").AsString(500).Nullable();
            Alter.Column("EventName").OnTable("PRF_Event_AUD").AsString(500).Nullable();
        }
    }
}
