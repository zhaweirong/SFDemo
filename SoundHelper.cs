using System;
using System.Configuration;
using System.Media;

namespace SFDemo
{
    internal class SoundHelper
    {
        public static void PlaySound(string soundname)
        {
            try
            {
                new SoundPlayer(ConfigurationManager.AppSettings[soundname].ToString()).Play();
            }
            catch (Exception ex)
            {
                //ex.ToString().LogForError();
            }
        }
    }
}
