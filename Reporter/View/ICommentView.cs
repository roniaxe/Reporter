using System;
using System.Windows.Forms;

namespace Reporter.View
{
    public interface ICommentView : IView
    {
        event Action Saved;

        event Action Canceled;

        TextBox MessageTextBox { get; set; }

        TextBox CommentTextBox { get; set; }
    }
}
