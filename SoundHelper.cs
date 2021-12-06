using Lin.LogHelper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace SFDemo
{
    internal class SoundHelper
    {
        public static void PlaySound(string soundname)
        {
            //如果目录不存在，则抛出异常
            try
            {
                new SoundPlayer(ConfigurationManager.AppSettings[soundname].ToString()).Play();
            }
            catch (Exception ex)
            {
                ex.ToString().LogForError();
            }
        }
    }
}
