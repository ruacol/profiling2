using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201307151701)]
    public class AddActiveScreening : Migration
    {
        public override void Down()
        {
            Delete.Table("PRF_ActiveScreening");
        }

        public override void Up()
        {
            Create.Table("PRF_ActiveScreening")
                .WithColumn("ActiveScreeningID").AsInt32().PrimaryKey().Identity().NotNullable()
                .WithColumn("PersonID").AsInt32().NotNullable()
                .WithColumn("RequestID").AsInt32().Nullable()
                .WithColumn("DateActivelyScreened").AsDateTime().NotNullable()
                .WithColumn("ScreenedByID").AsInt32().Nullable()
                .WithColumn("Notes").AsString(int.MaxValue).Nullable();
            Create.ForeignKey().FromTable("PRF_ActiveScreening").ForeignColumn("PersonID").ToTable("PRF_Person").PrimaryColumn("PersonID");
            Create.ForeignKey().FromTable("PRF_ActiveScreening").ForeignColumn("RequestID").ToTable("SCR_Request").PrimaryColumn("RequestID");
            Create.ForeignKey().FromTable("PRF_ActiveScreening").ForeignColumn("ScreenedByID").ToTable("PRF_AdminUser").PrimaryColumn("AdminUserID");
        }
    }
}
