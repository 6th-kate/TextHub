using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;

namespace TextHub
{
    /// <summary>
    /// Model view for Choosing Name Dialog
    /// </summary>
    class ChooseNameViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// The chosen name from the TextBox
        /// </summary>
        public string NewName { get; set; }

        /// <summary>
        /// Basic placeholder for the Name
        /// </summary>
        public const string NamePlaceHolder = "Выберите имя";

        /// <summary>
        /// The result of the dialog (if OK is pressed, the true, false otherwise)
        /// </summary>
        public bool DialogResult { get; set; }

        /// <summary>
        /// Initialization of starting values
        /// </summary>
        public ChooseNameViewModel()
        {
            NewName = NamePlaceHolder;
            DialogResult = false;
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
