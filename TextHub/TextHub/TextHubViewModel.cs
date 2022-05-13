using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Threading;

namespace TextHub
{
    /// <summary>
    /// The viewmodel for the main TextHub app window
    /// </summary>
    class TextHubViewModel : INotifyPropertyChanged
    {
        // Static values of width of documents and versions panels
        private const double sideBarsOpenedWidth = 250;
        private const double sideBarsClosedWidth = 45;
        /// <summary>
        /// The list of projects currently opened in the app
        /// </summary>
        public ObservableCollection<TextHubProject> TextHubProjects { get; set; }

        // Fields behind the window properties
        private TextHubProject selectedProject;
        private TextHubVersion selectedVersion;
        private DispatcherTimer autosaveTimer;

        /// <summary>
        /// Initialised a new instance of the model view, sets starting values to the window parameters, initializes commands
        /// </summary>
        public TextHubViewModel()
        {
            TextHubProjects = new ObservableCollection<TextHubProject>();
            SetStartValues();
            InitialiseCommands();
        }
        /// <summary>
        /// Sets starting values to the window parameters
        /// </summary>
        private void SetStartValues()
        {
            DocumentsMenuCloseButtonVisibility = System.Windows.Visibility.Visible;
            DocumentsMenuOpenButtonVisibility = System.Windows.Visibility.Collapsed;
            DocumentsMenuWidth = new System.Windows.GridLength(sideBarsOpenedWidth);
            VersionsMenuCloseButtonVisibility = System.Windows.Visibility.Visible;
            VersionsMenuOpenButtonVisibility = System.Windows.Visibility.Collapsed;
            VersionsMenuWidth = new System.Windows.GridLength(sideBarsOpenedWidth);
            DocumentsListVisibility = System.Windows.Visibility.Visible;
            VersionsListVisibility = System.Windows.Visibility.Visible;
            MainRTBVisibility = System.Windows.Visibility.Collapsed;
            DiffViewVisibility = System.Windows.Visibility.Collapsed;
            CloseComparisonButtonVisibility = System.Windows.Visibility.Collapsed;
            FileTabVisibility = System.Windows.Visibility.Visible;
            EditingTabVisibility = System.Windows.Visibility.Collapsed;
            ComparisonTabVisibility = System.Windows.Visibility.Collapsed;
            SaveButtonVisibility = System.Windows.Visibility.Collapsed;
            SaveAsNewVersionButtonVisibility = System.Windows.Visibility.Collapsed;
            MakeNewSubprojectButtonVisibility = System.Windows.Visibility.Collapsed;
            autosaveTimer = new DispatcherTimer(TimeSpan.FromSeconds(60), DispatcherPriority.Background,
                new EventHandler(Autosave), System.Windows.Application.Current.Dispatcher);
            autosaveTimer.Start();
        }

        /// <summary>
        /// Initializes this view model commands
        /// </summary>
        private void InitialiseCommands()
        {
            NewDocumentCommand = new TextHubCommands.NewDocumentCommand(this);
            OpenProjectCommand = new TextHubCommands.OpenProjectCommand(this);
            OpenDocumentCommand = new TextHubCommands.OpenDocumentCommand(this);
            SaveCommand = new TextHubCommands.SaveCommand(this);
            SaveNewVersionCommand = new TextHubCommands.SaveNewVersionCommand(this);
            MakeNewSubprojectCommand = new TextHubCommands.MakeNewSubprojectCommand(this);
            HighlightTextCommand = new TextHubCommands.HighlightTextCommand(this);
            ColorTextCommand = new TextHubCommands.ColorTextCommand(this);
            ChangeFontCommand = new TextHubCommands.ChangeFontCommand(this);
            InsertImageCommand = new TextHubCommands.InsertImageCommand();
            PasteImageCommand = new TextHubCommands.PasteImageCommand();
            CompareToPreviousCommand = new TextHubCommands.CompareToPreviousCommand(this);
            CompareToChosenVersionCommand = new TextHubCommands.CompareToChosenVersionCommand(this);
            CloseComparisonCommand = new TextHubCommands.CloseComparisonCommand(this);
        }

