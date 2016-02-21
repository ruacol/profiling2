using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201303041046)]
    public class CreatedColumnOrganizationResponsibility : Migration
    {
        public override void Down()
        {
            Execute.Sql(string.Format(@"
                    DROP TRIGGER [dbo].[PRF_TR_{0}_Created_I]
                ", "OrganizationResponsibility"));

            Delete.Column("Created").FromTable("PRF_OrganizationResponsibility");
        }

        public override void Up()
        {
            Alter.Table("PRF_OrganizationResponsibility").AddColumn("Created").AsDateTime().Nullable();

            Execute.Sql(string.Format(@"
                    CREATE TRIGGER [dbo].[PRF_TR_{0}_Created_I] ON [dbo].[PRF_{0}]    
                    AFTER INSERT  
                    AS    
                    BEGIN    
                      UPDATE P
                      SET P.[Created] = GETDATE()
                      FROM INSERTED I, PRF_{0} P
                      WHERE I.[{0}ID] = P.[{0}ID]
                    END
                ", "OrganizationResponsibility"));
        }
    }
}
