using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Prf.Persons
{
    public class PersonRelationshipType : Entity
    {
        public const string CODE_IS_A_SUPERIOR_TO = "IS_A_SUPERIOR_TO";
        public const string CODE_FOUGHT_WITH = "FOUGHT_WITH";
        public const string CODE_IS_A_SUBORDINATE_OF = "IS_A_SUBORDINATE_OF";
        public const string CODE_ACTED_TOGETHER_WITH = "ACTED_TOGETHER_WITH";
        public const string CODE_IS_THE_DEPUTY_OF = "IS_THE_DEPUTY_OF";
        public const string CODE_IS_THE_BODYGUARD_OF = "IS_THE_BODYGUARD_OF";
        public const string CODE_FOUGHT_IN_THE_SAME_GROUP_AS = "FOUGHT_IN_THE_SAME_GROUP_AS";
        public const string CODE_BELONGED_TO_THE_SAME_GROUP_AS = "BELONGED_TO_THE_SAME_GROUP_AS";
        public const string CODE_PROVIDED_WEAPONS_AND_AMMUNITION_TO = "PROVIDED_WEAPONS_AND_AMMUNITION_TO";

        [Audited]
        public virtual string PersonRelationshipTypeName { get; set; }
        [Audited]
        public virtual string Code { get; set; }
        [Audited]
        public virtual bool IsCommutative { get; set; }
        [Audited]
        public virtual bool Archive { get; set; }
        [Audited]
        public virtual string Notes { get; set; }

        [Audited]
        public virtual IList<PersonRelationship> PersonRelationships { get; set; }

        public PersonRelationshipType()
        {
            this.PersonRelationships = new List<PersonRelationship>();
        }

        public override string ToString()
        {
            return this.PersonRelationshipTypeName;
        }
    }
}
