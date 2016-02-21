using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201505151731)]
    public class PrimaryKeyForUnitOperationAUD : Migration
    {
        public override void Down()
        {
            Delete.PrimaryKey("PK_UnitOperation_AUD").FromTable("PRF_UnitOperation_AUD");
        }

        public override void Up()
        {
            Update.Table("PRF_UnitOperation_AUD").Set(new { UnitOperationID = 1 }).Where(new { UnitID = 1082, OperationID = 1 });
            Update.Table("PRF_UnitOperation_AUD").Set(new { UnitOperationID = 2 }).Where(new { UnitID = 2489, OperationID = 11 });
            Update.Table("PRF_UnitOperation_AUD").Set(new { UnitOperationID = 3 }).Where(new { UnitID = 2486, OperationID = 11 });
            Update.Table("PRF_UnitOperation_AUD").Set(new { UnitOperationID = 4 }).Where(new { UnitID = 3024, OperationID = 11 });
            Update.Table("PRF_UnitOperation_AUD").Set(new { UnitOperationID = 5 }).Where(new { UnitID = 1862, OperationID = 8 });

            Alter.Column("UnitOperationID").OnTable("PRF_UnitOperation_AUD").AsInt32().NotNullable();

            Create.PrimaryKey("PK_UnitOperation_AUD").OnTable("PRF_UnitOperation_AUD").Columns(new string[] { "UnitOperationID", "REV" });
        }
    }
}
