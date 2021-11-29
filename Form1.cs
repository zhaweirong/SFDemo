using FMDMOLib;
using Ini.Net;
using Lin.LogHelper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.Odbc;
using System.Diagnostics;
using System.Windows.Forms;

namespace SFDemo
{
    public partial class Form1 : Form
    {
        private System.Threading.Timer timer = null;
        private System.Threading.Timer CheckProcesstimer = null;

        private Dictionary<string, string> SFVar;

        private bool SFcheckSwitch = true;
        private bool SFlinkSwitch = true;

        private TrigVar SFcheckTrig = new TrigVar();
        private TrigVar SFlinkTrig = new TrigVar();

        private bool StartPollingTrig = true;

        private string connectionString = ConfigurationManager.AppSettings["odbc"].ToString();

        public Form1()

        {
            InitializeComponent();
            SFPanel.Visible = true;
            HivePanel.Visible = false;
            currentPaneltext.Text = "SF";
            SFButton.BackColor = System.Drawing.Color.LightGray;
            HiveButton.BackColor = System.Drawing.Color.White;

            //ini获取
            var iniFile = new IniFile("var.ini");
            SFVar = (Dictionary<string, string>)iniFile.ReadSection("SF");

            SFcheckSwitch = !String.IsNullOrEmpty(SFVar["CheckTrig"]);
            SFlinkSwitch = !String.IsNullOrEmpty(SFVar["LinkTrig"]);

            FileUtilHelper.CreateDirectory(SFVar["Path"]);

            SFcheckTrig.PropertyChanged += SFcheck;
            SFlinkTrig.PropertyChanged += SFlink;

            //Check FlexUI OPEN
            CheckFlexUIProcess();
            //Get SFTrig from flexUI
            PollingTrig();
        }

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
                        if (SFcheckSwitch)
                        {
                            SFcheckTrig.Trig = Convert.ToString(rundb.GetVarValueEx(SFVar["CheckTrig"]));
                        }
                        if (SFlinkSwitch)
                        {
                            SFlinkTrig.Trig = Convert.ToString(rundb.GetVarValueEx(SFVar["LinkTrig"]));
                        }
                    }
                    rundb.Close();
                }
            }, "1", 1000, 1000);
        }

        private void SFlink(object sender, PropertyChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(SFcheckTrig.Trig)) { return; }

            string output = string.Empty;
            string InputStr = string.Empty;
            string[] values;
            try
            {
                if (SFVar["TestResult"] == "PASS")
                {
                    values = GetVarFromFLEX("LinkSN", "linetype", "Machinenum", "LinkData");
                    InputStr = FileUtilHelper.TransferToInputStr(new string[] { "SN", "line", "Machinenum", "data" }, values) + "TestResult=PASS";
                }
                else
                {
                    values = GetVarFromFLEX("LinkSN", "linetype", "Machinenum", "LinkData", "TestResult");
                    InputStr = FileUtilHelper.TransferToInputStr(new string[] { "SN", "line", "Machinenum", "data", "TestResult" }, values);
                }
                using (OdbcConnection conn = new OdbcConnection(connectionString))

                {
                    OdbcCommand cmd = new OdbcCommand("{  call MonitorPortaltest (?,?,?,?,?)}", conn);

                    conn.Open();

                    OdbcParameter parameter1 = new OdbcParameter("@BU", OdbcType.Char);
                    parameter1.Direction = ParameterDirection.Input;
                    parameter1.Value = "NB6";
                    cmd.Parameters.Add(parameter1);

                    OdbcParameter parameter2 = new OdbcParameter("@Station", OdbcType.Char);
                    parameter2.Direction = ParameterDirection.Input;
                    parameter2.Value = "RHM";
                    cmd.Parameters.Add(parameter2);

                    OdbcParameter parameter3 = new OdbcParameter("@Step", OdbcType.Char);
                    parameter3.Direction = ParameterDirection.Input;
                    parameter3.Value = "Require";
                    cmd.Parameters.Add(parameter3);

                    OdbcParameter parameter4 = new OdbcParameter("@InputStr", OdbcType.Char);
                    parameter4.Direction = ParameterDirection.Input;
                    parameter4.Value = InputStr;
                    cmd.Parameters.Add(parameter4);

                    OdbcParameter parameter5 = new OdbcParameter("@Output", OdbcType.VarChar, 256);
                    parameter5.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(parameter5);

                    cmd.ExecuteNonQuery();
                    output = (string)parameter5.Value;
                    ReturnMessageToFlexUI(output);

                    FileUtilHelper.AppendText(SFVar["Path"] + DateTime.Now.ToString("yyyy-MM-dd") + "_" + SFVar["Machine"] + ".txt", "Link:" + DateTime.Now.ToLongTimeString().ToString() + " " + InputStr + " " + output + "\r\n");
                    BackgroundProcess(FileUtilHelper.formatOutputStr(DateTime.Now.ToLongTimeString().ToString(), InputStr, "SFReturn:", output));
                }
            }
            catch (Exception ex)
            {
                ex.ToString().LogForError();
            }
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

        private void SFcheck(object sender, PropertyChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(SFcheckTrig.Trig)) { return; }

            string output = string.Empty;
            try
            {
                string[] values = GetVarFromFLEX("CheckSN", "linetype");
                string InputStr = FileUtilHelper.TransferToInputStr(new string[] { "SN", "line" }, values);

                using (OdbcConnection conn = new OdbcConnection(connectionString))

                {
                    OdbcCommand cmd = new OdbcCommand("{ call MonitorPortaltest (?,?,?,?,?)}", conn);

                    conn.Open();

                    OdbcParameter parameter1 = new OdbcParameter("@BU", OdbcType.Char);
                    parameter1.Direction = ParameterDirection.Input;
                    parameter1.Value = "NB6";
                    cmd.Parameters.Add(parameter1);

                    OdbcParameter parameter2 = new OdbcParameter("@Station", OdbcType.Char);
                    parameter2.Direction = ParameterDirection.Input;
                    parameter2.Value = "RHM";
                    cmd.Parameters.Add(parameter2);

                    OdbcParameter parameter3 = new OdbcParameter("@Step", OdbcType.Char);
                    parameter3.Direction = ParameterDirection.Input;
                    parameter3.Value = "Require";
                    cmd.Parameters.Add(parameter3);

                    OdbcParameter parameter4 = new OdbcParameter("@InputStr", OdbcType.Char);
                    parameter4.Direction = ParameterDirection.Input;
                    parameter4.Value = InputStr;
                    cmd.Parameters.Add(parameter4);

                    OdbcParameter parameter5 = new OdbcParameter("@Output", OdbcType.VarChar, 256);
                    parameter5.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(parameter5);

                    cmd.ExecuteNonQuery();
                    output = (string)parameter5.Value;
                    ReturnMessageToFlexUI(output);
                    FileUtilHelper.AppendText(SFVar["Path"] + DateTime.Now.ToString("yyyy-MM-dd") + "_" + SFVar["Machine"] + ".txt", "Check:" + DateTime.Now.ToLongTimeString().ToString() + " " + InputStr + " " + output + "\r\n");
                    BackgroundProcess(FileUtilHelper.formatOutputStr(DateTime.Now.ToLongTimeString().ToString(), InputStr, "SFReturn:" + output));
                }
            }
            catch (Exception ex)
            {
                ex.ToString().LogForError();
            }
        }

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

        //界面切换
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

        public string[] GetVarFromFLEX(params string[] values)
        {
            ArrayList result = new ArrayList();

            FMDMOLib.Rundb rundb = new Rundb();
            object dbn = rundb.Open();
            if (Convert.ToInt16(dbn) == 1)
            {
                for (int x = 0; x < values.Length; x++)
                {
                    if (values[x] == "LinkData")
                    {
                        if (SFVar[values[x]] == "0")
                        {
                            result.Add("0");
                        }
                        else
                        {
                            string[] sArray = SFVar[values[x]].Split('/');
                            string data = string.Empty;
                            foreach (string i in sArray)
                            {
                                if (string.IsNullOrEmpty(data))
                                {
                                    data = Convert.ToString(rundb.GetVarValueEx(i));
                                }
                                else
                                {
                                    data = data + "\\" + Convert.ToString(rundb.GetVarValueEx(i));
                                }
                            }
                            result.Add(data);
                        }
                    }
                    else if (values[x] == "TestResult")
                    {
                        if (Convert.ToString(rundb.GetVarValueEx(SFVar[values[x]])) != "0")
                        {
                            result.Add("PASS");
                        }
                        else
                        {
                            result.Add("FAIL");
                        }
                    }
                    else
                    {
                        result.Add(Convert.ToString(rundb.GetVarValueEx(SFVar[values[x]])));
                    }
                }
            }
            rundb.Close();
            rundb = null;
            return (string[])result.ToArray(typeof(string));
        }
    }
}
