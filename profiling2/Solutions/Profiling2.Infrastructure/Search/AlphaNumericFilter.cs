using System.Linq;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Tokenattributes;

namespace Profiling2.Infrastructure.Search
{
    public class AlphaNumericFilter : TokenFilter
    {
        ITermAttribute term;

        /// <summary>
        /// Remove non-alphanumeric characters from given tokens.  Differs from AlphaNumericTokenizer which preserves the non-alphanumeric character's position.
        /// 
        /// TODO there are probably some issues with offsets and positions as this filter didn't work well when testing with Person.Name fields.
        /// </summary>
        /// <param name="input"></param>
        public AlphaNumericFilter(TokenStream input)
            : base(input)
        {
            this.term = this.AddAttribute<ITermAttribute>();
        }

        public override bool IncrementToken()
        {
            if (!input.IncrementToken())
            {
                return false;
            }
            else
            {
                // remove any char that doesn't satisfy char.IsLetterOrDigit
                char[] newTermBuffer = term.TermBuffer().Where(x => char.IsLetterOrDigit(x)).ToArray();

                this.term.SetTermBuffer(newTermBuffer, 0, newTermBuffer.Length);

                return true;
            }
        }
    }
}
