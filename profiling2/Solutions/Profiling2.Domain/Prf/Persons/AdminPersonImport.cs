using System;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Prf.Persons
{
    /// <summary>
    /// Import notes for those Persons that were imported from a previous database known as 'Sygeco'.
    /// </summary>
    public class AdminPersonImport : Entity
    {
        public virtual Person Person { get; set; }
        public virtual AdminPersonImportType AdminPersonImportType { get; set; }
        public virtual DateTime ImportDate { get; set; }
        public virtual int PreviousID { get; set; }
        public virtual string Notes { get; set; }
        public virtual bool Archive { get; set; }
    }
}
