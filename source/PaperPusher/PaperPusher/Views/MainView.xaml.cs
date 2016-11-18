using System.Windows;
using System.Windows.Data;

namespace PaperPusher.Views
{
    public partial class MainView
    {
        #region [ Constructors ]

        public MainView()
        {
            InitializeComponent();
        }

        #endregion

        #region [ Methods ]

        private void DeleteDocument_OnClick(object sender, RoutedEventArgs e)
        {
            DocumentDate.Focus();
        }

        private void RenameAndMoveDocument_OnClick(object sender, RoutedEventArgs e)
        {
            DocumentDate.Focus();
        }


        private void zoomBorder_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            ZoomBorder.Reset();
        }

        #endregion
    }
}