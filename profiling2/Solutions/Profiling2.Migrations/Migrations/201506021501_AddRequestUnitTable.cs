using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201506021501)]
    public class AddRequestUnitTable : Migration
    {
        public override void Down()
        {
            Delete.ForeignKey().FromTable("SCR_RequestUnit").ForeignColumn("RequestID").ToTable("SCR_Request").PrimaryColumn("RequestID");
            Delete.ForeignKey().FromTable("SCR_RequestUnit").ForeignColumn("UnitID").ToTable("PRF_Unit").PrimaryColumn("UnitID");
            Delete.Table("SCR_RequestUnit");
        }

        public override void Up()
        {
            Create.Table("SCR_RequestUnit")
                .WithColumn("RequestUnitID").AsInt32().PrimaryKey().Identity().NotNullable()
                .WithColumn("RequestID").AsInt32().NotNullable()
                .WithColumn("UnitID").AsInt32().NotNullable()
                .WithColumn("Notes").AsString(int.MaxValue).Nullable()
                .WithColumn("Archive").AsBoolean().WithDefaultValue(false).NotNullable();

            Create.ForeignKey().FromTable("SCR_RequestUnit").ForeignColumn("RequestID").ToTable("SCR_Request").PrimaryColumn("RequestID");
            Create.ForeignKey().FromTable("SCR_RequestUnit").ForeignColumn("UnitID").ToTable("PRF_Unit").PrimaryColumn("UnitID");
        }
    }
}
