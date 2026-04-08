namespace Winslop.Views
{
    partial class InstallView
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
            this.comboDisplayMode = new System.Windows.Forms.ComboBox();
            this.checkedListBoxApps = new System.Windows.Forms.CheckedListBox();
            this.btnInstall = new System.Windows.Forms.Button();
            this.btnUpgradeSelected = new System.Windows.Forms.Button();
            this.btnUpgradeAll = new System.Windows.Forms.Button();
            this.btnUninstall = new System.Windows.Forms.Button();
            this.chkInstalledOnly = new System.Windows.Forms.CheckBox();
            this.chkUpgradesOnly = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // comboDisplayMode
            // 
            this.comboDisplayMode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboDisplayMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboDisplayMode.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboDisplayMode.FormattingEnabled = true;
            this.comboDisplayMode.Location = new System.Drawing.Point(3, 3);
            this.comboDisplayMode.Name = "comboDisplayMode";
            this.comboDisplayMode.Size = new System.Drawing.Size(497, 21);
            this.comboDisplayMode.TabIndex = 339;
            // 
            // checkedListBoxApps
            // 
            this.checkedListBoxApps.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.checkedListBoxApps.BackColor = System.Drawing.SystemColors.Control;
            this.checkedListBoxApps.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.checkedListBoxApps.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.checkedListBoxApps.FormattingEnabled = true;
            this.checkedListBoxApps.Location = new System.Drawing.Point(4, 25);
            this.checkedListBoxApps.Name = "checkedListBoxApps";
            this.checkedListBoxApps.Size = new System.Drawing.Size(496, 288);
            this.checkedListBoxApps.Sorted = true;
            this.checkedListBoxApps.TabIndex = 340;
            // 
            // btnInstall
            // 
            this.btnInstall.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnInstall.Location = new System.Drawing.Point(425, 30);
            this.btnInstall.Name = "btnInstall";
            this.btnInstall.Size = new System.Drawing.Size(75, 23);
            this.btnInstall.TabIndex = 341;
            this.btnInstall.Text = "Install";
            this.btnInstall.UseVisualStyleBackColor = true;
            this.btnInstall.Visible = false;
            // 
            // btnUpgradeSelected
            // 
            this.btnUpgradeSelected.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnUpgradeSelected.Location = new System.Drawing.Point(3, 328);
            this.btnUpgradeSelected.Name = "btnUpgradeSelected";
            this.btnUpgradeSelected.Size = new System.Drawing.Size(101, 23);
            this.btnUpgradeSelected.TabIndex = 342;
            this.btnUpgradeSelected.Text = "Upgrade selected";
            this.btnUpgradeSelected.UseVisualStyleBackColor = true;
            // 
            // btnUpgradeAll
            // 
            this.btnUpgradeAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnUpgradeAll.Location = new System.Drawing.Point(108, 328);
            this.btnUpgradeAll.Name = "btnUpgradeAll";
            this.btnUpgradeAll.Size = new System.Drawing.Size(75, 23);
            this.btnUpgradeAll.TabIndex = 343;
            this.btnUpgradeAll.Text = "Upgrade all";
            this.btnUpgradeAll.UseVisualStyleBackColor = true;
            // 
            // btnUninstall
            // 
            this.btnUninstall.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnUninstall.Location = new System.Drawing.Point(375, 328);
            this.btnUninstall.Name = "btnUninstall";
            this.btnUninstall.Size = new System.Drawing.Size(75, 23);
            this.btnUninstall.TabIndex = 344;
            this.btnUninstall.Text = "Uninstall";
            this.btnUninstall.UseVisualStyleBackColor = true;
            // 
            // chkInstalledOnly
            // 
            this.chkInstalledOnly.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkInstalledOnly.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkInstalledOnly.AutoEllipsis = true;
            this.chkInstalledOnly.ForeColor = System.Drawing.Color.Black;
            this.chkInstalledOnly.Location = new System.Drawing.Point(187, 328);
            this.chkInstalledOnly.Name = "chkInstalledOnly";
            this.chkInstalledOnly.Size = new System.Drawing.Size(85, 23);
            this.chkInstalledOnly.TabIndex = 345;
            this.chkInstalledOnly.Text = "Installed";
            this.chkInstalledOnly.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkInstalledOnly.UseVisualStyleBackColor = true;
            // 
            // chkUpgradesOnly
            // 
            this.chkUpgradesOnly.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkUpgradesOnly.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkUpgradesOnly.AutoEllipsis = true;
            this.chkUpgradesOnly.ForeColor = System.Drawing.Color.Black;
            this.chkUpgradesOnly.Location = new System.Drawing.Point(275, 328);
            this.chkUpgradesOnly.Name = "chkUpgradesOnly";
            this.chkUpgradesOnly.Size = new System.Drawing.Size(95, 23);
            this.chkUpgradesOnly.TabIndex = 346;
            this.chkUpgradesOnly.Text = "Upgradeable";
            this.chkUpgradesOnly.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkUpgradesOnly.UseVisualStyleBackColor = true;
            // 
            // InstallView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.chkUpgradesOnly);
            this.Controls.Add(this.chkInstalledOnly);
            this.Controls.Add(this.btnUninstall);
            this.Controls.Add(this.btnUpgradeAll);
            this.Controls.Add(this.btnUpgradeSelected);
            this.Controls.Add(this.btnInstall);
            this.Controls.Add(this.checkedListBoxApps);
            this.Controls.Add(this.comboDisplayMode);
            this.Name = "InstallView";
            this.Size = new System.Drawing.Size(503, 354);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox comboDisplayMode;
        private System.Windows.Forms.CheckedListBox checkedListBoxApps;
        private System.Windows.Forms.Button btnInstall;
        private System.Windows.Forms.Button btnUpgradeSelected;
        private System.Windows.Forms.Button btnUpgradeAll;
        private System.Windows.Forms.Button btnUninstall;
        private System.Windows.Forms.CheckBox chkInstalledOnly;
        private System.Windows.Forms.CheckBox chkUpgradesOnly;
    }
}
