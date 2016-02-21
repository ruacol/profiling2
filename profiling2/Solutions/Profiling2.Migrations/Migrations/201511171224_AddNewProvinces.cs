using System;
using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201511171224)]
    public class AddNewProvinces : Migration
    {
        public override void Down()
        {
            if (Schema.Table("PRF_Province").Exists())
            {
                if (Schema.Table("PRF_Location").Column("ProvinceID").Exists())
                {
                    Delete.ForeignKey().FromTable("PRF_Location").ForeignColumn("ProvinceID").ToTable("PRF_Province").PrimaryColumn("ProvinceID");
                    Delete.Column("ProvinceID").FromTable("PRF_Location");

                    if (Schema.Table("PRF_LocationAUD").Column("ProvinceID").Exists())
                    {
                        Delete.Column("ProvinceID").FromTable("PRF_LocationAUD");
                    }
                }

                Delete.Table("PRF_Province");
            }
        }

        public override void Up()
        {
            if (!Schema.Table("PRF_Province").Exists())
            {
                Create.Table("PRF_Province")
                    .WithColumn("ProvinceID").AsInt32().PrimaryKey().Identity().NotNullable()
                    .WithColumn("ProvinceName").AsString(int.MaxValue).NotNullable()
                    .WithColumn("Notes").AsString(int.MaxValue).Nullable()
                    .WithColumn("Archive").AsBoolean().WithDefaultValue(false).NotNullable();

                if (!Schema.Table("PRF_Location").Column("ProvinceID").Exists())
                {
                    Alter.Table("PRF_Location").AddColumn("ProvinceID").AsInt32().Nullable();
                    Create.ForeignKey().FromTable("PRF_Location").ForeignColumn("ProvinceID").ToTable("PRF_Province").PrimaryColumn("ProvinceID");

                    if (!Schema.Table("PRF_Location_AUD").Column("ProvinceID").Exists())
                    {
                        Alter.Table("PRF_Location_AUD").AddColumn("ProvinceID").AsInt32().Nullable();
                    }
                }

                Insert.IntoTable("PRF_Province").Row(new { ProvinceName = "Bas-Uele" });
                Insert.IntoTable("PRF_Province").Row(new { ProvinceName = "Équateur" });
                Insert.IntoTable("PRF_Province").Row(new { ProvinceName = "Haut-Katanga" });
                Insert.IntoTable("PRF_Province").Row(new { ProvinceName = "Haut-Lomami" });
                Insert.IntoTable("PRF_Province").Row(new { ProvinceName = "Haut-Uele" });
                Insert.IntoTable("PRF_Province").Row(new { ProvinceName = "Ituri" });
                Insert.IntoTable("PRF_Province").Row(new { ProvinceName = "Kasaï-Central" });
                Insert.IntoTable("PRF_Province").Row(new { ProvinceName = "Kasaï-Occidental" });
                Insert.IntoTable("PRF_Province").Row(new { ProvinceName = "Kasaï-Oriental" });
                Insert.IntoTable("PRF_Province").Row(new { ProvinceName = "Kinshasa" });
                Insert.IntoTable("PRF_Province").Row(new { ProvinceName = "Kongo-Central" });
                Insert.IntoTable("PRF_Province").Row(new { ProvinceName = "Kwango" });
                Insert.IntoTable("PRF_Province").Row(new { ProvinceName = "Kwilu" });
                Insert.IntoTable("PRF_Province").Row(new { ProvinceName = "Lomami" });
                Insert.IntoTable("PRF_Province").Row(new { ProvinceName = "Lualaba" });
                Insert.IntoTable("PRF_Province").Row(new { ProvinceName = "Mai-Ndombe" });
                Insert.IntoTable("PRF_Province").Row(new { ProvinceName = "Maniema" });
                Insert.IntoTable("PRF_Province").Row(new { ProvinceName = "Mongala" });
                Insert.IntoTable("PRF_Province").Row(new { ProvinceName = "Nord-Kivu" });
                Insert.IntoTable("PRF_Province").Row(new { ProvinceName = "Nord-Ubangi" });
                Insert.IntoTable("PRF_Province").Row(new { ProvinceName = "Sankuru" });
                Insert.IntoTable("PRF_Province").Row(new { ProvinceName = "Sud-Kivu" });
                Insert.IntoTable("PRF_Province").Row(new { ProvinceName = "Sud-Ubangi" });
                Insert.IntoTable("PRF_Province").Row(new { ProvinceName = "Tanganyika" });
                Insert.IntoTable("PRF_Province").Row(new { ProvinceName = "Tshopo" });
                Insert.IntoTable("PRF_Province").Row(new { ProvinceName = "Tshuapa" });
            }
        }
    }
}
