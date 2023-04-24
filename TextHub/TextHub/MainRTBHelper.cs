using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TextHub
{
    class MainRTBHelper : INotifyPropertyChanged
    {
        public MainRTBHelper()
        {
            MainRTBVisibility = System.Windows.Visibility.Collapsed;
            DiffViewVisibility = System.Windows.Visibility.Collapsed;
        }

        private string newTextHeader;
        /// <summary>
        /// The header of the new text in comparison
        /// </summary>
        public string NewTextHeader
        {
            get { return newTextHeader; }
            set
            {
                newTextHeader = value;
                OnPropertyChanged("NewTextHeader");
            }
        }

        private string oldTextHeader;
        /// <summary>
        /// The header of the old text in comparison
        /// </summary>
        public string OldTextHeader
        {
            get { return oldTextHeader; }
            set
            {
                oldTextHeader = value;
                OnPropertyChanged("OldTextHeader");
            }
        }


        private string oldText;
        /// <summary>
        /// The old text in comparison
        /// </summary>
        public string OldText
        {
            get { return oldText; }
            set
            {
                oldText = value;
                OnPropertyChanged("OldText");
            }
        }

        private string newText;
        /// <summary>
        /// The new text in comparison
        /// </summary>
        public string NewText
        {
            get { return newText; }
            set
            {
                newText = value;
                OnPropertyChanged("NewText");
            }
        }

        private string currentText;
        /// <summary>
        /// The current text in the main RichTextBox
        /// </summary>
        public string CurrentText
        {
            get { return currentText; }
            set
            {
                currentText = value;
                OnPropertyChanged("CurrentText");
            }
        }

        private bool editingBlocked;
        /// <summary>
        /// An indicator if the current chosen version is Changeable or not. If true, the main RichTextBox becomes readonly
        /// </summary>
        public bool EditingBlocked
        {
            get { return editingBlocked; }
            set
            {
                editingBlocked = value;
                OnPropertyChanged("EditingBlocked");
            }
        }

        System.Windows.Visibility mainRTBVisibility;
        /// <summary>
        /// The visibility of the main RichTextBox
        /// </summary>
        public System.Windows.Visibility MainRTBVisibility
        {
            get { return mainRTBVisibility; }
            set
            {
                mainRTBVisibility = value;
                OnPropertyChanged("MainRTBVisibility");
            }
        }

        System.Windows.Visibility diffViewVisibility;
        /// <summary>
        /// The visibility of the main difference viewer
        /// </summary>
        public System.Windows.Visibility DiffViewVisibility
        {
            get { return diffViewVisibility; }
            set
            {
                diffViewVisibility = value;
                OnPropertyChanged("DiffViewVisibility");
            }
        }

        public void Update(MainRTBHelper other)
        {
            this.NewTextHeader = other.NewTextHeader;
            this.OldTextHeader = other.OldTextHeader;
            this.OldText = other.OldText;
            this.NewText = other.NewText;
            this.CurrentText = other.CurrentText;
            this.EditingBlocked = other.EditingBlocked;
            this.MainRTBVisibility = other.MainRTBVisibility;
            this.DiffViewVisibility = other.DiffViewVisibility;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}