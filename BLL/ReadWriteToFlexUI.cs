using FMDMOLib;
using System;
using System.Collections.Generic;

namespace SFDemo.BLL
{
    internal class ReadWriteToFlexUI
    {
        private const string spiltxt = ";$;";

        public static void SFReturnToFLEX(string result, string name)
        {
            FMDMOLib.Rundb rundb = new Rundb();
            object dbn = rundb.Open();
            if (Convert.ToInt16(dbn) == 1)
            {
                if (VarConfig.checkVarExist("ReturnSFMessage"))
                {
                    rundb.SetVarValueEx(VarConfig.SFVar["ReturnSFMessage"], result);
                }

                if (VarConfig.checkVarExist(name + "ReturnOK"))
                {
                    if (CheckContainUncaseString(result, "Result=PASS"))
                    {
                        rundb.SetVarValueEx(VarConfig.SFVar[name + "ReturnOK"], 1);
                    }
                    else
                    {
                        rundb.SetVarValueEx(VarConfig.SFVar[name + "ReturnOK"], 0);
                    }
                }
                if (VarConfig.checkVarExist(name + "ReturnNG"))
                {
                    if (CheckContainUncaseString(result, "Result=PASS"))
                    {
                        rundb.SetVarValueEx(VarConfig.SFVar[name + "ReturnNG"], 0);
                    }
                    else
                    {
                        rundb.SetVarValueEx(VarConfig.SFVar[name + "ReturnNG"], 1);
                    }
                }
            }
            rundb.Close();
            rundb = null;
        }

        //根据固定文本格式从flexui取得数据
        //params string[] values:values[0] = Dictionary<string, string>
        //values[] = "LinkData" ,Dictionary<string, string>=>String=value\String=value\...;$;
        //values[] = "TestResult" ,TestResult=PASS/FAIL;$;
        //Output:string  example:SN=123;$;LCD=123;$;Data=123\456\789;$;TestResult=PASS;$;
        public static string GetVarFromFLEX(params string[] values)
        {
            string result = string.Empty;

            FMDMOLib.Rundb rundb = new Rundb();
            object dbn = rundb.Open();
            if (Convert.ToInt16(dbn) == 1)
            {
                for (int x = 0; x < values.Length; x++)
                {
                    if (x == 0)
                    {
                        if (VarConfig.checkVarExist(values[x]))
                        {
                            Dictionary<string, string> InputstrPairs = new Dictionary<string, string>(GetInputStrVar(VarConfig.SFVar[values[x]]));
                            var enumerator = InputstrPairs.GetEnumerator();
                            while (enumerator.MoveNext())
                            {
                                if (enumerator.Current.Value.Contains("."))
                                {
                                    result = result + enumerator.Current.Key + "=" + Convert.ToString(rundb.GetVarValueEx(enumerator.Current.Value)) + spiltxt;
                                }
                                else
                                {
                                    result = result + enumerator.Current.Key + "=" + enumerator.Current.Value + spiltxt;
                                }
                            }
                            InputstrPairs = null;
                        }
                    }
                    else if (x == 1)
                    {
                        if (VarConfig.checkVarExist(values[x]))
                        {
                            int temp = 0;
                            result += "data=";
                            Dictionary<string, string> LinkDataPairs = new Dictionary<string, string>(GetInputStrVar(VarConfig.SFVar[values[x]]));

                            var enumeratorlink = LinkDataPairs.GetEnumerator();
                            while (enumeratorlink.MoveNext())
                            {
                                temp++;
                                if (string.IsNullOrEmpty(enumeratorlink.Current.Value))
                                {
                                    if (temp == LinkDataPairs.Count)
                                    {
                                        if (VarConfig.SFVar["Machine"].Equals("PI2"))
                                        {
                                            result = result + pi2OKNGTransfer(Convert.ToString(rundb.GetVarValueEx(enumeratorlink.Current.Key)));
                                        }
                                        else
                                        {
                                            result = result + Convert.ToString(rundb.GetVarValueEx(enumeratorlink.Current.Key));
                                        }
                                    }
                                    else
                                    {
                                        if (VarConfig.SFVar["Machine"].Equals("PI2"))
                                        {
                                            result = result + pi2OKNGTransfer(Convert.ToString(rundb.GetVarValueEx(enumeratorlink.Current.Key))) + @"\";
                                        }
                                        else
                                        {
                                            result = result + Convert.ToString(rundb.GetVarValueEx(enumeratorlink.Current.Key)) + @"\";
                                        }
                                    }
                                }
                                else
                                {
                                    if (temp == LinkDataPairs.Count)
                                    {
                                        result = result + enumeratorlink.Current.Key + " = " + Convert.ToString(rundb.GetVarValueEx(enumeratorlink.Current.Value));
                                    }
                                    else
                                    {
                                        result = result + enumeratorlink.Current.Key + " = " + Convert.ToString(rundb.GetVarValueEx(enumeratorlink.Current.Value)) + @"\";
                                    }
                                }
                            }
                            LinkDataPairs = null;
                            result += spiltxt;
                        }
                    }
                    else if (x == 2)
                    {
                        if (VarConfig.checkVarExist(values[x]))
                        {
                            string testResult = VarConfig.SFVar[values[x]];
                            if (FileUtilHelper.CheckIfUncaseString(testResult, "PASS") || FileUtilHelper.CheckIfUncaseString(testResult, "Fail"))
                            {
                                result = result + "TestResult=" + testResult + spiltxt;
                            }
                            else
                            {
                                string tempstr = Convert.ToString(rundb.GetVarValueEx(testResult));
                                if (tempstr.Equals("1"))
                                {
                                    result = result + "TestResult=PASS" + spiltxt;
                                }
                                else if (tempstr.Equals("0"))
                                {
                                    result = result + "TestResult=Fail" + spiltxt;
                                }
                                else
                                {
                                    result = result + "TestResult=" + tempstr + spiltxt;
                                }
                            }
                        }
                    }
                    else
                    {
                        result += spiltxt;
                    }
                }
            }
            rundb.Close();
            rundb = null;
            return result;
        }

