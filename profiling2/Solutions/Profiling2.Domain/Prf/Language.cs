using SharpArch.Domain.DomainModel;
using System;

namespace Profiling2.Domain.Prf
{
    public class Language : Entity
    {
        public virtual string LanguageName { get; set; }
        public virtual Boolean Archive { get; set; }

        public override string ToString()
        {
            return this.LanguageName;
        }
    }
}
