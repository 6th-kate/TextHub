using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using System.Windows.Media;

namespace TextHub
{
    internal interface IMessageService
    {
        void ShowInformation(string message, string subMessage);
        void ShowWarning(string message);
        void ShowError(string message);
        string OpenFolder(string title, bool allowNonFileSystemItems, bool multiselect);
        string OpenFile(string filter, string title, bool allowNonFileSystemItems, bool multiselect);
        bool ShowColorDialog(out Color selectedColor);
        bool ShowFontDialog(out string fontFamily, out double fontSize, out bool isBold, out bool isItalic);
        bool ShowOpeningDialog(string selectedFolder, object parameter, bool isFile);
        bool ShowNewNameDialog(object parameter);
        bool ShowChooseVersionDialog(object parameter, TextHubProject project);
        string OpeningDialogFullPath { get; }
        string SelectedOpenWindowMode { get; }
        string SelectedOpenFileMode { get; }
        string NewName { get; }
        string SelectedFormat { get; }
        TextHubVersion NewVersion { get; }
    }

    internal class MessageService : IMessageService
    {

        private string _openingDialogFullPath;
        private string _selectedOpenWindowMode;
        private string _selectedOpenFileMode;
        private string _selectedFormat;
        private string _newName;
        private TextHubVersion _newVersion;

        public string OpeningDialogFullPath => _openingDialogFullPath;
        public string SelectedOpenWindowMode => _selectedOpenWindowMode;
        public string SelectedOpenFileMode => _selectedOpenFileMode;
        public string SelectedFormat => _selectedFormat;
        public string NewName => _newName;
        TextHubVersion IMessageService.NewVersion => _newVersion;

        public bool ShowOpeningDialog(string selectedFolder, object parameter, bool isFile)
        {
            OpeningDialog openingDialog = new OpeningDialog(selectedFolder, !isFile)
            {
                Owner = (System.Windows.Window)parameter
            };
            openingDialog.ShowDialog();
            _openingDialogFullPath = ((OpeningDialogViewModel)openingDialog.DataContext).FullPath;
            _selectedOpenWindowMode = ((OpeningDialogViewModel)openingDialog.DataContext).SelectedOpenWindowMode;
            if (isFile)
            {
                _selectedOpenFileMode = ((OpeningDialogViewModel)openingDialog.DataContext).SelectedOpenFileMode;
            }
            else
            {
                _selectedFormat = ((OpeningDialogViewModel)openingDialog.DataContext).SelectedFormat;
            }
            return ((OpeningDialogViewModel)openingDialog.DataContext).DialogResult;
        }
        public bool ShowNewNameDialog(object parameter)
        {
            ChooseNameDialog dialog = new ChooseNameDialog
            {
                Owner = (MainWindow)parameter
            };
            dialog.ShowDialog();
            _newName = ((ChooseNameViewModel)dialog.DataContext).NewName;
            return ((ChooseNameViewModel)dialog.DataContext).DialogResult;
        }
        bool IMessageService.ShowChooseVersionDialog(object parameter, TextHubProject project)
        {
            ChooseVersionDialog dialog = new ChooseVersionDialog(project.Versions)
            {
                Owner = (MainWindow)parameter
            };
            dialog.ShowDialog();
            _newVersion = ((ChooseVersionViewModel)dialog.DataContext).SelectedVersion;
            return ((ChooseNameViewModel)dialog.DataContext).DialogResult;
        }
        public void ShowInformation(string message, string subMessage)
        {
            MessageBox.Show(message, subMessage, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void ShowWarning(string message)
        {
            MessageBox.Show(message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public string OpenFolder(string title, bool allowNonFileSystemItems, bool multiselect)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Multiselect = multiselect,
                AllowNonFileSystemItems = allowNonFileSystemItems,
                Title = title
            };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                return dialog.FileName;
            }
            return null;
        }

        public string OpenFile(string filter, string title, bool allowNonFileSystemItems, bool multiselect)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = false,
                Multiselect = multiselect,
                AllowNonFileSystemItems = allowNonFileSystemItems,
                Filters = { new CommonFileDialogFilter(filter.Split('|')[0], filter.Split('|')[1]) },
                Title = title
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                return dialog.FileName;
            }

            return null;
        }

        public bool ShowColorDialog(out Color selectedColor)
        {
            var dialog = new ColorDialog();
            var result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                selectedColor = Color.FromArgb(dialog.Color.A, dialog.Color.R, dialog.Color.G, dialog.Color.B);
                return true;
            }
            else
            {
                selectedColor = Colors.Transparent;
                return false;
            }
        }

        public bool ShowFontDialog(out string fontFamily, out double fontSize, out bool isBold, out bool isItalic)
        {
            FontDialog dialog = new FontDialog
            {
                ShowColor = false,
                ShowEffects = false
            };
            bool? result = (dialog.ShowDialog() == DialogResult.OK);
            if (result == true)
            {
                fontFamily = dialog.Font.Name;
                fontSize = Convert.ToDouble(dialog.Font.Size);
                isBold = dialog.Font.Bold;
                isItalic = dialog.Font.Italic;
                return true;
            }
            fontFamily = null;
            fontSize = 0;
            isBold = false;
            isItalic = false;
            return false;
        }
    }
}
