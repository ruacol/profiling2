using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201501021646)]
    public class AddCommanderCheckboxesToFunctions : Migration
    {
        public override void Down()
        {
            Delete.Column("IsCommander").FromTable("PRF_Role_AUD");
            Delete.Column("IsCommander").FromTable("PRF_Role");

            Delete.Column("IsDeputyCommander").FromTable("PRF_Role_AUD");
            Delete.Column("IsDeputyCommander").FromTable("PRF_Role");
        }

        public override void Up()
        {
            Alter.Table("PRF_Role").AddColumn("IsCommander").AsBoolean().WithDefaultValue(false).NotNullable();
            Alter.Table("PRF_Role_AUD").AddColumn("IsCommander").AsBoolean().WithDefaultValue(false).NotNullable();

            Alter.Table("PRF_Role").AddColumn("IsDeputyCommander").AsBoolean().WithDefaultValue(false).NotNullable();
            Alter.Table("PRF_Role_AUD").AddColumn("IsDeputyCommander").AsBoolean().WithDefaultValue(false).NotNullable();

            Execute.Sql(@"
                UPDATE PRF_Role SET IsCommander = 1
                WHERE RoleName = 'Commander';

                UPDATE PRF_Role SET IsDeputyCommander = 1
                WHERE RoleName LIKE '%Deputy Commander%'
                AND RoleName NOT LIKE '%assistant%'
            ");
        }
    }
}
