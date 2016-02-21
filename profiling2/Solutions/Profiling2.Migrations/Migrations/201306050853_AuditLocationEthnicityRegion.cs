using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201306050853)]
    public class AuditLocationEthnicityRegion : Migration
    {
        public override void Down()
        {
            Delete.Table("PRF_Location_AUD");
            Delete.Table("PRF_Ethnicity_AUD");
            Delete.Table("PRF_Region_AUD");
        }

        public override void Up()
        {
            Create.Table("PRF_Location_AUD")
                .WithColumn("LocationID").AsInt32().NotNullable()
                .WithColumn("REV").AsInt32()
                .WithColumn("REVTYPE").AsInt16()
                .WithColumn("LocationName").AsString(255).Nullable()
                .WithColumn("Province").AsString(500).Nullable()
                .WithColumn("Territory").AsString(500).Nullable()
                .WithColumn("Town").AsString(500).Nullable()
                .WithColumn("RegionID").AsInt32().Nullable()
                .WithColumn("Longitude").AsFloat().Nullable()
                .WithColumn("Latitude").AsFloat().Nullable()
                .WithColumn("Archive").AsBoolean().Nullable()
                .WithColumn("Notes").AsString(int.MaxValue).Nullable();
            Create.PrimaryKey().OnTable("PRF_Location_AUD").Columns(new string[] { "LocationID", "REV" });

            Create.Table("PRF_Ethnicity_AUD")
                .WithColumn("EthnicityID").AsInt32().NotNullable()
                .WithColumn("REV").AsInt32()
                .WithColumn("REVTYPE").AsInt16()
                .WithColumn("EthnicityName").AsString(255).Nullable()
                .WithColumn("Archive").AsBoolean().Nullable()
                .WithColumn("Notes").AsString(int.MaxValue).Nullable();
            Create.PrimaryKey().OnTable("PRF_Ethnicity_AUD").Columns(new string[] { "EthnicityID", "REV" });

            Create.Table("PRF_Region_AUD")
                .WithColumn("RegionID").AsInt32().NotNullable()
                .WithColumn("REV").AsInt32()
                .WithColumn("REVTYPE").AsInt16()
                .WithColumn("RegionName").AsString(255).Nullable()
                .WithColumn("Archive").AsBoolean().Nullable()
                .WithColumn("Notes").AsString(int.MaxValue).Nullable();
            Create.PrimaryKey().OnTable("PRF_Region_AUD").Columns(new string[] { "RegionID", "REV" });

            Execute.Sql(@"
                INSERT INTO PRF_Location_AUD (LocationID, REV, REVTYPE, LocationName, Province, Territory, Town, RegionID, Longitude, Latitude, Archive, Notes)
                    SELECT LocationID, 100, 0, LocationName, Province, Territory, Town, RegionID, Longitude, Latitude, Archive, Notes
                        FROM PRF_Location;

                INSERT INTO PRF_Ethnicity_AUD (EthnicityID, REV, REVTYPE, EthnicityName, Archive, Notes)
                    SELECT EthnicityID, 100, 0, EthnicityName, Archive, Notes
                        FROM PRF_Ethnicity;

                INSERT INTO PRF_Region_AUD (RegionID, REV, REVTYPE, RegionName, Archive, Notes)
                    SELECT RegionID, 100, 0, RegionName, Archive, Notes
                        FROM PRF_Region;
            ");
        }
    }
}
