using CommonServiceLocator;
using GalaSoft.MvvmLight.Messaging;
using HalconDotNet;
using HubDeburrSystem.ViewModel;
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

namespace HubDeburrSystem.Views.Pages
{
    /// <summary>
    /// MonitorPage.xaml 的交互逻辑
    /// </summary>
    public partial class MonitorPage : UserControl
    {
        public MonitorPage()
        {
            InitializeComponent();
            Loaded += UserControl_Loaded;
            // 订阅滚动请求消息
            Messenger.Default.Register<NotificationMessage>(this, (message) =>
            {
                if (message.Notification == "ScrollToEnd")
                {
                    // 滚动到ScrollViewer的底部
                    MessagesScrollViewer.ScrollToEnd();
                }
            });
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ServiceLocator.Current.GetInstance<MonitorPageViewModel>().ProcessingResultDisplayed = Visibility.Hidden;
            MonitorHWindow.HalconWindow.SetWindowParam("graphics_stack_max_element_num", 150);
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            ServiceLocator.Current.GetInstance<MonitorPageViewModel>().ProcessingResultDisplayed = Visibility.Hidden;
        }
    }
}
