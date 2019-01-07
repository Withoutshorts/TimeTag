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
            //tbxLto.Text = HelperSetting.UserInfo[0].Split(new[] {':'})[1];
            tbxLto.Text = "bf".ToString();
            if (HelperSetting.UserInfo.Length > 0)
            {
                tbxMid.Text = HelperSetting.UserInfo[1].Split(new[] { ':' })[1];
            }
            
            //tbxPa.Text = HelperSetting.UserInfo[2].Split(new[] { ':' })[1];
            tbxPa.Text = "2".ToString();
            //cbxNewDb.IsChecked = HelperSetting.UserInfo[3].Split(new[] { ':' })[1] == "new";
            cbxNewDb.IsChecked = true;
            //listBox1.ItemsSource = HelperSetting.UserInfo;

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
                HelperSetting.SaveUserInfo(HelperSetting.UserInfoPath, tbxLto.Text.Trim(), tbxMid.Text.Trim(), tbxPa.Text.Trim(), cbxNewDb.IsChecked == true);
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
            return !string.IsNullOrWhiteSpace(HelperSetting.UserInfo[0].Split(new[] {':'})[1]) &&
                   !string.IsNullOrWhiteSpace(HelperSetting.UserInfo[1].Split(new[] {':'})[1]) &&
                   !string.IsNullOrWhiteSpace(tbxPa.Text = HelperSetting.UserInfo[2].Split(new[] {':'})[1]);
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
