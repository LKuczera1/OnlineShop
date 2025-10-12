using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Common
{
    public static class Tools
    {
        private static readonly string UtilityPath = "D:\\Programming\\Projects\\Visual Studio\\OnlineShop\\Utility";
        public static string? GetProjectDirectory(string path)
        {
            var projectDir = Directory.GetParent(AppContext.BaseDirectory).Parent!.Parent!.Parent!.Parent!.FullName;
            return Path.Combine(projectDir, path);
        }

        public static string GetAppSettingsDirectory()
        {
            var path = GetProjectDirectory(Path.Combine("Utility", "AppSettings", "appsettings.json"));

            //Temporal solution
            if (path == null) return UtilityPath;

            return path;
        }

        public static PhysicalFileProvider GetPhysicalFileProviderToUtility()
        {
            return new PhysicalFileProvider(UtilityPath);
        }
    }
}

