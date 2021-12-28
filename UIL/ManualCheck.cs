using FMDMOLib;
using SFDemo.BLL;
using SFSATPortal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SFDemo.UIL
{
    public partial class ManualCheck : Form
    {
        private delegate void CrossThreadOperation();

        public ManualCheck()
        {
            InitializeComponent();
        }

        private void buttoncheck1_Click(object sender, EventArgs e)
        {
            string name = "Check1";
            string output = string.Empty;
            string InputStr = string.Empty;

            InputStr = "Line=" + GetLine() + ";$;Top Case=" + this.TCText.Text.Trim();
            if (FileUtilHelper.CheckIfUncaseString(GlobalConfig.SFWAY, "ODBC"))
            {
                output = ProcedureExecuter.ExecuteNonQuery(GlobalConfig.ProcedureName, GlobalConfig.BU[0], VarConfig.SFVar[name + "Station"], VarConfig.SFVar[name + "Step"], InputStr);
            }
            else if (FileUtilHelper.CheckIfUncaseString(GlobalConfig.SFWAY, "DLL"))
            {
                Portal portal = new Portal();
                output = portal.ATPortal(VarConfig.SFVar[name + "Station"], VarConfig.SFVar[name + "Step"], InputStr);
            }

            richTextBox1.Text = name + ":" + output;
        }

        private string GetLine()
        {
            string result = string.Empty;

            FMDMOLib.Rundb rundb = new Rundb();
            object dbn = rundb.Open();
            if (Convert.ToInt16(dbn) == 1)
            {
                result = rundb.GetVarValueEx("VT.LINE/K");
            }
            rundb.Close();
            rundb = null;
            return result;
        }

        private void buttoncheck2_Click(object sender, EventArgs e)
        {
            string name = "Check2";
            string output = string.Empty;
            string InputStr = string.Empty;
            InputStr = "Line=" + GetLine() + ";$;Top Case=" + this.TCText.Text.Trim() + ";$;Battery=" + this.BTText.Text.Trim() + ";$;Model=" + this.Linetext.Text.Trim() + "-" + this.ColorText.Text.Trim() + ";$;ModelName=Top Case";
            ;
            if (FileUtilHelper.CheckIfUncaseString(GlobalConfig.SFWAY, "ODBC"))
            {
                output = ProcedureExecuter.ExecuteNonQuery(GlobalConfig.ProcedureName, GlobalConfig.BU[1], VarConfig.SFVar[name + "Station"], VarConfig.SFVar[name + "Step"], InputStr);
            }
            else if (FileUtilHelper.CheckIfUncaseString(GlobalConfig.SFWAY, "DLL"))
            {
                Portal portal = new Portal();
                output = portal.ATPortal(VarConfig.SFVar[name + "Station"], VarConfig.SFVar[name + "Step"], InputStr);
            }

            richTextBox1.Text = name + ":" + output;
        }

        private void buttonlink1_Click(object sender, EventArgs e)
        {
            string name = "Link1";
            string output = string.Empty;
            string InputStr = string.Empty;
            InputStr = "Line=" + GetLine() + ";$;Top Case=" + this.TCText.Text.Trim() + ";$;Battery=" + this.BTText.Text.Trim() + ";$;Model=" + this.Linetext.Text.Trim() + "-" + this.ColorText.Text.Trim() + ";$;ModelName=Top Case;$;Result=Pass";

            if (FileUtilHelper.CheckIfUncaseString(GlobalConfig.SFWAY, "ODBC"))
            {
                output = ProcedureExecuter.ExecuteNonQuery(GlobalConfig.ProcedureName, GlobalConfig.BU[0], VarConfig.SFVar[name + "Station"], VarConfig.SFVar[name + "Step"], InputStr);
            }
            else if (FileUtilHelper.CheckIfUncaseString(GlobalConfig.SFWAY, "DLL"))
            {
                Portal portal = new Portal();
                output = portal.ATPortal(VarConfig.SFVar[name + "Station"], VarConfig.SFVar[name + "Step"], InputStr);
            }

            richTextBox1.Text = name + ":" + output;
        }
    }
}
