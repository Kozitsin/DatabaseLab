using System.Windows;
using System.Windows.Controls;

namespace DatabaseLab.Forms
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class Main : Page
    {
        #region Constructor

        public Main()
        {
            InitializeComponent();
        }

        #endregion

        #region Private Methods

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
            App.mainWindow.frame.Content = new Add(textBox.Text);
        }

        private void deleteDataButton_Click(object sender, RoutedEventArgs e)
        {
            App.mainWindow.frame.Content = new Delete(textBox.Text);
        }

        private void editDataButton_Click(object sender, RoutedEventArgs e)
        {
            App.mainWindow.frame.Content = new Edit(textBox.Text);
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            App.mainWindow.frame.Content = new Search(textBox.Text);
        }

        private void importButton_Click(object sender, RoutedEventArgs e)
        {
            bool value = App.DB.Import(textBox.Text);

            if (value)
            {
                MessageBox.Show("Csv-file was successfully created!", "Status",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Error occured", "Status",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void backupButton_Click(object sender, RoutedEventArgs e)
        {
            bool value = App.DB.BackUp(textBox.Text);

            if (value)
            {
                MessageBox.Show("Back-up file was successfully created!", "Status",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Error occured", "Status",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void restoreButton_Click(object sender, RoutedEventArgs e)
        {
            bool value = App.DB.BackUp(textBox.Text);

            if (value)
            {
                MessageBox.Show("Back-up file was successfully restored!", "Status",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Error occured", "Status",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion
    }
}
