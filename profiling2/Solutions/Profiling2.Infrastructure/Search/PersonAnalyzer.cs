using Lucene.Net.Analysis;
using System.IO;

namespace Profiling2.Infrastructure.Search
{
    /// <summary>
    /// Used on searchable person text fields.  Intent is to allow users to search via names, ID numbers, or rank/function.
    /// 
    /// We normalise text by converting to lower case, replacing special ASCII chars with their basic equivalents, and 
    /// removing non-alphanumeric chars.
    /// 
    /// During indexing, the MilitaryIDNumber field is further filtered by removing extraneous, non-ID number related text
    /// (however being a free-text field, some entries are not properly formatted)...
    /// 
    /// Created because WhitespaceAnalyzer doesn't do lower case, and SimpleAnalyzer removes digits.
    /// </summary>
    public class PersonAnalyzer : Analyzer
    {
        // the ordering of these filters is important!
        public override TokenStream TokenStream(string fieldName, TextReader reader)
        {
            if (string.Equals("MilitaryIDNumber", fieldName))
            {
                TokenStream result = new WhitespaceTokenizer(reader);
                result = new LowerCaseFilter(result);
                result = new ASCIIFoldingFilter(result);
                result = new AlphaNumericFilter(result);  // behaves weirdly when used on Name field

                // during indexing, we will encounter some of the following extraneous text we don't care about.
                string[] stopWords = new string[] { "", "formerly", "or", "former", "pir", "tbc", "id", "pnc" };
                return new StopFilter(false, result, new CharArraySet(stopWords, true), true);
            }
            else
            {
                TokenStream result = new AlphaNumericTokenizer(reader);
                result = new LowerCaseFilter(result);
                return new ASCIIFoldingFilter(result);
            }
        }
    }
}
