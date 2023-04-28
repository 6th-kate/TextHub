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
        }

        /// <summary>
        /// Sets starting values to the window parameters
        /// </summary>
        private void SetStartValues()
        {
            mainRTBHelper = new MainRTBHelper();
            editingTabHelper = new EditingTabHelper(this);
            comparisonTabHelper = new ComparisonTabHelper(this);
            fileTabHelper = new FileTabHelper(this);
            documentsMenuHelper = new DocumentsMenuHelper(sideBarsOpenedWidth, sideBarsClosedWidth);
            versionsMenuHelper = new VersionsMenuHelper(sideBarsOpenedWidth, sideBarsClosedWidth);

            autosaveTimer = new DispatcherTimer(TimeSpan.FromSeconds(60), DispatcherPriority.Background,
                new EventHandler(Autosave), System.Windows.Application.Current.Dispatcher);
            autosaveTimer.Start();
        }

        /// <summary>
        /// If the project is opened, autosaves it one time in two minutes
        /// </summary>
        private void Autosave(object sender, EventArgs e)
        {
            if (selectedVersion != null && selectedProject != null && selectedVersion.Changeable)
            {
                selectedVersion.Project.Save(MainRTBHelper.CurrentText);
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
                              project.Save(MainRTBHelper.CurrentText);
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
                FileTabHelper.FileTabVisibility = System.Windows.Visibility.Visible;
                if (selectedVersion != null && selectedProject != null && selectedVersion.Changeable)
                {
                    selectedVersion.Project.Save(MainRTBHelper.CurrentText);
                }
                if (value != null)
                {
                    try
                    {
                        MainRTBHelper.CurrentText = value.Project.GetText(value);
                        if (value.Changeable)
                        {
                            MainRTBHelper.EditingBlocked = false;
                            FileTabHelper.SaveButtonVisibility = System.Windows.Visibility.Visible;
                            FileTabHelper.SaveAsNewVersionButtonVisibility = System.Windows.Visibility.Visible;
                            EditingTabHelper.EditingTabVisibility = System.Windows.Visibility.Visible;
                        }
                        else
                        {
                            MainRTBHelper.EditingBlocked = true;
                            FileTabHelper.SaveButtonVisibility = System.Windows.Visibility.Collapsed;
                            EditingTabHelper.EditingTabVisibility = System.Windows.Visibility.Collapsed;
                            FileTabHelper.SaveAsNewVersionButtonVisibility = System.Windows.Visibility.Collapsed;
                        }
                        MainRTBHelper.DiffViewVisibility = System.Windows.Visibility.Collapsed;
                        MainRTBHelper.MainRTBVisibility = System.Windows.Visibility.Visible;
                        ComparisonTabHelper.CloseComparisonButtonVisibility = System.Windows.Visibility.Collapsed;
                        ComparisonTabHelper.CompareToPreceedingVersionButtonVisibility = System.Windows.Visibility.Visible;
                        ComparisonTabHelper.ChooseVersionToCompareButtonVisibility = System.Windows.Visibility.Visible;
                        FileTabHelper.MakeNewSubprojectButtonVisibility = System.Windows.Visibility.Visible;
                        ComparisonTabHelper.ComparisonTabVisibility = System.Windows.Visibility.Visible;
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
                    MainRTBHelper.DiffViewVisibility = System.Windows.Visibility.Collapsed;
                    MainRTBHelper.MainRTBVisibility = System.Windows.Visibility.Collapsed;
                    ComparisonTabHelper.ComparisonTabVisibility = System.Windows.Visibility.Collapsed;
                    EditingTabHelper.EditingTabVisibility = System.Windows.Visibility.Collapsed;
                    FileTabHelper.SaveButtonVisibility = System.Windows.Visibility.Collapsed;
                    FileTabHelper.MakeNewSubprojectButtonVisibility = System.Windows.Visibility.Collapsed;
                    FileTabHelper.SaveAsNewVersionButtonVisibility = System.Windows.Visibility.Collapsed;
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

        private MainRTBHelper mainRTBHelper;

        public MainRTBHelper MainRTBHelper
        {
            get { return mainRTBHelper; }
            set
            {
                mainRTBHelper.Update(value);
            }
        }

        private EditingTabHelper editingTabHelper;

        public EditingTabHelper EditingTabHelper
        {
            get { return editingTabHelper; }
            set
            {
                editingTabHelper.Update(value);
            }
        }

        private ComparisonTabHelper comparisonTabHelper;

        public ComparisonTabHelper ComparisonTabHelper
        {
            get { return comparisonTabHelper; }
            set
            {
                comparisonTabHelper.Update(value);
            }
        }

        private FileTabHelper fileTabHelper;

        public FileTabHelper FileTabHelper
        {
            get { return fileTabHelper; }
            set
            {
                fileTabHelper.Update(value);
            }
        }

        private DocumentsMenuHelper documentsMenuHelper;

        public DocumentsMenuHelper DocumentsMenuHelper
        {
            get { return documentsMenuHelper; }
            set
            {
                documentsMenuHelper.Update(value);
            }
        }

        private VersionsMenuHelper versionsMenuHelper;

        public VersionsMenuHelper VersionsMenuHelper
        {
            get { return versionsMenuHelper; }
            set
            {
                versionsMenuHelper.Update(value);
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
