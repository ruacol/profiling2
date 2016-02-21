using System;
using System.IO;
using Aspose.Slides;
using log4net;

namespace Profiling2.Infrastructure.Aspose.Licenses
{
    public class SlidesLicenseInitialiser
    {
        protected static ILog Log = LogManager.GetLogger(typeof(SlidesLicenseInitialiser));

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
