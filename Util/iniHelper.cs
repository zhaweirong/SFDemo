﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SFDemo.Util
{
    public class IniFile
    {
        /// <summary>
        /// Copies a string into the specified section of an initialization file.
        /// </summary>
        /// <returns>
        /// If the function successfully copies the string to the initialization file, the return value is nonzero.
        /// If the function fails, or if it flushes the cached version of the most recently accessed initialization file, the return value is zero.
        /// </returns>
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        /// <summary>
        /// Retrieves a string from the specified section in an initialization file.
        /// </summary>
        /// <returns>
        /// The return value is the number of characters copied to the buffer, not including the terminating null character.
        /// </returns>
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        /// <summary>
        /// Retrieves all the keys and values for the specified section of an initialization file.
        /// </summary>
        /// <returns>
        /// The return value is the number of characters copied to the buffer, not including the terminating null character.
        /// </returns>
        [DllImport("kernel32.dll")]
        private static extern int GetPrivateProfileSection(string lpAppName, byte[] lpszReturnBuffer, int nSize, string lpFileName);

        private const int SIZE = 255;
        private const string DATETIME_MASK = "MM/dd/yyyy hh:mm:ss tt";
        private const string DATE_MASK = "MM/dd/yyyy";

        public string FileName { get; private set; }

        public IniFile(string fileName)
        {
            this.SetFileName(fileName);
        }

        private void SetFileName(string fileName)
        {
            this.FileName = fileName;

            if (!Path.IsPathRooted(this.FileName))
            {
                string basePath = System.IO.Directory.GetCurrentDirectory();
                this.FileName = Path.Combine(basePath, this.FileName);
            }
        }

        private bool IsNumeric(string input)
        {
            return Regex.IsMatch(input, @"^\d+$");
        }

        public void DeleteKey(string section, string key)
        {
            WritePrivateProfileString(section, key, null, this.FileName);
        }

        public void DeleteSection(string section)
        {
            WritePrivateProfileString(section, null, null, this.FileName);
        }

        public string ReadString(string section, string key)
        {
            var temp = new StringBuilder(SIZE);
            GetPrivateProfileString(section, key, null, temp, SIZE, this.FileName);
            return temp.ToString();
        }

        public string ReadString(string section, string key, string notfound)
        {
            if (!KeyExists(section, key))
            {
                return notfound;
            }
            return ReadString(section, key);
        }

        public bool ReadBoolean(string section, string key)
        {
            string value = ReadString(section, key);
            bool rtn = (IsNumeric(value)) ? Convert.ToBoolean(Convert.ToInt32(value)) : value.ToUpper().Equals("TRUE");
            return rtn;
        }

        public bool ReadBoolean(string section, string key, bool notfound)
        {
            if (!KeyExists(section, key))
            {
                return notfound;
            }
            return ReadBoolean(section, key);
        }

        public decimal ReadDecimal(string section, string key)
        {
            if (!KeyExists(section, key))
            {
                return new decimal(-1);
            }
            string value = ReadString(section, key);
            return decimal.Parse(value.Trim(), System.Globalization.CultureInfo.InvariantCulture);
        }

        public decimal ReadDecimal(string section, string key, decimal notfound)
        {
            if (!KeyExists(section, key))
            {
                return notfound;
            }
            return ReadDecimal(section, key);
        }

        public double ReadDouble(string section, string key)
        {
            string value = ReadString(section, key);
            return double.Parse(value.Trim(), System.Globalization.CultureInfo.InvariantCulture);
        }

        public double ReadDouble(string section, string key, double notfound)
        {
            if (!KeyExists(section, key))
            {
                return notfound;
            }
            return ReadDouble(section, key);
        }

        public float ReadFloat(string section, string key)
        {
            string value = ReadString(section, key);
            return float.Parse(value, System.Globalization.CultureInfo.InvariantCulture);
        }

        public float ReadFloat(string section, string key, float notfound)
        {
            if (!KeyExists(section, key))
            {
                return notfound;
            }
            return ReadFloat(section, key);
        }

        public int ReadInteger(string section, string key)
        {
            string value = ReadString(section, key);
            return Convert.ToInt32(value.Trim());
        }

        public int ReadInteger(string section, string key, int notfound)
        {
            if (!KeyExists(section, key))
            {
                return notfound;
            }
            return ReadInteger(section, key);
        }

        public DateTime ReadDateTime(string section, string key)
        {
            string value = ReadString(section, key);
            return Convert.ToDateTime(value);
            //return DateTime.ParseExact(value, DATETIME_MASK,
            //                           System.Globalization.CultureInfo.InvariantCulture);
        }

        public DateTime ReadDateTime(string section, string key, DateTime notfound)
        {
            if (!KeyExists(section, key))
            {
                return notfound;
            }
            return ReadDateTime(section, key);
        }

        public DateTime ReadDate(string section, string key)
        {
            string value = ReadString(section, key);
            DateTime rtn = Convert.ToDateTime(value);
            return Convert.ToDateTime(rtn.ToString(DATE_MASK));
        }

        public DateTime ReadDate(string section, string key, DateTime notfound)
        {
            if (!KeyExists(section, key))
            {
                return notfound;
            }
            return ReadDate(section, key);
        }

        public bool WriteString(string section, string key, string value)
        {
            long l = WritePrivateProfileString(section, key, value, this.FileName);
            return l > 0;
        }

        public bool WriteBoolean(string section, string key, bool value)
        {
            string str = value.ToString().ToUpper();
            return WriteString(section, key, str);
        }

        public bool WriteDecimal(string section, string key, decimal value)
        {
            return WriteString(section, key, value.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }

        public bool WriteDouble(string section, string key, double value)
        {
            return WriteString(section, key, value.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }

        public bool WriteFloat(string section, string key, float value)
        {
            return WriteString(section, key, value.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }

        public bool WriteInteger(string section, string key, int value)
        {
            return WriteString(section, key, value.ToString());
        }

        public bool WriteDateTime(string section, string key, DateTime value)
        {
            return WriteString(section, key, value.ToString(DATETIME_MASK));
        }

        public bool WriteDate(string section, string key, DateTime value)
        {
            return WriteString(section, key, value.ToString(DATE_MASK));
        }

        public bool SectionExists(string section)
        {
            int i = GetPrivateProfileString(section, null, null, new StringBuilder(SIZE), SIZE, this.FileName);
            return i > 0;
        }

        public bool KeyExists(string section, string key)
        {
            int i = GetPrivateProfileString(section, key, null, new StringBuilder(SIZE), SIZE, this.FileName);
            return i > 0;
        }

        public IDictionary<string, string> ReadSection(string section)
        {
            var buffer = new byte[8192];
            GetPrivateProfileSection(section, buffer, 8192, this.FileName);
            var tmp = Encoding.ASCII.GetString(buffer).Trim('\0').Split('\0');
            var result = new Dictionary<string, string>();

            foreach (var entry in tmp)
            {
                var s = entry.Split(new string[] { "=" }, 2, StringSplitOptions.None);
                result.Add(s[0], s[1]);
            }
            return result;
        }
    }
}