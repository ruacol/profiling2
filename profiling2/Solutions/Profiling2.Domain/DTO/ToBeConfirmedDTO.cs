using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Profiling2.Domain.DTO
{
    public class ToBeConfirmedDTO
    {
        public string Type { get; set; }
        public string Priority { get; set; }
        public string Summary { get; set; }
        public string Text { get; set; }
    }
}
