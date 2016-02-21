using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201510061229)]
    public class FixPersonMergeExistingPersonSource : Migration
    {
        public override void Down()
        {

        }

        public override void Up()
        {
            Execute.EmbeddedScript("201510061229_SP_PersonMerge_NHibernate.sql");
        }
    }
}
