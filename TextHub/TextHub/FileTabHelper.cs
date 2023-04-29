using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TextHub
{
    class FileTabHelper : INotifyPropertyChanged
    {

        public FileTabHelper(TextHubViewModel viewModel)
        {
            FileTabVisibility = System.Windows.Visibility.Visible;
            SaveButtonVisibility = System.Windows.Visibility.Collapsed;
            SaveAsNewVersionButtonVisibility = System.Windows.Visibility.Collapsed;
            MakeNewSubprojectButtonVisibility = System.Windows.Visibility.Collapsed;
            NewDocumentCommand = new TextHubCommands.NewDocumentCommand(viewModel);
            OpenDocumentCommand = new TextHubCommands.OpenDocumentCommand(viewModel);
            OpenProjectCommand = new TextHubCommands.OpenProjectCommand(viewModel);
            SaveCommand = new TextHubCommands.SaveCommand(viewModel);
            SaveNewVersionCommand = new TextHubCommands.SaveNewVersionCommand(viewModel);
            MakeNewSubprojectCommand = new TextHubCommands.MakeNewSubprojectCommand(viewModel);
        }

        public TextHubCommands.NewDocumentCommand NewDocumentCommand { get; set; }
        public TextHubCommands.OpenDocumentCommand OpenDocumentCommand { get; set; }
        public TextHubCommands.OpenProjectCommand OpenProjectCommand { get; set; }
        public TextHubCommands.SaveCommand SaveCommand { get; set; }
        public TextHubCommands.SaveNewVersionCommand SaveNewVersionCommand { get; set; }
        public TextHubCommands.MakeNewSubprojectCommand MakeNewSubprojectCommand { get; set; }

        private System.Windows.Visibility fileTabVisibility;
        /// <summary>
        /// The visibility of the File tab
        /// </summary>
        public System.Windows.Visibility FileTabVisibility
        {
            get { return fileTabVisibility; }
            set
            {
                fileTabVisibility = value;
                OnPropertyChanged("FileTabVisibility");
            }
        }

        private System.Windows.Visibility saveButtonVisibility;
        /// <summary>
        /// The visibility of the save button
        /// </summary>
        public System.Windows.Visibility SaveButtonVisibility
        {
            get { return saveButtonVisibility; }
            set
            {
                saveButtonVisibility = value;
                OnPropertyChanged("SaveButtonVisibility");
            }
        }

        private System.Windows.Visibility saveAsNewVersionButtonVisibility;
        /// <summary>
        /// The visibility of the button of saving the selected version as a new version of the document
        /// </summary>
        public System.Windows.Visibility SaveAsNewVersionButtonVisibility
        {
            get { return saveAsNewVersionButtonVisibility; }
            set
            {
                saveAsNewVersionButtonVisibility = value;
                OnPropertyChanged("SaveAsNewVersionButtonVisibility");
            }
        }

        private System.Windows.Visibility makeNewSubprojectButtonVisibility;
        /// <summary>
        /// The visibility of the button of creating a new subproject based on the selected version
        /// </summary>
        public System.Windows.Visibility MakeNewSubprojectButtonVisibility
        {
            get { return makeNewSubprojectButtonVisibility; }
            set
            {
                makeNewSubprojectButtonVisibility = value;
                OnPropertyChanged("MakeNewSubprojectButtonVisibility");
            }
        }


        public void Update(FileTabHelper other)
        {
            this.FileTabVisibility = other.FileTabVisibility;
            this.SaveButtonVisibility = other.SaveButtonVisibility;
            this.SaveAsNewVersionButtonVisibility = other.SaveAsNewVersionButtonVisibility;
            this.MakeNewSubprojectButtonVisibility = other.MakeNewSubprojectButtonVisibility;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}