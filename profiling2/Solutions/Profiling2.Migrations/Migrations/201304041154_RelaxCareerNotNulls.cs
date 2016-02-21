using System;
using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201304041154)]
    public class RelaxCareerNotNulls : Migration
    {
        public override void Down()
        {
            // need to drop and recreate indexes
            //Alter.Column("OrganizationID").OnTable("PRF_Career").AsInt32().NotNullable();
            //Alter.Column("LocationID").OnTable("PRF_Career").AsInt32().NotNullable();
            //Alter.Column("RankID").OnTable("PRF_Career").AsInt32().NotNullable();
        }

        public override void Up()
        {
            Alter.Column("OrganizationID").OnTable("PRF_Career").AsInt32().Nullable();
            Alter.Column("LocationID").OnTable("PRF_Career").AsInt32().Nullable();
            Alter.Column("RankID").OnTable("PRF_Career").AsInt32().Nullable();

            Execute.Sql(@"UPDATE PRF_Career
                          SET RankID = NULL 
                          FROM PRF_Career AS c INNER JOIN PRF_Rank AS r ON c.RankID = r.RankID
                          WHERE r.RankName = '0'
            ");
        }
    }
}
