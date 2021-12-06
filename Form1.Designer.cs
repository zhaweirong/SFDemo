
namespace SFDemo
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.SFPanel = new System.Windows.Forms.Panel();
            this.SFlistBox = new System.Windows.Forms.ListBox();
            this.HivePanel = new System.Windows.Forms.Panel();
            this.HivelistBox = new System.Windows.Forms.ListBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.HiveButton = new System.Windows.Forms.Button();
            this.SFButton = new System.Windows.Forms.Button();
            this.HeadPanel = new System.Windows.Forms.Panel();
            this.SFswitchbutton = new System.Windows.Forms.Button();
            this.FlexUIStatus = new System.Windows.Forms.Label();
            this.currentPaneltext = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.SFPanel.SuspendLayout();
            this.HivePanel.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.HeadPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // SFPanel
            // 
            this.SFPanel.Controls.Add(this.SFlistBox);
            this.SFPanel.Location = new System.Drawing.Point(115, 48);
            this.SFPanel.Name = "SFPanel";
            this.SFPanel.Size = new System.Drawing.Size(686, 402);
            this.SFPanel.TabIndex = 2;
            // 
            // SFlistBox
            // 
            this.SFlistBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SFlistBox.FormattingEnabled = true;
            this.SFlistBox.HorizontalScrollbar = true;
            this.SFlistBox.ItemHeight = 17;
            this.SFlistBox.Location = new System.Drawing.Point(6, 4);
            this.SFlistBox.Name = "SFlistBox";
            this.SFlistBox.Size = new System.Drawing.Size(677, 395);
            this.SFlistBox.TabIndex = 2;
            // 
            // HivePanel
            // 
            this.HivePanel.Controls.Add(this.HivelistBox);
            this.HivePanel.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HivePanel.Location = new System.Drawing.Point(115, 48);
            this.HivePanel.Name = "HivePanel";
            this.HivePanel.Size = new System.Drawing.Size(686, 402);
            this.HivePanel.TabIndex = 3;
            // 
            // HivelistBox
            // 
            this.HivelistBox.BackColor = System.Drawing.SystemColors.Window;
            this.HivelistBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HivelistBox.FormattingEnabled = true;
            this.HivelistBox.ItemHeight = 17;
            this.HivelistBox.Location = new System.Drawing.Point(6, 4);
            this.HivelistBox.Name = "HivelistBox";
            this.HivelistBox.Size = new System.Drawing.Size(677, 395);
            this.HivelistBox.TabIndex = 2;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.White;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.HiveButton, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.SFButton, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(10, 48);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(105, 401);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // HiveButton
            // 
            this.HiveButton.BackColor = System.Drawing.Color.White;
            this.HiveButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HiveButton.Location = new System.Drawing.Point(0, 89);
            this.HiveButton.Margin = new System.Windows.Forms.Padding(0);
            this.HiveButton.Name = "HiveButton";
            this.HiveButton.Size = new System.Drawing.Size(105, 89);
            this.HiveButton.TabIndex = 1;
            this.HiveButton.Text = "Hive";
            this.HiveButton.UseVisualStyleBackColor = false;
            this.HiveButton.Visible = false;
            this.HiveButton.Click += new System.EventHandler(this.HiveButton_Click);
            // 
            // SFButton
            // 
            this.SFButton.BackColor = System.Drawing.Color.White;
            this.SFButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SFButton.Location = new System.Drawing.Point(0, 0);
            this.SFButton.Margin = new System.Windows.Forms.Padding(0);
            this.SFButton.Name = "SFButton";
            this.SFButton.Size = new System.Drawing.Size(105, 89);
            this.SFButton.TabIndex = 0;
            this.SFButton.Text = "SF";
            this.SFButton.UseVisualStyleBackColor = false;
            this.SFButton.Click += new System.EventHandler(this.SFButton_Click);
            // 
            // HeadPanel
            // 
            this.HeadPanel.BackColor = System.Drawing.Color.White;
            this.HeadPanel.Controls.Add(this.SFswitchbutton);
            this.HeadPanel.Controls.Add(this.FlexUIStatus);
            this.HeadPanel.Controls.Add(this.currentPaneltext);
            this.HeadPanel.Controls.Add(this.pictureBox1);
            this.HeadPanel.Location = new System.Drawing.Point(10, 5);
            this.HeadPanel.Name = "HeadPanel";
            this.HeadPanel.Size = new System.Drawing.Size(790, 40);
            this.HeadPanel.TabIndex = 4;
            // 
            // SFswitchbutton
            // 
            this.SFswitchbutton.AutoSize = true;
            this.SFswitchbutton.BackColor = System.Drawing.Color.LimeGreen;
            this.SFswitchbutton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.SFswitchbutton.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F);
            this.SFswitchbutton.Location = new System.Drawing.Point(606, 2);
            this.SFswitchbutton.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.SFswitchbutton.Name = "SFswitchbutton";
            this.SFswitchbutton.Size = new System.Drawing.Size(92, 38);
            this.SFswitchbutton.TabIndex = 5;
            this.SFswitchbutton.Text = "SF ON ";
            this.SFswitchbutton.UseVisualStyleBackColor = false;
            this.SFswitchbutton.Click += new System.EventHandler(this.SFswitchbutton_Click);
            // 
            // FlexUIStatus
            // 
            this.FlexUIStatus.BackColor = System.Drawing.Color.LimeGreen;
            this.FlexUIStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FlexUIStatus.Location = new System.Drawing.Point(704, 4);
            this.FlexUIStatus.Name = "FlexUIStatus";
            this.FlexUIStatus.Size = new System.Drawing.Size(83, 33);
            this.FlexUIStatus.TabIndex = 6;
            this.FlexUIStatus.Text = "FlexUI";
            this.FlexUIStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // currentPaneltext
            // 
            this.currentPaneltext.AutoSize = true;
            this.currentPaneltext.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.currentPaneltext.Location = new System.Drawing.Point(120, 7);
            this.currentPaneltext.Name = "currentPaneltext";
            this.currentPaneltext.Size = new System.Drawing.Size(39, 25);
            this.currentPaneltext.TabIndex = 4;
            this.currentPaneltext.Text = "SF";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pictureBox1.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(5);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(102, 40);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.HeadPanel);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.SFPanel);
            this.Controls.Add(this.HivePanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Form1";
            this.Text = "ABB_SF";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.SFPanel.ResumeLayout(false);
            this.HivePanel.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.HeadPanel.ResumeLayout(false);
            this.HeadPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel SFPanel;
        private System.Windows.Forms.ListBox SFlistBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel HeadPanel;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button HiveButton;
        private System.Windows.Forms.Button SFButton;
        private System.Windows.Forms.Panel HivePanel;
        private System.Windows.Forms.ListBox HivelistBox;
        private System.Windows.Forms.Label currentPaneltext;
        private System.Windows.Forms.Label FlexUIStatus;
        private System.Windows.Forms.Button SFswitchbutton;
    }
}

