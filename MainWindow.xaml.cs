using System.Collections.Generic;
using System.Security.Principal;
using System.Windows;
using System.Windows.Controls;

namespace PasswordManager
{
    public partial class MainWindow : Window
    {
        private VaultService vaultService = new VaultService();
        private List<Account> accounts = new List<Account>();
        private string masterPassword;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            masterPassword = PasswordBox.Password;

            var data = vaultService.Load(masterPassword);

            if (data == null)
            {
                MessageBox.Show("Неверный пароль!");
                return;
            }

            accounts = data;
            RefreshList();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var acc = new Account
            {
                Site = "example.com",
                Login = "user",
                Password = "1234"
            };

            accounts.Add(acc);
            vaultService.Save(accounts, masterPassword);
            RefreshList();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (ListBox.SelectedItem is Account selected)
            {
                accounts.Remove(selected);
                vaultService.Save(accounts, masterPassword);
                RefreshList();
            }
        }

        private void RefreshList()
        {
            ListBox.ItemsSource = null;
            ListBox.ItemsSource = accounts;
        }
    }
}