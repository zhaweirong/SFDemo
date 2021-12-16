using System.Collections.Generic;

namespace SFDemo.BLL
{
    internal class VarConfig
    {
        public static Dictionary<string, string> SFVar { set; get; }

        public static bool checkVarExist(string varname)
        {
            return !string.IsNullOrEmpty(VarConfig.SFVar[varname]);
        }
    }
}
