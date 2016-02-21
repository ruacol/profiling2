using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201311270843)]
    public class AddSameEthnicityTable : Migration
    {
        public override void Down()
        {
            if (Schema.Table("PRF_SameEthnicity").Index("UQ_SameEthnicity").Exists())
                Delete.UniqueConstraint("UQ_SameEthnicity").FromTable("PRF_SameEthnicity");
            Delete.ForeignKey().FromTable("PRF_SameEthnicity").ForeignColumn("Ethnicity1ID").ToTable("PRF_Ethnicity").PrimaryColumn("EthnicityID");
            Delete.ForeignKey().FromTable("PRF_SameEthnicity").ForeignColumn("Ethnicity2ID").ToTable("PRF_Ethnicity").PrimaryColumn("EthnicityID");
            if (Schema.Table("PRF_SameEthnicity").Exists())
                Delete.Table("PRF_SameEthnicity");
        }

        public override void Up()
        {
            Create.Table("PRF_SameEthnicity")
                .WithColumn("SameEthnicityID").AsInt32().PrimaryKey().Identity().NotNullable()
                .WithColumn("Ethnicity1ID").AsInt32().NotNullable()
                .WithColumn("Ethnicity2ID").AsInt32().NotNullable();

            Create.ForeignKey().FromTable("PRF_SameEthnicity").ForeignColumn("Ethnicity1ID").ToTable("PRF_Ethnicity").PrimaryColumn("EthnicityID");
            Create.ForeignKey().FromTable("PRF_SameEthnicity").ForeignColumn("Ethnicity2ID").ToTable("PRF_Ethnicity").PrimaryColumn("EthnicityID");

            Create.UniqueConstraint("UQ_SameEthnicity").OnTable("PRF_SameEthnicity").Columns(new string[] { "Ethnicity1ID", "Ethnicity2ID" });
        }
    }
}
