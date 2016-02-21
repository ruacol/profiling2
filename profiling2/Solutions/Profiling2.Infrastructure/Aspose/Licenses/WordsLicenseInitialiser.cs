using System;
using System.IO;
using Aspose.Words;
using log4net;

namespace Profiling2.Infrastructure.Aspose.Licenses
{
    public class WordsLicenseInitialiser
    {
        protected static ILog Log = LogManager.GetLogger(typeof(WordsLicenseInitialiser));

        public static void Initialise(Stream stream)
        {
            License license = new License();
            try
            {
                license.SetLicense(stream);
            }
            catch (Exception e)
            {
                Log.Error("Problem initialising Aspose license.", e);
            }
        }

        public static void Initialise(string path)
        {
            License license = new License();
            try
            {
                license.SetLicense(path);
            }
            catch (Exception e)
            {
                Log.Error("Problem initialising Aspose license.", e);
            }
        }
    }
}
