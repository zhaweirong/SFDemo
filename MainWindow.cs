using FMDMOLib;
using Ini.Net;
using Lin.LogHelper;
using SFDemo.BLL;
using SFSATPortal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;

namespace SFDemo
{
    public partial class MainWindow : Form
    {
        private System.Threading.Timer PollingFromFlextimer = null;
        private System.Threading.Timer CheckProcesstimer = null;

        private TrigVar SFCheck1Trig = new TrigVar();
        private TrigVar SFCheck2Trig = new TrigVar();
        private TrigVar SFLink1Trig = new TrigVar();
        private TrigVar SFLink2Trig = new TrigVar();

        //NEWDHA HARD CODE FOR LINK

        private TrigVar SFLink85Trig = new TrigVar();
        private TrigVar SFLink95Trig = new TrigVar();
        private TrigVar SFLink0Trig = new TrigVar();
        private TrigVar SFLink100Trig = new TrigVar();
        private TrigVar SFLink200Trig = new TrigVar();

        private bool StartPollingTrig = true;

        public delegate void DCloseWindow(string pwd);

        private string close = "";
        private bool SFtotalSwitch = true;

        public MainWindow()

        {
            InitializeComponent();
            InitalPanel();

            //ini获取
            var iniFile = new IniFile("var.ini");
            VarConfig.SFVar = (Dictionary<string, string>)iniFile.ReadSection("SF");

            FileUtilHelper.CreateDirectory(GlobalConfig.folderPath);

            SFCheck1Trig.PropertyChanged += SFCheck1;
            SFCheck2Trig.PropertyChanged += SFCheck2;
            SFLink1Trig.PropertyChanged += SFLink1;
            SFLink2Trig.PropertyChanged += SFLink2;

            if (VarConfig.SFVar["Machine"].Equals("DHA"))
            {
                SFLink85Trig.PropertyChanged += HardCodeLink85;
                SFLink95Trig.PropertyChanged += HardCodeLink95;
                SFLink0Trig.PropertyChanged += HardCodeLink0;
                SFLink100Trig.PropertyChanged += HardCodeLink100;
                SFLink200Trig.PropertyChanged += HardCodeLink200;
            }

            //Check FlexUI OPEN
            CheckFlexUIProcess();
            //Get SFTrig from flexUI
            PollingTrig();
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
            PollingFromFlextimer = new System.Threading.Timer((n) =>
            {
                if (StartPollingTrig)
                {
                    FMDMOLib.Rundb rundb = new Rundb();
                    object dbn = rundb.Open();
                    if (Convert.ToInt16(dbn) == 1)
                    {
                        if (SFtotalSwitch)
                        {
                            if (VarConfig.checkVarExist("Check1Trig"))
                            {
                                SFCheck1Trig.Trig = Convert.ToString(rundb.GetVarValueEx(VarConfig.SFVar["Check1Trig"]));
                            }
                            if (VarConfig.checkVarExist("Check2Trig"))
                            {
                                SFCheck2Trig.Trig = Convert.ToString(rundb.GetVarValueEx(VarConfig.SFVar["Check2Trig"]));
                            }
                            if (VarConfig.checkVarExist("Link1Trig"))
                            {
                                SFLink1Trig.Trig = Convert.ToString(rundb.GetVarValueEx(VarConfig.SFVar["Link1Trig"]));
                            }
                            if (VarConfig.checkVarExist("Link2Trig"))
                            {
                                SFLink2Trig.Trig = Convert.ToString(rundb.GetVarValueEx(VarConfig.SFVar["Link2Trig"]));
                            }

                            //DHA HARD CODE
                            if (VarConfig.SFVar["Machine"].Equals("DHA"))
                            {
                                SFLink85Trig.Trig = Convert.ToString(rundb.GetVarValueEx("AI.LINK_85S_TRIG"));
                                SFLink95Trig.Trig = Convert.ToString(rundb.GetVarValueEx("AI.LINK_95S_TRIG"));
                                SFLink0Trig.Trig = Convert.ToString(rundb.GetVarValueEx("AI.LINK_P0_TRIG"));
                                SFLink100Trig.Trig = Convert.ToString(rundb.GetVarValueEx("AI.LINK_P100_TRIG"));
                                SFLink200Trig.Trig = Convert.ToString(rundb.GetVarValueEx("AI.LINK_P200_TRIG"));
                            }
                        }
                    }
                    rundb.Close();
                }
            }, "1", 1000, GlobalConfig.TrigPollingTime);
        }

