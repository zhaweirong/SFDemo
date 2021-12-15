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
        public void DHACheckReturn(string value, string v)
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

        public void DHALinkReturn()
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

        public void DHACheck2Return(string value, string v)
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

        public void ReturnMessageToFlexUI(string value)
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

        public void ReturnMessageToFlexUI(string value, string SFresult)
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
    }
}