        // Commands
        public TextHubCommands.NewDocumentCommand NewDocumentCommand { get; set; }
        public TextHubCommands.OpenDocumentCommand OpenDocumentCommand { get; set; }
        public TextHubCommands.OpenProjectCommand OpenProjectCommand { get; set; }
        public TextHubCommands.SaveCommand SaveCommand { get; set; }
        public TextHubCommands.SaveNewVersionCommand SaveNewVersionCommand { get; set; }
        public TextHubCommands.MakeNewSubprojectCommand MakeNewSubprojectCommand { get; set; }
        public TextHubCommands.HighlightTextCommand HighlightTextCommand { get; set; }
        public TextHubCommands.ColorTextCommand ColorTextCommand { get; set; }
        public TextHubCommands.ChangeFontCommand ChangeFontCommand { get; set; }
        public TextHubCommands.InsertImageCommand InsertImageCommand { get; set; }
        public TextHubCommands.PasteImageCommand PasteImageCommand { get; set; }
        public TextHubCommands.CompareToPreviousCommand CompareToPreviousCommand { get; set; }
        public TextHubCommands.CompareToChosenVersionCommand CompareToChosenVersionCommand { get; set; }
        public TextHubCommands.CloseComparisonCommand CloseComparisonCommand { get; set; }

        /// <summary>
        /// If the project is opened, autosaves it one time in two minutes
        /// </summary>
        private void Autosave(object sender, EventArgs e)
        {
            if (selectedVersion != null && selectedProject != null && selectedVersion.Changeable)
            {
                selectedVersion.Project.Save(CurrentText);
            }
        }

