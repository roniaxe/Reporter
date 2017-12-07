using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Reporter.View;

namespace Reporter.Forms
{
    public partial class ReportForm : Form, IReportView
    {
        public event Action AddedPerson;
        public event Action DeletedPerson;
        public event Action CreateButtonPressed;
        public event Action CancelButtonPressed;
        public event Action EnvComboBoxChanged;
        public event Action DbComboBoxChanged;
        public event Action<object, DataGridViewCellEventArgs> DgvCellDoubleClicked;
        public event Action RunButtonPressed;

        public ReportForm()
        {
            InitializeComponent();
            RunButton.Click += RunButtonClicked;
            comboBox1.SelectedIndexChanged += InvokeEnvComboBoxChanged;
            comboBox2.SelectedIndexChanged += InvokeDbComboBoxChanged;
            dataGridView1.CellDoubleClick += InvokeCellDoubleClick;
            btnAdd.Click += AddPersonBtnClicked;
            btnDelete.Click += DeletePersonBtnClicked;
            CreateButton.Click += CreateReportButtonPressed;
            _btnCncl.Click += CnclPressed;
        }

        private void CnclPressed(object sender, EventArgs e)
        {
            CancelButtonPressed?.Invoke();
        }

        private void CreateReportButtonPressed(object sender, EventArgs e)
        {
            CreateButtonPressed?.Invoke();
        }

        private void DeletePersonBtnClicked(object sender, EventArgs e)
        {
            DeletedPerson?.Invoke();
        }

        private void AddPersonBtnClicked(object sender, EventArgs e)
        {
            AddedPerson?.Invoke();
        }

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

        public DataGridView EmailList
        {
            get => dataGridView2;
            set => dataGridView2 = value;
        }

        public ProgressBar ProgressBar
        {
            get => progressBar1;
            set => progressBar1 = value;
        }

        public BindingSource PersonBindingSource
        {
            get => personBindingSource;
            set => personBindingSource = value;
        }

        public DataGridView DataGridView2
        {
            get => dataGridView3;
            set => dataGridView3 = value;
        }

        public DataGridView DataGridView3
        {
            get => dataGridView4;
            set => dataGridView4 = value;
        }

        public Button RunButton
        {
            get => btnRun;
            set => btnRun = value;
        }

        public Button CnclButton
        {
            get => _btnCncl;
            set => _btnCncl = value;
        }

        public DataGridView AllErrorsGrid
        {
            get => dataGridView5;
            set => dataGridView5 = value;
        }

        private void InvokeCellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            DgvCellDoubleClicked?.Invoke(sender, e);
        }

        private void InvokeDbComboBoxChanged(object sender, EventArgs e)
        {
            DbComboBoxChanged?.Invoke();
        }

        private void InvokeEnvComboBoxChanged(object sender, EventArgs e)
        {
            EnvComboBoxChanged?.Invoke();
        }

        private void RunButtonClicked(object sender, EventArgs e)
        {
            RunButtonPressed?.Invoke();
        }
    }
}
