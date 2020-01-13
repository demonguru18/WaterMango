using System;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace DataAccessService
{
    public static class DBAccess
    {
        private static XmlDocument _dbAccessConfig;

        private static string DllPath
        {
            get
            {
                string dllCodeBase = Assembly.GetExecutingAssembly().CodeBase;
                var dllUri = new UriBuilder(dllCodeBase);
                var dllPath = Uri.UnescapeDataString(dllUri.Path);
                return Path.GetDirectoryName(dllPath);
            }
        }
        
        public static string GetConnectionString(string connName)
        {
            LoadConfig();
            return new SqlConnectionStringBuilder
            {
                DataSource = GetXmlProp(connName, "server"),
                InitialCatalog = GetXmlProp(connName, "database"),
                UserID = GetXmlProp(connName, "username"),
                Password = GetXmlProp(connName, "password")
            }.ConnectionString;
        }

        private static string GetXmlProp(string connName, string propName)
        {
            return _dbAccessConfig.SelectSingleNode(@"databases/" + connName + "/" + propName).InnerText;
        }
        
        private static void LoadConfig()
        {
            if (_dbAccessConfig != null)
            {
                return;
            }
            _dbAccessConfig = new XmlDocument();
            var filePath = File.ReadAllText(DllPath + $"{Path.DirectorySeparatorChar}DBAccess.xml", Encoding.UTF8);
            _dbAccessConfig.LoadXml(filePath);
        }
    }
}