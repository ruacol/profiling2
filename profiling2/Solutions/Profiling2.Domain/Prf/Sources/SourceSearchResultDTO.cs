using System;

namespace Profiling2.Domain.Prf.Sources
{
    public class SourceSearchResultDTO : BaseSourceDTO
    {
        /// <summary>
        /// Adds columns to a Source relevant to source search.
        /// </summary>
        public SourceSearchResultDTO() { }

        // pre-calculated columns from SQL query
        public int IsRelevant { get; set; }
        public DateTime? ReviewedDateTime { get; set; }
        public int? IsAttached { get; set; }
        public int Rank { get; set; }
    }
}
