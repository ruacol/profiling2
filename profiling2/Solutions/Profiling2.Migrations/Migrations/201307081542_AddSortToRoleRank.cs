using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201307081542)]
    public class AddSortToRoleRank : Migration
    {
        public override void Down()
        {
            Delete.Column("RankNameFr").FromTable("PRF_Rank");
            Delete.Column("Description").FromTable("PRF_Rank");
            Delete.Column("SortOrder").FromTable("PRF_Rank");

            Delete.Column("RankNameFr").FromTable("PRF_Rank_AUD");
            Delete.Column("Description").FromTable("PRF_Rank_AUD");
            Delete.Column("SortOrder").FromTable("PRF_Rank_AUD");

            Delete.Column("SortOrder").FromTable("PRF_Role");

            Delete.Column("SortOrder").FromTable("PRF_Role_AUD");
        }

        public override void Up()
        {
            Alter.Table("PRF_Rank").AddColumn("RankNameFr").AsString().Nullable();
            Alter.Table("PRF_Rank").AddColumn("Description").AsString(int.MaxValue).Nullable();
            Alter.Table("PRF_Rank").AddColumn("SortOrder").AsInt32().Nullable();

            Alter.Table("PRF_Rank_AUD").AddColumn("RankNameFr").AsString().Nullable();
            Alter.Table("PRF_Rank_AUD").AddColumn("Description").AsString(int.MaxValue).Nullable();
            Alter.Table("PRF_Rank_AUD").AddColumn("SortOrder").AsInt32().Nullable();

            Alter.Table("PRF_Role").AddColumn("SortOrder").AsInt32().Nullable();

            Alter.Table("PRF_Role_AUD").AddColumn("SortOrder").AsInt32().Nullable();
        }
    }
}