        #endregion 数据定时获取

        #region CHECK1/CHECK2/LINK1/LINK2

        private void SFCheck1(object sender, PropertyChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(SFCheck1Trig.Trig)) { return; }

            string name = "Check1";
            string output = string.Empty;
            string InputStr = string.Empty;
            int retrytime = 0;
            string retryReason = string.Empty;
            try
            {
                InputStr = ReadWriteToFlexUI.GetVarFromFLEX(name + "InputStr");

                Retry.RetryHandle(GlobalConfig.RetryCount, TimeSpan.FromSeconds(GlobalConfig.RetryInterval), false, delegate
                {
                    if (FileUtilHelper.CheckIfUncaseString(GlobalConfig.SFWAY, "ODBC"))
                    {
                        output = ProcedureExecuter.ExecuteNonQuery(GlobalConfig.ProcedureName, GlobalConfig.BU[0], VarConfig.SFVar[name + "Station"], VarConfig.SFVar[name + "Step"], InputStr);
                    }
                    else if (FileUtilHelper.CheckIfUncaseString(GlobalConfig.SFWAY, "DLL"))
                    {
                        Portal portal = new Portal();
                        output = portal.ATPortal(VarConfig.SFVar[name + "Station"], VarConfig.SFVar[name + "Step"], InputStr);
                    }
                    else
                    {
                        //HTTP
                    }

                    if (GlobalConfig.RetryCount != 0 && (output.IndexOf("PASS", StringComparison.OrdinalIgnoreCase) <= 0))
                    {
                        retrytime++;
                        retryReason = retryReason + "次数" + retrytime + ":" + output + ";";

                        throw new Exception("SFCheck1ReturnErrorMsg");
                    }
                });

                if (retrytime > 0)
                {
                    ("check1Retry:" + retryReason).LogForDebug();
                }

                ReadWriteToFlexUI.SFReturnToFLEX(output, name);

                if (ReadWriteToFlexUI.CheckContainUncaseString(output, "PASS"))
                {
                    SoundHelper.PlaySound("Check1OK");
                }
                else
                {
                    SoundHelper.PlaySound("Check1NG");
                }

                WriteSFLog(GlobalConfig.CheckLogName[0], e.PropertyName, InputStr, output, retrytime);
            }
            catch (Exception ex)
            {
                ex.ToString().LogForError();
                BackgroundProcess(ex.ToString());
            }
        }

        private void SFCheck2(object sender, PropertyChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(SFCheck2Trig.Trig)) { return; }

