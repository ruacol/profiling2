using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201312261531)]
    public class RemoveActionTakenUniqueIndex : Migration
    {
        public override void Down()
        {
            
        }

        public override void Up()
        {
            Delete.UniqueConstraint("UN_PRF_ActionTaken_SubjectActionTakenTypeObjectEvent").FromTable("PRF_ActionTaken");
        }
    }
}
