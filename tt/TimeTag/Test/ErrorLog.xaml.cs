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
            this.BindData();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            HelperSetting.ClearLog(HelperSetting.ErrorLogPath);
            this.BindData();
        }

        private void BindData()
        {
            lstErrorLog.ItemsSource = HelperSetting.ReadSettings(HelperSetting.ErrorLogPath);
        }
    }
}
