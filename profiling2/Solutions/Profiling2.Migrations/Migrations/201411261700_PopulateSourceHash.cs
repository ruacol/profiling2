using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201411261700)]
    public class PopulateSourceHash : Migration
    {
        public override void Down()
        {
            
        }

        public override void Up()
        {
            Execute.EmbeddedScript("201411261700_TR_PopulateSourceHash.sql");

            Execute.Sql(@"
                UPDATE [PRF_Source]
                SET Hash = master.sys.fn_repl_hash_binary(FileData)
                WHERE OriginalFileData IS NULL
                AND Hash IS NULL
                AND DATALENGTH(FileData) < 8500000;

                UPDATE [PRF_Source]
                SET Hash = master.sys.fn_repl_hash_binary(OriginalFileData)
                WHERE OriginalFileData IS NOT NULL
                AND Hash IS NULL
                AND DATALENGTH(OriginalFileData) < 8500000;
            ");
        }
    }
}
