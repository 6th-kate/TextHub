using System;
using System.Collections.Generic;
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
    /// Interaction logic for OpeningDialog.xaml
    /// </summary>
    public partial class OpeningDialog : Window
    {
        public OpeningDialog(string path, bool isNew = false)
        {
            InitializeComponent();
            DataContext = new OpeningDialogViewModel(path, isNew);
        }
    }
}
