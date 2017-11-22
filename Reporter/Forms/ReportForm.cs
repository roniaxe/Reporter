using System;
using System.Linq;
using System.Windows.Forms;
using Reporter.Model;
using Reporter.View;

namespace Reporter.Forms
{
    public partial class ReportForm : Form, IReportView
    {
        public event Action CreateButtonPressed;
        public event Action EnvComboBoxChanged;
        public event Action DbComboBoxChanged;

        public DataGridView DataGridView
        {
            get => dataGridView1;
            set => dataGridView1 = value;
        }

        public DateTimePicker FromDate {
            get => dateTimePicker1;
            set => dateTimePicker1 = value;
        }

        public DateTimePicker ToDate
        {
            get => dateTimePicker2;
            set => dateTimePicker2 = value;
        }

        public ComboBox EnvComboBox
        {
            get => comboBox1;
            set => comboBox1 = value;
        }

        public ComboBox DbComboBox
        {
            get => comboBox2;
            set => comboBox2 = value;
        }

        public ReportForm()
        {
            InitializeComponent();
            CreateButton.Click += CreateButtonClicked;
            comboBox1.SelectedIndexChanged += InvokeEnvComboBoxChanged;
            comboBox2.SelectedIndexChanged += InvokeDbComboBoxChanged;
        }

        private void InvokeDbComboBoxChanged(object sender, EventArgs e)
        {
            DbComboBoxChanged?.Invoke();
        }

        private void InvokeEnvComboBoxChanged(object sender, EventArgs e)
        {
            EnvComboBoxChanged?.Invoke();
        }

        private void CreateButtonClicked(object sender, EventArgs e)
        {
            CreateButtonPressed?.Invoke();
        }
    }
}
