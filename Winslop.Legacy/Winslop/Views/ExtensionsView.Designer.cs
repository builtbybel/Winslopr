namespace Winslop.Extensions
{
    partial class ExtensionsView
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
            this.lblStatus = new System.Windows.Forms.Label();
            this.comboFilter = new System.Windows.Forms.ComboBox();
            this.listTools = new System.Windows.Forms.ListBox();
            this.detailsControl = new Winslop.Extensions.ExtensionsItemControl();
            this.SuspendLayout();
            // 
            // lblStatus
            // 
            this.lblStatus.AutoEllipsis = true;
            this.lblStatus.BackColor = System.Drawing.Color.Transparent;
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI Variable Small Semibol", 10F, System.Drawing.FontStyle.Bold);
            this.lblStatus.Location = new System.Drawing.Point(80, 3);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(111, 18);
            this.lblStatus.TabIndex = 0;
            this.lblStatus.Text = "Loading...";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblStatus.UseCompatibleTextRendering = true;
            this.lblStatus.Visible = false;
            // 
            // comboFilter
            // 
            this.comboFilter.BackColor = System.Drawing.Color.White;
            this.comboFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboFilter.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.comboFilter.Font = new System.Drawing.Font("Consolas", 8.25F);
            this.comboFilter.ForeColor = System.Drawing.Color.Black;
            this.comboFilter.FormattingEnabled = true;
            this.comboFilter.Location = new System.Drawing.Point(3, 3);
            this.comboFilter.Name = "comboFilter";
            this.comboFilter.Size = new System.Drawing.Size(71, 21);
            this.comboFilter.TabIndex = 351;
            this.comboFilter.TabStop = false;
            this.comboFilter.SelectedIndexChanged += new System.EventHandler(this.comboFilter_SelectedIndexChanged);
            // 
            // listTools
            // 
            this.listTools.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listTools.BackColor = System.Drawing.SystemColors.Control;
            this.listTools.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listTools.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.listTools.FormattingEnabled = true;
            this.listTools.ItemHeight = 15;
            this.listTools.Location = new System.Drawing.Point(0, 30);
            this.listTools.Name = "listTools";
            this.listTools.Size = new System.Drawing.Size(200, 497);
            this.listTools.TabIndex = 352;
            // 
            // detailsControl
            // 
            this.detailsControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.detailsControl.AutoScroll = true;
            this.detailsControl.BackColor = System.Drawing.SystemColors.Control;
            this.detailsControl.Location = new System.Drawing.Point(203, 0);
            this.detailsControl.Name = "detailsControl";
            this.detailsControl.Size = new System.Drawing.Size(301, 524);
            this.detailsControl.TabIndex = 353;
            // 
            // ExtensionsView
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.detailsControl);
            this.Controls.Add(this.listTools);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.comboFilter);
            this.Name = "ExtensionsView";
            this.Size = new System.Drawing.Size(507, 527);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ComboBox comboFilter;
        private System.Windows.Forms.ListBox listTools;
        private ExtensionsItemControl detailsControl;
    }
}
