using System.IO;

namespace InterviewTest.Database
{
    public static class DatabasePaths
    {
        public const string DatabaseFileName = "interview-test.db";

        public static string GetDatabasePath()
        {
            return Path.Combine(Directory.GetCurrentDirectory(), DatabaseFileName);
        }
    }
}
