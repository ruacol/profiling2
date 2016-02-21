using Profiling2.Domain.Prf;
using System.Diagnostics;
using System.Reflection;

namespace Profiling2.Web.Mvc
{
    public class GlobalViewSingleton
    {
        private static GlobalViewSingleton instance;
        private static string productVersion;

        private GlobalViewSingleton() { }

        public static GlobalViewSingleton Instance
        {
            get 
            {
                if (instance == null)
                {
                    instance = new GlobalViewSingleton();
                }
                return instance;
            }
        }

        public string ProductVersion
        {
            get
            {
                if (productVersion == null)
                {
                    Assembly assembly = Assembly.GetAssembly(typeof(AdminRole));
                    FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                    productVersion = fvi.ProductVersion;
                }
                return productVersion;
            }
        }
    }
}