using System;
using System.Configuration;
using System.IO;

namespace Profiling2.Infrastructure.Aspose.Licenses
{
    /// <summary>
    /// Use Aspose license file located on file system path configured in .config file.
    /// If not configured or not present, fallback to Aspose's built-in search paths.
    /// </summary>
    public class AsposeInitialiser
    {
        /// <summary>
        /// Each Aspose license is called from a separate class due to namespace conflict with 'Aspose' keyword.
        /// </summary>
        public static void SetLicenses()
        {
            if (HasLicenseFile)
            {
                WordsLicenseInitialiser.Initialise(GetLicenseStream());
                CellsLicenseInitialiser.Initialise(GetLicenseStream());
                SlidesLicenseInitialiser.Initialise(GetLicenseStream());
                PdfLicenseInitialiser.Initialise(GetLicenseStream());
            }
            else
            {
                WordsLicenseInitialiser.Initialise(FallbackLicense);
                CellsLicenseInitialiser.Initialise(FallbackLicense);
                SlidesLicenseInitialiser.Initialise(FallbackLicense);
                PdfLicenseInitialiser.Initialise(FallbackLicense);
            }
        }

        static string LicensePath = ConfigurationManager.AppSettings["AsposeLicenseFile"];

        static bool HasLicenseFile
        {
            get
            {
                return !string.IsNullOrEmpty(LicensePath) && File.Exists(LicensePath);
            }
        }

        static Stream GetLicenseStream()
        {
            if (HasLicenseFile)
            {
                try
                {
                    FileInfo file = new FileInfo(LicensePath);
                    return file.OpenRead();
                }
                catch (Exception) { }
            }
            return null;
        }

        const string FallbackLicense = "Aspose.Total.lic";
    }
}
