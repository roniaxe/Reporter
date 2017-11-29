using System.Windows.Forms;
using Reporter.Data.Services;
using Reporter.Forms;
using Reporter.View;
using Reporter.ViewModel;

namespace Reporter.Presentor
{
    public class CommentPresentor
    {
        private readonly ICommentView _view;
        private readonly ErrorCommentConn _viewModel;

        public CommentPresentor(ICommentView view, ErrorCommentConn viewModel)
        {
            _view = view;
            _viewModel = viewModel;
            InitActions();
            SetViewPropertiesFromModel();
        }

        private void SetViewPropertiesFromModel()
        {
            _view.MessageTextBox.Text = _viewModel.ErrorMessage;
            _view.CommentTextBox.Text = _viewModel.Comments;
        }

        private void InitActions()
        {
            _view.Saved += SaveAction;
            _view.Canceled += CancelAction;
        }

        private void CancelAction()
        {
            ((CommentForm)_view).Close();
        }

        private void SaveAction()
        {
            ErrorCommentConn entity = CommentService.GetById(_viewModel);
            _viewModel.Comments = _view.CommentTextBox.Text;
            try
            {
                if (entity == null)
                {
                    CommentService.Insert(_viewModel);
                }
                else
                {
                    CommentService.Update(_viewModel);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, @"Ooops.. Error!");
            }

            CancelAction();
        }
    }
}
