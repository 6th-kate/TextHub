using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TextHub
{
    /// <summary>
    /// Interaction logic for ChooseVersionDialog.xaml
    /// </summary>
    public partial class ChooseVersionDialog : Window
    {
        internal ChooseVersionDialog(ObservableCollection<TextHubVersion> versions)
        {
            InitializeComponent();
            this.DataContext = new ChooseVersionViewModel(versions);
        }
    }
}
