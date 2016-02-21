using Lucene.Net.Analysis;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Util;

namespace Profiling2.Infrastructure.Search
{
    /// <summary>
    /// Custom query parser that caters for our Person.Id field which is numeric: will create a NumericRangeQuery
    /// in order to correctly query the index if it detects an Id search term.
    /// </summary>
    public class NumericQueryParser : QueryParser
    {
        public NumericQueryParser(Version version, string field, Analyzer analyzer)
            : base(version, field, analyzer)
        {

        }

        // Single value numeric search on Id field
        protected override Query GetFieldQuery(string field, string queryText)
        {
            Query query = base.GetFieldQuery(field, queryText);

            if (string.Equals("Id", field))
            {
                int value;
                if (int.TryParse(queryText, out value))
                    return NumericRangeQuery.NewIntRange(field, value, value, true, true);
            }
            return query;
        }

        // Ranged numeric search on Id field
        protected override Query GetRangeQuery(string field, string part1, string part2, bool inclusive)
        {
            TermRangeQuery query = (TermRangeQuery)base.GetRangeQuery(field, part1, part2, inclusive);

            if (string.Equals("Id", field))
            {
                return NumericRangeQuery.NewIntRange(field, int.Parse(query.LowerTerm), int.Parse(query.UpperTerm), true, true);
            }
            else
            {
                return query;
            }
        }
    }
}
