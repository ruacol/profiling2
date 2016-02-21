using System.Collections.Generic;
using System.IO;
using Profiling2.Domain.Prf.Sources;

namespace Profiling2.Domain.Contracts.Services
{
    public interface IAsposeService
    {
        /// <summary>
        /// Front-end method to create HTML from the given source; responsibility for closing the destination stream rests with the caller.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        Stream ConvertToHtml(Source source, Stream destination);

        IDictionary<string, object> GetWordDocumentProperties(Source source);

        /// <summary>
        /// If password is correct, returns Stream representing unprotected Source.  Otherwise return null.  Responsibility for closing the
        /// destination stream rests with the caller.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="password"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        Stream StripPassword(Source source, string password, Stream destination);
    }
}
