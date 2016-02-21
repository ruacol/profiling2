using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201505191036)]
    public class MakeSourceFullReferenceNotNull : Migration
    {
        public override void Down()
        {
            
        }

        public override void Up()
        {
            Alter.Column("FullReference").OnTable("PRF_Source").AsString(int.MaxValue).Nullable();
        }
    }
}
