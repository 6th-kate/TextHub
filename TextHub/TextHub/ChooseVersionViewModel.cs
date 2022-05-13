using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;

namespace TextHub
{
    /// <summary>
    /// The view model class of the Choose Version Dialog
    /// </summary>
    class ChooseVersionViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// List of project versions from which to choose
        /// </summary>
        public ObservableCollection<TextHubVersion> Versions { get; set; }

        /// <summary>
        /// Initialized starting values of elements
        /// </summary>
        public ChooseVersionViewModel(ObservableCollection<TextHubVersion> versions)
        {
            Versions = versions;
            SelectedVersion = Versions[Versions.Count - 1];
        }

        /// <summary>
        /// The result of the dialog (if OK is pressed, the true, false otherwise)
        /// </summary>
        public bool DialogResult { get; set; }

        private TextHubVersion selectedVersion;
        /// <summary>
        /// The version, currently selected in the ComboBox to compare to
        /// </summary>
        public TextHubVersion SelectedVersion
        {
            get { return selectedVersion; }
            set
            {
                selectedVersion = value;
                OnPropertyChanged("SelectedVersion");
            }
        }

        private TextHubCommands.SimpleCommand okCommand;
        /// <summary>
        /// Represents the action taking place when the OK button is pressed. 
        /// When invoked, changes the DialogResult to true
        /// </summary>
        public TextHubCommands.SimpleCommand OKCommand
        {
            get
            {
                return okCommand ??
                  (okCommand = new TextHubCommands.SimpleCommand(obj =>
                  {
                      Window window = obj as Window;
                      window.Close();
                      DialogResult = true;

                  }));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Resets the view values of the changed properties
        /// </summary>
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
