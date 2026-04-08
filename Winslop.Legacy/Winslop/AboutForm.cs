using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Winslop
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
            InitializeUI();
            checkDonated.Checked = DonationHelper.HasDonated();
        }

        private void InitializeUI()
        {
            // Update version label
            this.lblVersionInfo.Text = $"v{Program.GetAppVersion()} ";
        }

        private void btnDonate_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://www.paypal.com/donate/?hosted_button_id=BNVXAGPQ8CTR6",
                UseShellExecute = true
            });
        }

        private void linkCopyright_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/builtbybel/Winslop/releases");
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void checkDonated_CheckedChanged(object sender, EventArgs e)
        {
            DonationHelper.SetDonationStatus(checkDonated.Checked);
        }
    }
}