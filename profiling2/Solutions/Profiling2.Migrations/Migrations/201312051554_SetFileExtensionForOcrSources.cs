using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201312051554)]
    public class SetFileExtensionForOcrSources : Migration
    {
        public override void Down()
        {
            
        }

        public override void Up()
        {
            // this will trigger an indexing of these rows.
            Execute.Sql(@"
                UPDATE PRF_Source SET FileExtension = 'txt' WHERE OriginalFileData IS NOT NULL;
                UPDATE PRF_Source SET FileExtension = 'abcd' WHERE OriginalFileData IS NOT NULL AND FileExtension = 'pdf';
                UPDATE PRF_Source SET FileExtension = 'pdf' WHERE OriginalFileData IS NOT NULL AND FileExtension = 'abcd';
            ");
        }
    }
}
