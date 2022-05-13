using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;

namespace TextHub
{
    class OpeningDialogViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// PlaceHolder of the name of the project if it needs to be chosen
        /// </summary>
        public const string NewFileStandardPlaceHolder = "ВыберитеИмя";
        /// <summary>
        /// The path of the file or the directory chosen
        /// </summary>
        public string FullPath { get; set; }
        /// <summary>
        /// List of modes of opening the file: "View" or "Editing"
        /// HIdden when the project is created.
        /// </summary>
        public ObservableCollection<string> OpenFIleModes { get; set; }
        /// <summary>
        /// List of window modes in which the project can be opened: "This window" or "New window"
        /// </summary>
        public ObservableCollection<string> OpenWindowModes { get; set; }
        /// <summary>
        /// The mode of opening the file user selects in ComboBox
        /// </summary>
        public string SelectedOpenFileMode { get; set; }
        /// <summary>
        /// The mode of opening the window user selects in ComboBox
        /// </summary>
        public string SelectedOpenWindowMode { get; set; }
        /// <summary>
        /// The result of the dialog (if OK is pressed, the true, false otherwise)
        /// </summary>
        public bool DialogResult { get; set; }

        /// <summary>
        /// Initializes starting values of the window
        /// </summary>
        /// <param name="path">The path of the file or the directory chosen before</param>
        /// <param name="isNew">A semaphor, reflecting, if it is a new project creation (true) or an opening of an existing one (false)</param>
        public OpeningDialogViewModel(string path, bool isNew = false)
        {
            OpenFIleModes = new ObservableCollection<string>() { "Просмотр", "Редактирование" };
            OpenWindowModes = new ObservableCollection<string>() { "Текущее окно", "Новое окно" };
            SelectedOpenFileMode = OpenFIleModes[1];
            SelectedOpenWindowMode = OpenWindowModes[0];
            FullPath = isNew ? path + Path.DirectorySeparatorChar + NewFileStandardPlaceHolder : path;
            FileModeChoiceVisibility = isNew ? Visibility.Collapsed : Visibility.Visible;
            DialogResult = false;
        }

        private Visibility fileModeChoiceVisibility;
        /// <summary>
        /// Visibility of the ComboBox representing the choice of the opening mode of the file
        /// Visible, when the project is opened
        /// Collapsed, when the project is created
        /// </summary>
        public Visibility FileModeChoiceVisibility
        {
            get { return fileModeChoiceVisibility; }
            set
            {
                fileModeChoiceVisibility = value;
                OnPropertyChanged("FileModeChoiceVisibility");
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
