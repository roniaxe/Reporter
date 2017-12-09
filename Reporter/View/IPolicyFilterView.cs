using System;
using System.Windows.Forms;

namespace Reporter.View
{
    public interface IPolicyFilterView
    {
        event Action RunButtonPressed;

        DateTimePicker FromDate { get; set; }

        DateTimePicker ToDate { get; set; }

        TextBox PolicyNo { get; set; }

        TextBox ExtPolicyNo { get; set; }

        TextBox ClientNo { get; set; }

        DataGridView MainGrid { get; set; }

        Button RunButton { get; set; }
    }
}
