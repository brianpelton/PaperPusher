using System.Windows.Controls;
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

        private void CurrentFiles_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // this is okay, but it fires too early.
            //ZoomBorder.Reset();
        }


        private void zoomBorder_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            ZoomBorder.Reset();
        }

        #endregion
    }
}