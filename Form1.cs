using FMDMOLib;
using Ini.Net;
using Lin.LogHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Windows.Forms;

namespace SFDemo
{
    public partial class Form1 : Form
    {
        private const string spiltxt = ";$;";
        private System.Threading.Timer timer = null;
        private System.Threading.Timer CheckProcesstimer = null;

        private Dictionary<string, string> SFVar;

        private bool SFcheckSwitch = true;
        private bool SFlinkSwitch = true;
        private bool SFcheck2Switch = true;
        private bool SFtotalSwitch = true;

        private TrigVar SFcheckTrig = new TrigVar();
        private TrigVar SFlinkTrig = new TrigVar();
        private TrigVar SFcheck2Trig = new TrigVar();

        private bool StartPollingTrig = true;

        private readonly string folderPath = GetAppsettingStr("Path");
        private readonly int trigPollingtime = int.Parse(GetAppsettingStr("TrigPollingTime"));
        private readonly string CHECK1ProcedureName = GetAppsettingStr("CHECK1ProcedureName");
        private readonly string CHECK2ProcedureName = GetAppsettingStr("CHECK2ProcedureName");
        private readonly string LINKProcedureName = GetAppsettingStr("LINKProcedureName");

        private readonly int RetryCount = int.Parse(GetAppsettingStr("RetryCount"));
        private readonly double RetryInterval = (double.Parse(GetAppsettingStr("RetryInterval"))) / 1000;

        private readonly string[] CheckLogName = GetAppsettingStr("CheckLogName").Split(',');
        private readonly string[] LinkLogName = GetAppsettingStr("LinkLogName").Split(',');

        public delegate void DCloseWindow(string pwd);

        private string close = "";

        public Form1()

        {
            InitializeComponent();
            SFPanel.Visible = true;
            HivePanel.Visible = false;
            currentPaneltext.Text = "SF";
            SFButton.BackColor = System.Drawing.Color.LightGray;
            HiveButton.BackColor = System.Drawing.Color.White;

            try
            {
                //ini获取
                var iniFile = new IniFile("var.ini");
                SFVar = (Dictionary<string, string>)iniFile.ReadSection("SF");

                SFcheckSwitch = !String.IsNullOrEmpty(SFVar["CheckTrig"]);
                SFlinkSwitch = !String.IsNullOrEmpty(SFVar["LinkTrig"]);

                SFcheck2Switch = !String.IsNullOrEmpty(SFVar["Check2Trig"]);

                FileUtilHelper.CreateDirectory(folderPath);

                SFcheckTrig.PropertyChanged += SFcheck;
                SFlinkTrig.PropertyChanged += SFlink;

                SFcheck2Trig.PropertyChanged += SFcheck2;
                //Check FlexUI OPEN
                CheckFlexUIProcess();
                //Get SFTrig from flexUI
                PollingTrig();
            }
            catch (Exception ex)
            {
                ex.ToString().LogForError();
                throw new Exception("配置文件读取异常");
            }
        }

        #region 数据源状态定时检查

        private void CheckFlexUIProcess()
        {
            CheckProcesstimer = new System.Threading.Timer((n) =>
            {
                Process[] processesClient = Process.GetProcessesByName("view");
                if (processesClient.Length == 0)
                {
                    StartPollingTrig = false;
                    FlexUIStatus.BackColor = System.Drawing.Color.Red;
                    GC.Collect();
                }
                else
                {
                    StartPollingTrig = true;
                    FlexUIStatus.BackColor = System.Drawing.Color.LimeGreen;
                }
            }, "1", 0, 10000);
        }

        #endregion 数据源状态定时检查

        #region 数据定时获取

        private void PollingTrig()
        {
            timer = new System.Threading.Timer((n) =>
            {
                if (StartPollingTrig)
                {
                    FMDMOLib.Rundb rundb = new Rundb();
                    object dbn = rundb.Open();
                    if (Convert.ToInt16(dbn) == 1)
                    {
                        if (SFtotalSwitch)
                        {
                            if (SFcheckSwitch)
                            {
                                SFcheckTrig.Trig = Convert.ToString(rundb.GetVarValueEx(SFVar["CheckTrig"]));
                            }
                            if (SFcheck2Switch)
                            {
                                SFcheck2Trig.Trig = Convert.ToString(rundb.GetVarValueEx(SFVar["Check2Trig"]));
                            }
                            if (SFlinkSwitch)
                            {
                                SFlinkTrig.Trig = Convert.ToString(rundb.GetVarValueEx(SFVar["LinkTrig"]));
                            }
                        }
                    }
                    rundb.Close();
                }
            }, "1", 1000, trigPollingtime);
        }