        private TextHubCommands.SimpleCommand deleteCommand;
        /// <summary>
        /// Based on a SimpleCommand class, a command which deletes a project or a version from the side panels
        /// </summary>
        public TextHubCommands.SimpleCommand DeleteCommand
        {
            get
            {
                return deleteCommand ??
                  (deleteCommand = new TextHubCommands.SimpleCommand(obj =>
                  {
                      if (obj is TextHubVersion version)
                      {
                          try
                          {
                              if (SelectedProject.Versions.Count > 1)
                              {
                                  SelectedProject.DeleteVersion(version);
                              }
                              else
                              {
                                  MessageBox.Show("Это самая ранняя версия проекта, предыдущая не существует", 
                                      "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                              }
                          }
                          catch (Exception ex)
                          {
                              MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                          }
                      }
                      if (obj is TextHubProject project)
                      {
                          if (project.Equals(SelectedProject))
                          {
                              project.Save(CurrentText);
                          }
                          TextHubProjects.Remove(project);
                      }
                  }));
            }
        }

        /// <summary>
        /// The project version currently opened in the window. When changed, the window parameters adapt to the new version settings
        /// </summary>
        public TextHubVersion SelectedVersion
        {
            get { return selectedVersion; }
            set
            {
                FileTabVisibility = System.Windows.Visibility.Visible;
                if (selectedVersion != null && selectedProject != null && selectedVersion.Changeable)
                {
                    selectedVersion.Project.Save(CurrentText);
                }
                if (value != null)
                {
                    try
                    {
                        CurrentText = value.Project.GetText(value);
                        if (value.Changeable)
                        {
                            EditingBlocked = false;
                            SaveButtonVisibility = System.Windows.Visibility.Visible;
                            SaveAsNewVersionButtonVisibility = System.Windows.Visibility.Visible;
                            EditingTabVisibility = System.Windows.Visibility.Visible;
                        }
                        else
                        {
                            EditingBlocked = true;
                            SaveButtonVisibility = System.Windows.Visibility.Collapsed;
                            EditingTabVisibility = System.Windows.Visibility.Collapsed;
                            SaveAsNewVersionButtonVisibility = System.Windows.Visibility.Collapsed;
                        }
                        DiffViewVisibility = System.Windows.Visibility.Collapsed;
                        MainRTBVisibility = System.Windows.Visibility.Visible;
                        CloseComparisonButtonVisibility = System.Windows.Visibility.Collapsed;
                        CompareToPreceedingVersionButtonVisibility = System.Windows.Visibility.Visible;
                        ChooseVersionToCompareButtonVisibility = System.Windows.Visibility.Visible;
                        MakeNewSubprojectButtonVisibility = System.Windows.Visibility.Visible;
                        ComparisonTabVisibility = System.Windows.Visibility.Visible;
                    }
                    catch (Exception ex)
                    {
                        value.Project.DeleteVersion(value);
                        value = null;
                        MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                if (value == null)
                {
                    DiffViewVisibility = System.Windows.Visibility.Collapsed;
                    MainRTBVisibility = System.Windows.Visibility.Collapsed;
                    ComparisonTabVisibility = System.Windows.Visibility.Collapsed;
                    EditingTabVisibility = System.Windows.Visibility.Collapsed;
                    SaveButtonVisibility = System.Windows.Visibility.Collapsed;
                    MakeNewSubprojectButtonVisibility = System.Windows.Visibility.Collapsed;
                    SaveAsNewVersionButtonVisibility = System.Windows.Visibility.Collapsed;
                }
                selectedVersion = value;
                OnPropertyChanged("SelectedVersion");
            }
        }

        /// <summary>
        /// The project currently opened in the window. When changed, the newest version automatically opens
        /// </summary>
        public TextHubProject SelectedProject
        {
            get { return selectedProject; }
            set
            {
                if (value != null)
                {
                    if (value.Versions != null && value.Versions.Count >= 1)
                    {
                        SelectedVersion = value.Versions[value.Versions.Count - 1];
                    }
                    else
                    {
                        SelectedVersion = null;
                    }
                }
                selectedProject = value;
                OnPropertyChanged("SelectedProject");
            }
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

        private System.Windows.Visibility comparisonTabVisibility;
        /// <summary>
        /// The visibility of the Comparison tab
        /// </summary>
        public System.Windows.Visibility ComparisonTabVisibility
        {
            get { return comparisonTabVisibility; }
            set
            {
                comparisonTabVisibility = value;
                OnPropertyChanged("ComparisonTabVisibility");
            }
        }


        private System.Windows.Visibility compareToPreceedingVersionButtonVisibility;
        /// <summary>
        /// The visibility of the button of comparison to the previous version
        /// </summary>
        public System.Windows.Visibility CompareToPreceedingVersionButtonVisibility
        {
            get { return compareToPreceedingVersionButtonVisibility; }
            set
            {
                compareToPreceedingVersionButtonVisibility = value;
                OnPropertyChanged("CompareToPreceedingVersionButtonVisibility");
            }
        }


        private System.Windows.Visibility chooseVersionToCompareButtonVisibility;
        /// <summary>
        /// The visibility of the button of comparison to the chosen version
        /// </summary>
        public System.Windows.Visibility ChooseVersionToCompareButtonVisibility
        {
            get { return chooseVersionToCompareButtonVisibility; }
            set
            {
                chooseVersionToCompareButtonVisibility = value;
                OnPropertyChanged("ChooseVersionToCompareButtonVisibility");
            }
        }

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

        System.Windows.Visibility closeComparisonButtonVisibility;
        /// <summary>
        /// The visibility of the button of closing the opened comparison
        /// </summary>
        public System.Windows.Visibility CloseComparisonButtonVisibility
        {
            get { return closeComparisonButtonVisibility; }
            set
            {
                closeComparisonButtonVisibility = value;
                OnPropertyChanged("CloseComparisonButtonVisibility");
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
                      DocumentsMenuCloseButtonVisibility = System.Windows.Visibility.Collapsed;
                      DocumentsMenuOpenButtonVisibility = System.Windows.Visibility.Visible;
                      DocumentsMenuWidth = new System.Windows.GridLength(sideBarsClosedWidth);
                      DocumentsListVisibility = System.Windows.Visibility.Collapsed;
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
                      DocumentsMenuCloseButtonVisibility = System.Windows.Visibility.Visible;
                      DocumentsMenuOpenButtonVisibility = System.Windows.Visibility.Collapsed;
                      DocumentsMenuWidth = new System.Windows.GridLength(sideBarsOpenedWidth);
                      DocumentsListVisibility = System.Windows.Visibility.Visible;
                  }));
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
                      VersionsMenuCloseButtonVisibility = System.Windows.Visibility.Collapsed;
                      VersionsMenuOpenButtonVisibility = System.Windows.Visibility.Visible;
                      VersionsMenuWidth = new System.Windows.GridLength(sideBarsClosedWidth);
                      VersionsListVisibility = System.Windows.Visibility.Collapsed;
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
                      VersionsMenuCloseButtonVisibility = System.Windows.Visibility.Visible;
                      VersionsMenuOpenButtonVisibility = System.Windows.Visibility.Collapsed;
                      VersionsMenuWidth = new System.Windows.GridLength(sideBarsOpenedWidth);
                      VersionsListVisibility = System.Windows.Visibility.Visible;
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
