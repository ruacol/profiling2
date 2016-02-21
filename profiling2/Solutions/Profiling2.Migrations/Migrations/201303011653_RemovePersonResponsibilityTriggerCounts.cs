using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    // Triggers on this table mean NHibernate throws a TooManyRowsAffectedException
    // when a delete or update occurs.
    // See http://www.codewrecks.com/blog/index.php/2009/03/25/nhibernate-and-toomanyrowsaffectedexception/
    // We set the triggers not to return the count result here.
    [Migration(201303011653)]
    public class RemoveTriggerCounts : Migration
    {
        public override void Down()
        {
            //throw new System.NotImplementedException();
        }

        public override void Up()
        {
            Execute.EmbeddedScript("201303011653_PersonResponsibilityDeleteTrigger.sql");
            Execute.EmbeddedScript("201303011653_PersonResponsibilityUpdateTrigger.sql");
        }
    }
}
