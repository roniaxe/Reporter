using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reporter.View
{
    public interface ICommentView
    {
        event Action Saved;

        event Action Canceled;

        TextBox MessageTextBox { get; set; }

        TextBox CommentTextBox { get; set; }
    }
}
