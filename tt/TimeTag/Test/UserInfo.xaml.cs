using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for UserInfo.xaml
    /// </summary>
    public partial class UserInfo : Window
    {
        public UserInfo()
        {
            InitializeComponent();
            //tbxLto.Text = Properties.Settings.Default.UserInfo[0].Split(new[] {':'})[1];
            tbxLto.Text = "bf".ToString();
            tbxMid.Text = Properties.Settings.Default.UserInfo[1].Split(new[] {':'})[1];
            //tbxPa.Text = Properties.Settings.Default.UserInfo[2].Split(new[] { ':' })[1];
            tbxPa.Text = "2".ToString();
            //cbxNewDb.IsChecked = Properties.Settings.Default.UserInfo[3].Split(new[] { ':' })[1] == "new";
            cbxNewDb.IsChecked = true;
            //listBox1.ItemsSource = Properties.Settings.Default.UserInfo;

            cbxNewDb.Content = FindResource("DBtxt").ToString();
            btnCancel.Content = FindResource("UserinfoCancel").ToString();
            btnSave.Content = FindResource("UserinfoSave").ToString();

            if (string.IsNullOrWhiteSpace(tbxLto.Text))
            {
                tbxLto.IsEnabled = true;
                cbxNewDb.IsEnabled = true;
            }

            if (string.IsNullOrWhiteSpace(tbxMid.Text))
            {
                tbxMid.IsEnabled = true;
                cbxNewDb.IsEnabled = true;
            }

            if (string.IsNullOrWhiteSpace(tbxPa.Text))
            {
                tbxPa.IsEnabled = true;
                cbxNewDb.IsEnabled = true;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateField())
            {
                Properties.Settings.Default.UserInfo.Clear();
                Properties.Settings.Default.UserInfo.Add("lto:" + tbxLto.Text);
                Properties.Settings.Default.UserInfo.Add("medid:" + tbxMid.Text);
                Properties.Settings.Default.UserInfo.Add("pa:" + tbxPa.Text);
                Properties.Settings.Default.UserInfo.Add("db:" + (cbxNewDb.IsChecked == true ? "new" : ""));
                Properties.Settings.Default.Save();
                Close();
            }
        }

        bool ValidateField()
        {
            bool result = true;
            tbxLto.Background = Brushes.White;
            tbxMid.Background = Brushes.White;
            tbxPa.Background = Brushes.White;
            if (string.IsNullOrWhiteSpace(tbxLto.Text))
            {
                tbxLto.Background = Brushes.Tomato;
                result = false;
            }
            if (string.IsNullOrWhiteSpace(tbxMid.Text))
            {
                tbxMid.Background = Brushes.Tomato;
                result = false;
            }
            if (string.IsNullOrWhiteSpace(tbxPa.Text))
            {
                tbxPa.Background = Brushes.Tomato;
                result = false;
            }
            return result;
        }

        private bool IsConfigurationSaved()
        {
            return !string.IsNullOrWhiteSpace(Properties.Settings.Default.UserInfo[0].Split(new[] {':'})[1]) &&
                   !string.IsNullOrWhiteSpace(Properties.Settings.Default.UserInfo[1].Split(new[] {':'})[1]) &&
                   !string.IsNullOrWhiteSpace(tbxPa.Text = Properties.Settings.Default.UserInfo[2].Split(new[] {':'})[1]);
        }


        private void UserInfo_OnClosing(object sender, CancelEventArgs e)
        {
            if (!IsConfigurationSaved())
            {
                MessageBoxResult messageBoxResult =
                    MessageBox.Show(
                        "Numéro de licence invalide. Voulez-vous fermer TimeTag?",
                        "TimeTag", System.Windows.MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    Application.Current.Shutdown();
                    e.Cancel = false;
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        private void TbxLto_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            ((TextBox) sender).Background = Brushes.White;
        }
    }
}
