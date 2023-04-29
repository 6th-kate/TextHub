using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TextHub
{
    class VersionsMenuHelper : INotifyPropertyChanged
    {
        private double sideBarsOpenedWidth;
        private double sideBarsClosedWidth;

        public VersionsMenuHelper(double sideBarsOpenedWidth, double sideBarsClosedWidth)
        {
            this.sideBarsClosedWidth = sideBarsClosedWidth;
            this.sideBarsOpenedWidth = sideBarsOpenedWidth;
            VersionsListVisibility = System.Windows.Visibility.Visible;
            VersionsMenuWidth = new System.Windows.GridLength(sideBarsOpenedWidth);
            VersionsMenuCloseButtonVisibility = System.Windows.Visibility.Visible;
            VersionsMenuOpenButtonVisibility = System.Windows.Visibility.Collapsed;
        }

        System.Windows.Visibility versionsListVisibility;
        /// <summary>
        /// The visibility of the versions side panel
        /// </summary>
        public System.Windows.Visibility VersionsListVisibility
        {
            get { return versionsListVisibility; }
            set
            {
                versionsListVisibility = value;
                OnPropertyChanged("VersionsListVisibility");
            }
        }

        System.Windows.GridLength versionsMenuWidth;
        /// <summary>
        /// The width of the versions side panel
        /// </summary>
        public System.Windows.GridLength VersionsMenuWidth
        {
            get { return versionsMenuWidth; }
            set
            {
                versionsMenuWidth = value;
                OnPropertyChanged("VersionsMenuWidth");
            }
        }

        System.Windows.Visibility versionsMenuCloseButtonVisibility;
        /// <summary>
        /// The visibility of the button of closing the versions side panel
        /// </summary>
        public System.Windows.Visibility VersionsMenuCloseButtonVisibility
        {
            get { return versionsMenuCloseButtonVisibility; }
            set
            {
                versionsMenuCloseButtonVisibility = value;
                OnPropertyChanged("VersionsMenuCloseButtonVisibility");
            }
        }

        System.Windows.Visibility versionsMenuOpenButtonVisibility;
        /// <summary>
        /// The visibility of the button of opening the versions side panel
        /// </summary>
        public System.Windows.Visibility VersionsMenuOpenButtonVisibility
        {
            get { return versionsMenuOpenButtonVisibility; }
            set
            {
                versionsMenuOpenButtonVisibility = value;
                OnPropertyChanged("VersionsMenuOpenButtonVisibility");
            }
        }

        private TextHubCommands.SimpleCommand closeVersionsMenuCommand;
        /// <summary>
        /// The command closing the versions side panel
        /// </summary>
        public TextHubCommands.SimpleCommand CloseVersionsMenuCommand
        {
            get
            {
                return closeVersionsMenuCommand ??
                  (closeVersionsMenuCommand = new TextHubCommands.SimpleCommand(obj =>
                  {
                      this.VersionsMenuCloseButtonVisibility = System.Windows.Visibility.Collapsed;
                      this.VersionsMenuOpenButtonVisibility = System.Windows.Visibility.Visible;
                      this.VersionsMenuWidth = new System.Windows.GridLength(sideBarsClosedWidth);
                      this.VersionsListVisibility = System.Windows.Visibility.Collapsed;
                  }));
            }
        }

        private TextHubCommands.SimpleCommand openVersionsMenuCommand;
        /// <summary>
        /// The command opening the versions side panel
        /// </summary>
        public TextHubCommands.SimpleCommand OpenVersionsMenuCommand
        {
            get
            {
                return openVersionsMenuCommand ??
                  (openVersionsMenuCommand = new TextHubCommands.SimpleCommand(obj =>
                  {
                      this.VersionsMenuCloseButtonVisibility = System.Windows.Visibility.Visible;
                      this.VersionsMenuOpenButtonVisibility = System.Windows.Visibility.Collapsed;
                      this.VersionsMenuWidth = new System.Windows.GridLength(sideBarsOpenedWidth);
                      this.VersionsListVisibility = System.Windows.Visibility.Visible;
                  }));
            }
        }

        public void Update(VersionsMenuHelper other)
        {
            this.VersionsListVisibility = other.VersionsListVisibility;
            this.VersionsMenuWidth = other.VersionsMenuWidth;
            this.VersionsMenuCloseButtonVisibility = other.VersionsMenuCloseButtonVisibility;
            this.VersionsMenuOpenButtonVisibility = other.VersionsMenuOpenButtonVisibility;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}