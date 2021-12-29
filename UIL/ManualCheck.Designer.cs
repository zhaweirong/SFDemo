
namespace SFDemo.UIL
{
    partial class ManualCheck
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.TCText = new System.Windows.Forms.TextBox();
            this.BTText = new System.Windows.Forms.TextBox();
            this.ColorText = new System.Windows.Forms.TextBox();
            this.buttoncheck1 = new System.Windows.Forms.Button();
            this.buttoncheck2 = new System.Windows.Forms.Button();
            this.buttonlink1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.Linetext = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // TCText
            // 
            this.TCText.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.TCText.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TCText.Location = new System.Drawing.Point(153, 44);
            this.TCText.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.TCText.Name = "TCText";
            this.TCText.Size = new System.Drawing.Size(229, 30);
            this.TCText.TabIndex = 0;
            // 
            // BTText
            // 
            this.BTText.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.BTText.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BTText.Location = new System.Drawing.Point(153, 95);
            this.BTText.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.BTText.Name = "BTText";
            this.BTText.Size = new System.Drawing.Size(229, 30);
            this.BTText.TabIndex = 1;
            // 
            // ColorText
            // 
            this.ColorText.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ColorText.Location = new System.Drawing.Point(153, 201);
            this.ColorText.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ColorText.Name = "ColorText";
            this.ColorText.Size = new System.Drawing.Size(229, 30);
            this.ColorText.TabIndex = 2;
            // 
            // buttoncheck1
            // 
            this.buttoncheck1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttoncheck1.Location = new System.Drawing.Point(513, 30);
            this.buttoncheck1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.buttoncheck1.Name = "buttoncheck1";
            this.buttoncheck1.Size = new System.Drawing.Size(181, 59);
            this.buttoncheck1.TabIndex = 3;
            this.buttoncheck1.Text = "手动CHECK1";
            this.buttoncheck1.UseVisualStyleBackColor = true;
            this.buttoncheck1.Click += new System.EventHandler(this.buttoncheck1_Click);
            // 
            // buttoncheck2
            // 
            this.buttoncheck2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttoncheck2.Location = new System.Drawing.Point(513, 98);
            this.buttoncheck2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.buttoncheck2.Name = "buttoncheck2";
            this.buttoncheck2.Size = new System.Drawing.Size(181, 62);
            this.buttoncheck2.TabIndex = 4;
            this.buttoncheck2.Text = "手动CHECK2";
            this.buttoncheck2.UseVisualStyleBackColor = true;
            this.buttoncheck2.Click += new System.EventHandler(this.buttoncheck2_Click);
            // 
            // buttonlink1
            // 
            this.buttonlink1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonlink1.Location = new System.Drawing.Point(513, 174);
            this.buttonlink1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.buttonlink1.Name = "buttonlink1";
            this.buttonlink1.Size = new System.Drawing.Size(181, 57);
            this.buttonlink1.TabIndex = 5;
            this.buttonlink1.Text = "手动LINK";
            this.buttonlink1.UseVisualStyleBackColor = true;
            this.buttonlink1.Click += new System.EventHandler(this.buttonlink1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(21, 47);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 25);
            this.label1.TabIndex = 6;
            this.label1.Text = "手动过站TC";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(21, 98);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(118, 25);
            this.label2.TabIndex = 7;
            this.label2.Text = "手动过站BT";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(51, 201);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 25);
            this.label3.TabIndex = 8;
            this.label3.Text = "颜色";
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(4, 253);
            this.richTextBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(743, 164);
            this.richTextBox1.TabIndex = 9;
            this.richTextBox1.Text = "";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(5, 148);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(132, 25);
            this.label4.TabIndex = 10;
            this.label4.Text = "手动过站线别";
            // 
            // Linetext
            // 
            this.Linetext.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Linetext.Location = new System.Drawing.Point(153, 144);
            this.Linetext.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Linetext.Name = "Linetext";
            this.Linetext.Size = new System.Drawing.Size(229, 30);
            this.Linetext.TabIndex = 11;
            // 
            // ManualCheck
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(751, 422);
            this.Controls.Add(this.Linetext);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonlink1);
            this.Controls.Add(this.buttoncheck2);
            this.Controls.Add(this.buttoncheck1);
            this.Controls.Add(this.ColorText);
            this.Controls.Add(this.BTText);
            this.Controls.Add(this.TCText);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ManualCheck";
            this.ShowInTaskbar = false;
            this.Text = "ManualCheck";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TCText;
        private System.Windows.Forms.TextBox BTText;
        private System.Windows.Forms.TextBox ColorText;
        private System.Windows.Forms.Button buttoncheck1;
        private System.Windows.Forms.Button buttoncheck2;
        private System.Windows.Forms.Button buttonlink1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox Linetext;
    }
}