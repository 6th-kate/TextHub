using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TextHub
{
    class DocumentsMenuHelper : INotifyPropertyChanged
    {
        private double sideBarsOpenedWidth;
        private double sideBarsClosedWidth;

        public DocumentsMenuHelper(double sideBarsOpenedWidth, double sideBarsClosedWidth)
        {
            this.sideBarsClosedWidth = sideBarsClosedWidth;
            this.sideBarsOpenedWidth = sideBarsOpenedWidth;
            DocumentsMenuWidth = new System.Windows.GridLength(sideBarsOpenedWidth);
            DocumentsListVisibility = System.Windows.Visibility.Visible;
            DocumentsMenuCloseButtonVisibility = System.Windows.Visibility.Visible;
            DocumentsMenuOpenButtonVisibility = System.Windows.Visibility.Collapsed;
        }


        System.Windows.GridLength documentsMenuWidth;
        /// <summary>
        /// The width of the documents side panel
        /// </summary>
        public System.Windows.GridLength DocumentsMenuWidth
        {
            get { return documentsMenuWidth; }
            set
            {
                documentsMenuWidth = value;
                OnPropertyChanged("DocumentsMenuWidth");
            }
        }

        System.Windows.Visibility documentsListVisibility;
        /// <summary>
        /// The visibility of the documents side panel
        /// </summary>
        public System.Windows.Visibility DocumentsListVisibility
        {
            get { return documentsListVisibility; }
            set
            {
                documentsListVisibility = value;
                OnPropertyChanged("DocumentsListVisibility");
            }
        }

        System.Windows.Visibility documentsMenuCloseButtonVisibility;
        /// <summary>
        /// The visibility of the button of closing the documents side panel
        /// </summary>
        public System.Windows.Visibility DocumentsMenuCloseButtonVisibility
        {
            get { return documentsMenuCloseButtonVisibility; }
            set
            {
                documentsMenuCloseButtonVisibility = value;
                OnPropertyChanged("DocumentsMenuCloseButtonVisibility");
            }
        }

        System.Windows.Visibility documentsMenuOpenButtonVisibility;
        /// <summary>
        /// The visibility of the button of opening the documents side panel
        /// </summary>
        public System.Windows.Visibility DocumentsMenuOpenButtonVisibility
        {
            get { return documentsMenuOpenButtonVisibility; }
            set
            {
                documentsMenuOpenButtonVisibility = value;
                OnPropertyChanged("DocumentsMenuOpenButtonVisibility");
            }
        }

        private TextHubCommands.SimpleCommand closeDocumentMenuCommand;
        /// <summary>
        /// The command closing the documents side panel
        /// </summary>
        public TextHubCommands.SimpleCommand CloseDocumentMenuCommand
        {
            get
            {
                return closeDocumentMenuCommand ??
                  (closeDocumentMenuCommand = new TextHubCommands.SimpleCommand(obj =>
                  {
                      this.DocumentsMenuCloseButtonVisibility = System.Windows.Visibility.Collapsed;
                      this.DocumentsMenuOpenButtonVisibility = System.Windows.Visibility.Visible;
                      this.DocumentsMenuWidth = new System.Windows.GridLength(sideBarsClosedWidth);
                      this.DocumentsListVisibility = System.Windows.Visibility.Collapsed;
                  }));
            }
        }

        private TextHubCommands.SimpleCommand openDocumentMenuCommand;
        /// <summary>
        /// The command opening the documents side panel
        /// </summary>
        public TextHubCommands.SimpleCommand OpenDocumentMenuCommand
        {
            get
            {
                return openDocumentMenuCommand ??
                  (openDocumentMenuCommand = new TextHubCommands.SimpleCommand(obj =>
                  {
                      this.DocumentsMenuCloseButtonVisibility = System.Windows.Visibility.Visible;
                      this.DocumentsMenuOpenButtonVisibility = System.Windows.Visibility.Collapsed;
                      this.DocumentsMenuWidth = new System.Windows.GridLength(sideBarsOpenedWidth);
                      this.DocumentsListVisibility = System.Windows.Visibility.Visible;
                  }));
            }
        }

        public void Update(DocumentsMenuHelper other)
        {
            this.DocumentsMenuWidth = other.DocumentsMenuWidth;
            this.DocumentsListVisibility = other.DocumentsListVisibility;
            this.DocumentsMenuCloseButtonVisibility = other.DocumentsMenuCloseButtonVisibility;
            this.DocumentsMenuOpenButtonVisibility = other.DocumentsMenuOpenButtonVisibility;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}