            string name = "Check2";
            string output = string.Empty;
            string InputStr = string.Empty;
            int retrytime = 0;
            string retryReason = string.Empty;
            try
            {
                InputStr = ReadWriteToFlexUI.GetVarFromFLEX(name + "InputStr");

                Retry.RetryHandle(GlobalConfig.RetryCount, TimeSpan.FromSeconds(GlobalConfig.RetryInterval), false, delegate
                {
                    if (FileUtilHelper.CheckIfUncaseString(GlobalConfig.SFWAY, "ODBC"))
                    {
                        output = ProcedureExecuter.ExecuteNonQuery(GlobalConfig.ProcedureName, GlobalConfig.BU[1], VarConfig.SFVar[name + "Station"], VarConfig.SFVar[name + "Step"], InputStr);
                    }
                    else if (FileUtilHelper.CheckIfUncaseString(GlobalConfig.SFWAY, "DLL"))
                    {
                        Portal portal = new Portal();
                        output = portal.ATPortal(VarConfig.SFVar[name + "Station"], VarConfig.SFVar[name + "Step"], InputStr);
                    }
                    else
                    {
                        //HTTP
                    }
                    if (GlobalConfig.RetryCount != 0 && (output.IndexOf("PASS", StringComparison.OrdinalIgnoreCase) <= 0))
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

                ReadWriteToFlexUI.SFReturnToFLEX(output, name);

                if (ReadWriteToFlexUI.CheckContainUncaseString(output, "PASS"))
                {
                    SoundHelper.PlaySound("Check2OK");
                }
                else
                {
                    SoundHelper.PlaySound("Check2NG");
                }
                WriteSFLog(GlobalConfig.CheckLogName[1], e.PropertyName, InputStr, output, retrytime);
            }
            catch (Exception ex)
            {
                ex.ToString().LogForError();
                BackgroundProcess(ex.ToString());
            }
        }

        private void SFLink1(object sender, PropertyChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(SFLink1Trig.Trig)) { return; }

            string name = "Link1";
            string output = string.Empty;
            string InputStr = string.Empty;
            int retrytime = 0;
            string retryReason = string.Empty;
            try
            {
                InputStr = ReadWriteToFlexUI.GetVarFromFLEX(name + "InputStr", name + "Data", name + "TestResult");

                Retry.RetryHandle(GlobalConfig.RetryCount, TimeSpan.FromSeconds(GlobalConfig.RetryInterval), false, delegate
                {
                    if (FileUtilHelper.CheckIfUncaseString(GlobalConfig.SFWAY, "ODBC"))
                    {
                        output = ProcedureExecuter.ExecuteNonQuery(GlobalConfig.ProcedureName, GlobalConfig.BU[2], VarConfig.SFVar[name + "Station"], VarConfig.SFVar[name + "Step"], InputStr);
                    }
                    else if (FileUtilHelper.CheckIfUncaseString(GlobalConfig.SFWAY, "DLL"))
                    {
                        Portal portal = new Portal();
                        output = portal.ATPortal(VarConfig.SFVar[name + "Station"], VarConfig.SFVar[name + "Step"], InputStr);
                    }
                    else
                    {
                        //HTTP
                    }
                    if (GlobalConfig.RetryCount != 0 && output.IndexOf("PASS", StringComparison.OrdinalIgnoreCase) <= 0 && output.IndexOf("FAIL", StringComparison.OrdinalIgnoreCase) <= 0)
                    {
                        retrytime++;
                        retryReason = retryReason + "次数" + retrytime + ":" + output + ";";
                        throw new Exception("SFLink1ReturnErrorMsg");
                    }
                });

                if (retrytime > 0)
                {
                    ("link1Retry:" + retryReason).LogForDebug();
                }

                ReadWriteToFlexUI.SFReturnToFLEX(output, name);

                if (ReadWriteToFlexUI.CheckContainUncaseString(output, "PASS"))
                {
                    SoundHelper.PlaySound("Link1OK");
                }
                else
                {
                    SoundHelper.PlaySound("Link1NG");
                }

                WriteSFLog(GlobalConfig.LinkLogName[0], e.PropertyName, InputStr, output, retrytime);
            }
            catch (Exception ex)
            {
                ex.ToString().LogForError();
                BackgroundProcess(ex.ToString());
            }
        }

        private void SFLink2(object sender, PropertyChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(SFLink2Trig.Trig)) { return; }

