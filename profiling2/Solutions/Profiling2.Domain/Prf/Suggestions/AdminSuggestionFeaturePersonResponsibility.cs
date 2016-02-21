using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Prf.Suggestions
{
    public class AdminSuggestionFeaturePersonResponsibility : Entity
    {
        public const string IS_SUPERIOR_TO = "IS_SUPERIOR_TO";
        public const string FOUGHT_WITH = "FOUGHT_WITH";
        public const string IS_A_SUBORDINATE = "IS_A_SUBORDINATE";
        public const string ACTED_WITH = "ACTED_WITH";
        public const string IS_DEPUTY_OF = "IS_DEPUTY_OF";
        public const string IS_BODYGUARD_OF = "IS_BODYGUARD_OF";
        public const string FOUGHT_IN_SAME_GROUP_AS = "FOUGHT_IN_SAME_GROUP_AS";
        public const string BELONGED_TO_SAME_GROUP_AS = "BELONGED_TO_SAME_GROUP_AS";
        public const string PROVIDED_WEAPONS_AND_AMMUNITION_TO = "PROVIDED_WEAPONS_AND_AMMUNITION_TO";

        public const string RESPONSIBLE_FOR_RELATED_EVENT = "RESPONSIBLE_FOR_RELATED_EVENT";
        public const string LAST_NAME_APPEARS = "LAST_NAME_APPEARS";
        public const string FIRST_NAME_APPEARS = "FIRST_NAME_APPEARS";
        public const string COMMON_SOURCE = "COMMON_SOURCE";
        public const string ALIAS_APPEARS = "ALIAS_APPEARS";
        public const string CAREER_IN_LOCATION = "CAREER_IN_LOCATION";
        public const string CAREER_IN_ORG_RESPONSIBLE = "CAREER_IN_ORG_RESPONSIBLE";
        public const string CAREER_IN_UNIT_RESPONSIBLE = "CAREER_IN_UNIT_RESPONSIBLE";
        public const string RESPONSIBILITY_IN_LOCATION = "RESPONSIBILITY_IN_LOCATION";

        public virtual string Feature { get; set; }
        public virtual float CurrentWeight { get; set; }
        public virtual bool Archive { get; set; }
        public virtual string Notes { get; set; }
        public virtual string Code { get; set; }
    }
}
