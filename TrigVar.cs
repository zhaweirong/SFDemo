using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SFDemo
{
    internal class TrigVar : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void PropChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        //定义属性
        public string _Trig;

        /// <summary>
        /// 触发变量值变更时，当变量值非空且为整数且与比之前变量大时触发事件
        /// </summary>
        public string Trig
        {
            get { return _Trig; }
            set
            {
                if (int.TryParse(value, out int result) && (this._Trig != value))
                {
                    if (string.IsNullOrEmpty(this._Trig) || string.IsNullOrEmpty(value))
                    {
                        this._Trig = value;
                    }
                    else if (int.Parse(value) > int.Parse(this._Trig))
                    {
                        PropChanged(value);
                        this._Trig = value;
                    }
                    else
                    {
                        this._Trig = value;
                    }
                }
            }
        }
    }
}
