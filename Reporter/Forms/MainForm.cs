using System;
using System.Windows.Forms;
using Reporter.Presentor;

namespace Reporter.Forms
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DateTime.Today < new DateTime(2018, 4, 20))
            {
                ReportForm view = new ReportForm();
                new ReportPresentor(view);
                view.Show();
            }           
        }
    }
}
