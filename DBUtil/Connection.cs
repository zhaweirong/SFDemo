using System.Configuration;
using System.Data.Odbc;

namespace SFDemo
{
    public static class Connection
    {
        private const string DefaultConnectionStringName = "ODBC";
        private static string _connectionStringName;
        private static string _connectionString;

        private static string _ConnectionStringName
        {
            get { return _connectionStringName ?? DefaultConnectionStringName; }
        }

        private static string _ConnectionString
        {
            get { return _connectionString ?? ConfigurationManager.ConnectionStrings[_ConnectionStringName].ConnectionString; }
        }

        public static void ConnectionStringName(string connectionStringName)
        {
            _connectionStringName = connectionStringName;
        }

        public static void ConnectionString(string connectionString)
        {
            _connectionString = connectionString;
        }

        internal static bool EnsureIsOpened()
        {
            if (Instance == null || Instance.State != System.Data.ConnectionState.Open)
            {
                Close();
                Instance = new OdbcConnection(_ConnectionString);
                Instance.Open();
                return true;
            }
            return false;
        }

        internal static OdbcConnection Instance { get; private set; }

        internal static void Close()
        {
            if (Instance == null) return;
            Instance.Close();
            Instance.Dispose();
        }
    }
}
