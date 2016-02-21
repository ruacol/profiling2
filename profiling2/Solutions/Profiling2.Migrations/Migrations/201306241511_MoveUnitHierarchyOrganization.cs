using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201306241511)]
    public class MoveUnitHierarchyOrganization : Migration
    {
        public override void Down()
        {
            Delete.ForeignKey().FromTable("PRF_Unit").ForeignColumn("OrganizationID").ToTable("PRF_Organization").PrimaryColumn("OrganizationID");
            Delete.Column("OrganizationID").FromTable("PRF_Unit");

            Create.Column("OrganizationID").OnTable("PRF_UnitHierarchy").AsInt32().Nullable();
            Create.ForeignKey().FromTable("PRF_UnitHierarchy").ForeignColumn("OrganizationID").ToTable("PRF_Organization").PrimaryColumn("OrganizationID");
            Create.ForeignKey("FK_PRF_UnitHierarchy_OrganizationID")
                .FromTable("PRF_UnitHierarchy").ForeignColumn("OrganizationID")
                .ToTable("PRF_Organization").PrimaryColumn("OrganizationID");
        }

        public override void Up()
        {
            Create.Column("OrganizationID").OnTable("PRF_Unit").AsInt32().Nullable();
            Create.ForeignKey().FromTable("PRF_Unit").ForeignColumn("OrganizationID").ToTable("PRF_Organization").PrimaryColumn("OrganizationID");

            Execute.Sql(@"
                UPDATE PRF_Unit
                SET OrganizationID = uh.OrganizationID
                FROM PRF_Unit u INNER JOIN PRF_UnitHierarchy uh
                ON u.UnitID = uh.UnitID
            ");

            //Delete.ForeignKey("FK_PRF_UnitHierarchy_OrganizationID").OnTable("PRF_UnitHierarchy");
            Execute.Sql(@"
                IF EXISTS (SELECT * FROM sys.all_objects WHERE name = 'FK_PRF_UnitHierarchy_OrganizationID')
                    ALTER TABLE PRF_UnitHierarchy DROP CONSTRAINT FK_PRF_UnitHierarchy_OrganizationID
            ");
            Execute.Sql(@"
                IF EXISTS (SELECT * FROM sys.all_objects WHERE name = 'FK_PRF_UnitHierarchy_OrganizationID_PRF_Organization_OrganizationID')
                    ALTER TABLE PRF_UnitHierarchy DROP CONSTRAINT FK_PRF_UnitHierarchy_OrganizationID_PRF_Organization_OrganizationID
            ");
            //Execute.Sql("DROP STATISTICS PRF_UnitHierarchy._dta_stat_1778821399_13_4");
            Execute.Sql(@"
                IF EXISTS (SELECT * FROM sys.stats WHERE name = '_dta_stat_1778821399_13_4')
                    DROP STATISTICS PRF_UnitHierarchy._dta_stat_1778821399_13_4
            ");
            //Execute.Sql("DROP STATISTICS PRF_UnitHierarchy._dta_stat_1778821399_2_4_13");
            Execute.Sql(@"
                IF EXISTS (SELECT * FROM sys.stats WHERE name = '_dta_stat_1778821399_2_4_13')
                    DROP STATISTICS PRF_UnitHierarchy._dta_stat_1778821399_2_4_13
            ");
            Delete.Column("OrganizationID").FromTable("PRF_UnitHierarchy");
        }
    }
}
