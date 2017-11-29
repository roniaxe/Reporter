using System;
using System.Windows.Forms;

namespace Reporter.View
{
    public interface IReportView
    {
        event Action AddedPerson;

        event Action DeletedPerson;

        event Action CreateButtonPressed;

        event Action RunButtonPressed;

        event Action EnvComboBoxChanged;

        event Action DbComboBoxChanged;

        event Action<object, DataGridViewCellEventArgs> DgvCellDoubleClicked;

        DataGridView DataGridView { get; set; }

        DateTimePicker FromDate { get; set; }

        DateTimePicker ToDate { get; set; }

        ComboBox EnvComboBox { get; set; }

        ComboBox DbComboBox { get; set; }

        DataGridView EmailList { get; set; }

        ProgressBar ProgressBar { get; set; }

        BindingSource PersonBindingSource { get; set; }
    }
}
