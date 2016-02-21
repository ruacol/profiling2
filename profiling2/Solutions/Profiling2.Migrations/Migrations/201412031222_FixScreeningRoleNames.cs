using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201412031222)]
    public class FixScreeningRoleNames : Migration
    {
        public override void Down()
        {

        }

        public override void Up()
        {
            Execute.Sql(@"
                UPDATE [PRF_AdminRole]
                SET Name = 'ScreeningRequestValidator'
                WHERE Name = 'ScreeningRequestConsolidator';

                UPDATE [PRF_AdminRole]
                SET Name = 'ScreeningRequestConsolidator'
                WHERE Name = 'ScreeningRequestAggregator';
            ");
        }
    }
}
