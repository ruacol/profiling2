using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201301231549)]
    public class CreatedTriggers : Migration
    {

        public override void Down()
        {
            foreach (string table in new string[] { "Person", "Organization", "Event", "Career", "PersonResponsibility" })
            {
                Execute.Sql(string.Format(@"
                    DROP TRIGGER [dbo].[PRF_TR_{0}_Created_I]
                ", table));
            }
        }

        public override void Up()
        {
            foreach (string table in new string[] { "Person", "Organization", "Event", "Career", "PersonResponsibility" })
            {
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
                ", table));
            }
        }
    }
}
