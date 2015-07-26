using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Manifest
{
    public static class Logger
    {
        private static readonly XDocument ManifestDocument;
        private static readonly object SyncObject = new object();
        private static readonly string ManifestPath;

        static Logger()
        {
            var zd = Environment.GetEnvironmentVariable("ZeroDeployment") ?? "C:\\ZeroDeployment";
            ManifestPath = Path.Combine(zd, "PublishedPackages", "Patches", "patches.xml");
            if (File.Exists(ManifestPath))
            {
                ManifestDocument = XDocument.Load(ManifestPath);
            }
            else
            {
                ManifestDocument = new XDocument("patches");
                SaveToDisk();
            }
        }

        public static XElement GetPatch(string path)
        {
            lock (SyncObject)
            {
                var query =
                    (from element in ManifestDocument.Elements("patch")
                     where element.Element("path").Value == path
                     select element).FirstOrDefault();
                return query;
            }
        }

        public static void SetPatch(XElement patch)
        {
            lock (SyncObject)
            {

            }
        }
        public static void Write(string logMessage)
        {
            try
            {
                Log(logMessage, tw);
            }
            catch (IOException e)
            {
                tw.Close();
            }
        }

        public static void SaveToDisk()
        {
            lock (SyncObject)
            {
                ManifestDocument.Save(ManifestPath);
            }
        }


        public static void Log(string logMessage, TextWriter w)
        {
            // only one thread can own this lock, so other threads
            // entering this method will wait here until lock is
            // available.
            lock (SyncObject)
            {
                w.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
                    DateTime.Now.ToLongDateString());
                w.WriteLine("  :");
                w.WriteLine("  :{0}", logMessage);
                w.WriteLine("-------------------------------");
                // Update the underlying file.
                w.Flush();
            }
        }
    }
}
