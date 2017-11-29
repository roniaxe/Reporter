using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Reporter.View;

namespace Reporter.Forms
{
    public partial class CommentForm : Form, ICommentView
    {
        public event Action Saved;
        public event Action Canceled;

        public TextBox MessageTextBox
        {
            get => textBox1;
            set => textBox1 = value;
        }

        public TextBox CommentTextBox
        {
            get => textBox2;
            set => textBox2 = value;
        }

        public CommentForm()
        {
            InitializeComponent();
            btnSave.Click += SavedButtonClicked;
            btnCancel.Click += CancelButtonClicked;
        }

        private void CancelButtonClicked(object sender, EventArgs e)
        {
            Canceled?.Invoke();
        }

        private void SavedButtonClicked(object sender, EventArgs e)
        {
            Saved?.Invoke();
        }
    }
}