        #endregion 数据定时获取

        #region CHECK/CHECK2/LINK

        private void SFcheck(object sender, PropertyChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(SFcheckTrig.Trig)) { return; }

            string output = string.Empty;
            string InputStr = string.Empty;
            int retrytime = 0;
            string retryReason = string.Empty;
            try
            {
                InputStr = GetVarFromFLEX("CheckInputStr");

                Retry.RetryHandle(RetryCount, TimeSpan.FromSeconds(RetryInterval), false, delegate
                {
                    //output = ProcedureExecuter.ExecuteNonQuery(CHECK1ProcedureName, SFVar["BU"], SFVar["Station"], "Request", InputStr);

                    if ((output.IndexOf("PASS", StringComparison.OrdinalIgnoreCase) <= 0))
                    {
                        retrytime++;
                        retryReason = retryReason + "次数" + retrytime + ":" + output + ";";

                        throw new Exception("SFCheckReturnErrorMsg");
                    }
                });
                if (retrytime > 0)
                {
                    ("checkRetry:" + retryReason).LogForDebug();
                }

                if (string.Equals(output.Substring(11, 4), "PASS", StringComparison.OrdinalIgnoreCase))
                {
                    if (SFVar["Machine"].Equals("DHA"))
                    {
                        DHACheckReturn(output, "OK");
                    }
                    else
                    {
                        ReturnMessageToFlexUI(output, "OK");
                    }
                    SoundHelper.PlaySound("CheckOK");
                }
                else
                {
                    if (SFVar["Machine"].Equals("DHA"))
                    {
                        DHACheckReturn(output, "NG");
                    }
                    else
                    {
                        ReturnMessageToFlexUI(output, "NG");
                    }
                    SoundHelper.PlaySound("CheckNG");
                }

                WriteSFLog(CheckLogName[0], e.PropertyName, InputStr, output, retrytime);
            }
            catch (Exception ex)
            {
                ex.ToString().LogForError();
                BackgroundProcess(ex.ToString());
            }
        }

        private void SFcheck2(object sender, PropertyChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(SFcheck2Trig.Trig)) { return; }

