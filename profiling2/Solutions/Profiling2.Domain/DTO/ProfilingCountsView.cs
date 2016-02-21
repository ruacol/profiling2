using System;
using System.Collections.Generic;
using Profiling2.Domain.Prf.Persons;

namespace Profiling2.Domain.DTO
{
    public class ProfilingCountsView
    {
        public string AsOfDate { get; set; }
        public IDictionary<ProfileStatus, int> ProfileStatus { get; set; }
        public int Career { get; set; }
        public int Organization { get; set; }
        public int Event { get; set; }
        public int PersonResponsibility { get; set; }
        public int OrganizationResponsibility { get; set; }
        public int Source { get; set; }

        public ProfilingCountsView() { }
    }
}