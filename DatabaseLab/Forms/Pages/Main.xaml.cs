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

namespace DatabaseLab.Forms
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class Main : Page
    {
        public Main()
        {
            InitializeComponent();
        }

        private void createTableButton_Click(object sender, RoutedEventArgs e)
        {
            App.mainWindow.frame.Content = new Config(textBox.Text);
        }

        private void deleteTableButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(string.Format("Are you sure that you want to delete table {0}", textBox.Text), "Confirmation",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                App.DB.DeleteTable(textBox.Text);
            }
        }

        private void addDataButton_Click(object sender, RoutedEventArgs e)
        {
            this.Content = new Add();
        }

        private void deleteDataButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void editDataButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void importButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void backupButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void restoreButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
