using HubDeburrSystem.DataAccess;
using HubDeburrSystem.Models;
using HubDeburrSystem.Views.Dialog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace HubDeburrSystem
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            bool _createNew;
            Mutex mutex = new Mutex(true, "HubDeburrSystem", out _createNew);
            {
                if(!_createNew)
                {
                    UMessageBox.Show("程序已运行！", MessageType.Default);
                    App.Current.Shutdown();
                    return;
                }
                InitializeComponent();
            }


        }
    }
}