            string output = string.Empty;
            string InputStr = string.Empty;
            int retrytime = 0;
            string retryReason = string.Empty;
            try
            {
                InputStr = GetVarFromFLEX("Check2InputStr", "Check2Data", "Check2Result");
                Retry.RetryHandle(RetryCount, TimeSpan.FromSeconds(RetryInterval), false, delegate
                {
                    //output = ProcedureExecuter.ExecuteNonQuery(CHECK2ProcedureName, SFVar["BU2"], SFVar["Station2"], "Require", InputStr);
                    if (output.IndexOf("PASS", StringComparison.OrdinalIgnoreCase) <= 0 && output.IndexOf("FAIL", StringComparison.OrdinalIgnoreCase) <= 0)
                    {
                        retrytime++;
                        retryReason = retryReason + "次数" + retrytime + ":" + output + ";";
                        throw new Exception("SFCheck2ReturnErrorMsg");
                    }
                });

                if (retrytime > 0)
                {
                    ("check2Retry:" + retryReason).LogForDebug();
                }

                if (string.Equals(output.Substring(11, 4), "PASS", StringComparison.OrdinalIgnoreCase))
                {
                    if (SFVar["Machine"].Equals("DHA"))
                    {
                        DHACheck2Return(output, "OK");
                    }
                    else
                    {
                        ReturnMessageToFlexUI(output);
                    }
                    SoundHelper.PlaySound("Link1OK");
                }
                else
                {
                    if (SFVar["Machine"].Equals("DHA"))
                    {
                        DHACheck2Return(output, "NG");
                    }
                    else
                    {
                        ReturnMessageToFlexUI(output);
                    }

                    SoundHelper.PlaySound("Link1NG");
                }
                if (SFVar["Machine"].Equals("DHA"))
                {
                    WriteSFLog("TFT", "LinkTFT", e.PropertyName, InputStr, output, retrytime);
                }
                else
                {
                    WriteSFLog(LinkLogName[0], LinkLogName[0], e.PropertyName, InputStr, output, retrytime);
                }
            }
            catch (Exception ex)
            {
                ex.ToString().LogForError();
                BackgroundProcess(ex.ToString());
            }
        }

        private void SFlink(object sender, PropertyChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(SFlinkTrig.Trig)) { return; }

            string output = string.Empty;
            string InputStr = string.Empty;
            int retrytime = 0;
            string retryReason = string.Empty;
            try
            {
                InputStr = GetVarFromFLEX("LinkInputStr", "LinkData", "TestResult");
                Retry.RetryHandle(RetryCount, TimeSpan.FromSeconds(RetryInterval), false, delegate
                {
                    //output = ProcedureExecuter.ExecuteNonQuery(LINKProcedureName, SFVar["BU"], SFVar["Station"], "Require", InputStr);
                    if (output.IndexOf("PASS", StringComparison.OrdinalIgnoreCase) <= 0 && output.IndexOf("FAIL", StringComparison.OrdinalIgnoreCase) <= 0)
                    {
                        retrytime++;
                        retryReason = retryReason + "次数" + retrytime + ":" + output + ";";
                        throw new Exception("SFLinkReturnErrorMsg");
                    }
                });

                if (retrytime > 0)
                {
                    ("linkRetry:" + retryReason).LogForDebug();
                }

                ReturnMessageToFlexUI(output);

                if (string.Equals(output.Substring(11, 4), "PASS", StringComparison.OrdinalIgnoreCase))
                {
                    if (SFVar["Machine"].Equals("DHA"))
                    {
                        DHALinkReturn();
                    }
                    SoundHelper.PlaySound("Link2OK");
                }
                else
                {
                    SoundHelper.PlaySound("Link2NG");
                }

                WriteSFLog(LinkLogName[1], e.PropertyName, InputStr, output, retrytime);
            }
            catch (Exception ex)
            {
                ex.ToString().LogForError();
                BackgroundProcess(ex.ToString());
            }
        }

        private void DHACheckReturn(string value, string v)
        {
            FMDMOLib.Rundb rundb = new Rundb();
            object dbn = rundb.Open();
            if (Convert.ToInt16(dbn) == 1)
            {
                if (!string.IsNullOrEmpty(SFVar["ReturnSFMessage"]))
                {
                    rundb.SetVarValueEx(SFVar["ReturnSFMessage"], value);
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

        private void DHALinkReturn()
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

        private void DHACheck2Return(string value, string v)
        {
            FMDMOLib.Rundb rundb = new Rundb();
            object dbn = rundb.Open();
            if (Convert.ToInt16(dbn) == 1)
            {
                if (!string.IsNullOrEmpty(SFVar["ReturnSFMessage"]))
                {
                    rundb.SetVarValueEx(SFVar["ReturnSFMessage"], value);
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

        private void ReturnMessageToFlexUI(string value)
        {
            FMDMOLib.Rundb rundb = new Rundb();
            object dbn = rundb.Open();
            if (Convert.ToInt16(dbn) == 1)
            {
                if (!string.IsNullOrEmpty(SFVar["ReturnSFMessage"]))
                {
                    rundb.SetVarValueEx(SFVar["ReturnSFMessage"], value);
                }
            }
            rundb.Close();
            rundb = null;
        }

        private void ReturnMessageToFlexUI(string value, string SFresult)
        {
            FMDMOLib.Rundb rundb = new Rundb();
            object dbn = rundb.Open();
            if (Convert.ToInt16(dbn) == 1)
            {
                if (!string.IsNullOrEmpty(SFVar["ReturnSFMessage"]))
                {
                    rundb.SetVarValueEx(SFVar["ReturnSFMessage"], value);
                }

                if (SFresult.Equals("OK"))
                {
                    if (!string.IsNullOrEmpty(SFVar["ReturnOK"]))
                    {
                        rundb.SetVarValueEx(SFVar["ReturnOK"], 1);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(SFVar["ReturnNG"]))
                    {
                        rundb.SetVarValueEx(SFVar["ReturnNG"], 1);
                    }
                }
            }
            rundb.Close();
            rundb = null;
        }

        #endregion CHECK/CHECK2/LINK

        #region 画面线程传参数

        private delegate void CrossThreadOperationControl();

        private void BackgroundProcess(string outputstr)
        {
            CrossThreadOperationControl CrossAdd = delegate ()
            {
                if (SFlistBox.Items.Count >= 20)
                {
                    SFlistBox.Items.Clear();
                }
                SFlistBox.Items.Add(outputstr);
            };
            SFlistBox.Invoke(CrossAdd);
        }

        #endregion 画面线程传参数

        #region 界面切换

        private void SFButton_Click(object sender, EventArgs e)
        {
            SFPanel.Visible = true;
            HivePanel.Visible = false;
            currentPaneltext.Text = "SF";
            SFButton.BackColor = System.Drawing.Color.LightGray;
            HiveButton.BackColor = System.Drawing.Color.White;
        }

        private void HiveButton_Click(object sender, EventArgs e)
        {
            HivePanel.Visible = true;
            SFPanel.Visible = false;
            currentPaneltext.Text = "Hive";
            HiveButton.BackColor = System.Drawing.Color.LightGray;
            SFButton.BackColor = System.Drawing.Color.White;
        }

        private void SFswitchbutton_Click(object sender, EventArgs e)
        {
            if (SFtotalSwitch is false)
            {
                SFtotalSwitch = true;
                SFswitchbutton.Text = "SF ON ";
                SFswitchbutton.BackColor = System.Drawing.Color.LimeGreen;
            }
            else
            {
                SFtotalSwitch = false;
                SFswitchbutton.Text = "SF OFF";
                SFswitchbutton.BackColor = System.Drawing.Color.Red;
                SFcheckTrig._Trig = null;
                SFlinkTrig._Trig = null;
                SFcheck2Trig._Trig = null;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Form2 doc = new Form2();
            doc.StartPosition = FormStartPosition.CenterParent;
            doc.EventCloseWindow += new DCloseWindow(doc_EventCloseWindow);
            doc.ShowDialog();
            if (close == "yes")
            {
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void doc_EventCloseWindow(string pwd)
        {
            if (pwd == "yes")
            {
                close = "yes";
            }
        }

        #endregion 界面切换

        #region 存储过程所需文本 拼接

        //根据固定文本格式从flexui取得数据
        //params string[] values:values[0] = Dictionary<string, string>
        //values[] = "LinkData" ,Dictionary<string, string>=>String=value\String=value\...;$;
        //values[] = "TestResult" ,TestResult=PASS/FAIL;$;
        //Output:string  example:SN=123;$;LCD=123;$;Data=123\456\789;$;TestResult=PASS;$;

        public string GetVarFromFLEX(params string[] values)
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
                        Dictionary<string, string> InputstrPairs = new Dictionary<string, string>(GetInputStrVar(SFVar[values[x]]));
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
                        Dictionary<string, string> LinkDataPairs = new Dictionary<string, string>(GetInputStrVar(SFVar[values[x]]));

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
                        string testResult = SFVar["TestResult"];
                        if (CheckIfUncaseString(testResult, "PASS") || CheckIfUncaseString(testResult, "Fail"))
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
                        string testResult = SFVar["Check2Result"];
                        if (CheckIfUncaseString(testResult, "PASS") || CheckIfUncaseString(testResult, "Fail"))
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
        public Dictionary<string, string> GetInputStrVar(string inputstr)
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

        public void WriteSFLog(string SFtype, string trignum, string SFinput, string SFoutput, int retrytime)
        {
            string floder = folderPath + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
            string logstr = FileUtilHelper.GetLogString(SFtype, DateTime.Now.ToLongTimeString().ToString(), " Trig:" + trignum, SFinput + "SFreturn:" + SFoutput, "重试次数:" + retrytime + "\r\n");

            FileUtilHelper.AppendText(floder, logstr);
            BackgroundProcess(logstr);
        }

        public void WriteSFLog(string logname, string SFtype, string trignum, string SFinput, string SFoutput, int retrytime)
        {
            string floder = folderPath + DateTime.Now.ToString("yyyy-MM-dd") + "_" + logname + ".txt";
            string logstr = FileUtilHelper.GetLogString(SFtype, DateTime.Now.ToLongTimeString().ToString(), " Trig:" + trignum, SFinput + "SFreturn:" + SFoutput, "重试次数:" + retrytime + "\r\n");

            FileUtilHelper.AppendText(floder, logstr);
            BackgroundProcess(logstr);
        }

        public static string GetAppsettingStr(string str)
        {
            AppSettingsReader appReader = new AppSettingsReader();
            return appReader.GetValue(str, typeof(string)).ToString();
        }

        public bool CheckIfUncaseString(string str, string value)
        {
            return string.Equals(str, value, StringComparison.OrdinalIgnoreCase);
        }

        #endregion 存储过程所需文本 拼接
    }
}