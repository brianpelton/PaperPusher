using Caliburn.Micro;
using PropertyChanged;

namespace PaperPusher.ViewModels
{
    [ImplementPropertyChanged]
    public class MainViewModel : Screen
    {
        #region [ Properties ]

        public string Name { get; set; } = "Fred";

        public void ChangeName()
        {
            Name = "Tim";
        }

        #endregion
    }
}