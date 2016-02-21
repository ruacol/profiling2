using System;
using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201311141602)]
    public class AddUniqueIndexToPersonPhoto : Migration
    {
        public override void Down()
        {
            Delete.UniqueConstraint("UQ_PersonPhoto").FromTable("PRF_PersonPhoto");
        }

        public override void Up()
        {
            Execute.Sql(@"
                DELETE FROM PRF_PersonPhoto WHERE PersonPhotoID NOT IN (SELECT MIN(PersonPhotoID) FROM PRF_PersonPhoto GROUP BY PersonID, PhotoID)
            ");

            Create.UniqueConstraint("UQ_PersonPhoto").OnTable("PRF_PersonPhoto").Columns(new string[] { "PersonID", "PhotoID" });
        }
    }
}
