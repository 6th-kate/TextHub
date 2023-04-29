using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TextHub
{
    class ComparisonTabHelper : INotifyPropertyChanged
    {
        public ComparisonTabHelper(TextHubViewModel viewModel, IMessageService messageService)
        {
            ComparisonTabVisibility = System.Windows.Visibility.Collapsed;
            CloseComparisonButtonVisibility = System.Windows.Visibility.Collapsed;
            CompareToPreviousCommand = new TextHubCommands.CompareToPreviousCommand(viewModel, messageService);
            CompareToChosenVersionCommand = new TextHubCommands.CompareToChosenVersionCommand(viewModel, messageService);
            CloseComparisonCommand = new TextHubCommands.CloseComparisonCommand(viewModel);
        }

        public TextHubCommands.CompareToPreviousCommand CompareToPreviousCommand { get; set; }
        public TextHubCommands.CompareToChosenVersionCommand CompareToChosenVersionCommand { get; set; }
        public TextHubCommands.CloseComparisonCommand CloseComparisonCommand { get; set; }

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


        public void Update(ComparisonTabHelper other)
        {
            this.ComparisonTabVisibility = other.ComparisonTabVisibility;
            this.CompareToPreceedingVersionButtonVisibility = other.CompareToPreceedingVersionButtonVisibility;
            this.ChooseVersionToCompareButtonVisibility = other.ChooseVersionToCompareButtonVisibility;
            this.CloseComparisonButtonVisibility = other.CloseComparisonButtonVisibility;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}