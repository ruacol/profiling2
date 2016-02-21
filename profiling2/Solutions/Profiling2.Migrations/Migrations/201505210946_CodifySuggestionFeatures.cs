using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201505210946)]
    public class CodifySuggestionFeatures : Migration
    {
        public override void Down()
        {
            Delete.Column("Code").FromTable("PRF_AdminSuggestionFeaturePersonResponsibility");
        }

        public override void Up()
        {
            Alter.Table("PRF_AdminSuggestionFeaturePersonResponsibility").AddColumn("Code").AsString(255).WithDefaultValue(string.Empty).NotNullable();

            Update.Table("PRF_AdminSuggestionFeaturePersonResponsibility").Set(new { Code = "HAS_RELATIONSHIP_WITH_RESPONSIBLE_PERSON" })
                .Where(new { Feature = "Profiled person has a relationship with a person bearing responsibility for suggested event." });
            Update.Table("PRF_AdminSuggestionFeaturePersonResponsibility").Set(new { Code = "RESPONSIBLE_FOR_RELATED_EVENT" })
                .Where(new { Feature = "Profiled person bears responsibility to an event related to suggested event." });
            Update.Table("PRF_AdminSuggestionFeaturePersonResponsibility").Set(new { Code = "LAST_NAME_APPEARS" })
                .Where(new { Feature = "Profiled person's last name appears in suggested event's narrative." });
            Update.Table("PRF_AdminSuggestionFeaturePersonResponsibility").Set(new { Code = "FIRST_NAME_APPEARS" })
                .Where(new { Feature = "Profiled person's first name appears in suggested event's narrative." });
            Update.Table("PRF_AdminSuggestionFeaturePersonResponsibility").Set(new { Code = "COMMON_SOURCE" })
                .Where(new { Feature = "Profiled person shares a source with suggested event." });
            Update.Table("PRF_AdminSuggestionFeaturePersonResponsibility").Set(new { Code = "ALIAS_APPEARS" })
                .Where(new { Feature = "Profiled person's alias appears in suggested event's narrative." });
            Update.Table("PRF_AdminSuggestionFeaturePersonResponsibility").Set(new { Code = "CAREER_IN_LOCATION" })
                .Where(new { Feature = "Profiled person has a career in the location or territory of the suggested event." });
            Update.Table("PRF_AdminSuggestionFeaturePersonResponsibility").Set(new { Code = "CAREER_IN_ORG_RESPONSIBLE" })
                .Where(new { Feature = "Profiled person has a career in an organization bearing responsibility for suggested event." });
            Update.Table("PRF_AdminSuggestionFeaturePersonResponsibility").Set(new { Code = "RESPONSIBILITY_IN_LOCATION" })
                .Where(new { Feature = "Profiled person bears responsibility for an event occurring in the same location or territory as suggested event." });
            Update.Table("PRF_AdminSuggestionFeaturePersonResponsibility").Set(new { Code = "IS_SUPERIOR_TO" })
                .Where(new { Feature = "Profiled person is part of an \"is superior to\" relationship with a person bearing responsibility for suggested event." });
            Update.Table("PRF_AdminSuggestionFeaturePersonResponsibility").Set(new { Code = "IS_A_SUBORDINATE" })
                .Where(new { Feature = "Profiled person is part of an \"is a subordinate of\" relationship with a person bearing responsibility for suggested event." });
            Update.Table("PRF_AdminSuggestionFeaturePersonResponsibility").Set(new { Code = "IS_DEPUTY_OF" })
                .Where(new { Feature = "Profiled person is part of an \"is the deputy of\" relationship with a person bearing responsibility for suggested event." });
            Execute.Sql(@"
                UPDATE PRF_AdminSuggestionFeaturePersonResponsibility SET Code = 'PROVIDED_WEAPONS_AND_AMMUNITION_TO'
                WHERE Feature LIKE '%provided weapons%'
            ");
            Update.Table("PRF_AdminSuggestionFeaturePersonResponsibility").Set(new { Code = "FOUGHT_WITH" })
                .Where(new { Feature = "Profiled person is part of an \"fought with\" relationship with a person bearing responsibility for suggested event." });
            Update.Table("PRF_AdminSuggestionFeaturePersonResponsibility").Set(new { Code = "ACTED_WITH" })
                .Where(new { Feature = "Profiled person is part of an \"acted together with\" relationship with a person bearing responsibility for suggested event." });
            Update.Table("PRF_AdminSuggestionFeaturePersonResponsibility").Set(new { Code = "IS_BODYGUARD_OF" })
                .Where(new { Feature = "Profiled person is part of an \"is the bodyguard of\" relationship with a person bearing responsibility for suggested event." });
            Update.Table("PRF_AdminSuggestionFeaturePersonResponsibility").Set(new { Code = "FOUGHT_IN_SAME_GROUP_AS" })
                .Where(new { Feature = "Profiled person is part of an \"fought in same group as\" relationship with a person bearing responsibility for suggested event." });
            Update.Table("PRF_AdminSuggestionFeaturePersonResponsibility").Set(new { Code = "BELONGED_TO_SAME_GROUP_AS" })
                .Where(new { Feature = "Profiled person is part of an \"belonged to the same group as\" relationship with a person bearing responsibility for suggested event." });
            Update.Table("PRF_AdminSuggestionFeaturePersonResponsibility").Set(new { Code = "CAREER_IN_UNIT_RESPONSIBLE" })
                .Where(new { Feature = "Profiled person has a career a unit bearing responsibility for suggested event." });
        }
    }
}
