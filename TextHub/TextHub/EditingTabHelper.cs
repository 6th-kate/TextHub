using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TextHub
{
    class EditingTabHelper : INotifyPropertyChanged
    {
        public EditingTabHelper(TextHubViewModel viewModel)
        {
            ChangeFontCommand = new TextHubCommands.ChangeFontCommand(viewModel);
            ColorTextCommand = new TextHubCommands.ColorTextCommand(viewModel);
            HighlightTextCommand = new TextHubCommands.HighlightTextCommand(viewModel);
            PasteImageCommand = new TextHubCommands.PasteImageCommand();
            InsertImageCommand = new TextHubCommands.InsertImageCommand();
            editingTabVisibility = System.Windows.Visibility.Collapsed;
        }

        public TextHubCommands.ChangeFontCommand ChangeFontCommand { get; set; }
        public TextHubCommands.ColorTextCommand ColorTextCommand { get; set; }
        public TextHubCommands.HighlightTextCommand HighlightTextCommand { get; set; }
        public TextHubCommands.PasteImageCommand PasteImageCommand { get; set; }
        public TextHubCommands.InsertImageCommand InsertImageCommand { get; set; }

        private System.Windows.Visibility editingTabVisibility;
        /// <summary>
        /// The visibility of the Editing tab
        /// </summary>
        public System.Windows.Visibility EditingTabVisibility
        {
            get { return editingTabVisibility; }
            set
            {
                editingTabVisibility = value;
                OnPropertyChanged("EditingTabVisibility");
            }
        }

        public void Update(EditingTabHelper other)
        {
            this.EditingTabVisibility = other.EditingTabVisibility;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}