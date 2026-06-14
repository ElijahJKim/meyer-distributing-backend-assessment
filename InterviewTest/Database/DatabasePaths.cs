using System;
using System.IO;

namespace InterviewTest.Database
{
    public static class DatabasePaths
    {
        public const string DatabaseFileName = "interview-test.db";

        private static string _overridePath;

        public static string GetDatabasePath()
        {
            return _overridePath ?? Path.Combine(Directory.GetCurrentDirectory(), DatabaseFileName);
        }

        public static void SetDatabasePath(string path)
        {
            _overridePath = path;
        }

        public static void ResetDatabasePath()
        {
            _overridePath = null;
        }
    }
}
