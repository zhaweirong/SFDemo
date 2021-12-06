using System.Data.Odbc;

namespace SFDemo
{
    internal class CurrentContext
    {
        internal static OdbcConnection Connection
        {
            get
            {
                return SFDemo.Connection.Instance;
            }
        }
    }
}
