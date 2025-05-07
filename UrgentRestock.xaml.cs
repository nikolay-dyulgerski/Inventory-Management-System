    using Senior_Project;
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

    namespace Senior_Project_WPF
    {
        public partial class UrgentRestock : Page
        {
            public UrgentRestock()
            {
                InitializeComponent();
                LoadUrgentProducts();
            }
            private void LoadUrgentProducts()
            {
            RestockManager.RebuildUrgentQ();
                var urgentList = RestockManager.GetUrgentProducts();
                
                if(urgentList.Count == 0)
                {
                    MessageBox.Show("All products are in stock! No urgent restocking needed","Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                restockListView.ItemsSource = urgentList;
            }
            private void BackButton_Click(object sender, RoutedEventArgs e)
            {
                NavigationService.GoBack();
            }
        }
    }