        public static string GetVarHandleRecheck(string value)
        {
            string result = string.Empty;

            FMDMOLib.Rundb rundb = new Rundb();
            object dbn = rundb.Open();
            if (Convert.ToInt16(dbn) == 1)
            {
                Dictionary<string, string> InputstrPairs = new Dictionary<string, string>(GetInputStrVar(VarConfig.SFVar[value]));
                var enumerator = InputstrPairs.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    result = result + enumerator.Current.Key + "=" + Convert.ToString(rundb.GetVarValueEx(enumerator.Current.Value)) + spiltxt;
                }
                InputstrPairs = null;
            }
            rundb.Close();
            rundb = null;
            return result;
        }

        public static string GetHardCodeLinkInput(string sn, string data)
        {
            string result = string.Empty;
            FMDMOLib.Rundb rundb = new Rundb();
            object dbn = rundb.Open();
            if (Convert.ToInt16(dbn) == 1)
            {
                result = "SN=" + rundb.GetVarValueEx(sn) + ";$;line=" + rundb.GetVarValueEx("VT.@LINETYPE") + ";$;Machinenum=" + rundb.GetVarValueEx("VT.@STATION_NAME") + ";$;data=0" + rundb.GetVarValueEx(data) + ";$;TestResult=Fail";
            }
            rundb.Close();
            rundb = null;
            return result;
        }

        //解析inputstr生成变量键值对
        public static Dictionary<string, string> GetInputStrVar(string inputstr)
        {
            Dictionary<string, string> NameVarPairs = new Dictionary<string, string>();
            string[] sArray = inputstr.Split('|');
            string[] teampArray;
            foreach (string i in sArray)
            {
                teampArray = i.Split(':');
                if (teampArray.Length > 1)
                {
                    NameVarPairs.Add(teampArray[0], teampArray[1]);
                }
                else
                {
                    NameVarPairs.Add(teampArray[0], null);
                }
            }
            return NameVarPairs;
        }

        public static bool CheckContainUncaseString(string str, string value)
        {
            return str.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public static string pi2OKNGTransfer(string str)
        {
            if (str.Equals("0"))
            {
                return "OK";
            }
            else
            {
                return "NG";
            }
        }
    }
}
