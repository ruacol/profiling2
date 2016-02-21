using System;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Prf.Sources
{
    /// <summary>
    /// Log of exceptions during Lucene indexing of the given SourceID.
    /// </summary>
    public class SourceIndexLog : Entity
    {
        public virtual int SourceID { get; set; }
        public virtual string LogSummary { get; set; }
        public virtual string Log { get; set; }
        public virtual DateTime? DateTime { get; set; }
    }
}
