using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201407241611)]
    public class AddUnitMergeSPForNHibernate : Migration
    {
        public override void Down()
        {
            
        }

        public override void Up()
        {
            Execute.EmbeddedScript("201407241611_SP_UnitMerge_NHibernate.sql");
        }
    }
}
