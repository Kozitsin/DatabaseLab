using DatabaseLab.DataBase;
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
    /// Interaction logic for Search.xaml
    /// </summary>
    public partial class Search : Page
    {
        private string TableName { get; set; }

        public Search(string tableName)
        {
            InitializeComponent();
            TableName = tableName;
        }

        private void stringButton_Click(object sender, RoutedEventArgs e)
        {
            List<Record> records = App.DB.Search(TableName, textBox.Text);

            if (records != null && records.Count > 0)
            {
                ShowRawData(records);
            }
            else
            {
                Logger.Write("Nothing is found!", Logger.Level.Warn);
                MessageBox.Show("Nothing is found!", "Status", MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }

        private void uidButton_Click(object sender, RoutedEventArgs e)
        {
            int position = App.DB.Search(TableName, Convert.ToInt32(textBox.Text));

            if (position == -1)
            {
                Logger.Write("Nothing is found!", Logger.Level.Warn);
                MessageBox.Show("Nothing is found!", "Status", MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            else
            {
                Record record = App.DB.GetRecord(TableName, position);

                if (record != null)
                {
                    ShowRawData(new List<Record>() { record });
                }
                else
                {
                    Logger.Write("Nothing is found!", Logger.Level.Warn);
                    MessageBox.Show("Nothing is found!", "Status", MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
        }

        private void ShowRawData(List<Record> records)
        {
            for (int i = 0; i < records.Count; i++)
            {
                for (int j = 0; j < records[i].data.Count; j++)
                {
                    richTextBox.AppendText(records[i].data[j].ToString().TrimEnd(' '));
                    richTextBox.AppendText("\t");
                }
                richTextBox.AppendText(Environment.NewLine);
            }
        }
    }
}
