namespace Winslop
{
    partial class MainForm
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panelContainer = new System.Windows.Forms.Panel();
            this.panelContent = new System.Windows.Forms.Panel();
            this.rtbLogger = new System.Windows.Forms.RichTextBox();
            this.comboLogActions = new System.Windows.Forms.ComboBox();
            this.btnAnalyze = new System.Windows.Forms.Button();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.Windows = new System.Windows.Forms.TabPage();
            this.Apps = new System.Windows.Forms.TabPage();
            this.Install = new System.Windows.Forms.TabPage();
            this.Extensions = new System.Windows.Forms.TabPage();
            this.btnFix = new System.Windows.Forms.Button();
            this.btnMenu = new System.Windows.Forms.Button();
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuToggle = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuRestore = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuPlugins = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuUpdate = new System.Windows.Forms.ToolStripMenuItem();
            this.textSearch = new System.Windows.Forms.TextBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.lblRightHeader = new System.Windows.Forms.Label();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnSupport = new System.Windows.Forms.Button();
            this.panelContainer.SuspendLayout();
            this.panelContent.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.contextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelContainer
            // 
            this.panelContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelContainer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(134)))), ((int)(((byte)(110)))));
            this.panelContainer.Controls.Add(this.panelContent);
            this.panelContainer.Location = new System.Drawing.Point(0, 40);
            this.panelContainer.Name = "panelContainer";
            this.panelContainer.Size = new System.Drawing.Size(432, 485);
            this.panelContainer.TabIndex = 198;
            // 
            // panelContent
            // 
            this.panelContent.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelContent.AutoScroll = true;
            this.panelContent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(229)))));
            this.panelContent.Controls.Add(this.rtbLogger);
            this.panelContent.Controls.Add(this.comboLogActions);
            this.panelContent.Controls.Add(this.btnAnalyze);
            this.panelContent.Controls.Add(this.tabControl);
            this.panelContent.Controls.Add(this.btnFix);
            this.panelContent.Location = new System.Drawing.Point(8, 10);
            this.panelContent.Name = "panelContent";
            this.panelContent.Size = new System.Drawing.Size(416, 465);
            this.panelContent.TabIndex = 205;
            // 
            // rtbLogger
            // 
            this.rtbLogger.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbLogger.BackColor = System.Drawing.Color.White;
            this.rtbLogger.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbLogger.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbLogger.Location = new System.Drawing.Point(7, 251);
            this.rtbLogger.Name = "rtbLogger";
            this.rtbLogger.ReadOnly = true;
            this.rtbLogger.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.rtbLogger.ShowSelectionMargin = true;
            this.rtbLogger.Size = new System.Drawing.Size(400, 142);
            this.rtbLogger.TabIndex = 195;
            this.rtbLogger.TabStop = false;
            this.rtbLogger.Text = "";
            this.toolTip.SetToolTip(this.rtbLogger, "Inspection output will appear here.");
            // 
            // comboLogActions
            // 
            this.comboLogActions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboLogActions.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.comboLogActions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboLogActions.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.comboLogActions.Font = new System.Drawing.Font("Segoe UI Variable Small Semilig", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboLogActions.FormattingEnabled = true;
            this.comboLogActions.Location = new System.Drawing.Point(7, 399);
            this.comboLogActions.Name = "comboLogActions";
            this.comboLogActions.Size = new System.Drawing.Size(400, 24);
            this.comboLogActions.TabIndex = 210;
            this.comboLogActions.TabStop = false;
            // 
            // btnAnalyze
            // 
            this.btnAnalyze.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAnalyze.AutoEllipsis = true;
            this.btnAnalyze.BackColor = System.Drawing.Color.Transparent;
            this.btnAnalyze.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(207)))), ((int)(((byte)(208)))));
            this.btnAnalyze.Font = new System.Drawing.Font("Segoe UI Variable Display", 9.25F);
            this.btnAnalyze.Location = new System.Drawing.Point(7, 429);
            this.btnAnalyze.Name = "btnAnalyze";
            this.btnAnalyze.Size = new System.Drawing.Size(164, 29);
            this.btnAnalyze.TabIndex = 10;
            this.btnAnalyze.TabStop = false;
            this.btnAnalyze.Text = "&Inspect system";
            this.btnAnalyze.UseVisualStyleBackColor = false;
            this.btnAnalyze.Click += new System.EventHandler(this.btnAnalyze_Click);
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Appearance = System.Windows.Forms.TabAppearance.Buttons;
            this.tabControl.Controls.Add(this.Windows);
            this.tabControl.Controls.Add(this.Apps);
            this.tabControl.Controls.Add(this.Install);
            this.tabControl.Controls.Add(this.Extensions);
            this.tabControl.Cursor = System.Windows.Forms.Cursors.Hand;
            this.tabControl.Font = new System.Drawing.Font("Segoe UI Variable Display", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl.HotTrack = true;
            this.tabControl.ItemSize = new System.Drawing.Size(30, 28);
            this.tabControl.Location = new System.Drawing.Point(7, 8);
            this.tabControl.Name = "tabControl";
            this.tabControl.Padding = new System.Drawing.Point(15, 3);
            this.tabControl.SelectedIndex = 0;
            this.tabControl.ShowToolTips = true;
            this.tabControl.Size = new System.Drawing.Size(400, 242);
            this.tabControl.TabIndex = 199;
            this.tabControl.TabStop = false;
            // 
            // Windows
            // 
            this.Windows.AutoScroll = true;
            this.Windows.BackColor = System.Drawing.SystemColors.Control;
            this.Windows.Cursor = System.Windows.Forms.Cursors.Default;
            this.Windows.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Windows.Location = new System.Drawing.Point(4, 32);
            this.Windows.Name = "Windows";
            this.Windows.Size = new System.Drawing.Size(392, 206);
            this.Windows.TabIndex = 0;
            this.Windows.Text = "Windows";
            // 
            // Apps
            // 
            this.Apps.BackColor = System.Drawing.SystemColors.Control;
            this.Apps.Cursor = System.Windows.Forms.Cursors.Default;
            this.Apps.Location = new System.Drawing.Point(4, 32);
            this.Apps.Name = "Apps";
            this.Apps.Size = new System.Drawing.Size(392, 206);
            this.Apps.TabIndex = 1;
            this.Apps.Text = "Apps";
            this.Apps.UseVisualStyleBackColor = true;
            // 
            // Install
            // 
            this.Install.Location = new System.Drawing.Point(4, 32);
            this.Install.Name = "Install";
            this.Install.Size = new System.Drawing.Size(392, 206);
            this.Install.TabIndex = 3;
            this.Install.Text = "Install";
            this.Install.UseVisualStyleBackColor = true;
            // 
            // Extensions
            // 
            this.Extensions.Cursor = System.Windows.Forms.Cursors.Default;
            this.Extensions.Location = new System.Drawing.Point(4, 32);
            this.Extensions.Name = "Extensions";
            this.Extensions.Size = new System.Drawing.Size(392, 206);
            this.Extensions.TabIndex = 2;
            this.Extensions.Text = "Tools";
            // 
            // btnFix
            // 
            this.btnFix.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFix.AutoEllipsis = true;
            this.btnFix.BackColor = System.Drawing.Color.Transparent;
            this.btnFix.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(207)))), ((int)(((byte)(208)))));
            this.btnFix.Font = new System.Drawing.Font("Segoe UI Variable Display", 9.25F);
            this.btnFix.Location = new System.Drawing.Point(243, 429);
            this.btnFix.Name = "btnFix";
            this.btnFix.Size = new System.Drawing.Size(164, 29);
            this.btnFix.TabIndex = 2;
            this.btnFix.TabStop = false;
            this.btnFix.Text = "&Apply selected changes";
            this.btnFix.UseVisualStyleBackColor = false;
            this.btnFix.Click += new System.EventHandler(this.btnFix_Click);
            // 
            // btnMenu
            // 
            this.btnMenu.AutoEllipsis = true;
            this.btnMenu.BackColor = System.Drawing.Color.Transparent;
            this.btnMenu.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnMenu.FlatAppearance.BorderSize = 0;
            this.btnMenu.Font = new System.Drawing.Font("Segoe MDL2 Assets", 11F, System.Drawing.FontStyle.Bold);
            this.btnMenu.ForeColor = System.Drawing.Color.Black;
            this.btnMenu.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnMenu.Location = new System.Drawing.Point(4, 10);
            this.btnMenu.Name = "btnMenu";
            this.btnMenu.Size = new System.Drawing.Size(38, 23);
            this.btnMenu.TabIndex = 1;
            this.btnMenu.TabStop = false;
            this.btnMenu.Text = "...";
            this.toolTip.SetToolTip(this.btnMenu, "Main Menu");
            this.btnMenu.UseVisualStyleBackColor = false;
            this.btnMenu.Click += new System.EventHandler(this.btnMenu_Click);
            // 
            // contextMenu
            // 
            this.contextMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuToggle,
            this.toolStripSeparator1,
            this.toolStripMenuRestore,
            this.toolStripMenuPlugins,
            this.toolStripSeparator3,
            this.toolStripMenuHelp,
            this.toolStripMenuAbout,
            this.toolStripMenuUpdate});
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.contextMenu.Size = new System.Drawing.Size(180, 148);
            // 
            // toolStripMenuToggle
            // 
            this.toolStripMenuToggle.Name = "toolStripMenuToggle";
            this.toolStripMenuToggle.Size = new System.Drawing.Size(179, 22);
            this.toolStripMenuToggle.Text = "Toggle all";
            this.toolStripMenuToggle.Click += new System.EventHandler(this.toolStripMenuSelection_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(176, 6);
            // 
            // toolStripMenuRestore
            // 
            this.toolStripMenuRestore.Name = "toolStripMenuRestore";
            this.toolStripMenuRestore.Size = new System.Drawing.Size(179, 22);
            this.toolStripMenuRestore.Text = "Undo last changes";
            this.toolStripMenuRestore.Click += new System.EventHandler(this.toolStripMenuRestore_Click);
            // 
            // toolStripMenuPlugins
            // 
            this.toolStripMenuPlugins.Name = "toolStripMenuPlugins";
            this.toolStripMenuPlugins.Size = new System.Drawing.Size(179, 22);
            this.toolStripMenuPlugins.Tag = "Tools";
            this.toolStripMenuPlugins.Text = "Manage plugins...";
            this.toolStripMenuPlugins.Click += new System.EventHandler(this.toolStripMenuPlugins_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(176, 6);
            // 
            // toolStripMenuHelp
            // 
            this.toolStripMenuHelp.Name = "toolStripMenuHelp";
            this.toolStripMenuHelp.Size = new System.Drawing.Size(179, 22);
            this.toolStripMenuHelp.Tag = "Help";
            this.toolStripMenuHelp.Text = "Help";
            // 
            // toolStripMenuAbout
            // 
            this.toolStripMenuAbout.Name = "toolStripMenuAbout";
            this.toolStripMenuAbout.Size = new System.Drawing.Size(179, 22);
            this.toolStripMenuAbout.Text = "About";
            // 
            // toolStripMenuUpdate
            // 
            this.toolStripMenuUpdate.Name = "toolStripMenuUpdate";
            this.toolStripMenuUpdate.Size = new System.Drawing.Size(179, 22);
            this.toolStripMenuUpdate.Text = "Check for updates...";
            // 
            // textSearch
            // 
            this.textSearch.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.textSearch.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.75F);
            this.textSearch.ForeColor = System.Drawing.Color.DimGray;
            this.textSearch.Location = new System.Drawing.Point(116, 12);
            this.textSearch.Multiline = true;
            this.textSearch.Name = "textSearch";
            this.textSearch.Size = new System.Drawing.Size(209, 20);
            this.textSearch.TabIndex = 520;
            this.textSearch.TabStop = false;
            this.textSearch.Text = "Search";
            this.textSearch.Click += new System.EventHandler(this.textSearch_Click);
            this.textSearch.TextChanged += new System.EventHandler(this.textSearch_TextChanged);
            // 
            // lblRightHeader
            // 
            this.lblRightHeader.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblRightHeader.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblRightHeader.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblRightHeader.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRightHeader.Location = new System.Drawing.Point(325, 526);
            this.lblRightHeader.Name = "lblRightHeader";
            this.lblRightHeader.Padding = new System.Windows.Forms.Padding(0, 0, 10, 0);
            this.lblRightHeader.Size = new System.Drawing.Size(75, 23);
            this.lblRightHeader.TabIndex = 521;
            this.lblRightHeader.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip.SetToolTip(this.lblRightHeader, "Versioning information");
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.BackColor = System.Drawing.Color.Transparent;
            this.btnRefresh.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRefresh.FlatAppearance.BorderSize = 0;
            this.btnRefresh.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRefresh.ForeColor = System.Drawing.Color.Black;
            this.btnRefresh.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRefresh.Location = new System.Drawing.Point(367, 10);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(61, 23);
            this.btnRefresh.TabIndex = 522;
            this.btnRefresh.TabStop = false;
            this.btnRefresh.Text = "&Refresh";
            this.toolTip.SetToolTip(this.btnRefresh, "Refresh");
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnSupport
            // 
            this.btnSupport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSupport.AutoEllipsis = true;
            this.btnSupport.BackColor = System.Drawing.Color.Transparent;
            this.btnSupport.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSupport.FlatAppearance.BorderSize = 0;
            this.btnSupport.FlatAppearance.MouseDownBackColor = System.Drawing.Color.WhiteSmoke;
            this.btnSupport.FlatAppearance.MouseOverBackColor = System.Drawing.Color.WhiteSmoke;
            this.btnSupport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSupport.Font = new System.Drawing.Font("Segoe MDL2 Assets", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSupport.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(65)))), ((int)(((byte)(211)))));
            this.btnSupport.Location = new System.Drawing.Point(284, 526);
            this.btnSupport.Name = "btnSupport";
            this.btnSupport.Size = new System.Drawing.Size(35, 23);
            this.btnSupport.TabIndex = 523;
            this.btnSupport.TabStop = false;
            this.toolTip.SetToolTip(this.btnSupport, "Support the development");
            this.btnSupport.UseVisualStyleBackColor = false;
            this.btnSupport.Click += new System.EventHandler(this.btnSupport_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(229)))));
            this.ClientSize = new System.Drawing.Size(432, 546);
            this.Controls.Add(this.btnSupport);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.textSearch);
            this.Controls.Add(this.btnMenu);
            this.Controls.Add(this.panelContainer);
            this.Controls.Add(this.lblRightHeader);
            this.ForeColor = System.Drawing.Color.Black;
            this.MinimumSize = new System.Drawing.Size(400, 500);
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Winslop.exe";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.panelContainer.ResumeLayout(false);
            this.panelContent.ResumeLayout(false);
            this.tabControl.ResumeLayout(false);
            this.contextMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel panelContainer;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage Windows;
        private System.Windows.Forms.TabPage Apps;
        private System.Windows.Forms.Button btnFix;
        private System.Windows.Forms.Button btnAnalyze;
        private System.Windows.Forms.Panel panelContent;
        private System.Windows.Forms.Button btnMenu;
        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuRestore;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuPlugins;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuHelp;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuToggle;
        public System.Windows.Forms.RichTextBox rtbLogger;
        private System.Windows.Forms.ComboBox comboLogActions;
        private System.Windows.Forms.TabPage Extensions;
        private System.Windows.Forms.TextBox textSearch;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Label lblRightHeader;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuAbout;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuUpdate;
        private System.Windows.Forms.TabPage Install;
        private System.Windows.Forms.Button btnSupport;
    }
}

