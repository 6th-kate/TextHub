using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.IO;
using System.Text;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;

namespace TextHub
{
    /// <summary>
    /// The main commands class of the TextHub app
    /// </summary>
    public class TextHubCommands
    {
        /// <summary>
        /// The basic class specified for short commands
        /// </summary>
        public class SimpleCommand : ICommand
        {
            // Resettable basic commands delegates
            private Action<object> execute;
            private Func<object, bool> canExecute;

            // Reset of basic canExecute delegate
            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }

            /// <summary>
            /// Initialises a new instance of SimpleCommand
            /// </summary>
            /// <param name="execute">The action to execute when the command is called</param>
            /// <param name="canExecute">The check is the command can be executed</param>
            public SimpleCommand(Action<object> execute, Func<object, bool> canExecute = null)
            {
                this.execute = execute;
                this.canExecute = canExecute;
            }

            /// <summary>
            /// Indicates if the command can be executed or not
            /// </summary>
            /// <param name="parameter">Command parameter</param>
            /// <returns>True, if the command can be executed, false otherwise</returns>
            public bool CanExecute(object parameter)
            {
                return canExecute == null || canExecute(parameter);
            }

            /// <summary>
            /// Executes the command
            /// </summary>
            /// <param name="parameter">Command parameter</param>
            public void Execute(object parameter)
            {
                execute(parameter);
            }
        }

        /// <summary>
        /// The class of command responsible for creating a new document
        /// </summary>
        public class NewDocumentCommand : ICommand
        {
            private readonly TextHubViewModel textHubViewModel;
            private readonly IMessageService messageService;

            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }

