using System.IO;
using Lucene.Net.Analysis;

namespace Profiling2.Infrastructure.Search
{
    public class AlphaNumericTokenizer : CharTokenizer
    {
        /// <summary>
        /// Creates tokens consisting of letters and digits.  Differs from AlphaNumericFilter which will attempt to remove non-alphanumeric
        /// characters.
        /// </summary>
        /// <param name="reader"></param>
        public AlphaNumericTokenizer(TextReader reader)
            : base(reader) { }

        protected override bool IsTokenChar(char c)
        {
            return char.IsLetterOrDigit(c);
        }
    }
}
