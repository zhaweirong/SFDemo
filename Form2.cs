using System;
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

        private void button1_Click(object sender, EventArgs e)
        {
            this.textBox1.Text = this.textBox1.Text + sender.ToString().Substring(sender.ToString().Length - 1, 1);
        }
    }
}
