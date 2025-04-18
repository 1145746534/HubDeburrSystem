using HubDeburrSystem.DataAccess;
using HubDeburrSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;

namespace HubDeburrSystem.Views
{
    /// <summary>
    /// LoginView.xaml 的交互逻辑
    /// </summary>
    public partial class LoginView : Window
    {
        SqlAccess sqlAccess = new SqlAccess();
        public LoginView()
        {
            InitializeComponent();
            sqlAccess.InitializeTable();
            this.Closed += LoginView_Closed;

            //系统存储目录
            string deburrSystemPath = @"D:\DeburrSystem";
            string templateImagePath = @"D:\DeburrSystem\TemplateImages";
            string activeTemplatePath = @"D:\DeburrSystem\ActiveTemplates";
            string notActiveTemplatePath = @"D:\DeburrSystem\NotActiveTemplates";
            string grabImagesPath = @"D:\DeburrSystem\GrabImages";
            string historicalImages = @"D:\DeburrSystem\HistoricalImages";
            if (Directory.Exists(deburrSystemPath) == false) Directory.CreateDirectory(deburrSystemPath);
            if (Directory.Exists(templateImagePath) == false) Directory.CreateDirectory(templateImagePath);
            if (Directory.Exists(activeTemplatePath) == false) Directory.CreateDirectory(activeTemplatePath);
            if (Directory.Exists(notActiveTemplatePath) == false) Directory.CreateDirectory(notActiveTemplatePath);
            if (Directory.Exists(grabImagesPath) == false) Directory.CreateDirectory(grabImagesPath);
            if (Directory.Exists(historicalImages) == false) Directory.CreateDirectory(historicalImages);
        }

        private void LoginView_Closed(object sender, EventArgs e)
        {
            ViewModelLocator.Cleanup<LoginViewModel>();
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
