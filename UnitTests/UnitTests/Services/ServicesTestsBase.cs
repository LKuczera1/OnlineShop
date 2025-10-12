using Microsoft.EntityFrameworkCore;

namespace UnitTests.UnitTests.Services
{
    public class ServicesTestsBase
    {
        public ServicesTestsBase()
        {

        }

        protected string? getDbSourceDirectory(string dbFileName)
        {
            var projectDir = Directory.GetParent(AppContext.BaseDirectory).Parent!.Parent!.Parent!.FullName;
            return Path.Combine(projectDir, "UnitTests", "DbSource", dbFileName);
        }

        protected string? loadDbSource(string dbFileName)
        {
            return File.ReadAllText(getDbSourceDirectory(dbFileName));
        }
        protected string? getSolutionDirectory()
        {
            var projectDir = Directory.GetParent(AppContext.BaseDirectory).Parent!.Parent!.Parent!.Parent!.FullName;
            return projectDir;
        }
    }
}

