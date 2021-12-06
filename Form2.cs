using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static SFDemo.Form1;

namespace SFDemo
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
        }

        public event DCloseWindow EventCloseWindow;

        private void buttonOk_Click(object sender, EventArgs e)
        {
            if (EventCloseWindow != null)
            {
                if (this.textBox1.Text.Trim() == "123")
                {
                    EventCloseWindow("yes");
                    this.Close();
                }
                else
                {
                    this.textBox1.Clear();
                    this.PassError.Text = "密码错误";
                }
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.textBox1.Clear();
            this.Close();
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            this.PassError.Text = "";
        }
    }
}
