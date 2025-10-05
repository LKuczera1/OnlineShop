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
            return Path.Combine(projectDir, "UnitTests", "DbSource", "CatalogDb.json");
        }

        protected string? loadDbSource(string dbFileName)
        {
            return File.ReadAllText(getDbSourceDirectory(dbFileName));
        }
    }
}
