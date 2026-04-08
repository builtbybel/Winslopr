namespace Winslop.Views
{
    partial class PluginsView
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

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnPluginInstall = new System.Windows.Forms.Button();
            this.btnDescription = new System.Windows.Forms.Button();
            this.progressBarDownload = new System.Windows.Forms.ProgressBar();
            this.btnPluginEdit = new System.Windows.Forms.Button();
            this.btnPluginRemove = new System.Windows.Forms.Button();
            this.btnPluginSubmit = new System.Windows.Forms.Button();
            this.btnPluginUpdateAll = new System.Windows.Forms.Button();
            this.btnHelp = new System.Windows.Forms.Button();
            this.textSearch = new System.Windows.Forms.TextBox();
            this.linkPluginUsage = new System.Windows.Forms.LinkLabel();
            this.listPlugins = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // btnPluginInstall
            // 
            this.btnPluginInstall.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPluginInstall.Location = new System.Drawing.Point(500, 46);
            this.btnPluginInstall.Name = "btnPluginInstall";
            this.btnPluginInstall.Size = new System.Drawing.Size(109, 29);
            this.btnPluginInstall.TabIndex = 7;
            this.btnPluginInstall.Text = "Install";
            this.btnPluginInstall.UseVisualStyleBackColor = true;
            this.btnPluginInstall.Click += new System.EventHandler(this.btnPluginInstall_Click);
            // 
            // btnDescription
            // 
            this.btnDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDescription.AutoEllipsis = true;
            this.btnDescription.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(52)))), ((int)(((byte)(130)))));
            this.btnDescription.FlatAppearance.BorderColor = System.Drawing.Color.Gainsboro;
            this.btnDescription.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDescription.ForeColor = System.Drawing.Color.White;
            this.btnDescription.Location = new System.Drawing.Point(15, 10);
            this.btnDescription.Name = "btnDescription";
            this.btnDescription.Padding = new System.Windows.Forms.Padding(20, 0, 100, 0);
            this.btnDescription.Size = new System.Drawing.Size(594, 25);
            this.btnDescription.TabIndex = 6;
            this.btnDescription.TabStop = false;
            this.btnDescription.Text = "Slop Feeds (App restart needed after install)";
            this.btnDescription.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDescription.UseVisualStyleBackColor = false;
            // 
            // progressBarDownload
            // 
            this.progressBarDownload.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBarDownload.Location = new System.Drawing.Point(15, 380);
            this.progressBarDownload.Name = "progressBarDownload";
            this.progressBarDownload.Size = new System.Drawing.Size(470, 12);
            this.progressBarDownload.TabIndex = 9;
            this.progressBarDownload.Visible = false;
            // 
            // btnPluginEdit
            // 
            this.btnPluginEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPluginEdit.Location = new System.Drawing.Point(500, 151);
            this.btnPluginEdit.Name = "btnPluginEdit";
            this.btnPluginEdit.Size = new System.Drawing.Size(109, 29);
            this.btnPluginEdit.TabIndex = 10;
            this.btnPluginEdit.Text = "Edit";
            this.btnPluginEdit.UseVisualStyleBackColor = true;
            this.btnPluginEdit.Click += new System.EventHandler(this.btnPluginEdit_Click);
            // 
            // btnPluginRemove
            // 
            this.btnPluginRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPluginRemove.Location = new System.Drawing.Point(500, 116);
            this.btnPluginRemove.Name = "btnPluginRemove";
            this.btnPluginRemove.Size = new System.Drawing.Size(109, 29);
            this.btnPluginRemove.TabIndex = 11;
            this.btnPluginRemove.Text = "Remove";
            this.btnPluginRemove.UseVisualStyleBackColor = true;
            this.btnPluginRemove.Click += new System.EventHandler(this.btnPluginRemove_Click);
            // 
            // btnPluginSubmit
            // 
            this.btnPluginSubmit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPluginSubmit.Location = new System.Drawing.Point(500, 345);
            this.btnPluginSubmit.Name = "btnPluginSubmit";
            this.btnPluginSubmit.Size = new System.Drawing.Size(109, 29);
            this.btnPluginSubmit.TabIndex = 12;
            this.btnPluginSubmit.Text = "Submit Plugin";
            this.btnPluginSubmit.UseVisualStyleBackColor = true;
            this.btnPluginSubmit.Click += new System.EventHandler(this.btnPluginSubmit_Click);
            // 
            // btnPluginUpdateAll
            // 
            this.btnPluginUpdateAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPluginUpdateAll.Location = new System.Drawing.Point(500, 81);
            this.btnPluginUpdateAll.Name = "btnPluginUpdateAll";
            this.btnPluginUpdateAll.Size = new System.Drawing.Size(109, 29);
            this.btnPluginUpdateAll.TabIndex = 13;
            this.btnPluginUpdateAll.Text = "Update All";
            this.btnPluginUpdateAll.UseVisualStyleBackColor = true;
            this.btnPluginUpdateAll.Click += new System.EventHandler(this.btnPluginUpdateAll_Click);
            // 
            // btnHelp
            // 
            this.btnHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnHelp.Location = new System.Drawing.Point(500, 186);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(109, 29);
            this.btnHelp.TabIndex = 14;
            this.btnHelp.Text = "Help";
            this.btnHelp.UseVisualStyleBackColor = true;
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // textSearch
            // 
            this.textSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textSearch.Location = new System.Drawing.Point(500, 233);
            this.textSearch.Name = "textSearch";
            this.textSearch.Size = new System.Drawing.Size(109, 20);
            this.textSearch.TabIndex = 245;
            this.textSearch.Text = "Search";
            this.textSearch.Click += new System.EventHandler(this.textSearch_Click);
            this.textSearch.TextChanged += new System.EventHandler(this.textSearch_TextChanged);
            // 
            // linkPluginUsage
            // 
            this.linkPluginUsage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.linkPluginUsage.AutoSize = true;
            this.linkPluginUsage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(52)))), ((int)(((byte)(130)))));
            this.linkPluginUsage.LinkColor = System.Drawing.Color.White;
            this.linkPluginUsage.Location = new System.Drawing.Point(535, 16);
            this.linkPluginUsage.Name = "linkPluginUsage";
            this.linkPluginUsage.Size = new System.Drawing.Size(67, 13);
            this.linkPluginUsage.TabIndex = 246;
            this.linkPluginUsage.TabStop = true;
            this.linkPluginUsage.Text = "Usage notes";
            this.linkPluginUsage.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkPluginUsage_LinkClicked);
            // 
            // listPlugins
            // 
            this.listPlugins.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listPlugins.CheckBoxes = true;
            this.listPlugins.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.listPlugins.FullRowSelect = true;
            this.listPlugins.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listPlugins.HideSelection = false;
            this.listPlugins.Location = new System.Drawing.Point(15, 46);
            this.listPlugins.Name = "listPlugins";
            this.listPlugins.Size = new System.Drawing.Size(470, 328);
            this.listPlugins.TabIndex = 247;
            this.listPlugins.UseCompatibleStateImageBehavior = false;
            this.listPlugins.View = System.Windows.Forms.View.Details;
            this.listPlugins.SelectedIndexChanged += new System.EventHandler(this.listPlugins_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Plugin";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Installed";
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Type";
            this.columnHeader3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // PluginsView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(232)))), ((int)(((byte)(236)))));
            this.Controls.Add(this.listPlugins);
            this.Controls.Add(this.linkPluginUsage);
            this.Controls.Add(this.textSearch);
            this.Controls.Add(this.btnHelp);
            this.Controls.Add(this.btnPluginUpdateAll);
            this.Controls.Add(this.btnPluginSubmit);
            this.Controls.Add(this.btnPluginRemove);
            this.Controls.Add(this.btnPluginEdit);
            this.Controls.Add(this.progressBarDownload);
            this.Controls.Add(this.btnPluginInstall);
            this.Controls.Add(this.btnDescription);
            this.Name = "PluginsView";
            this.Size = new System.Drawing.Size(625, 395);
            this.Load += new System.EventHandler(this.PluginsView_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnPluginInstall;
        private System.Windows.Forms.Button btnDescription;
        private System.Windows.Forms.ProgressBar progressBarDownload;
        private System.Windows.Forms.Button btnPluginEdit;
        private System.Windows.Forms.Button btnPluginRemove;
        private System.Windows.Forms.Button btnPluginSubmit;
        private System.Windows.Forms.Button btnPluginUpdateAll;
        private System.Windows.Forms.Button btnHelp;
        private System.Windows.Forms.TextBox textSearch;
        private System.Windows.Forms.LinkLabel linkPluginUsage;
        private System.Windows.Forms.ListView listPlugins;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
    }
}
