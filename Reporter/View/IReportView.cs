using System;
using System.Windows.Forms;

namespace Reporter.View
{
    public interface IReportView
    {
        event Action CreateButtonPressed;

        event Action EnvComboBoxChanged;

        event Action DbComboBoxChanged;

        DataGridView DataGridView { get; set; }

        DateTimePicker FromDate { get; set; }

        DateTimePicker ToDate { get; set; }

        ComboBox EnvComboBox { get; set; }

        ComboBox DbComboBox { get; set; }
    }
}
