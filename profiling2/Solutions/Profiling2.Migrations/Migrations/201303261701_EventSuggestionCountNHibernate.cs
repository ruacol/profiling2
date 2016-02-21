using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201303261701)]
    public class EventSuggestionCountNHibernate : Migration
    {
        public override void Down()
        {

        }

        public override void Up()
        {
            Execute.EmbeddedScript("201303261701_EventSuggestionCountNHibernate.sql");
        }
    }
}
