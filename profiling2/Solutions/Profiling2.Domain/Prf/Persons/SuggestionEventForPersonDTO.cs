using System.Collections.Generic;

namespace Profiling2.Domain.Prf.Persons
{
    public class SuggestionEventForPersonDTO
    {
        public int EventID { get; set; }
        public string EventName { get; set; }
        public string Score { get; set; }
        public string Features { get; set; }
        public string SuggestionReason { get; set; }

        public IList<string> SuggestionReasons { get; set; }

        public SuggestionEventForPersonDTO() { }
    }
}
