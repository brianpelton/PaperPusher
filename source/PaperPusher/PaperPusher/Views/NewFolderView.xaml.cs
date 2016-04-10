using System;
using System.Windows.Controls;

namespace PaperPusher.Views
{
    public partial class NewFolderView : UserControl
    {
        #region [ Constructors ]

        public NewFolderView()
        {
            InitializeComponent();
            Loaded += delegate { FolderName.Focus(); };
        }

        #endregion
    }
}