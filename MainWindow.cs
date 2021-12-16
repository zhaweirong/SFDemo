using FMDMOLib;
using Ini.Net;
using Lin.LogHelper;
using SFDemo.BLL;
using SFSATPortal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
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
                                SFCheck1Trig.Trig = Convert.ToString(rundb.GetVarValueEx(VarConfig.SFVar["Check2Trig"]));
                            }
                            if (VarConfig.checkVarExist("Link1Trig"))
                            {
                                SFCheck1Trig.Trig = Convert.ToString(rundb.GetVarValueEx(VarConfig.SFVar["Link1Trig"]));
                            }
                            if (VarConfig.checkVarExist("Link2Trig"))
                            {
                                SFCheck1Trig.Trig = Convert.ToString(rundb.GetVarValueEx(VarConfig.SFVar["Link2Trig"]));
                            }
                        }
                    }
                    rundb.Close();
                }
            }, "1", 1000, GlobalConfig.trigPollingtime);
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
                Portal portal = new Portal();
                InputStr = ReadWriteToFlexUI.GetVarFromFLEX(name + "InputStr");

                Retry.RetryHandle(GlobalConfig.RetryCount, TimeSpan.FromSeconds(GlobalConfig.RetryInterval), false, delegate
                {
                    output = portal.ATPortal(VarConfig.SFVar[name + "Station"], VarConfig.SFVar[name + "Step"], InputStr);
                    if ((output.IndexOf("PASS", StringComparison.OrdinalIgnoreCase) <= 0))
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
                Portal portal = new Portal();
                InputStr = ReadWriteToFlexUI.GetVarFromFLEX(name + "InputStr");

                Retry.RetryHandle(GlobalConfig.RetryCount, TimeSpan.FromSeconds(GlobalConfig.RetryInterval), false, delegate
                {
                    output = portal.ATPortal(VarConfig.SFVar[name + "Station"], VarConfig.SFVar[name + "Step"], InputStr);
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
                Portal portal = new Portal();
                InputStr = ReadWriteToFlexUI.GetVarFromFLEX(name + "InputStr", name + "Data", name + "TestResult");

                Retry.RetryHandle(GlobalConfig.RetryCount, TimeSpan.FromSeconds(GlobalConfig.RetryInterval), false, delegate
                {
                    output = portal.ATPortal(VarConfig.SFVar[name + "Station"], VarConfig.SFVar[name + "Step"], InputStr);
                    if (output.IndexOf("PASS", StringComparison.OrdinalIgnoreCase) <= 0 && output.IndexOf("FAIL", StringComparison.OrdinalIgnoreCase) <= 0)
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
                Portal portal = new Portal();
                InputStr = ReadWriteToFlexUI.GetVarFromFLEX(name + "InputStr", name + "Data", name + "TestResult");

                Retry.RetryHandle(GlobalConfig.RetryCount, TimeSpan.FromSeconds(GlobalConfig.RetryInterval), false, delegate
                {
                    output = portal.ATPortal(VarConfig.SFVar[name + "Station"], VarConfig.SFVar[name + "Step"], InputStr);
                    if (output.IndexOf("PASS", StringComparison.OrdinalIgnoreCase) <= 0 && output.IndexOf("FAIL", StringComparison.OrdinalIgnoreCase) <= 0)
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
            currentPaneltext.Text = "SF";
            SFButton.BackColor = System.Drawing.Color.LightGray;
            HiveButton.BackColor = System.Drawing.Color.White;
            SettingButton.BackColor = System.Drawing.Color.White;
        }

        private void SFButton_Click(object sender, EventArgs e)
        {
            SFPanel.Visible = true;
            HivePanel.Visible = false;
            SettingPanel.Visible = false;
            currentPaneltext.Text = "SF";
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

        #region 存储过程所需文本 拼接

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

        #endregion 存储过程所需文本 拼接

        private void ReCheck1_Click(object sender, EventArgs e)
        {
            string InputStr = ReadWriteToFlexUI.GetVarHandleRecheck("Check1InputStr");
            Portal portal = new Portal();
            string output = portal.ATPortal(VarConfig.SFVar["Check1Station"], VarConfig.SFVar["Check1Step"], InputStr);
            BackgroundProcess(output);
        }

        private void ReCheck2_Click(object sender, EventArgs e)
        {
            string InputStr = ReadWriteToFlexUI.GetVarHandleRecheck("Check2InputStr");
            Portal portal = new Portal();
            string output = portal.ATPortal(VarConfig.SFVar["Check2Station"], VarConfig.SFVar["Check2Step"], InputStr);
            BackgroundProcess(output);
        }
    }
}