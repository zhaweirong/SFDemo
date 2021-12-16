﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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