            internal NewDocumentCommand(TextHubViewModel viewModel, IMessageService messageService)
            {
                textHubViewModel = viewModel;
                this.messageService = messageService;
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                string selectedFolder = messageService.OpenFolder("Выберите папку, в которой хотите создать проект", true, false);
                if (selectedFolder == null)
                {
                    messageService.ShowInformation("Папка не была выбрана", "Проект не создан");
                    return;
                }
                if (messageService.ShowOpeningDialog(selectedFolder, parameter, false))
                {
                    try
                    {
                        TextHubProject project = TextHubProject.MakeNewProject(messageService.OpeningDialogFullPath);
                        if (messageService.SelectedOpenWindowMode == "Новое окно")
                        {
                            MainWindow newWindow = new MainWindow();
                            ((TextHubViewModel)newWindow.DataContext).TextHubProjects.Add(project);
                            newWindow.Show();
                        }
                        else
                        {
                            textHubViewModel.TextHubProjects.Add(project);
                        }
                    }
                    catch (Exception ex)
                    {
                        messageService.ShowError(ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// The class of command responsible for opening a text document
        /// </summary>
        public class OpenDocumentCommand : ICommand
        {
            // The viewModel, in which the command is called
            private readonly TextHubViewModel textHubViewModel;
            private readonly IMessageService messageService;

            // Reset of basic canExecute delegate
            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }

            /// <summary>
            /// Initialises a new instance of OpenDocumentCommand
            /// </summary>
            /// <param name="viewModel">The viewModel, in which the command is called</param>
            internal OpenDocumentCommand(TextHubViewModel viewModel, IMessageService messageService)
            {
                textHubViewModel = viewModel;
                this.messageService = messageService;
            }

            /// <summary>
            /// Indicates if the command can be executed or not
            /// </summary>
            /// <param name="parameter">Command parameter</param>
            /// <returns>True, if the command can be executed, false otherwise</returns>
            public bool CanExecute(object parameter)
            {
                return true;
            }

            /// <summary>
            /// Implements the logic of the opening of an .rtf document
            /// </summary>
            /// <param name="parameter">The main window</param>
            public void Execute(object parameter)
            {
                // Dialog to choose file
                string selectedFile = messageService.OpenFile("Rich text format files (*.rtf)|*.rtf", "Выберите документ, который хотите открыть в качестве проекта", false, false);
                if (selectedFile == null)
                {
                    messageService.ShowInformation("Документ не был выбран", "Проект не создан");
                    //MessageBox.Show("Документ не был выбран", "Проект не создан", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (messageService.ShowOpeningDialog(selectedFile, parameter, true))
                {
                    try
                    {
                        // Opens the document
                        TextHubProject project = TextHubProject.ParseFile(messageService.OpeningDialogFullPath);
                        if (messageService.SelectedOpenFileMode == "Просмотр")
                        {
                            project.Versions[project.Versions.Count - 1].Changeable = false;
                        }
                        if (messageService.SelectedOpenWindowMode == "Новое окно")
                        {
                            MainWindow newWindow = new MainWindow();
                            ((TextHubViewModel)newWindow.DataContext).TextHubProjects.Add(project);
                            newWindow.Show();
                        }
                        else
                        {
                            textHubViewModel.TextHubProjects.Add(project);
                        }
                    }
                    catch (Exception ex)
                    {
                        messageService.ShowError(ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// The class of command responsible for opening a project
        /// </summary>
        public class OpenProjectCommand : ICommand
        {
            // The viewModel, in which the command is called
            private readonly TextHubViewModel textHubViewModel;
            private readonly IMessageService messageService;

            // Reset of basic canExecute delegate
            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }

            /// <summary>
            /// Initialises a new instance of OpenProjectCommand
            /// </summary>
            /// <param name="viewModel">The viewModel, in which the command is called</param>
            internal OpenProjectCommand(TextHubViewModel viewModel, IMessageService messageService)
            {
                textHubViewModel = viewModel;
                this.messageService = messageService;
            }

            /// <summary>
            /// Indicates if the command can be executed or not
            /// </summary>
            /// <param name="parameter">Command parameter</param>
            /// <returns>True, if the command can be executed, false otherwise</returns>
            public bool CanExecute(object parameter)
            {
                return true;
            }

            /// <summary>
            /// Implements the logic of the opening of an existing project
            /// </summary>
            /// <param name="parameter">The main window</param>
            public void Execute(object parameter)
            {

                string selectedFolder = messageService.OpenFolder("Выберите проект, который хотите открыть", false, false);
                if (selectedFolder == null)
                {
                    messageService.ShowInformation("Проект не был выбран", "Проект не открыт");
                    return;
                }
                /*OpeningDialog openingDialog = new OpeningDialog(selectedFolder)
                {
                    Owner = (System.Windows.Window)parameter
                };
                // The dialog to confirm the choice and settings
                openingDialog.ShowDialog();
                if (((OpeningDialogViewModel)openingDialog.DataContext).DialogResult)
                {
                    try
                    {
                        // Opens the project
                        TextHubProject project = TextHubProject.ParseProject(((OpeningDialogViewModel)openingDialog.DataContext).FullPath);
                        if (((OpeningDialogViewModel)openingDialog.DataContext).SelectedOpenFileMode == "Просмотр")
                        {
                            project.Versions[project.Versions.Count - 1].Changeable = false;
                        }
                        if (((OpeningDialogViewModel)openingDialog.DataContext).SelectedOpenWindowMode == "Новое окно")
                        {
                            MainWindow newWindow = new MainWindow();
                            ((TextHubViewModel)newWindow.DataContext).TextHubProjects.Add(project);
                            newWindow.Show();
                        }
                        else
                        {
                            textHubViewModel.TextHubProjects.Add(project);
                        }
                    }
                    catch (Exception ex)
                    {
                        messageService.ShowError(ex.Message);
                    }
                }*/
                if (messageService.ShowOpeningDialog(selectedFolder, parameter, true))
                {
                    try
                    {
                        // Opens the project
                        TextHubProject project = TextHubProject.ParseProject(messageService.OpeningDialogFullPath);
                        if (messageService.SelectedOpenFileMode == "Просмотр")
                        {
                            project.Versions[project.Versions.Count - 1].Changeable = false;
                        }
                        if (messageService.SelectedOpenWindowMode == "Новое окно")
                        {
                            MainWindow newWindow = new MainWindow();
                            ((TextHubViewModel)newWindow.DataContext).TextHubProjects.Add(project);
                            newWindow.Show();
                        }
                        else
                        {
                            textHubViewModel.TextHubProjects.Add(project);
                        }
                    }
                    catch (Exception ex)
                    {
                        messageService.ShowError(ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// The class of command responsible for saving a project
        /// </summary>
        public class SaveCommand : ICommand
        {
            // The viewModel, in which the command is called
            private readonly TextHubViewModel textHubViewModel;

            // Reset of basic canExecute delegate
            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }

            /// <summary>
            /// Initialises a new instance of SaveCommand
            /// </summary>
            /// <param name="viewModel">The viewModel, in which the command is called</param>
            internal SaveCommand(TextHubViewModel viewModel)
            {
                textHubViewModel = viewModel;
            }

            /// <summary>
            /// Indicates if the command can be executed or not
            /// </summary>
            /// <param name="parameter">Command parameter</param>
            /// <returns>True, if the command can be executed, false otherwise</returns>
            public bool CanExecute(object parameter)
            {
                return true;
            }

            /// <summary>
            /// Implements the logic of the save of a project
            /// </summary>
            /// <param name="parameter">No parameter required</param>
            public void Execute(object parameter)
            {
                if (textHubViewModel.SelectedVersion != null)
                {
                    textHubViewModel.SelectedVersion.Project.Save(textHubViewModel.CurrentText);
                }
            }
        }

        /// <summary>
        /// The class of command responsible for saving a new version of a project
        /// </summary>
        public class SaveNewVersionCommand : ICommand
        {
            // The viewModel, in which the command is called
            private readonly TextHubViewModel textHubViewModel;
            private readonly IMessageService messageService;

            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }

            /// <summary>
            /// Initialises a new instance of SaveNewVersionCommand
            /// </summary>
            /// <param name="viewModel">The viewModel, in which the command is called</param>
            internal SaveNewVersionCommand(TextHubViewModel viewModel, IMessageService messageService)
            {
                textHubViewModel = viewModel;
                this.messageService = messageService;
            }

            /// <summary>
            /// Indicates if the command can be executed or not
            /// </summary>
            /// <param name="parameter">Command parameter</param>
            /// <returns>True, if the command can be executed, false otherwise</returns>
            public bool CanExecute(object parameter)
            {
                return true;
            }

            /// <summary>
            /// Implements the logic of the creation of a new version of the project
            /// </summary>
            /// <param name="parameter">The main window</param>
            public void Execute(object parameter)
            {
                if (textHubViewModel.SelectedVersion != null)
                {
                    // A dialog to choose the name for the version
                    /*ChooseNameDialog dialog = new ChooseNameDialog
                    {
                        Owner = (MainWindow)parameter
                    };
                    dialog.ShowDialog();
                    // Creates the version
                    if (((ChooseNameViewModel)dialog.DataContext).DialogResult)
                    {
                        try
                        {
                            if (textHubViewModel.SelectedVersion.Changeable)
                            {
                                textHubViewModel.SelectedVersion.Project.Save(textHubViewModel.CurrentText);
                            }
                            textHubViewModel.SelectedVersion.Project.SaveNewVersion(((ChooseNameViewModel)dialog.DataContext).NewName);
                        }
                        catch (Exception ex)
                        {
                            messageService.ShowError(ex.Message);
                        }
                    }*/
                    // Creates the version
                    if (messageService.ShowNewNameDialog(parameter))
                    {
                        try
                        {
                            if (textHubViewModel.SelectedVersion.Changeable)
                            {
                                textHubViewModel.SelectedVersion.Project.Save(textHubViewModel.CurrentText);
                            }
                            textHubViewModel.SelectedVersion.Project.SaveNewVersion(messageService.NewName);
                        }
                        catch (Exception ex)
                        {
                            messageService.ShowError(ex.Message);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The class of command responsible for creating a new subproject of a project
        /// </summary>
        public class MakeNewSubprojectCommand : ICommand
        {
            // The viewModel, in which the command is called
            private readonly TextHubViewModel textHubViewModel;
            private readonly IMessageService messageService;

            // Reset of basic canExecute delegate
            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }

            /// <summary>
            /// Initialises a new instance of MakeNewSubprojectCommand
            /// </summary>
            /// <param name="viewModel">The viewModel, in which the command is called</param>
            internal MakeNewSubprojectCommand(TextHubViewModel viewModel, IMessageService messageService)
            {
                textHubViewModel = viewModel;
                this.messageService = messageService;
            }

            /// <summary>
            /// Indicates if the command can be executed or not
            /// </summary>
            /// <param name="parameter">Command parameter</param>
            /// <returns>True, if the command can be executed, false otherwise</returns>
            public bool CanExecute(object parameter)
            {
                return true;
            }

            /// <summary>
            /// Implements the logic of the creation of a new subproject
            /// </summary>
            /// <param name="parameter">The main window</param>
            public void Execute(object parameter)
            {
                if (textHubViewModel.SelectedVersion != null)
                {
                    /*// Dialog to choose a name for the subproject
                    ChooseNameDialog dialog = new ChooseNameDialog
                    {
                        Owner = (MainWindow)parameter
                    };
                    dialog.ShowDialog();
                    if (((ChooseNameViewModel)dialog.DataContext).DialogResult)
                    {
                        // Creates the subproject
                        try
                        {
                            textHubViewModel.TextHubProjects.Add(textHubViewModel.SelectedVersion.Project.MakeSubproject(textHubViewModel.SelectedVersion,
                                ((ChooseNameViewModel)dialog.DataContext).NewName));
                        }
                        catch (Exception ex)
                        {
                            //MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            messageService.ShowError(ex.Message);
                        }
                    }*/
                    if (messageService.ShowNewNameDialog(parameter))
                    {
                        // Creates the subproject
                        try
                        {
                            textHubViewModel.TextHubProjects.Add(textHubViewModel.SelectedVersion.Project.MakeSubproject(textHubViewModel.SelectedVersion,
                                messageService.NewName));
                        }
                        catch (Exception ex)
                        {
                            //MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            messageService.ShowError(ex.Message);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The class of command responsible for highlighting the text of the document
        /// </summary>
        public class HighlightTextCommand : ICommand
        {
            // The viewModel, in which the command is called
            private readonly TextHubViewModel textHubViewModel;
            private readonly IMessageService messageService;

            // Reset of basic canExecute delegate
            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }

            /// <summary>
            /// Initialises a new instance of HighlightTextCommand
            /// </summary>
            /// <param name="viewModel">The viewModel, in which the command is called</param>
            internal HighlightTextCommand(TextHubViewModel viewModel, IMessageService messageService)
            {
                textHubViewModel = viewModel;
                this.messageService = messageService;
            }

            /// <summary>
            /// Indicates if the command can be executed or not
            /// </summary>
            /// <param name="parameter">Command parameter</param>
            /// <returns>True, if the command can be executed, false otherwise</returns>
            public bool CanExecute(object parameter)
            {
                return true;
            }

            /// <summary>
            /// Implements the logic of highlighting the selection of the text
            /// </summary>
            /// <param name="parameter">The main RichTextBox</param>
            public void Execute(object parameter)
            {
                if (textHubViewModel.SelectedVersion != null)
                {

                    if (messageService.ShowColorDialog(out Color selectedColor) && parameter is System.Windows.Controls.RichTextBox box)
                    {
                        box.Selection.ApplyPropertyValue(TextElement.BackgroundProperty,
                            new SolidColorBrush(selectedColor));
                    }
                    /*ColorDialog dialog = new ColorDialog();
                    if (dialog.ShowDialog() == DialogResult.OK && parameter is System.Windows.Controls.RichTextBox box)
                    {
                        box.Selection.ApplyPropertyValue(TextElement.BackgroundProperty,
                           new SolidColorBrush(Color.FromArgb(dialog.Color.A, dialog.Color.R, dialog.Color.G, dialog.Color.B)));
                    }*/
                }
            }
        }

        /// <summary>
        /// The class of command responsible for coloring the text of the document
        /// </summary>
        public class ColorTextCommand : ICommand
        {
            // The viewModel, in which the command is called
            private readonly TextHubViewModel textHubViewModel;
            private readonly IMessageService messageService;

            // Reset of basic canExecute delegate
            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }

            /// <summary>
            /// Initialises a new instance of ColorTextCommand
            /// </summary>
            /// <param name="viewModel">The viewModel, in which the command is called</param>
            internal ColorTextCommand(TextHubViewModel viewModel, IMessageService messageService)
            {
                textHubViewModel = viewModel;
                this.messageService = messageService;
            }

            /// <summary>
            /// Indicates if the command can be executed or not
            /// </summary>
            /// <param name="parameter">Command parameter</param>
            /// <returns>True, if the command can be executed, false otherwise</returns>
            public bool CanExecute(object parameter)
            {
                return true;
            }
            /// <summary>
            /// Implements the logic of coloring the selection of the text
            /// </summary>
            /// <param name="parameter">The main RichTextBox</param>
            public void Execute(object parameter)
            {
                if (textHubViewModel.SelectedVersion != null)
                {

                    if (messageService.ShowColorDialog(out Color selectedColor) && parameter is System.Windows.Controls.RichTextBox box)
                    {
                        box.Selection.ApplyPropertyValue(TextElement.ForegroundProperty,
                            new SolidColorBrush(selectedColor));
                    }
                    /*ColorDialog dialog = new ColorDialog();
                    if (dialog.ShowDialog() == DialogResult.OK && parameter is System.Windows.Controls.RichTextBox box)
                    {
                        box.Selection.ApplyPropertyValue(TextElement.ForegroundProperty,
                            new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(dialog.Color.A, dialog.Color.R, dialog.Color.G, dialog.Color.B)));
                    }*/
                }
            }
        }

        /// <summary>
        /// The class of command responsible for changing the font of the text of the document
        /// </summary>
        public class ChangeFontCommand : ICommand
        {
            // The viewModel, in which the command is called
            private readonly TextHubViewModel textHubViewModel;
            private readonly IMessageService messageService;

            // Reset of basic canExecute delegate
            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }

            /// <summary>
            /// Initialises a new instance of ChangeFontCommand
            /// </summary>
            /// <param name="viewModel">The viewModel, in which the command is called</param>
            internal ChangeFontCommand(TextHubViewModel viewModel, IMessageService messageService)
            {
                textHubViewModel = viewModel;
                this.messageService = messageService;
            }

            /// <summary>
            /// Indicates if the command can be executed or not
            /// </summary>
            /// <param name="parameter">Command parameter</param>
            /// <returns>True, if the command can be executed, false otherwise</returns>
            public bool CanExecute(object parameter)
            {
                return true;
            }
            /// <summary>
            /// Implements the logic of the font change the selection of the text
            /// </summary>
            /// <param name="parameter">The main RichTextBox</param>
            public void Execute(object parameter)
            {
                if (textHubViewModel.SelectedVersion != null)
                {
                    if (messageService.ShowFontDialog(out string fontFamily, out double fontSize, out bool isBold, out bool isItalic) && parameter is System.Windows.Controls.RichTextBox box)
                    {
                        box.Selection.ApplyPropertyValue(TextElement.FontFamilyProperty, new System.Windows.Media.FontFamily(fontFamily));
                        box.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, fontSize);
                        if (isBold)
                        {
                            box.Selection.ApplyPropertyValue(TextElement.FontWeightProperty, System.Windows.FontWeights.Bold);
                        }
                        if (isItalic)
                        {
                            box.Selection.ApplyPropertyValue(TextElement.FontStyleProperty, System.Windows.FontStyles.Italic);
                        }
                    }

                    /*FontDialog dialog = new FontDialog
                    {
                        ShowColor = false,
                        ShowEffects = false
                    };
                    if (dialog.ShowDialog() == DialogResult.OK && parameter is System.Windows.Controls.RichTextBox box)
                    {
                        box.Selection.ApplyPropertyValue(TextElement.FontFamilyProperty, new System.Windows.Media.FontFamily(dialog.Font.Name));
                        box.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, Convert.ToDouble(dialog.Font.Size));
                        if (dialog.Font.Bold)
                        {
                            box.Selection.ApplyPropertyValue(TextElement.FontWeightProperty, System.Windows.FontWeights.Bold);
                        }
                        if (dialog.Font.Italic)
                        {
                            box.Selection.ApplyPropertyValue(TextElement.FontStyleProperty, System.Windows.FontStyles.Italic);
                        }
                    }*/
                }
            }
        }

        /// <summary>
        /// The class of command responsible for inserting an image from the memory of the computer to the text of the document
        /// </summary>
        public class InsertImageCommand : ICommand
        {
            private readonly IMessageService messageService;

            // Reset of basic canExecute delegate
            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }

            internal InsertImageCommand(IMessageService messageService)
            {
                this.messageService = messageService;
            }

            /// <summary>
            /// Indicates if the command can be executed or not
            /// </summary>
            /// <param name="parameter">Command parameter</param>
            /// <returns>True, if the command can be executed, false otherwise</returns>
            public bool CanExecute(object parameter)
            {
                return true;
            }
            /// <summary>
            /// Implements the logic of inserting an image from computer memory to the document
            /// </summary>
            /// <param name="parameter">The main RichTextBox</param>
            public void Execute(object parameter)
            {
                if (parameter is System.Windows.Controls.RichTextBox box)
                {
                    string selectedFile = messageService.OpenFile("Image files|*.jpeg,*.jpg,*.png,*.bmp", "Выберите изображение", true, false);
                    if (selectedFile != null)
                    {
                        Clipboard.SetDataObject(new System.Drawing.Bitmap(selectedFile));
                        box.Paste();
                    }
                    /*CommonOpenFileDialog dialog = new CommonOpenFileDialog
                    {
                        IsFolderPicker = false,
                        Multiselect = false
                    };
                    dialog.Filters.Add(new CommonFileDialogFilter("Image files", "*.jpeg,*.jpg,*.png,*.bmp"));
                    if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                    {
                        Clipboard.SetDataObject(new System.Drawing.Bitmap(dialog.FileName));
                        box.Paste();
                    }*/
                }
            }
        }

        /// <summary>
        /// The class of command responsible for pasting images to the text of the document
        /// </summary>
        public class PasteImageCommand : ICommand
        {
            // Reset of basic canExecute delegate
            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }

            /// <summary>
            /// Indicates if the command can be executed or not
            /// </summary>
            /// <param name="parameter">Command parameter</param>
            /// <returns>True, if the command can be executed, false otherwise</returns>
            public bool CanExecute(object parameter)
            {
                return true;
            }

            /// <summary>
            /// Implements the logic of pasting an image to the document
            /// </summary>
            /// <param name="parameter">The main RichTextBox</param>
            public void Execute(object parameter)
            {
                if (parameter is System.Windows.Controls.RichTextBox box)
                {
                    var data = System.Windows.Clipboard.GetDataObject();
                    if (data.GetDataPresent(DataFormats.Bitmap))
                    {
                        box.Paste();
                    }
                }
            }
        }

        /// <summary>
        /// The class of command responsible for comparing the text of the document to its previous version
        /// </summary>
        public class CompareToPreviousCommand : ICommand
        {
            // The viewModel, in which the command is called
            private readonly TextHubViewModel textHubViewModel;
            private readonly IMessageService messageService;

            // Reset of basic canExecute delegate
            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }

            /// <summary>
            /// Initialises a new instance of CompareToPreviousCommand
            /// </summary>
            /// <param name="viewModel">The viewModel, in which the command is called</param>
            internal CompareToPreviousCommand(TextHubViewModel viewModel, IMessageService messageService)
            {
                textHubViewModel = viewModel;
                this.messageService = messageService;
            }

            /// <summary>
            /// Indicates if the command can be executed or not
            /// </summary>
            /// <param name="parameter">Command parameter</param>
            /// <returns>True, if the command can be executed, false otherwise</returns>
            public bool CanExecute(object parameter)
            {
                return true;
            }

            /// <summary>
            /// Implements the logic of the comparing a version to its previous one
            /// </summary>
            /// <param name="parameter">No parameter requiredx</param>
            public void Execute(object parameter)
            {
                if (textHubViewModel.SelectedVersion != null && textHubViewModel.SelectedProject != null && textHubViewModel.SelectedVersion.Changeable)
                {
                    textHubViewModel.SelectedVersion.Project.Save(textHubViewModel.CurrentText);
                }
                if (textHubViewModel.SelectedVersion.Project.Versions.IndexOf(textHubViewModel.SelectedVersion) == 0)
                {
                    //MessageBox.Show("Это самая ранняя версия", "Сравнение невозможно", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    messageService.ShowInformation("Это самая ранняя версия", "Сравнение невозможно");
                }
                else
                {
                    try
                    {
                        FlowDocument newText = new FlowDocument();
                        TextRange newTextRange = new TextRange(newText.ContentStart, newText.ContentEnd);
                        using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(textHubViewModel.SelectedVersion.Project.GetText(textHubViewModel.SelectedVersion))))
                        {
                            newTextRange.Load(stream, DataFormats.Rtf);
                        }
                        textHubViewModel.NewText = newTextRange.Text;
                        FlowDocument oldText = new FlowDocument();
                        TextRange oldTextRange = new TextRange(oldText.ContentStart, oldText.ContentEnd);
                        using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(textHubViewModel.SelectedVersion.Project.GetText(
                            textHubViewModel.SelectedVersion.Project.Versions[textHubViewModel.SelectedVersion.Project.Versions.IndexOf(textHubViewModel.SelectedVersion) - 1]))))
                        {
                            oldTextRange.Load(stream, DataFormats.Rtf);
                        }
                        textHubViewModel.NewText = newTextRange.Text;
                        textHubViewModel.OldText = oldTextRange.Text;
                        textHubViewModel.NewTextHeader = textHubViewModel.SelectedVersion.Title;
                        textHubViewModel.OldTextHeader =
                            textHubViewModel.SelectedVersion.Project.Versions[textHubViewModel.SelectedVersion.Project.Versions.IndexOf(textHubViewModel.SelectedVersion) - 1].Title;

                        textHubViewModel.EditingTabVisibility = System.Windows.Visibility.Collapsed;
                        textHubViewModel.FileTabVisibility = System.Windows.Visibility.Collapsed;

                        textHubViewModel.MainRTBVisibility = System.Windows.Visibility.Collapsed;
                        textHubViewModel.DiffViewVisibility = System.Windows.Visibility.Visible;

                        textHubViewModel.CloseComparisonButtonVisibility = System.Windows.Visibility.Visible;
                        textHubViewModel.CompareToPreceedingVersionButtonVisibility = System.Windows.Visibility.Collapsed;
                        textHubViewModel.ChooseVersionToCompareButtonVisibility = System.Windows.Visibility.Collapsed;
                    }
                    catch (Exception ex)
                    {
                        //MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        messageService.ShowError(ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// The class of command responsible for comparing the text of the document to a chosen version
        /// </summary>
        public class CompareToChosenVersionCommand : ICommand
        {
            // The viewModel, in which the command is called
            private readonly TextHubViewModel textHubViewModel;
            private readonly IMessageService messageService;

            // Reset of basic canExecute delegate
            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }

            /// <summary>
            /// Initialises a new instance of CompareToChosenVersionCommand
            /// </summary>
            /// <param name="viewModel">The viewModel, in which the command is called</param>
            internal CompareToChosenVersionCommand(TextHubViewModel viewModel, IMessageService messageService)
            {
                textHubViewModel = viewModel;
                this.messageService = messageService;
            }

            /// <summary>
            /// Indicates if the command can be executed or not
            /// </summary>
            /// <param name="parameter">Command parameter</param>
            /// <returns>True, if the command can be executed, false otherwise</returns>
            public bool CanExecute(object parameter) { return true; }

            /// <summary>
            /// Implements the logic of the comparing a version to another one
            /// </summary>
            /// <param name="parameter">The main windowx</param>
            public void Execute(object parameter)
            {
                if (textHubViewModel.SelectedVersion != null && textHubViewModel.SelectedProject != null && textHubViewModel.SelectedVersion.Changeable)
                {
                    textHubViewModel.SelectedVersion.Project.Save(textHubViewModel.CurrentText);
                }
                if (textHubViewModel.SelectedVersion.Project.Versions.Count < 2)
                {
                    //MessageBox.Show("Это последняя версия", "Сравнение невозможно", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    messageService.ShowInformation("Это самая ранняя версия", "Сравнение невозможно");
                }
                else
                {
                    /*ChooseVersionDialog dialog = new ChooseVersionDialog(textHubViewModel.SelectedVersion.Project.Versions)
                    {
                        Owner = (MainWindow)parameter
                    };
                    dialog.ShowDialog();
                    if (((ChooseVersionViewModel)dialog.DataContext).DialogResult)
                    {
                        try
                        {
                            FlowDocument newText = new FlowDocument();
                            TextRange newTextRange = new TextRange(newText.ContentStart, newText.ContentEnd);
                            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(textHubViewModel.SelectedVersion.Project.GetText(textHubViewModel.SelectedVersion))))
                            {
                                newTextRange.Load(stream, DataFormats.Rtf);
                            }
                            textHubViewModel.NewText = newTextRange.Text;
                            FlowDocument oldText = new FlowDocument();
                            TextRange oldTextRange = new TextRange(oldText.ContentStart, oldText.ContentEnd);
                            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(textHubViewModel.SelectedVersion.Project.GetText(((ChooseVersionViewModel)dialog.DataContext).SelectedVersion))))
                            {
                                oldTextRange.Load(stream, DataFormats.Rtf);
                            }
                            textHubViewModel.NewText = newTextRange.Text;
                            textHubViewModel.OldText = oldTextRange.Text;
                            textHubViewModel.NewTextHeader = textHubViewModel.SelectedVersion.Title;
                            textHubViewModel.OldTextHeader = ((ChooseVersionViewModel)dialog.DataContext).SelectedVersion.Title;

                            textHubViewModel.EditingTabVisibility = System.Windows.Visibility.Collapsed;
                            textHubViewModel.FileTabVisibility = System.Windows.Visibility.Collapsed;

                            textHubViewModel.MainRTBVisibility = System.Windows.Visibility.Collapsed;
                            textHubViewModel.DiffViewVisibility = System.Windows.Visibility.Visible;

                            textHubViewModel.CloseComparisonButtonVisibility = System.Windows.Visibility.Visible;
                            textHubViewModel.CompareToPreceedingVersionButtonVisibility = System.Windows.Visibility.Collapsed;
                            textHubViewModel.ChooseVersionToCompareButtonVisibility = System.Windows.Visibility.Collapsed;
                        }
                        catch (Exception ex)
                        {
                            //MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            messageService.ShowError(ex.Message);
                        }
                    }*/
                    if (messageService.ShowChooseVersionDialog(parameter, textHubViewModel.SelectedVersion.Project))
                    {
                        try
                        {
                            FlowDocument newText = new FlowDocument();
                            TextRange newTextRange = new TextRange(newText.ContentStart, newText.ContentEnd);
                            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(textHubViewModel.SelectedVersion.Project.GetText(textHubViewModel.SelectedVersion))))
                            {
                                newTextRange.Load(stream, DataFormats.Rtf);
                            }
                            textHubViewModel.NewText = newTextRange.Text;
                            FlowDocument oldText = new FlowDocument();
                            TextRange oldTextRange = new TextRange(oldText.ContentStart, oldText.ContentEnd);
                            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(textHubViewModel.SelectedVersion.Project.GetText(messageService.NewVersion))))
                            {
                                oldTextRange.Load(stream, DataFormats.Rtf);
                            }
                            textHubViewModel.NewText = newTextRange.Text;
                            textHubViewModel.OldText = oldTextRange.Text;
                            textHubViewModel.NewTextHeader = textHubViewModel.SelectedVersion.Title;
                            textHubViewModel.OldTextHeader = messageService.NewVersion.Title;

                            textHubViewModel.EditingTabVisibility = System.Windows.Visibility.Collapsed;
                            textHubViewModel.FileTabVisibility = System.Windows.Visibility.Collapsed;

                            textHubViewModel.MainRTBVisibility = System.Windows.Visibility.Collapsed;
                            textHubViewModel.DiffViewVisibility = System.Windows.Visibility.Visible;

                            textHubViewModel.CloseComparisonButtonVisibility = System.Windows.Visibility.Visible;
                            textHubViewModel.CompareToPreceedingVersionButtonVisibility = System.Windows.Visibility.Collapsed;
                            textHubViewModel.ChooseVersionToCompareButtonVisibility = System.Windows.Visibility.Collapsed;
                        }
                        catch (Exception ex)
                        {
                            //MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            messageService.ShowError(ex.Message);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The class of command responsible for closing the comparison
        /// </summary>
        public class CloseComparisonCommand : ICommand
        {
            // The viewModel, in which the command is called
            private readonly TextHubViewModel textHubViewModel;

            // Reset of basic canExecute delegate
            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }

            /// <summary>
            /// Initialises a new instance of CloseComparisonCommand
            /// </summary>
            /// <param name="viewModel">The viewModel, in which the command is called</param>
            internal CloseComparisonCommand(TextHubViewModel viewModel)
            {
                textHubViewModel = viewModel;
            }

            /// <summary>
            /// Indicates if the command can be executed or not
            /// </summary>
            /// <param name="parameter">Command parameter</param>
            /// <returns>True, if the command can be executed, false otherwise</returns>
            public bool CanExecute(object parameter)
            {
                return true;
            }
            /// <summary>
            /// Implements the logic of closing the comparison.
            /// </summary>
            /// <param name="parameter">No parameter requiredx</param>
            public void Execute(object parameter)
            {
                textHubViewModel.SelectedVersion = textHubViewModel.SelectedVersion;
            }
        }
    }
}
