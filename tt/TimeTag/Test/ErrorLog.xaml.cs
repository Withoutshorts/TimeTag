using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TimeTag
{
    /// <summary>
    /// Interaction logic for ErrorLog.xaml
    /// </summary>
    public partial class ErrorLog : Window
    {
        public ErrorLog()
        {
            InitializeComponent();

            listBox1.ItemsSource = Properties.Settings.Default.ErrorLog;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.ErrorLog.Clear();
            Properties.Settings.Default.Save();
            listBox1.Items.Refresh();
        }
    }
}
