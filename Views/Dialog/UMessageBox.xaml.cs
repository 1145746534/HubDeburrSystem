using HelixToolkit.Wpf;
using HubDeburrSystem.Models;
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
using System.Windows.Shapes;

namespace HubDeburrSystem.Views.Dialog
{
    /// <summary>
    /// MessageBoxDialog.xaml 的交互逻辑
    /// </summary>
    public partial class UMessageBox : Window
    {
        public UMessageBox()
        {
            InitializeComponent();
        }

        public new string Title
        {
            get { return this.MessageBoxTitle.Text; }   
            set { this.MessageBoxTitle.Text = value;}
        }

        public string Message
        {
            get { return this.Messages.Text; }
            set { this.Messages.Text = value; }
        }

        public Brush BackgroundColor
        {
            get { return this.TitleBackground.Background; }
            set { this.TitleBackground.Background = value; }
        }

        public Brush BottomColor
        {
            get { return this.BottomBackground.Background; }
            set { this.BottomBackground.Background = value; }
        }

        public Visibility CancelButtonVisibility
        {
            get { return this.Cancel_btn.Visibility; }
            set { this.Cancel_btn.Visibility = value; }
        }

        public static bool Result = false;
        /// <summary>
        ///模拟Message.Show方法
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="msg">消息</param>
        /// <returns></returns>
        public static bool Show(string title, string msg)
        {
            var msgBox = new UMessageBox();
            msgBox.Title = title;
            Uri uri = new Uri("pack://application:,,,/HubDeburrSystem;component/Assets/Images/警告.png", UriKind.RelativeOrAbsolute);
            msgBox.IconImage.Source = new BitmapImage(uri);
            msgBox.BackgroundColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#88E86214"));
            msgBox.BottomColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#88E86214"));
            msgBox.Message = msg;
            msgBox.ShowDialog();
            return Result;
        }

        public static bool Show(string msg, MessageType type)
        {
            var msgBox = new UMessageBox();
            msgBox.CancelButtonVisibility = Visibility.Collapsed;
            if (type == MessageType.Default)
            {
                msgBox.Title = "提  示";
                Uri uri = new Uri("pack://application:,,,/HubDeburrSystem;component/Assets/Images/提示.png", UriKind.RelativeOrAbsolute);
                msgBox.IconImage.Source = new BitmapImage(uri);
                msgBox.BackgroundColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFDDE3FB"));
                msgBox.BottomColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFDDE3FB"));
            }
            else if (type == MessageType.Warning)
            {
                msgBox.Title = "警  告";
                Uri uri = new Uri("pack://application:,,,/HubDeburrSystem;component/Assets/Images/警告.png", UriKind.RelativeOrAbsolute);
                msgBox.IconImage.Source = new BitmapImage(uri);
                msgBox.BackgroundColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF3F5C8"));
                msgBox.BottomColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF3F5C8"));
            }
            else if (type == MessageType.Success)
            {
                msgBox.Title = "成  功";
                Uri uri = new Uri("pack://application:,,,/HubDeburrSystem;component/Assets/Images/成功.png", UriKind.RelativeOrAbsolute);
                msgBox.IconImage.Source = new BitmapImage(uri);
                msgBox.BackgroundColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFAADEB7"));
                msgBox.BottomColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFAADEB7"));
            }
            else if (type == MessageType.Error)
            {
                msgBox.Title = "错  误";
                Uri uri = new Uri("pack://application:,,,/HubDeburrSystem;component/Assets/Images/错误.png", UriKind.RelativeOrAbsolute);
                msgBox.IconImage.Source = new BitmapImage(uri); ;
                msgBox.BackgroundColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF78883"));
                msgBox.BottomColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF78883"));
            }
            msgBox.Message = msg;
            msgBox.ShowDialog();
            return Result;
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void Cancel_btn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            Result = false;
        }

        private void Confirm_btn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            Result = true;
        }
    }
}
