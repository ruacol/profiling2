using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201407281020)]
    public class AddOperationUnitJoinTable : Migration
    {
        public override void Down()
        {
            Delete.Table("PRF_UnitOperation");
            Delete.Table("PRF_UnitOperation_AUD");
        }

        public override void Up()
        {
            Create.Table("PRF_UnitOperation")
                .WithColumn("UnitOperationID").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("UnitID").AsInt32().NotNullable()
                .WithColumn("OperationID").AsInt32().NotNullable();
            Create.ForeignKey().FromTable("PRF_UnitOperation").ForeignColumn("UnitID").ToTable("PRF_Unit").PrimaryColumn("UnitID");
            Create.ForeignKey().FromTable("PRF_UnitOperation").ForeignColumn("OperationID").ToTable("PRF_Operation").PrimaryColumn("OperationID");

            Create.Table("PRF_UnitOperation_AUD")
                .WithColumn("UnitOperationID").AsInt32().Nullable()
                .WithColumn("REV").AsInt32()
                .WithColumn("REVTYPE").AsInt16()
                .WithColumn("UnitID").AsInt32().Nullable()
                .WithColumn("OperationID").AsInt32().Nullable();
            //Create.PrimaryKey().OnTable("PRF_UnitOperation_AUD").Columns(new string[] { "UnitOperationID", "REV" });
        }
    }
}
