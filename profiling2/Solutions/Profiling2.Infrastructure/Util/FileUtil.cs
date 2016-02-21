using System;
using System.Configuration;
using System.IO;
using log4net;
using Profiling2.Domain.Prf.Sources;

namespace Profiling2.Infrastructure.Util
{
    public static class FileUtil
    {
        private static ILog Log = LogManager.GetLogger(typeof(FileUtil));

        /// <summary>
        /// Given a file name, return a lower case version of the file extension sans the dot character.
        /// 
        /// Catches and logs ArgumentExceptions.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetExtension(string path)
        {
            try
            {
                if (!string.IsNullOrEmpty(path))
                {
                    string ext = string.Empty;
                    int i = path.LastIndexOf('.');
                    if (i > -1)
                        ext = path.Substring(i);
                    //string ext = Path.GetExtension(path);
                    if (!string.IsNullOrEmpty(ext))
                        return ext.Trim().ToLower().Remove(0, 1);
                }
            }
            catch (ArgumentException e)
            {
                Log.Error("Error getting extension for " + path, e);
            }
            return string.Empty;
        }

        /// <summary>
        /// Extension method for FileUtil.GetExtension(string path), allowing it to be called directly on a Source.
        /// 
        /// Progressively tries to determine file extension for this source using several of its attributes.
        /// 
        /// Defaults to application/octet-stream if none found.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string GetFileExtension<T>(this T source)
            where T : Source
        {
            if (source != null)
            {
                if (!string.IsNullOrEmpty(source.FileExtension))
                    return source.FileExtension;
                else if (!string.IsNullOrEmpty(GetExtension(source.SourceName)))
                    return GetExtension(source.SourceName);
                else if (!string.IsNullOrEmpty(GetExtension(source.SourcePath)))
                    return GetExtension(source.SourcePath);
            }
            return "application/octet-stream";
        }

        /// <summary>
        /// Same as GetFileExtension().
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string GetDTOFileExtension<T>(this T source)
            where T : SourceDTO
        {
            if (source != null)
            {
                if (!string.IsNullOrEmpty(source.FileExtension))
                    return source.FileExtension;
                else if (!string.IsNullOrEmpty(GetExtension(source.SourceName)))
                    return GetExtension(source.SourceName);
                else if (!string.IsNullOrEmpty(GetExtension(source.SourcePath)))
                    return GetExtension(source.SourcePath);
            }
            return "application/octet-stream";
        }

        public static void DeleteTempDirFiles()
        {
            string dirStr = ConfigurationManager.AppSettings["PreviewTempFolder"];
            if (!string.IsNullOrEmpty(dirStr))
            {
                DirectoryInfo di = new DirectoryInfo(dirStr);
                if (di != null && di.Exists)
                {
                    Log.Info("Deleting jpeg and png from " + dirStr + "...");
                    int deleted = 0;
                    foreach (FileInfo fi in di.GetFiles())
                    {
                        if (fi.Extension.EndsWith("jpeg") || fi.Extension.EndsWith("png"))
                        {
                            fi.Delete();
                            deleted++;
                        }
                    }
                    Log.Info("Deleted " + deleted + " files.");
                }
            }
        }
    }
}
