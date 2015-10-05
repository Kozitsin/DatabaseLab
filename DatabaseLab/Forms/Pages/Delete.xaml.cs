using DatabaseLab.Logging;
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
    /// Interaction logic for Delete.xaml
    /// </summary>
    public partial class Delete : Page
    {
        private string TableName { get; set; }


        public Delete(string tableName)
        {
            InitializeComponent();
            TableName = tableName;
        }

        private void stringButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool value = App.DB.DeleteRecord(TableName, stringTextBox.Text);

                if (value)
                {
                    MessageBox.Show("Deleting proceed successfully", "Status", MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Error occured!", "Status", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                MessageBox.Show("Error occured!", "Status", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}
