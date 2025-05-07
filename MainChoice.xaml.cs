using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Senior_Project;

namespace Senior_Project_WPF
{
    /// <summary>
    /// Interaction logic for MainChoice.xaml
    /// </summary>
    public partial class MainChoice : Page
    {
        public MainChoice()
        {
            InitializeComponent();
        }
        private void LoginButon_Click(object sender, RoutedEventArgs e)
        { 
            NavigationService.Navigate(new LoginPage());
        }
        private void RegisterButon_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegistrationWindow());
        }   
    }
}
