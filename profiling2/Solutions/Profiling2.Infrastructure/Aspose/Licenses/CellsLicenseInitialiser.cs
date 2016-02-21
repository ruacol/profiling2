using System;
using System.IO;
using Aspose.Cells;
using log4net;

namespace Profiling2.Infrastructure.Aspose.Licenses
{
    public class CellsLicenseInitialiser
    {
        protected static ILog Log = LogManager.GetLogger(typeof(CellsLicenseInitialiser));

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
