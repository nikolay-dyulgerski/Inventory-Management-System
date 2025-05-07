using System;
using System.Windows;
using System.Windows.Controls;
using Senior_Project;

namespace Senior_Project_WPF
{
    public partial class RegistrationWindow : Page
    {
        public RegistrationWindow()
        {
            InitializeComponent();
        }

        private void RegisterButon_Click(object sender, RoutedEventArgs e)
        {
            string username = regUsernameTextBox.Text;
            string password = regPasswordBox.Password;
            string confirmPassword = confirmPasswordBox.Password;
            string role = ((ComboBoxItem)roleComboBox.SelectedItem)?.Content.ToString();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Username or Password cannot be empty", "Registration Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!password.Equals(confirmPassword))
            {
                MessageBox.Show("Passwords do not match", "Registration Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Senior_Project.User newUser = role switch
            {
                "Admin" => new Senior_Project.Admin(username, password),
                "Employee" => new Senior_Project.Employee(username, password),
                _ => null
            };


            if (newUser == null)
            {
                MessageBox.Show("Please select a valid role", "Registration Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            newUser.RegisterUser();
            MessageBox.Show("User registered successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            NavigationService.Navigate(new LoginPage());
        }
        private void BackButon_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
