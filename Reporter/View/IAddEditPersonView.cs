using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reporter.View
{
    public interface IAddEditPersonView
    {
        event Action Saved;

        event Action Canceled;

        TextBox NameTextBox { get; set; }

        TextBox EmailTextBox { get; set; }
    }
}
