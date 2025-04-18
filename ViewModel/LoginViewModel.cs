using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HubDeburrSystem.DataAccess;
using HubDeburrSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HubDeburrSystem.ViewModel
{
    public class LoginViewModel:ViewModelBase
    {
        /// <summary>
        /// 用户
        /// </summary>
        public UserModel User { get; set; }
        /// <summary>
        /// 登录命令
        /// </summary>
        public RelayCommand<object> LoginCommand { get; set; }

        public string _failedMsg;
        /// <summary>
        /// 错误信息
        /// </summary>
        public string FailedMsg
        {
            get { return _failedMsg; }
            set { Set(ref _failedMsg, value); }
        }

        ILocalDataAccess _localDataAccess;
        public LoginViewModel(ILocalDataAccess localDataAccess)
        {
            _localDataAccess = localDataAccess;
            if (!IsInDesignMode)
            {
                User = new UserModel();
                LoginCommand = new RelayCommand<object>(DoLogin);
            }
        }

        private void DoLogin(object obj)
        {
            try
            {
                // 对接数据库
                UserModel data = _localDataAccess.Login(User.UserName, User.Password);
                if (data == null) throw new Exception("登录失败，没有用户信息!");
                //将登录用户信息记录到主窗口MainViewModel实例中，对于SimpleIOC,main与MainView中的DataContext拿到的是同一个实例,默认是单例
                var main = ServiceLocator.Current.GetInstance<MonitorPageViewModel>();
                if (main != null)
                {
                    main.GlobalUserInfo.UserName = data.UserName;
                    main.GlobalUserInfo.Password = data.Password;
                    main.GlobalUserInfo.UserType = data.UserType;
                }
                //登录成功打开主窗口
                (obj as Window).DialogResult = true;
            }
            catch (Exception ex)
            {
                FailedMsg = ex.Message;
            }

        }
    }
}
