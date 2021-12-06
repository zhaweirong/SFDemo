using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;

namespace SFDemo
{
    internal class Command
    {
        private const string ParametersQuery = "SELECT PARAMETER_NAME FROM INFORMATION_SCHEMA.Parameters WHERE SPECIFIC_NAME = @SpecificName ORDER BY ORDINAL_POSITION ASC";
        private const string ParameterNameColumn = "PARAMETER_NAME";

        internal static int? CommandTimeout { get; set; }

        internal static OdbcParameter AddReturnValueAsParameter(OdbcCommand cmd)
        {
            var r = new OdbcParameter { ParameterName = "ReturnValue", Direction = ParameterDirection.ReturnValue };
            cmd.Parameters.Add(r);
            return r;
        }

        internal static void Prepare(OdbcCommand cmd, string storeProcedureName, IDictionary<string, object> parameters)
        {
            DictionaryToParameters(cmd, parameters);
            Prepare(cmd, storeProcedureName);
        }

        internal static void Prepare(OdbcCommand cmd, string storeProcedureName, params object[] parameters)
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = storeProcedureName;
            cmd.CommandTimeout = CommandTimeout ?? 30;
            ArgumentsToParameters(cmd, parameters);
        }

        internal static string Prepare(OdbcCommand cmd, string storeProcedureName, string para1, string para2, string para3, string para4)
        {
            string output = string.Empty;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "{ call " + storeProcedureName + " (?,?,?,?,?)}";
            cmd.CommandTimeout = CommandTimeout ?? 30;

            OdbcParameter parameter1 = new OdbcParameter("@BU", OdbcType.Char);
            parameter1.Direction = ParameterDirection.Input;
            parameter1.Value = para1;
            cmd.Parameters.Add(parameter1);

            OdbcParameter parameter2 = new OdbcParameter("@Station", OdbcType.Char);
            parameter2.Direction = ParameterDirection.Input;
            parameter2.Value = para2;
            cmd.Parameters.Add(parameter2);

            OdbcParameter parameter3 = new OdbcParameter("@Step", OdbcType.Char);
            parameter3.Direction = ParameterDirection.Input;
            parameter3.Value = para3;
            cmd.Parameters.Add(parameter3);

            OdbcParameter parameter4 = new OdbcParameter("@InputStr", OdbcType.Char);
            parameter4.Direction = ParameterDirection.Input;
            parameter4.Value = para4;
            cmd.Parameters.Add(parameter4);

            OdbcParameter parameter5 = new OdbcParameter("@Output", OdbcType.VarChar, 256);
            parameter5.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(parameter5);

            cmd.ExecuteNonQuery();
            output = (string)parameter5.Value;
            return output;
        }

        private static void DictionaryToParameters(OdbcCommand cmd, IDictionary<string, object> parameters)
        {
            if (parameters == null) return;
            foreach (var parameter in parameters)
                cmd.Parameters.AddWithValue(parameter.Key, parameter.Value);
        }

        private static void ArgumentsToParameters(OdbcCommand cmd, params object[] parameters)
        {
            if (parameters == null || parameters.Length == 0) return;
            using (var command = new OdbcCommand(ParametersQuery, cmd.Connection))
            {
                //command.Parameters.AddWithValue("@SpecificName", cmd.CommandText);
                using (var reader = command.ExecuteReader())
                {
                    var i = 0;
                    while (reader.Read() && i < parameters.Length)
                        cmd.Parameters.AddWithValue(reader[ParameterNameColumn] as string, parameters[i++]);
                }
            }
        }
    }
}
