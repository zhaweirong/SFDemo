using System.Collections.Generic;
using System.Data.Odbc;

namespace SFDemo
{
    public class ProcedureExecuter
    {
        private static object _lock = new object();

        public static int? CommandTimeout
        {
            get { return Command.CommandTimeout; }
            set { Command.CommandTimeout = value; }
        }

        public static object ExecuteScalar(string storedProcedureName)
        {
            return ExecuteScalar(storedProcedureName, new object[] { });
        }

        public static object ExecuteScalar(string storedProcedureName, IDictionary<string, object> parameters)
        {
            object ret;
            lock (_lock)
            {
                var justopened = Connection.EnsureIsOpened();

                using (var cmd = CurrentContext.Connection.CreateCommand())
                {
                    Command.Prepare(cmd, storedProcedureName, parameters);
                    var r = Command.AddReturnValueAsParameter(cmd);
                    ret = cmd.ExecuteScalar() ?? r.Value;
                }
                if (justopened)
                    Connection.Close();
            }
            return ret;
        }

        public static object ExecuteScalar(string storedProcedureName, params object[] parameters)
        {
            object ret;
            lock (_lock)
            {
                var justopened = Connection.EnsureIsOpened();

                using (var cmd = CurrentContext.Connection.CreateCommand())
                {
                    Command.Prepare(cmd, storedProcedureName, parameters);
                    var r = Command.AddReturnValueAsParameter(cmd);
                    ret = cmd.ExecuteScalar() ?? r.Value;
                }
                if (justopened)
                    Connection.Close();
            }
            return ret;
        }

        public static int ExecuteNonQuery(string storedProcedure)
        {
            return ExecuteNonQuery(storedProcedure, new object[] { });
        }

        public static string ExecuteNonQuery(string storedProcedureName, string pa1, string pa2, string pa3, string pa4)
        {
            string output = string.Empty;
            lock (_lock)
            {
                var justopened = Connection.EnsureIsOpened();

                using (var cmd = CurrentContext.Connection.CreateCommand())
                {
                    output = Command.Prepare(cmd, storedProcedureName, pa1, pa2, pa3, pa4);
                }
                if (justopened)
                    Connection.Close();
            }
            return output;
        }

        public static int ExecuteNonQuery(string storedProcedureName, IDictionary<string, object> parameters)
        {
            int count;
            lock (_lock)
            {
                var justopened = Connection.EnsureIsOpened();

                using (var cmd = CurrentContext.Connection.CreateCommand())
                {
                    Command.Prepare(cmd, storedProcedureName, parameters);
                    count = cmd.ExecuteNonQuery();
                }
                if (justopened)
                    Connection.Close();
            }
            return count;
        }

        public static int ExecuteNonQuery(string storedProcedureName, params object[] parameters)
        {
            int count;
            lock (_lock)
            {
                var justopened = Connection.EnsureIsOpened();

                using (var cmd = CurrentContext.Connection.CreateCommand())
                {
                    Command.Prepare(cmd, storedProcedureName, parameters);
                    count = cmd.ExecuteNonQuery();
                }
                if (justopened)
                    Connection.Close();
            }
            return count;
        }

        public static IEnumerable<T> ExecuteReader<T>(string storedProcedureName) where T : new()
        {
            return ExecuteReader<T>(storedProcedureName, new object[] { });
        }

        public static IEnumerable<T> ExecuteReader<T>(string storedProcedureName, IDictionary<string, object> parameters) where T : new()
        {
            IList<T> list = new List<T>();
            lock (_lock)
            {
                var justopened = Connection.EnsureIsOpened();
                var modelType = typeof(T);
                using (var cmd = CurrentContext.Connection.CreateCommand())
                {
                    Command.Prepare(cmd, storedProcedureName, parameters);

                    using (OdbcDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var item = ResultReader.ToDomainObject<T>(modelType, reader);
                            list.Add(item);
                        }
                    }
                }
                if (justopened)
                    Connection.Close();
            }
            return list;
        }

        public static IEnumerable<T> ExecuteReader<T>(string storedProcedureName, params object[] parameters) where T : new()
        {
            IList<T> list = new List<T>();
            lock (_lock)
            {
                var justopened = Connection.EnsureIsOpened();
                var modelType = typeof(T);
                using (var cmd = CurrentContext.Connection.CreateCommand())
                {
                    Command.Prepare(cmd, storedProcedureName, parameters);

                    using (OdbcDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var item = ResultReader.ToDomainObject<T>(modelType, reader);
                            list.Add(item);
                        }
                    }
                }
                if (justopened)
                    Connection.Close();
            }
            return list;
        }
    }
}