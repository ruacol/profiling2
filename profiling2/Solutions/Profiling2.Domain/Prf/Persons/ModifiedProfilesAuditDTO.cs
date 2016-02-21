using System;

namespace Profiling2.Domain.Prf.Persons
{
    public class ModifiedProfilesAuditDTO
    {
        public int LogNo { get; set; }
        public string Who { get; set; }
        public string What { get; set; }
        public DateTime When { get; set; }
        public string PreviousValues { get; set; }
        public string NonProfilingChange { get; set; }
        public string PersonID { get; set; }
        public string Person { get; set; }
        // Status should be populated by new audit records; old audit records don't have the data at the time
        public string Status { get; set; }  
    }
}
