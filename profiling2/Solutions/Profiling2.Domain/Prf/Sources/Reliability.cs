using System;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Prf.Sources
{
    public class Reliability : Entity, IComparable
    {
        public const string NAME_HIGH = "High";
        public const string NAME_MODERATE = "Moderate";
        public const string NAME_LOW = "Low";

        public virtual string ReliabilityName { get; set; }
        public virtual bool Archive { get; set; }
        public virtual string Notes { get; set; }

        protected virtual int CompareValue
        {
            get
            {
                switch (this.ReliabilityName)
                {
                    case Reliability.NAME_HIGH:
                        return 3;
                    case Reliability.NAME_MODERATE:
                        return 2;
                    case Reliability.NAME_LOW:
                        return 1;
                    default:
                        return int.MinValue;
                }
            }
        }

        public override string ToString()
        {
            return this.ReliabilityName;
        }

        public virtual object ToJSON()
        {
            return new
                {
                    Id = this.Id,
                    Name = this.ReliabilityName
                };
        }

        public virtual int CompareTo(object obj)
        {
            if (obj == null) return 1;
            
            Reliability otherReliability = obj as Reliability;
            if (otherReliability != null)
                return this.CompareValue.CompareTo(otherReliability.CompareValue);
            else
                throw new ArgumentException("Object is not a Reliability");
        }
    }
}
