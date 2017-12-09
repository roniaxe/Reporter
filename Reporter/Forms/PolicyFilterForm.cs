using System;
using System.Windows.Forms;
using Reporter.View;

namespace Reporter.Forms
{
    public partial class PolicyFilterForm : Form, IPolicyFilterView
    {
        public PolicyFilterForm()
        {
            InitializeComponent();
            InitProps();
        }

        private void InitProps()
        {
            btnRun.Click += InvokeRunButton;
        }

        private void InvokeRunButton(object sender, EventArgs e)
        {
            RunButtonPressed?.Invoke();
        }

        public event Action RunButtonPressed;
        public DateTimePicker FromDate
        {
            get => dtpFromDate;
            set => dtpFromDate = value;
        }
        public DateTimePicker ToDate {
            get => dtpToDate;
            set => dtpToDate = value;
        }
        public TextBox PolicyNo {
            get => txtPolicy;
            set => txtPolicy = value;
        }

        public TextBox ExtPolicyNo
        {
            get => txtExtPolNo;
            set => txtExtPolNo = value;
        }

        public TextBox ClientNo
        {
            get => txtClientNo;
            set => txtClientNo = value;
        }

        public DataGridView MainGrid {
            get => dataGridView1;
            set => dataGridView1 = value;
        }
        public Button RunButton {
            get => btnRun;
            set => btnRun = value;
        }
    }
}
