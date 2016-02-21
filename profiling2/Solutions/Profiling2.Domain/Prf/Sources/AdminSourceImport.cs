using System;
using SharpArch.Domain.DomainModel;
using System.Web.Script.Serialization;

namespace Profiling2.Domain.Prf.Sources
{
    /// <summary>
    /// Log of when a Source is originally imported - populated by DocumentImportConsole, but on the road to deprecation.
    /// 
    /// See SourceLog - which stores errors related to processing Sources.
    /// </summary>
    public class AdminSourceImport : Entity
    {
        [ScriptIgnore]
        public virtual Source Source { get; set; }
        public virtual string SourcePath { get; set; }
        public virtual string ModifiedPath { get; set; }
        public virtual bool HadPassword { get; set; }
        public virtual DateTime ImportDate { get; set; }
    }
}
