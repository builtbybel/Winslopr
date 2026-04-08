namespace Winslop.Views
{
    partial class AppsView
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
            this.checkedListBoxApps = new System.Windows.Forms.CheckedListBox();
            this.comboDisplayMode = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
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
            this.checkedListBoxApps.Items.AddRange(new object[] {
            "No inspection yet"});
            this.checkedListBoxApps.Location = new System.Drawing.Point(0, 34);
            this.checkedListBoxApps.Name = "checkedListBoxApps";
            this.checkedListBoxApps.Size = new System.Drawing.Size(503, 320);
            this.checkedListBoxApps.Sorted = true;
            this.checkedListBoxApps.TabIndex = 337;
            // 
            // comboDisplayMode
            // 
            this.comboDisplayMode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboDisplayMode.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboDisplayMode.FormattingEnabled = true;
            this.comboDisplayMode.Location = new System.Drawing.Point(3, 3);
            this.comboDisplayMode.Name = "comboDisplayMode";
            this.comboDisplayMode.Size = new System.Drawing.Size(497, 21);
            this.comboDisplayMode.TabIndex = 338;
            // 
            // AppsView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.comboDisplayMode);
            this.Controls.Add(this.checkedListBoxApps);
            this.Name = "AppsView";
            this.Size = new System.Drawing.Size(503, 354);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckedListBox checkedListBoxApps;
        private System.Windows.Forms.ComboBox comboDisplayMode;
    }
}
