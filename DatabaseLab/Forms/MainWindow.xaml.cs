using System.Windows;

namespace DatabaseLab
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            App.DB.CreateDatabaseFolder();
            frame.Content = App.mainPage;
        }
    }
}
