using FMDMOLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFDemo.BLL
{
    internal class ReadWriteToFlexUI
    {
        private const string spiltxt = ";$;";

        public static void DHACheckReturn(string value, string v)
        {
            FMDMOLib.Rundb rundb = new Rundb();
            object dbn = rundb.Open();
            if (Convert.ToInt16(dbn) == 1)
            {
                if (!string.IsNullOrEmpty(VarConfig.SFVar["ReturnSFMessage"]))
                {
                    rundb.SetVarValueEx(VarConfig.SFVar["ReturnSFMessage"], value);
                }

                if (v == "OK")
                {
                    rundb.SetVarValueEx("DR.CHECKOK", 1);
                    rundb.SetVarValueEx("VA.CHIN_COLOUR", 1);
                }
                else
                {
                    rundb.SetVarValueEx("DR.CHECKNOK", 1);
                    rundb.SetVarValueEx("VA.CHIN_COLOUR", 2);
                }
            }
            rundb.Close();
            rundb = null;
        }

        public static void DHALinkReturn()
        {
            FMDMOLib.Rundb rundb = new Rundb();
            object dbn = rundb.Open();
            if (Convert.ToInt16(dbn) == 1)
            {
                rundb.SetVarValueEx("DR.UPLOADFINISH", 1);
            }
            rundb.Close();
            rundb = null;
        }

        public static void DHACheck2Return(string value, string v)
        {
            FMDMOLib.Rundb rundb = new Rundb();
            object dbn = rundb.Open();
            if (Convert.ToInt16(dbn) == 1)
            {
                if (!string.IsNullOrEmpty(VarConfig.SFVar["ReturnSFMessage"]))
                {
                    rundb.SetVarValueEx(VarConfig.SFVar["ReturnSFMessage"], value);
                }

                if (v == "OK")
                {
                    rundb.SetVarValueEx("DR.LINK_OK_TFT", 1);
                    rundb.SetVarValueEx("DR.LINK_NG_TFT", 0);
                    rundb.SetVarValueEx("DR.LINK_REQUESR_TFT", 0);
                }
                else
                {
                    rundb.SetVarValueEx("DR.LINK_OK_TFT", 0);
                    rundb.SetVarValueEx("DR.LINK_NG_TFT", 1);
                    rundb.SetVarValueEx("DR.LINK_REQUESR_TFT", 0);
                }
            }
            rundb.Close();
            rundb = null;
        }

        public static void ReturnMessageToFlexUI(string value)
        {
            FMDMOLib.Rundb rundb = new Rundb();
            object dbn = rundb.Open();
            if (Convert.ToInt16(dbn) == 1)
            {
                if (!string.IsNullOrEmpty(VarConfig.SFVar["ReturnSFMessage"]))
                {
                    rundb.SetVarValueEx(VarConfig.SFVar["ReturnSFMessage"], value);
                }
            }
            rundb.Close();
            rundb = null;
        }

        public static void ReturnMessageToFlexUI(string value, string SFresult)
        {
            FMDMOLib.Rundb rundb = new Rundb();
            object dbn = rundb.Open();
            if (Convert.ToInt16(dbn) == 1)
            {
                if (!string.IsNullOrEmpty(VarConfig.SFVar["ReturnSFMessage"]))
                {
                    rundb.SetVarValueEx(VarConfig.SFVar["ReturnSFMessage"], value);
                }

                if (SFresult.Equals("OK"))
                {
                    if (!string.IsNullOrEmpty(VarConfig.SFVar["ReturnOK"]))
                    {
                        rundb.SetVarValueEx(VarConfig.SFVar["ReturnOK"], 1);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(VarConfig.SFVar["ReturnNG"]))
                    {
                        rundb.SetVarValueEx(VarConfig.SFVar["ReturnNG"], 1);
                    }
                }
            }
            rundb.Close();
            rundb = null;
        }

        public static void WriteAny()
        {
        }

        public static void ReadAny()
        {
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
                        Dictionary<string, string> InputstrPairs = new Dictionary<string, string>(GetInputStrVar(VarConfig.SFVar[values[x]]));
                        var enumerator = InputstrPairs.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            result = result + enumerator.Current.Key + "=" + Convert.ToString(rundb.GetVarValueEx(enumerator.Current.Value)) + spiltxt;
                        }
                        InputstrPairs = null;
                    }
                    else if (values[x].Equals("LinkData"))
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
                                    result = result + Convert.ToString(rundb.GetVarValueEx(enumeratorlink.Current.Key));
                                }
                                else
                                {
                                    result = result + Convert.ToString(rundb.GetVarValueEx(enumeratorlink.Current.Key)) + @"\";
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
                    else if (values[x].Equals("TestResult"))
                    {
                        string testResult = VarConfig.SFVar["TestResult"];
                        if (FileUtilHelper.CheckIfUncaseString(testResult, "PASS") || FileUtilHelper.CheckIfUncaseString(testResult, "Fail"))
                        {
                            result = result + "TestResult=" + testResult + spiltxt;
                        }
                        else
                        {
                            string tempstr = Convert.ToString(rundb.GetVarValueEx(testResult));
                            if (tempstr.Equals("0"))
                            {
                                result = result + "TestResult=PASS" + spiltxt;
                            }
                            else if (tempstr.Equals("1"))
                            {
                                result = result + "TestResult=Fail" + spiltxt;
                            }
                            else
                            {
                                result = result + "TestResult=" + tempstr + spiltxt;
                            }
                        }
                    }
                    else if (values[x].Equals("Check2Result"))
                    {
                        string testResult = VarConfig.SFVar["Check2Result"];
                        if (FileUtilHelper.CheckIfUncaseString(testResult, "PASS") || FileUtilHelper.CheckIfUncaseString(testResult, "Fail"))
                        {
                            result = result + "TestResult=" + testResult + spiltxt;
                        }
                        else
                        {
                            string tempstr = Convert.ToString(rundb.GetVarValueEx(testResult));
                            if (tempstr.Equals(0))
                            {
                                result = result + "TestResult=PASS" + spiltxt;
                            }
                            else if (tempstr.Equals(1))
                            {
                                result = result + "TestResult=Fail" + spiltxt;
                            }
                            else
                            {
                                result = result + "TestResult=" + tempstr + spiltxt;
                            }
                        }
                    }
                    else if (values[x].Equals("Check2Data"))
                    {
                        result = result + "data=0" + spiltxt;
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

        //解析inputstr生成变量键值对
        public static Dictionary<string, string> GetInputStrVar(string inputstr)
        {
            Dictionary<string, string> NameVarPairs = new Dictionary<string, string>();
            string[] sArray = inputstr.Split('/');
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
    }
}
