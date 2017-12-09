using System;
using System.Windows.Forms;

namespace Reporter.View
{
    public interface IAddEditPersonView : IView
    {
        event Action Saved;

        event Action Canceled;

        TextBox NameTextBox { get; set; }

        TextBox EmailTextBox { get; set; }
    }
}
