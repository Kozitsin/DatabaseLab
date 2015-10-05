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
    /// Interaction logic for Edit.xaml
    /// </summary>
    public partial class Edit : Page
    {
        private string TableName { get; set; }


        public Edit(string tableName)
        {
            InitializeComponent();
            TableName = tableName;
        }

        private void applyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Record record = CreateRecordFromRawData();

                if (record != null)
                {
                    App.DB.Update(TableName, Convert.ToInt32(textBox.Text), record);

                    MessageBox.Show("Editing finished", "Status", MessageBoxButton.OK,
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

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            App.mainWindow.frame.Content = App.mainPage;
        }

        private Record CreateRecordFromRawData()
        {
            try
            {
                List<Types.Type> types = App.DB.GetTypesOfRecord(TableName);

                Record record = new Record(types);

                char[] separator = { '\r', '\n' };
                string[] s = GetRichTextBoxContent(richTextBox).Split(separator)
                    .Where(str => !String.IsNullOrEmpty(str))
                    .ToArray();

                for (int i = 0; i < s.Count() - 1; i++)
                {
                    switch (types[i])
                    {
                        case Types.Type.BOOLEAN:
                            record.data[i] = Convert.ToBoolean(s[i]);
                            break;

                        case Types.Type.INTEGER:
                            record.data[i] = Convert.ToInt32(s[i]);
                            break;

                        case Types.Type.VARCHAR:
                            record.data[i] = s[i];
                            break;

                        default:
                            break;
                    }
                }
                return record;
            }

            catch (Exception ex)
            {
                Logger.Write(ex);
                return null;
            }
        }

        private string GetRichTextBoxContent(RichTextBox richTextBox)
        {
            // Create a FlowDocument to contain content for the RichTextBox.
            FlowDocument myFlowDoc = new FlowDocument();

            TextRange textRange = new TextRange(
                richTextBox.Document.ContentStart,
                richTextBox.Document.ContentEnd
            );
            return textRange.Text;
        }
    }
}
