using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DatabaseLab.DataBase;
using DatabaseLab.Forms;

namespace DatabaseLab
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static Database DB = new Database();
        public static Main mainPage = new Main();
        public static MainWindow mainWindow = new MainWindow();

        void App_Startup(object sender, StartupEventArgs e)
        {
            mainWindow.Show();
        }


    }
}
