using System;
using System.Windows.Forms;

namespace Reporter.View
{
    public interface IReportView : IView
    {
        event Action AddedPerson;

        event Action DeletedPerson;

        event Action CreateButtonPressed;

        event Action RunButtonPressed;

        event Action CancelButtonPressed;

        event Action EnvComboBoxChanged;

        event Action DbComboBoxChanged;

        event Action<object, DataGridViewCellEventArgs> DgvCellDoubleClicked;

        event Action PolicyFilterPressed;

        DataGridView DataGridView { get; set; }

        DateTimePicker FromDate { get; set; }

        DateTimePicker ToDate { get; set; }

        ComboBox EnvComboBox { get; set; }

        ComboBox DbComboBox { get; set; }

        DataGridView EmailList { get; set; }

        ProgressBar ProgressBar { get; set; }

        BindingSource PersonBindingSource { get; set; }

        DataGridView DataGridView2 { get; set; }

        DataGridView DataGridView3 { get; set; }

        Button RunButton { get; set; }

        Button CnclButton { get; set; }

        DataGridView AllErrorsGrid { get; set; }

        Button PolicyFilterButton { get; set; }
    }
}
