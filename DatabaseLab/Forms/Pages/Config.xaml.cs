using DatabaseLab.DataBase;
using DatabaseLab.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace DatabaseLab.Forms
{
    /// <summary>
    /// Interaction logic for Config.xaml
    /// </summary>
    public partial class Config : Page
    {
        private string TableName { get; set; }

        public Config(string tableName)
        {
            InitializeComponent();
            TableName = tableName;
        }

        private void label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Label lbl = (Label)sender;
            DragDrop.DoDragDrop(lbl, lbl.Content, DragDropEffects.Copy);
        }

        private void txtTarget_Drop(object sender, DragEventArgs e)
        {
            richTextBoxTypes.AppendText((string)e.Data.GetData(DataFormats.Text));
        }

        private void applyButton_Click(object sender, RoutedEventArgs e)
        {
            List<Types.Type> types = ParseTypes();
            List<string> headers = ParseHeaders();

            bool value = false;

            if (types != null && headers != null)
            {
                value = App.DB.CreateTable(TableName, types, headers);

                if (!value)
                {
                    MessageBox.Show("Attempting to save invalid config!", "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                    return;
                }
                App.mainWindow.frame.Content = App.mainPage;
            }
            else
            {
                Logger.Write("Config.xml: Attempting to save wrong types or headers", Logger.Level.Warn);
                MessageBox.Show("Attempting to save invalid config!", "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            }
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            TableName = string.Empty;
            App.mainWindow.frame.Content = App.mainPage;
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

        private List<Types.Type> ParseTypes()
        {
            try
            {
                string content = GetRichTextBoxContent(richTextBoxTypes);
                List<Types.Type> types = new List<Types.Type>();

                char[] separator = { '\r', '\n' };
                foreach (var item in content.Split(separator).ToList())
                {
                    switch (item)
                    {
                        case "Varchar":
                            types.Add(Types.Type.VARCHAR);
                            break;
                        case "Integer":
                            types.Add(Types.Type.INTEGER);
                            break;
                        case "Boolean":
                            types.Add(Types.Type.BOOLEAN);
                            break;
                        default:
                            break;
                    }
                }

                // add one more field for unique identifier,
                // which will be generated automatically
                types.Add(Types.Type.INTEGER);

                return types;
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                return null;
            }
        }

        private List<string> ParseHeaders()
        {
            try
            {
                string content = GetRichTextBoxContent(richTextBoxHeaders);
                List<string> headers = new List<string>();

                char[] separator = { '\r', '\n' };
                foreach (var item in content.Split(separator).ToList())
                {
                    if (!string.IsNullOrEmpty(item))
                        headers.Add(item);
                }

                // add unique identifier field name
                headers.Add("uid");

                return headers;
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                return null;
            }
        }
    }
}