            string name = "Link2";
            string output = string.Empty;
            string InputStr = string.Empty;
            int retrytime = 0;
            string retryReason = string.Empty;
            try
            {
                InputStr = ReadWriteToFlexUI.GetVarFromFLEX(name + "InputStr", name + "Data", name + "TestResult");

                Retry.RetryHandle(GlobalConfig.RetryCount, TimeSpan.FromSeconds(GlobalConfig.RetryInterval), false, delegate
                {
                    if (FileUtilHelper.CheckIfUncaseString(GlobalConfig.SFWAY, "ODBC"))
                    {
                        output = ProcedureExecuter.ExecuteNonQuery(GlobalConfig.ProcedureName, GlobalConfig.BU[3], VarConfig.SFVar[name + "Station"], VarConfig.SFVar[name + "Step"], InputStr);
                    }
                    else if (FileUtilHelper.CheckIfUncaseString(GlobalConfig.SFWAY, "DLL"))
                    {
                        Portal portal = new Portal();
                        output = portal.ATPortal(VarConfig.SFVar[name + "Station"], VarConfig.SFVar[name + "Step"], InputStr);
                    }
                    else
                    {
                        //HTTP
                    }
                    if (GlobalConfig.RetryCount != 0 && output.IndexOf("PASS", StringComparison.OrdinalIgnoreCase) <= 0 && output.IndexOf("FAIL", StringComparison.OrdinalIgnoreCase) <= 0)
                    {
                        retrytime++;
                        retryReason = retryReason + "次数" + retrytime + ":" + output + ";";
                        throw new Exception("SFLink2ReturnErrorMsg");
                    }
                });

                if (retrytime > 0)
                {
                    ("link2Retry:" + retryReason).LogForDebug();
                }

                ReadWriteToFlexUI.SFReturnToFLEX(output, name);

                if (ReadWriteToFlexUI.CheckContainUncaseString(output, "PASS"))
                {
                    SoundHelper.PlaySound("Link2OK");
                }
                else
                {
                    SoundHelper.PlaySound("Link2NG");
                }

                WriteSFLog(GlobalConfig.LinkLogName[1], e.PropertyName, InputStr, output, retrytime);
            }
            catch (Exception ex)
            {
                ex.ToString().LogForError();
                BackgroundProcess(ex.ToString());
            }
        }

        private void HardCodeLink85(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(SFLink85Trig.Trig)) { return; }
                string output = string.Empty;
                string InputStr = ReadWriteToFlexUI.GetHardCodeLinkInput("VT.LINK_P0_UNIT_SN", "VT.LINK_85S_TFT_SN");
                if (FileUtilHelper.CheckIfUncaseString(GlobalConfig.SFWAY, "ODBC"))
                {
                    output = ProcedureExecuter.ExecuteNonQuery(GlobalConfig.ProcedureName, GlobalConfig.BU[0], "CHK85", "Require", InputStr);
                }
                else if (FileUtilHelper.CheckIfUncaseString(GlobalConfig.SFWAY, "DLL"))
                {
                    Portal portal = new Portal();
                    output = portal.ATPortal("CHK85", "Require", InputStr);
                }
                else
                {
                    //HTTP
                }

                WriteSFLog("85S", "Link85s", e.PropertyName, InputStr, output, 0);
            }
            catch
            {
            }
        }

        private void HardCodeLink95(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(SFLink95Trig.Trig)) { return; }
                string output = string.Empty;
                string InputStr = ReadWriteToFlexUI.GetHardCodeLinkInput("VT.LINK_P0_UNIT_SN", "VT.LINK_95S_TFT_SN");
                if (FileUtilHelper.CheckIfUncaseString(GlobalConfig.SFWAY, "ODBC"))
                {
                    output = ProcedureExecuter.ExecuteNonQuery(GlobalConfig.ProcedureName, GlobalConfig.BU[0], "CHK95", "Require", InputStr);
                }
                else if (FileUtilHelper.CheckIfUncaseString(GlobalConfig.SFWAY, "DLL"))
                {
                    Portal portal = new Portal();
                    output = portal.ATPortal("CHK95", "Require", InputStr);
                }
                else
                {
                    //HTTP
                }

                WriteSFLog("95S", "Link95s", e.PropertyName, InputStr, output, 0);
            }
            catch
            {
            }
        }

        private void HardCodeLink0(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(SFLink0Trig.Trig)) { return; }
                string output = string.Empty;
                string InputStr = ReadWriteToFlexUI.GetHardCodeLinkInput("VT.LINK_P0_UNIT_SN", "VT.LINK_P0_TFT_SN");
                if (FileUtilHelper.CheckIfUncaseString(GlobalConfig.SFWAY, "ODBC"))
                {
                    output = ProcedureExecuter.ExecuteNonQuery(GlobalConfig.ProcedureName, GlobalConfig.BU[0], "P0", "Require", InputStr);
                }
                else if (FileUtilHelper.CheckIfUncaseString(GlobalConfig.SFWAY, "DLL"))
                {
                    Portal portal = new Portal();
                    output = portal.ATPortal("P0", "Require", InputStr);
                }
                else
                {
                    //HTTP
                }

                WriteSFLog("P0", "LinkP0", e.PropertyName, InputStr, output, 0);
            }
            catch
            {
            }
        }

        private void HardCodeLink100(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(SFLink100Trig.Trig)) { return; }
                string output = string.Empty;
                string InputStr = ReadWriteToFlexUI.GetHardCodeLinkInput("VT.LINK_P0_UNIT_SN", "VT.LINK_P0_TFT_SN");
                if (FileUtilHelper.CheckIfUncaseString(GlobalConfig.SFWAY, "ODBC"))
                {
                    output = ProcedureExecuter.ExecuteNonQuery(GlobalConfig.ProcedureName, GlobalConfig.BU[0], "P100", "Require", InputStr);
                }
                else if (FileUtilHelper.CheckIfUncaseString(GlobalConfig.SFWAY, "DLL"))
                {
                    Portal portal = new Portal();
                    output = portal.ATPortal("P100", "Require", InputStr);
                }
                else
                {
                    //HTTP
                }

                WriteSFLog("P100", "LinkP100", e.PropertyName, InputStr, output, 0);
            }
            catch
            {
            }
        }

        private void HardCodeLink200(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(SFLink200Trig.Trig)) { return; }
                string output = string.Empty;
                string InputStr = ReadWriteToFlexUI.GetHardCodeLinkInput("VT.LINK_P0_UNIT_SN", "VT.LINK_P0_TFT_SN");
                if (FileUtilHelper.CheckIfUncaseString(GlobalConfig.SFWAY, "ODBC"))
                {
                    output = ProcedureExecuter.ExecuteNonQuery(GlobalConfig.ProcedureName, GlobalConfig.BU[0], "P200", "Require", InputStr);
                }
                else if (FileUtilHelper.CheckIfUncaseString(GlobalConfig.SFWAY, "DLL"))
                {
                    Portal portal = new Portal();
                    output = portal.ATPortal("P200", "Require", InputStr);
                }
                else
                {
                    //HTTP
                }

                WriteSFLog("P200", "LinkP200", e.PropertyName, InputStr, output, 0);
            }
            catch
            {
            }
        }

        #endregion CHECK1/CHECK2/LINK1/LINK2

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

        private void InitalPanel()
        {
            SFPanel.Visible = true;
            HivePanel.Visible = false;
            SettingPanel.Visible = false;
            currentPaneltext.Text = "SF_" + GlobalConfig.SFWAY;
            SFButton.BackColor = System.Drawing.Color.LightGray;
            HiveButton.BackColor = System.Drawing.Color.White;
            SettingButton.BackColor = System.Drawing.Color.White;
            this.ShowInTaskbar = false;
        }

        private void SFButton_Click(object sender, EventArgs e)
        {
            SFPanel.Visible = true;
            HivePanel.Visible = false;
            SettingPanel.Visible = false;
            currentPaneltext.Text = "SF_" + GlobalConfig.SFWAY;
            SFButton.BackColor = System.Drawing.Color.LightGray;
            HiveButton.BackColor = System.Drawing.Color.White;
            SettingButton.BackColor = System.Drawing.Color.White;
        }

        private void HiveButton_Click(object sender, EventArgs e)
        {
            HivePanel.Visible = true;
            SFPanel.Visible = false;
            SettingPanel.Visible = false;
            currentPaneltext.Text = "Hive";
            HiveButton.BackColor = System.Drawing.Color.LightGray;
            SFButton.BackColor = System.Drawing.Color.White;
            SettingButton.BackColor = System.Drawing.Color.White;
        }

        private void SettingButton_Click(object sender, EventArgs e)
        {
            HivePanel.Visible = false;
            SFPanel.Visible = false;
            SettingPanel.Visible = true;
            currentPaneltext.Text = "Setting";
            SettingButton.BackColor = System.Drawing.Color.LightGray;
            HiveButton.BackColor = System.Drawing.Color.White;
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
                SFCheck1Trig._Trig = null;
                SFLink1Trig._Trig = null;
                SFCheck2Trig._Trig = null;
                SFLink2Trig._Trig = null;
            }
        }

        private void MainWindow_Closing(object sender, FormClosingEventArgs e)
        {
            PassWindow doc = new PassWindow();
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

        #region 写SFlog

        public void WriteSFLog(string SFtype, string trignum, string SFinput, string SFoutput, int retrytime)
        {
            string floder = GlobalConfig.folderPath + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
            string logstr = FileUtilHelper.GetLogString(SFtype, DateTime.Now.ToLongTimeString().ToString(), " Trig:" + trignum, SFinput + "SFreturn:" + SFoutput, "重试次数:" + retrytime + "\r\n");

            FileUtilHelper.AppendText(floder, logstr);
            BackgroundProcess(logstr);
        }

        public void WriteSFLog(string logname, string SFtype, string trignum, string SFinput, string SFoutput, int retrytime)
        {
            string floder = GlobalConfig.folderPath + DateTime.Now.ToString("yyyy-MM-dd") + "_" + logname + ".txt";
            string logstr = FileUtilHelper.GetLogString(SFtype, DateTime.Now.ToLongTimeString().ToString(), " Trig:" + trignum, SFinput + "SFreturn:" + SFoutput, "重试次数:" + retrytime + "\r\n");

            FileUtilHelper.AppendText(floder, logstr);
            BackgroundProcess(logstr);
        }

        #endregion 写SFlog

        #region RECHECK

        private void ReCheck1_Click(object sender, EventArgs e)
        {
            string InputStr = ReadWriteToFlexUI.GetVarHandleRecheck("Check1InputStr");
            string output = string.Empty;
            if (FileUtilHelper.CheckIfUncaseString(GlobalConfig.SFWAY, "ODBC"))
            {
                output = ProcedureExecuter.ExecuteNonQuery(GlobalConfig.ProcedureName, GlobalConfig.BU[0], VarConfig.SFVar["Check1Station"], VarConfig.SFVar["Check1Step"], InputStr);
            }
            else if (FileUtilHelper.CheckIfUncaseString(GlobalConfig.SFWAY, "DLL"))
            {
                Portal portal = new Portal();
                output = portal.ATPortal(VarConfig.SFVar["Check1Station"], VarConfig.SFVar["Check1Step"], InputStr);
            }
            else
            {
                //HTTP
            }

            BackgroundProcess(output);
        }

        private void ReCheck2_Click(object sender, EventArgs e)
        {
            string InputStr = ReadWriteToFlexUI.GetVarHandleRecheck("Check2InputStr");
            string output = string.Empty;
            if (FileUtilHelper.CheckIfUncaseString(GlobalConfig.SFWAY, "ODBC"))
            {
                output = ProcedureExecuter.ExecuteNonQuery(GlobalConfig.ProcedureName, GlobalConfig.BU[1], VarConfig.SFVar["Check2Station"], VarConfig.SFVar["Check2Step"], InputStr);
            }
            else if (FileUtilHelper.CheckIfUncaseString(GlobalConfig.SFWAY, "DLL"))
            {
                Portal portal = new Portal();
                output = portal.ATPortal(VarConfig.SFVar["Check2Station"], VarConfig.SFVar["Check2Step"], InputStr);
            }
            else
            {
                //HTTP
            }
            BackgroundProcess(output);
        }

        #endregion RECHECK

        private void showTaskbar_Click(object sender, EventArgs e)
        {
            //点击时判断form是否显示,显示就隐藏,隐藏就显示
            if (this.WindowState == FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Minimized;
                this.Hide();
            }
            else if (this.WindowState == FormWindowState.Minimized)
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
                this.Activate();
            }
        }
    }
}