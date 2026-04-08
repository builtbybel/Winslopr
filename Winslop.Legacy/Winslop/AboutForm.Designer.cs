namespace Winslop
{
    partial class AboutForm
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
            this.btnDonate = new System.Windows.Forms.Button();
            this.lblHeader = new System.Windows.Forms.Label();
            this.lblVersionInfo = new System.Windows.Forms.Label();
            this.panelForm = new System.Windows.Forms.Panel();
            this.checkDonated = new System.Windows.Forms.CheckBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.linkCopyright = new System.Windows.Forms.LinkLabel();
            this.btnClose = new System.Windows.Forms.Button();
            this.panelForm.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnDonate
            // 
            this.btnDonate.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnDonate.AutoEllipsis = true;
            this.btnDonate.BackColor = System.Drawing.Color.Transparent;
            this.btnDonate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDonate.FlatAppearance.BorderSize = 0;
            this.btnDonate.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnDonate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.btnDonate.ForeColor = System.Drawing.Color.Black;
            this.btnDonate.Location = new System.Drawing.Point(188, 111);
            this.btnDonate.Name = "btnDonate";
            this.btnDonate.Size = new System.Drawing.Size(50, 24);
            this.btnDonate.TabIndex = 1;
            this.btnDonate.Text = "Donate";
            this.btnDonate.UseVisualStyleBackColor = false;
            this.btnDonate.Click += new System.EventHandler(this.btnDonate_Click);
            // 
            // lblHeader
            // 
            this.lblHeader.AutoEllipsis = true;
            this.lblHeader.AutoSize = true;
            this.lblHeader.BackColor = System.Drawing.Color.Transparent;
            this.lblHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeader.ForeColor = System.Drawing.Color.Black;
            this.lblHeader.Location = new System.Drawing.Point(185, 8);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(45, 13);
            this.lblHeader.TabIndex = 235;
            this.lblHeader.Text = "Winslop";
            this.lblHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblVersionInfo
            // 
            this.lblVersionInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblVersionInfo.AutoEllipsis = true;
            this.lblVersionInfo.BackColor = System.Drawing.Color.Transparent;
            this.lblVersionInfo.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVersionInfo.ForeColor = System.Drawing.Color.Black;
            this.lblVersionInfo.Location = new System.Drawing.Point(306, 9);
            this.lblVersionInfo.Name = "lblVersionInfo";
            this.lblVersionInfo.Size = new System.Drawing.Size(83, 13);
            this.lblVersionInfo.TabIndex = 238;
            this.lblVersionInfo.Text = "v";
            this.lblVersionInfo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // panelForm
            // 
            this.panelForm.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(232)))), ((int)(((byte)(236)))));
            this.panelForm.Controls.Add(this.checkDonated);
            this.panelForm.Controls.Add(this.pictureBox1);
            this.panelForm.Controls.Add(this.linkCopyright);
            this.panelForm.Controls.Add(this.btnClose);
            this.panelForm.Controls.Add(this.btnDonate);
            this.panelForm.Controls.Add(this.lblHeader);
            this.panelForm.Controls.Add(this.lblVersionInfo);
            this.panelForm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelForm.Font = new System.Drawing.Font("Segoe UI Variable Small Semilig", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelForm.ForeColor = System.Drawing.Color.White;
            this.panelForm.Location = new System.Drawing.Point(0, 0);
            this.panelForm.Name = "panelForm";
            this.panelForm.Size = new System.Drawing.Size(392, 143);
            this.panelForm.TabIndex = 243;
            // 
            // checkDonated
            // 
            this.checkDonated.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkDonated.AutoEllipsis = true;
            this.checkDonated.Cursor = System.Windows.Forms.Cursors.Hand;
            this.checkDonated.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkDonated.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.checkDonated.ForeColor = System.Drawing.Color.Black;
            this.checkDonated.Location = new System.Drawing.Point(244, 111);
            this.checkDonated.Name = "checkDonated";
            this.checkDonated.Size = new System.Drawing.Size(130, 24);
            this.checkDonated.TabIndex = 240;
            this.checkDonated.Text = "I have already donated";
            this.checkDonated.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkDonated.UseVisualStyleBackColor = true;
            this.checkDonated.CheckedChanged += new System.EventHandler(this.checkDonated_CheckedChanged);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::Winslop.Properties.Resources.AppLogo;
            this.pictureBox1.Location = new System.Drawing.Point(188, 31);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(36, 40);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 239;
            this.pictureBox1.TabStop = false;
            // 
            // linkCopyright
            // 
            this.linkCopyright.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.linkCopyright.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkCopyright.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.linkCopyright.LinkColor = System.Drawing.Color.Black;
            this.linkCopyright.Location = new System.Drawing.Point(12, 85);
            this.linkCopyright.Name = "linkCopyright";
            this.linkCopyright.Size = new System.Drawing.Size(362, 13);
            this.linkCopyright.TabIndex = 100;
            this.linkCopyright.TabStop = true;
            this.linkCopyright.Text = "© 2026 ·  A Belim App creation · Open Source: View on GitHub";
            this.linkCopyright.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.linkCopyright.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkCopyright_LinkClicked);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnClose.BackColor = System.Drawing.Color.Silver;
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.ForeColor = System.Drawing.Color.Black;
            this.btnClose.Location = new System.Drawing.Point(132, 111);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(50, 24);
            this.btnClose.TabIndex = 1;
            this.btnClose.TabStop = false;
            this.btnClose.Text = "OK";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // AboutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(392, 143);
            this.Controls.Add(this.panelForm);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Winslop";
            this.panelForm.ResumeLayout(false);
            this.panelForm.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnDonate;
        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.Label lblVersionInfo;
        private System.Windows.Forms.Panel panelForm;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.LinkLabel linkCopyright;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.CheckBox checkDonated;
    }
}