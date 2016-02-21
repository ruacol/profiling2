using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201512080943)]
    public class AddOperationNameChange : Migration
    {
        public override void Down()
        {
            if (Schema.Table("PRF_Operation").Column("NextOperationID").Exists())
            {
                Delete.ForeignKey().FromTable("PRF_Operation").ForeignColumn("NextOperationID").ToTable("PRF_Operation").PrimaryColumn("OperationID");

                Delete.Column("NextOperationID").FromTable("PRF_Operation_AUD");
                Delete.Column("NextOperationID").FromTable("PRF_Operation");
            }
        }

        public override void Up()
        {
            if (!Schema.Table("PRF_Operation").Column("NextOperationID").Exists())
            {
                Alter.Table("PRF_Operation").AddColumn("NextOperationID").AsInt32().Nullable();
                Alter.Table("PRF_Operation_AUD").AddColumn("NextOperationID").AsInt32().Nullable();

                Create.ForeignKey().FromTable("PRF_Operation").ForeignColumn("NextOperationID").ToTable("PRF_Operation").PrimaryColumn("OperationID");
            }
        }
    }
}
