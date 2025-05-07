using System;
using System.Windows;
using System.Windows.Controls;
using Senior_Project;

namespace Senior_Project_WPF
{
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void LoginButon_Click(object sender, RoutedEventArgs e)
        {
            string username = usernameTextBox.Text;
            string password = passwordBox.Password;
            User loggedinUser = User.LoginUser(username, password);

            if (loggedinUser != null)
            {
                SessionManager.CurrentUser = username;
                LogActivity.WriteLog("User logged in: " + username);

                switch (loggedinUser.GetTargetView())
                {
                    case "Admin":
                        NavigationService.Navigate(new AdminMenu());
                        break;
                    case "Employee":
                        NavigationService.Navigate(new EmployeeMenu());
                        break;
                    default:
                        MessageBox.Show("Unknown role.");
                        break;
                }
            }
            else
            {
                MessageBox.Show("Invalid login credentials.");
            }

        }
        private void BackButon_Click(Object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MainChoice());
        }
    }
}
