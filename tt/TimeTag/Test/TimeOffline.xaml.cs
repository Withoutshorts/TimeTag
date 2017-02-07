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
    /// Interaction logic for TimeOffline.xaml
    /// </summary>
    public partial class TimeOffline : Window
    {
        public TimeOffline()
        {
            InitializeComponent();

            listBox1.ItemsSource = Properties.Settings.Default.TimeOffline;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.StatusLog.Clear();
            Properties.Settings.Default.Save();
            listBox1.Items.Refresh();
        }
    }
}
