using CommonServiceLocator;
using HubDeburrSystem.Public;
using HubDeburrSystem.ViewModel;
using HubDeburrSystem.Views.Dialog;
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

namespace HubDeburrSystem.Views
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            if (new LoginView().ShowDialog() != true) Application.Current.Shutdown();
            InitializeComponent();

            //窗口级别的交互，注册，需要返回状态
            DialogManager.Register<object>("DialoAddWheelTypeWindow", new Func<object, bool>(ShowAddWheelTypeWindow));
            DialogManager.Register<object>("2DLocusParameterSettingsDialog", new Func<object, bool>(Show2DLocusParameterSettingsWindow));
            DialogManager.Register<object>("3DLocusParameterSettingsDialog", new Func<object, bool>(Show3DLocusParameterSettingsWindow));
            DialogManager.Register<object>("TemplateDataEditDialog", new Func<object, bool>(ShowTemplateDataEditDialog));
        }

        private bool ShowTemplateDataEditDialog(object arg)
        {
            return new TemplateDataEditDialog() { Owner = this }.ShowDialog() == true;
        }

        private bool Show3DLocusParameterSettingsWindow(object arg)
        {
            return new _3DLocusParameterSettingsDialog() { Owner = this }.ShowDialog() == true;
        }

        private bool Show2DLocusParameterSettingsWindow(object arg)
        {
            return new LocusParameterSettingsDialog() { Owner = this }.ShowDialog() == true;
        }

        private bool ShowAddWheelTypeWindow(object obj)
        {
            //使用Owner = this和主窗口建立联系
            return new AddWheelTypeDialog() { Owner = this }.ShowDialog() == true;
            //return ShowDialog(new AddWheelTypeDialog() { Owner = this });
        }
        private void WindowMin_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void WindowMax_Click(object sender, RoutedEventArgs e)
        {//&#xe65d;
            this.WindowState = this.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void WindowClose_Click(object sender, RoutedEventArgs e)
        {
            bool result = UMessageBox.Show("退出系统", "确认退出系统吗？");
            if (result)
            {
                ServiceLocator.Current.GetInstance<MonitorPageViewModel>().ToStop();
                ServiceLocator.Current.GetInstance<TemplatePageViewModel>().Dispose();
                ServiceLocator.Current.GetInstance<LocusPageViewModel>().Dispose();
                ServiceLocator.Current.GetInstance<MonitorPageViewModel>().Dispose();
                ServiceLocator.Current.GetInstance<ReportPageViewModel>().Dispose();
                this.Close();
                Environment.Exit(0);
            }
        }
    }
}
