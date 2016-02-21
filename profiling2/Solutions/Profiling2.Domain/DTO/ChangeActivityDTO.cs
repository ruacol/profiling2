using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Profiling2.Domain.DTO
{
    public class ChangeActivityDTO
    {
        public int LogNo { get; set; }
        public string Who { get; set; }
        public string What { get; set; }
        public DateTime When { get; set; }
        public string PreviousValues { get; set; }
        public string NonProfilingChange { get; set; }

        public ChangeActivityDTO() { }
    }
}
