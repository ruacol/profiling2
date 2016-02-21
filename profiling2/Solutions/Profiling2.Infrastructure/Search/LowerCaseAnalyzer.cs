using System.IO;
using Lucene.Net.Analysis;

namespace Profiling2.Infrastructure.Search
{
    public class LowerCaseAnalyzer : Analyzer
    {
        public override TokenStream TokenStream(string fieldName, TextReader reader)
        {
            var result = new KeywordTokenizer(reader);
            return new LowerCaseFilter(result);
        }
    }
}
