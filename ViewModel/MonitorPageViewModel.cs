using ABB.Robotics.Controllers;
using ABB.Robotics.Controllers.Discovery;
using ABB.Robotics.Controllers.IOSystemDomain;
using ABB.Robotics.Controllers.RapidDomain;
using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HalconDotNet;
using HubDeburrSystem.DataAccess;
using HubDeburrSystem.Models;
using HubDeburrSystem.Public;
using HubDeburrSystem.Views.Dialog;
using HubDeburrSystem.Views.Pages;
using Sharp7;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Threading;
using Application = System.Windows.Application;
using static HubDeburrSystem.Models.LocusParameterSettingModel;
using static HubDeburrSystem.Public.PlcHelper;
using Adapters;
using System.Collections.Specialized;
using GalaSoft.MvvmLight.Messaging;
using System.Windows.Documents;

namespace HubDeburrSystem.ViewModel
{
    public class MonitorPageViewModel : ViewModelBase
    {
        /// <summary>
        /// 全局用户信息
        /// </summary>
        public UserModel GlobalUserInfo { get; set; } = new UserModel();

        #region ===================日期时间属性定义====================
        private string _currentDate;
        /// <summary>
        /// 当前日期
        /// </summary>
        public string CurrentDate
        {
            get { return _currentDate; }
            set { Set(ref _currentDate, value); }
        }

        private string _currentTime;
        /// <summary>
        /// 当前时间
        /// </summary>
        public string CurrentTime
        {
            get { return _currentTime; }
            set { Set(ref _currentTime, value); }
        }

        private string _currentWeek;
        /// <summary>
        /// 当前星期
        /// </summary>
        public string CurrentWeek
        {
            get { return _currentWeek; }
            set { Set(ref _currentWeek, value); }
        }

        #endregion

        #region ===================机器人相关属性字段定义====================
        private string _robotConnectionState;
        /// <summary>
        /// 机器人连接状态
        /// </summary>
        public string RobotConnectionState
        {
            get { return _robotConnectionState; }
            set { Set(ref _robotConnectionState, value); }
        }

        private LinearGradientBrush _robotConnectBackground;
        /// <summary>
        /// 机器人连接状态背景颜色
        /// </summary>
        public LinearGradientBrush RobotConnectBackground
        {
            get { return _robotConnectBackground; }
            set
            {
                _robotConnectBackground = value;
                RaisePropertyChanged("RobotConnectBackground");
            }
        }

        private string _robotControllerState;
        /// <summary>
        /// 控制器状态
        /// </summary>
        public string RobotControllerState
        {
            get { return _robotControllerState; }
            set { Set(ref _robotControllerState, value); }
        }

        private LinearGradientBrush _controllerStateBackground;
        /// <summary>
        /// 控制器状态背景色
        /// </summary>
        public LinearGradientBrush ControllerStateBackground
        {
            get { return _controllerStateBackground; }
            set { Set(ref _controllerStateBackground, value); }
        }

        private string _executionState;
        /// <summary>
        /// 控制器运行状态
        /// </summary>
        public string ExecutionState
        {
            get { return _executionState; }
            set { Set(ref _executionState, value); }
        }

        private LinearGradientBrush _executionStateBackground;
        /// <summary>
        /// 控制器运行状态背景色
        /// </summary>
        public LinearGradientBrush ExecutionStateBackground
        {
            get { return _executionStateBackground; }
            set { Set(ref _executionStateBackground, value); }
        }

        private string _operatingMode;
        /// <summary>
        /// 控制器操作模式
        /// </summary>
        public string OperatingMode
        {
            get { return _operatingMode; }
            set { Set(ref _operatingMode, value); }
        }

        private LinearGradientBrush _operatingModeBackground;
        /// <summary>
        /// 控制器操作模式背景色
        /// </summary>
        public LinearGradientBrush OperatingModeBackground
        {
            get { return _operatingModeBackground; }
            set { Set(ref _operatingModeBackground, value); }
        }

        private LinearGradientBrush _airPressStateBackground;
        /// <summary>
        /// 气源压力状态背景颜色
        /// </summary>
        public LinearGradientBrush AirPressStateBackground
        {
            get { return _airPressStateBackground; }
            set
            {
                _airPressStateBackground = value;
                RaisePropertyChanged("AirPressStateBackground");
            }
        }
        /// <summary>
        /// 气源压力状态
        /// </summary>
        private bool AirPressState { get; set; } = false;

        private string _robotSpeed;
        /// <summary>
        /// 机器人速度
        /// </summary>
        public string RobotSpeed
        {
            get { return _robotSpeed; }
            set { Set(ref _robotSpeed, value); }
        }

        private bool _buttonEnabled;
        /// <summary>
        /// 按钮使能
        /// </summary>
        public bool ButtonEnabled
        {
            get { return _buttonEnabled; }
            set { Set(ref _buttonEnabled, value); }
        }

        /// <summary>
        /// 系统消息列表
        /// </summary>
        public ObservableCollection<MessageModel> MessageList { get; set; } = new ObservableCollection<MessageModel>();

        /// <summary>
        /// 机器人启动
        /// </summary>
        public RelayCommand RobotStartCommand { get; set; }
        /// <summary>
        /// 机器人停止
        /// </summary>
        public RelayCommand RobotStopCommand { get; set; }
        /// <summary>
        /// 电机上电
        /// </summary>
        public RelayCommand MotorOnCommand { get; set; }
        /// <summary>
        /// 电机下电
        /// </summary>
        public RelayCommand MotorOffCommand { get; set; }
        /// <summary>
        /// PP移至Main
        /// </summary>
        public RelayCommand PpToMainCommand { get; set; }
        /// <summary>
        /// 刷新
        /// </summary>
        public RelayCommand FlushedCommand { get; set; }
        /// <summary>
        /// 机器人网络扫描器NetworkScanner类的实例化对象
        /// </summary>
        private NetworkScanner Scanner = null;

        /// <summary>
        /// 机器人控制器Controller类的实例化对象
        /// </summary>
        public ABB.Robotics.Controllers.Controller RobotController { get; set; }

        /// <summary>
        /// 发送给机器人数据的总数
        /// </summary>
        private RapidData TargetsNum = null;
        /// <summary>
        /// 发送给机器人的轨迹点数据
        /// </summary>
        private RapidData LocusPoints = null;

        /// <summary>
        /// 机器人加工状态值
        /// </summary>
        private RapidData RobotProcessingStateValue = null;

        /// <summary>
        /// 机器人运行控制
        /// </summary>
        private RapidData RunControl;
        /// <summary>
        /// 运行控制标志
        /// </summary>
        private Num RunControlFlag;
        /// <summary>
        /// 机器人加工状态：0: 加工停止，1：加工完成，2：加工中，3：加工异常
        /// </summary>
        public int RobotProcessingState = 0;

        private string _processingState;
        /// <summary>
        /// 机器人加工状态显示
        /// </summary>
        public string ProcessingState
        {
            get { return _processingState; }
            set { Set(ref _processingState, value); }
        }

        private LinearGradientBrush _processingStateBackground;
        /// <summary>
        /// 加工状态背景颜色
        /// </summary>
        public LinearGradientBrush ProcessingStateBackground
        {
            get { return _processingStateBackground; }
            set { Set(ref _processingStateBackground, value); }
        }
        /// <summary>
        /// 灯在原点
        /// </summary>
        private bool _lampAtOrigin = false;
        /// <summary>
        /// 灯在工作
        /// </summary>
        private bool _lampAtWork = false;


        #endregion

        #region ===================相机相关属性字段定义====================
        private string _cameraState;
        /// <summary>
        /// 相机状态
        /// </summary>
        public string CameraState
        {
            get { return _cameraState; }
            set
            {
                Set(ref _cameraState, value);
                if (CameraState == "连接成功")
                {
                    CameraStateBackground = TemplateHelper.GetLinearGradientBrush(Colors.LightGreen);
                    MessageShow("相机连接成功！");
                }
                else if (CameraState != "连接失败")
                {
                    CameraStateBackground = TemplateHelper.GetLinearGradientBrush(Colors.Red);
                    MessageShow("相机连接失败！");
                }
            }
        }

        private LinearGradientBrush _cameraStateBackground;
        /// <summary>
        /// 相机状态背景颜色
        /// </summary>
        public LinearGradientBrush CameraStateBackground
        {
            get { return _cameraStateBackground; }
            set
            {
                _cameraStateBackground = value;
                RaisePropertyChanged("CameraStateBackground");
            }
        }
        #endregion

        #region ===================PLC相关属性字段定义====================
        private string _plcState;
        /// <summary>
        /// Plc状态
        /// </summary>
        public string PlcState
        {
            get { return _plcState; }
            set
            {
                Set(ref _plcState, value);
                if (PlcState == "连接成功")
                {
                    PlcStateBackground = TemplateHelper.GetLinearGradientBrush(Colors.LightGreen);
                    MessageShow("PLC连接成功！");
                }

                else if (PlcState != "连接失败")
                {
                    PlcStateBackground = TemplateHelper.GetLinearGradientBrush(Colors.Red);
                    MessageShow("PLC连接失败！");
                }
            }
        }

        private LinearGradientBrush _plcStateBackground;
        /// <summary>
        /// Plc状态背景颜色
        /// </summary>
        public LinearGradientBrush PlcStateBackground
        {
            get { return _plcStateBackground; }
            set
            {
                _plcStateBackground = value;
                RaisePropertyChanged("PlcStateBackground");
            }
        }

        private LinearGradientBrush _emergencyStopBackground;
        /// <summary>
        /// 电控柜急停按钮状态背景颜色
        /// </summary>
        public LinearGradientBrush EmergencyStopBackground
        {
            get { return _emergencyStopBackground; }
            set
            {
                _emergencyStopBackground = value;
                RaisePropertyChanged("EmergencyStopBackground");
            }
        }
        /// <summary>
        /// 电控柜急停按钮状态
        /// </summary>
        public bool EmergencyStopState { get; set; } = false;
        /// <summary>
        /// 允许相机拍照
        /// </summary>
        public bool TakePhotos { get; set; } = false;

        private string _processingPressure = "";
        /// <summary>
        /// 浮动磨头的加工气压
        /// </summary>
        public string ProcessingPressure
        {
            get { return _processingPressure; }
            set { Set(ref _processingPressure, value); }
        }

        #endregion

        #region ===================系统控制相关属性字段定义====================
        /// <summary>
        /// 系统操作模式切换命令
        /// </summary>
        public RelayCommand SystemOperatingModelSwitchingCommand { get; set; }

        /// <summary>
        /// 系统启动停止命令
        /// </summary>
        public RelayCommand SystemStartStopCommand { get; set; }

        /// <summary>
        /// 系统复位命令
        /// </summary>
        public RelayCommand SystemResetCommand { get; set; }

        private bool _operatingModelSwitchingButtonEnable;
        /// <summary>
        /// 操作模式切换按钮使能
        /// </summary>
        public bool OperatingModeSwitchingButtonEnable
        {
            get { return _operatingModelSwitchingButtonEnable; }
            set { Set(ref _operatingModelSwitchingButtonEnable, value); }
        }

        private bool _startStopButtonEnable;
        /// <summary>
        /// 启动停止按钮使能
        /// </summary>
        public bool StartStopButtonEnable
        {
            get { return _startStopButtonEnable; }
            set { Set(ref _startStopButtonEnable, value); }
        }

        private string _operatingModeSwitchingButtonContent;
        /// <summary>
        /// 操作模式按钮显示文本
        /// </summary>
        public string OperatingModeSwitchingButtonContent
        {
            get { return _operatingModeSwitchingButtonContent; }
            set { Set(ref _operatingModeSwitchingButtonContent, value); }
        }

        private string _startStopButtonContent;
        /// <summary>
        /// 启动停止按钮显示文本
        /// </summary>
        public string StartStopButtonContent
        {
            get { return _startStopButtonContent; }
            set { Set(ref _startStopButtonContent, value); }
        }

        /// <summary>
        /// 轮毂去毛刺线程控制
        /// </summary>
        public bool HubDeburrThreadControl = false;

        /// <summary>
        /// 外部连接线程控制
        /// </summary>
        public bool ExternalConnectionThreadControl = false;
        #endregion

        #region===================识别结果相关属性字段定义====================
        private HObject _currentImage;
        /// <summary>
        /// 当前机器人加工轮毂的图像
        /// </summary>
        public HObject CurrentImage
        {
            get { return _currentImage; }
            set { Set(ref _currentImage, value); }
        }

        private HObject _sourceLocus;
        /// <summary>
        /// 源轨迹
        /// </summary>
        public HObject SourceLocus
        {
            get { return _sourceLocus; }
            set { Set(ref _sourceLocus, value); }
        }

        private HObject _processingLocus;
        /// <summary>
        /// 加工轨迹
        /// </summary>
        public HObject ProcessingLocus
        {
            get { return _processingLocus; }
            set { Set(ref _processingLocus, value); }
        }

        private HObject _processingLocusOffset;
        /// <summary>
        /// 加工轨迹偏移，最终加工轨迹
        /// </summary>
        public HObject ProcessingLocusOffset
        {
            get { return _processingLocusOffset; }
            set { Set(ref _processingLocusOffset, value); }
        }

        private HObject _oldCenterCross;
        /// <summary>
        /// 旧中心十字
        /// </summary>
        public HObject OldCenterCross
        {
            get { return _oldCenterCross; }
            set { Set(ref _oldCenterCross, value); }
        }

        private HObject _newCenterCross;
        /// <summary>
        /// 新中心十字
        /// </summary>
        public HObject NewCenterCross
        {
            get { return _newCenterCross; }
            set { Set(ref _newCenterCross, value); }
        }

        private HObject _locusCross;
        /// <summary>
        /// 轨迹十字
        /// </summary>
        public HObject LocusCross
        {
            get { return _locusCross; }
            set { Set(ref _locusCross, value); }
        }

        private HObject _locusRectangle;
        /// <summary>
        /// 轨迹矩形
        /// </summary>
        public HObject LocusRectangle
        {
            get { return _locusRectangle; }
            set { Set(ref _locusRectangle, value); }
        }

        private HObject _burrContours;
        /// <summary>
        /// 毛刺轮廓
        /// </summary>
        public HObject BurrContours
        {
            get { return _burrContours; }
            set { Set(ref _burrContours, value); }
        }


        //private string _currentColor;
        ///// <summary>
        ///// 当前窗口显示颜色
        ///// </summary>
        //public string CurrentColor
        //{
        //    get { return _currentColor; }
        //    set { Set(ref _currentColor, value); }
        //}

        private Visibility _processingResultDisplayed;
        /// <summary>
        /// 处理结果显示
        /// </summary>
        public Visibility ProcessingResultDisplayed
        {
            get { return _processingResultDisplayed; }
            set { Set(ref _processingResultDisplayed, value); }
        }

        private string _wheelType;
        /// <summary>
        /// 轮型
        /// </summary>
        public string WheelType
        {
            get { return _wheelType; }
            set { Set(ref _wheelType, value); }
        }

        private string _similarity;
        /// <summary>
        /// 识别相似度
        /// </summary>
        public string Similarity
        {
            get { return _similarity; }
            set { Set(ref _similarity, value); }
        }

        private string _robotCenterX;
        /// <summary>
        /// 机器人中心X坐标
        /// </summary>
        public string RobotCenterX
        {
            get { return _robotCenterX; }
            set { Set(ref _robotCenterX, value); }
        }

        private string _robotCenterY;
        /// <summary>
        /// 机器人中心Y坐标
        /// </summary>
        public string RobotCenterY
        {
            get { return _robotCenterY; }
            set { Set(ref _robotCenterY, value); }
        }

        private string _rotationAngle;
        /// <summary>
        /// 模板旋转角度
        /// </summary>
        public string RotationAngle
        {
            get { return _rotationAngle; }
            set { Set(ref _rotationAngle, value); }
        }

        private string _imageCenterRow;
        /// <summary>
        /// 图像中心行坐标
        /// </summary>
        public string ImageCenterRow
        {
            get { return _imageCenterRow; }
            set { Set(ref _imageCenterRow, value); }
        }

        private string _imageCenterColumn;
        /// <summary>
        /// 图像中心列坐标
        /// </summary>
        public string ImageCenterColumn
        {
            get { return _imageCenterColumn; }
            set { Set(ref _imageCenterColumn, value); }
        }

        private string _imageCenterRowOffset;
        /// <summary>
        /// 图像中心行偏移
        /// </summary>
        public string ImageCenterRowOffset
        {
            get { return _imageCenterRowOffset; }
            set { Set(ref _imageCenterRowOffset, value); }
        }

        private string _imageCenterColumnOffset;
        /// <summary>
        /// 图像中心列偏移
        /// </summary>
        public string ImageCenterColumnOffset
        {
            get { return _imageCenterColumnOffset; }
            set { Set(ref _imageCenterColumnOffset, value); }
        }

        private string _totalPoints;
        /// <summary>
        /// 总轨迹点数
        /// </summary>
        public string TotalPoints
        {
            get { return _totalPoints; }
            set { Set(ref _totalPoints, value); }
        }

        private string _processingTime;
        /// <summary>
        /// 处理用时
        /// </summary>
        public string ProcessingTime
        {
            get { return _processingTime; }
            set { Set(ref _processingTime, value); }
        }

        private string _processingResult;

        public string ProcessingResult
        {
            get { return _processingResult; }
            set { Set(ref _processingResult, value); }
        }


        /// <summary>
        /// 保存数据状态标志
        /// </summary>
        private bool SaveDatasState { get; set; } = false;
        #endregion

        #region===================加工数据相关属性字段定义====================
        private int _processingTotal;
        /// <summary>
        /// 加工总数
        /// </summary>
        public int ProcessingTotal
        {
            get { return _processingTotal; }
            set
            {
                Set(ref _processingTotal, value);
                ConfigEdit.SystemDatasWrite("ProcessingTotal", value.ToString());
            }
        }

        private int _monthProcessing;
        /// <summary>
        /// 当月加工数量
        /// </summary>
        public int MonthProcessing
        {
            get { return _monthProcessing; }
            set
            {
                Set(ref _monthProcessing, value);
                ConfigEdit.SystemDatasWrite("MonthProcessing", value.ToString());
            }
        }

        private int _dayProcessing;
        /// <summary>
        /// 当日加工数量
        /// </summary>
        public int DayProcessing
        {
            get { return _dayProcessing; }
            set
            {
                Set(ref _dayProcessing, value);
                ConfigEdit.SystemDatasWrite("DayProcessing", value.ToString());
            }
        }

        private string currentBeat;
        /// <summary>
        /// 当前节拍
        /// </summary>
        public string CurrentBeat
        {
            get { return currentBeat; }
            set
            {
                Set(ref currentBeat, value);
                ConfigEdit.SystemDatasWrite("CurrentBeat", value);
            }
        }

        private int _toolLoss;
        /// <summary>
        /// 刀具损耗
        /// </summary>
        public int ToolLoss
        {
            get { return _toolLoss; }
            set
            {
                Set(ref _toolLoss, value);
                ConfigEdit.SystemDatasWrite("ToolLoss", value.ToString());
            }
        }

        /// <summary>
        /// 系统数据
        /// </summary>
        public List<SystemDatas> SystemDatas { get; set; } = new List<SystemDatas>();

        /// <summary>
        /// 当前月份改变事件
        /// </summary>
        public event EventHandler<string> CurrentMonthChanged;
        /// <summary>
        /// 当前日期改变事件
        /// </summary>
        public event EventHandler<string> CurrentDayChanged;
        private string _currentMonth;
        /// <summary>
        /// 当前月份
        /// </summary>
        public string CurrentMonth
        {
            get { return _currentMonth; }
            set
            {
                if (_currentMonth != value)
                {
                    _currentMonth = value;
                    CurrentMonthChanged?.Invoke(this, _currentMonth);
                }
            }
        }

        private string _currentDay;
        /// <summary>
        /// 当前日期
        /// </summary>
        public string CuurentDay
        {
            get { return _currentDay; }
            set
            {
                if (_currentDay != value)
                {
                    _currentDay = value;
                    CurrentDayChanged?.Invoke(this, _currentDay);
                }
            }
        }


        #endregion

        #region===================自动模式数据更新控制====================
        /// <summary>
        /// 增加模板数据更新控制
        /// </summary>
        public static bool TemplateDataUpdateControl { get; set; } = false;

        /// <summary>
        /// 删除模板更新控制
        /// </summary>
        public static bool DeleteTemplateUpdateControl { get; set; } = false;

        /// <summary>
        /// 加工轨迹数据更新控制
        /// </summary>
        public static bool ProcessLocusDataUpdateControl { get; set; } = false;

        /// <summary>
        /// 删除加工轨迹控制
        /// </summary>
        public static bool DeleteProcessLoucsControl { get; set; } = false;

        /// <summary>
        /// 是否可更新数据（去毛刺线程控制）
        /// </summary>
        public static bool WhetherUpdateData { get; set; } = true;
        #endregion

        public MonitorPageViewModel()
        {
            ConfigEdit.ReadAppSettings("CameraIdentifier", out string cameraIdentifier);
            TemplateHelper.CameraIdentifier = cameraIdentifier;
            ConfigEdit.ReadAppSettings("PlcIP", out string plcIP);
            PlcHelper.PlcIP = plcIP;
            ConfigEdit.ReadAppSettings("ReadPlcDB", out string readPlcDB);
            PlcHelper.ReadPlcDB = int.Parse(readPlcDB);
            ConfigEdit.ReadAppSettings("WritePlcDB", out string writePlcDB);
            PlcHelper.WritePlcDB = int.Parse(writePlcDB);
            ConfigEdit.ReadAppSettings("ReadPlcDataLength", out string readPlcDataLength);
            PlcHelper.ReadPlcDataLength = int.Parse(readPlcDataLength);
            ConfigEdit.ReadAppSettings("WritePlcDataLength", out string writePlcDataLength);
            PlcHelper.WritePlcDataLength = int.Parse(writePlcDataLength);
            ConfigEdit.ReadAppSettings("PlcDataExchangeBufferSize", out string plcDataExchangeBufferSize);
            PlcHelper.PlcReadDataBuffer = new byte[int.Parse(plcDataExchangeBufferSize)];
            PlcHelper.PlcWriteDataBuffer = new byte[int.Parse(plcDataExchangeBufferSize)];
            ConfigEdit.ReadAppSettings("NumberOfCoordinates", out string numberOfCoordinates);
            RobotHelper.NumberOfCoordinates = int.Parse(numberOfCoordinates);

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += Timer_Tick;
            timer.Start();

            //机器人
            RobotConnectionState = "";
            RobotConnectBackground = TemplateHelper.GetLinearGradientBrush(Colors.Gray);
            //相机
            CameraState = "";
            CameraStateBackground = TemplateHelper.GetLinearGradientBrush(Colors.Red);
            //PLC
            PlcState = "";
            PlcStateBackground = TemplateHelper.GetLinearGradientBrush(Colors.Red);
            //电控柜急停
            EmergencyStopBackground = TemplateHelper.GetLinearGradientBrush(Colors.Gray);
            AirPressStateBackground = TemplateHelper.GetLinearGradientBrush(Colors.Gray);
            ControllerStateBackground = TemplateHelper.GetLinearGradientBrush(Colors.Gray);
            ExecutionStateBackground = TemplateHelper.GetLinearGradientBrush(Colors.Gray);
            OperatingModeBackground = TemplateHelper.GetLinearGradientBrush(Colors.Gray);
            ButtonEnabled = true;
            //机器人控制按钮命令初始化
            RobotStartCommand = new RelayCommand(RobotStart);
            RobotStopCommand = new RelayCommand(RobotStop);
            SystemResetCommand = new RelayCommand(SystemReset);
            MotorOnCommand = new RelayCommand(MotorOn);
            MotorOffCommand = new RelayCommand(MotorOff);
            PpToMainCommand = new RelayCommand(PpToMain);
            FlushedCommand = new RelayCommand(Flushed);
            //系统控制按钮命令初始化
            SystemOperatingModelSwitchingCommand = new RelayCommand(SystemOperatingModelSwitching);
            SystemStartStopCommand = new RelayCommand(SystemStartStop);
            OperatingModeSwitchingButtonContent = "手动模式";
            OperatingModeSwitchingButtonEnable = true;
            ServiceLocator.Current.GetInstance<EquipmentPageViewModel>().ManualButtonEnable = true;
            //初始化PLC为手动模式
            S7.SetBitAt(PlcHelper.PlcWriteDataBuffer, 0, 0, false);
            StartStopButtonContent = "系统停止";
            //初始化PLC为系统停止
            S7.SetBitAt(PlcHelper.PlcWriteDataBuffer, 1, 0, false);
            //初始化机器人加工状态显示
            ProcessingState = "";
            ProcessingStateBackground = TemplateHelper.GetLinearGradientBrush(Colors.Gray);
            //监控窗口显示属性初始化
            ProcessingResultDisplayed = Visibility.Hidden;
            CurrentImage = new HObject();
            SourceLocus = new HObject();
            ProcessingLocus = new HObject();
            OldCenterCross = new HObject();
            NewCenterCross = new HObject();
            LocusCross = new HObject();
            LocusRectangle = new HObject();
            BurrContours = new HObject();
            ProcessingLocusOffset = new HObject();

            //监控界面数据读取
            var sDB = new SqlAccess().SystemDataAccess;
            SystemDatas = sDB.Queryable<SystemDatas>().ToList();
            ProcessingTotal = int.Parse(SystemDatas[0].DataValue);
            MonthProcessing = int.Parse(SystemDatas[1].DataValue);
            DayProcessing = int.Parse(SystemDatas[2].DataValue);
            CurrentBeat = SystemDatas[3].DataValue;
            ToolLoss = int.Parse(SystemDatas[4].DataValue);
            CurrentMonth = SystemDatas[5].DataValue;
            CuurentDay = SystemDatas[6].DataValue;
            ServiceLocator.Current.GetInstance<SettingPageViewModel>().CalibrationPlateHeight = double.Parse(SystemDatas[7].DataValue);
            ServiceLocator.Current.GetInstance<SettingPageViewModel>().CalibrationHeight1 = double.Parse(SystemDatas[8].DataValue);
            ServiceLocator.Current.GetInstance<SettingPageViewModel>().PhysicsDistance1 = double.Parse(SystemDatas[9].DataValue);
            ServiceLocator.Current.GetInstance<SettingPageViewModel>().PixelDistance1 = double.Parse(SystemDatas[10].DataValue);
            ServiceLocator.Current.GetInstance<SettingPageViewModel>().CalibrationHeight2 = double.Parse(SystemDatas[11].DataValue);
            ServiceLocator.Current.GetInstance<SettingPageViewModel>().PhysicsDistance2 = double.Parse(SystemDatas[12].DataValue);
            ServiceLocator.Current.GetInstance<SettingPageViewModel>().PixelDistance2 = double.Parse(SystemDatas[13].DataValue);
            ServiceLocator.Current.GetInstance<SettingPageViewModel>().RobotAxisCompensateX = double.Parse(SystemDatas[14].DataValue);
            ServiceLocator.Current.GetInstance<SettingPageViewModel>().RobotAxisCompensateY = double.Parse(SystemDatas[15].DataValue);
            ServiceLocator.Current.GetInstance<SettingPageViewModel>().RobotAxisCompensateZ = double.Parse(SystemDatas[16].DataValue);
            ServiceLocator.Current.GetInstance<SettingPageViewModel>().RobotAngleCompensate = double.Parse(SystemDatas[17].DataValue);

            CurrentMonthChanged += MonitorPageViewModel_CurrentMonthChanged;
            CurrentDayChanged += MonitorPageViewModel_CurrentDayChanged;
            //启动线程
            ExternalConnectionThread();
            HubDeburrThread();
        }

        private void MonitorPageViewModel_CurrentDayChanged(object sender, string e)
        {
            ConfigEdit.SystemDatasWrite("CurrentDay", e);
            DayProcessing = 0;
        }

        private void MonitorPageViewModel_CurrentMonthChanged(object sender, string e)
        {
            ConfigEdit.SystemDatasWrite("CurrentMonth", e);
            MonthProcessing = 0;
        }


        /// <summary>
        /// 系统运行模式切换
        /// </summary>
        private void SystemOperatingModelSwitching()
        {
            if (!PlcHelper.PlcCilent.Connected)
            {
                UMessageBox.Show("未连接PLC！", MessageType.Default);
                return;
            }
            if (OperatingModeSwitchingButtonContent == "手动模式")
            {
                var result = UMessageBox.Show("系统运行模式切换", "确认切换到自动模式？");
                if (result)
                {
                    S7.SetBitAt(PlcHelper.PlcWriteDataBuffer, 24, 0, true); //机器人完成工作
                    S7.SetBitAt(PlcHelper.PlcWriteDataBuffer, 0, 0, true); //自动模式
                    S7.SetBitAt(PlcHelper.PlcWriteDataBuffer, 0, 6, false);
                    OperatingModeSwitchingButtonContent = "自动模式";
                    StartStopButtonEnable = true;
                    ServiceLocator.Current.GetInstance<EquipmentPageViewModel>().ManualButtonEnable = false;
                }
            }
            else
            {
                var result = UMessageBox.Show("系统运行模式切换", "确认切换到手动模式？");
                if (result)
                {
                    S7.SetBitAt(PlcHelper.PlcWriteDataBuffer, 6, 2, false); //拍照完成
                    S7.SetBitAt(PlcHelper.PlcWriteDataBuffer, 24, 0, false); //机器人完成工作
                    S7.SetBitAt(PlcHelper.PlcWriteDataBuffer, 0, 0, false); //手动模式
                    S7.SetBitAt(PlcHelper.PlcWriteDataBuffer, 1, 0, false);
                    S7.SetBitAt(PlcHelper.PlcWriteDataBuffer, 0, 6, false);

                    //复位加工完成
                    S7.SetBitAt(PlcHelper.PlcWriteDataBuffer, 1, 1, false);
                    //给机器人发手动
                    OperatingModeSwitchingButtonContent = "手动模式";
                    StartStopButtonContent = "系统停止";
                    ServiceLocator.Current.GetInstance<EquipmentPageViewModel>().ManualButtonEnable = true;
                    StopRobot();
                }
            }
        }

        /// <summary>
        /// 系统启动停止
        /// </summary>
        private void SystemStartStop()
        {
            if (!PlcHelper.PlcCilent.Connected)
            {
                UMessageBox.Show("PLC连接异常！", MessageType.Default);
                return;
            }
            if (TemplateHelper.CameraHandle == null)
            {
                UMessageBox.Show("相机连接异常！", MessageType.Default);
                return;
            }
            if (!RobotController.Connected)
            {
                UMessageBox.Show("机器人连接异常！", MessageType.Default);
                return;
            }
            if (OperatingModeSwitchingButtonContent != "自动模式")
            {
                UMessageBox.Show("请切换到自动模式！", MessageType.Default);
                return;
            }
            if (StartStopButtonContent == "系统停止")
            {
                if (RobotController.Rapid.ExecutionStatus != ExecutionStatus.Running)
                {
                    UMessageBox.Show("请先启动机器人！", MessageType.Default);
                    return;
                }
                S7.SetBitAt(PlcHelper.PlcWriteDataBuffer, 1, 0, true);
                StartStopButtonContent = "系统启动";
            }
            else
            {
                S7.SetBitAt(PlcHelper.PlcWriteDataBuffer, 1, 0, false);
                StartStopButtonContent = "系统停止";
            }
        }

        /// <summary>
        /// 系统复位
        /// </summary>
        private void SystemReset()
        {
            S7.SetBitAt(PlcHelper.PlcWriteDataBuffer, 0, 7, true);
            S7.SetBitAt(PlcHelper.PlcWriteDataBuffer, 0, 7, false);
        }

        /// <summary>
        /// 时间刷新定时器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Tick(object sender, EventArgs e)
        {

            CurrentDate = DateTime.Now.ToString("yyyy年MM月dd日");
            CurrentTime = DateTime.Now.ToString("HH:mm:ss");
            CurrentWeek = DateTime.Now.ToString("dddd");
            CurrentMonth = DateTime.Now.Month.ToString();
            CuurentDay = DateTime.Now.Day.ToString();
        }

        /// <summary>
        /// 外部连接线程
        /// </summary>
        public void ExternalConnectionThread()
        {
            ExternalConnectionThreadControl = true;
            System.Threading.Tasks.Task.Run(() =>
            {
                while (ExternalConnectionThreadControl)
                {
                    if (!PlcHelper.PlcCilent.Connected)
                    {
                        //连接PLC
                        int result = PlcHelper.PlcCilent.ConnectTo(PlcHelper.PlcIP, 0, 0);
                        if (result == 0 && PlcHelper.PlcCilent.Connected)
                        {
                            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                PlcState = "连接成功";
                                //写入加工压力
                                byte[] bytes = BitConverter.GetBytes(0.1f);
                                byte[] buffer = new byte[] { bytes[3], bytes[2], bytes[1], bytes[0] };
                                Buffer.BlockCopy(buffer, 0, PlcWriteDataBuffer, 2, buffer.Length);
                                ReadWritePLCThread();
                            }));
                        }
                        else
                        {
                            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                PlcState = "连接失败";
                            }));
                        }
                    }

                    if (TemplateHelper.CameraHandle == null)
                    {
                        //连接相机
                        TemplateHelper.CameraHandle = TemplateHelper.ConnectCamera(TemplateHelper.CameraIdentifier);

                        if (TemplateHelper.CameraHandle != null)
                        {
                            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                CameraState = "连接成功";
                            }));
                        }
                        else
                        {
                            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                CameraState = "连接失败";
                            }));
                        }
                    }

                    if (PlcState == "连接成功" && CameraState == "连接成功") ExternalConnectionThreadControl = false;
                    Thread.Sleep(1000);
                }
            });
        }

        /// <summary>
        /// 轮毂去毛刺线程
        /// </summary>
        public void HubDeburrThread()
        {
            HubDeburrThreadControl = true;
            ProcessingResultDisplayed = Visibility.Hidden;
            bool inProcessing = false;//加工中标志
            int grabImageFail = 0;//抓取图像失败次数
            DateTime startTime = new DateTime();

            System.Threading.Tasks.Task.Run(() =>
            {
                while (HubDeburrThreadControl)
                {
                    if (PlcCilent.Connected && TemplateHelper.CameraHandle != null && StartStopButtonContent == "系统启动"
                    && !inProcessing && RobotController.Rapid.ExecutionStatus == ExecutionStatus.Running && RobotProcessingState == 0
                    && !ProcessLocusDataUpdateControl && !TemplateDataUpdateControl && !DeleteProcessLoucsControl && !SaveDatasState &&
                    AirPressState && EmergencyStopState && TakePhotos)
                    {
                        S7.SetBitAt(PlcHelper.PlcWriteDataBuffer, 24, 0, false); //机器人完成工作
                        //获取夹紧完成信号
                        bool inPos = S7.GetBitAt(PlcReadDataBuffer, 6, 0);
                        //轮毂夹紧后采集图像
                        if (inPos && !inProcessing)
                        {
                            WhetherUpdateData = false;
                            ProcessingResultDisplayed = Visibility.Hidden;
                            inProcessing = true;
                            startTime = DateTime.Now;
                            SourceLocus.Dispose();
                            OldCenterCross.Dispose();
                            ProcessingLocus.Dispose();
                            NewCenterCross.Dispose();
                            ProcessingLocusOffset.Dispose();
                            LocusCross.Dispose();
                            LocusRectangle.Dispose();
                            BurrContours.Dispose();
                            CurrentImage.Dispose();
                            try
                            {
                                HOperatorSet.GrabImage(out HObject currentImage, TemplateHelper.CameraHandle);
                                CurrentImage = currentImage;
                            }
                            catch (Exception ex)
                            {
                                MessageShow("抓取图像失败：" + ex.Message);
                                grabImageFail++;
                                //如果抓取图像失败大于3次，重新连接相机
                                if (grabImageFail > 2)
                                {
                                    //加工标志复位
                                    inProcessing = false;
                                    //相机句柄置空
                                    TemplateHelper.CameraHandle = null;
                                    //启动外部连接线程连接相机
                                    if (!ExternalConnectionThreadControl)
                                    {
                                        WhetherUpdateData = true;
                                        ExternalConnectionThread();
                                    }
                                    grabImageFail = 0;
                                }
                                else
                                {
                                    inProcessing = false;
                                    Thread.Sleep(500);
                                }
                            }
                            grabImageFail = 0;
                        }
                        //拍照完成信号
                        S7.SetBitAt(PlcHelper.PlcWriteDataBuffer, 6, 2, true);
                        //采集成功处理图像
                        if (inProcessing && CurrentImage.IsInitialized())
                        {
                            HOperatorSet.ZoomImageFactor(CurrentImage, out HObject imageZoomed, ImageScale, ImageScale, "constant");
                            //轮型识别
                            IdentifyDataModel result = TemplateHelper.IdentifyAlgorithm(imageZoomed, TemplatePageViewModel.TemplateList,
                                TemplateAngleStart, TemplateAngleExtent, MinSimilarity);
                            //识别不成功
                            if (result == null)
                            {
                                //结果显示
                                DisplayResult("NG", "_", "_", "_", "_", "_", "_", "_", "识别失败", startTime);
                                WhetherUpdateData = true;
                                //发送加工完成信号
                                S7.SetBitAt(PlcHelper.PlcWriteDataBuffer, 1, 1, true);
                                //保存数据
                                SaveResultData("NG", startTime, ProcessingTime, "识别失败", CurrentImage);
                                Thread.Sleep(2000);
                            }
                            //识别成功
                            else
                            {
                                //获取模板数据
                                TemplateDataModel data = ServiceLocator.Current.GetInstance<TemplatePageViewModel>().ProcessingTemplateDatas.First(s => s.WheelType == result.IdentifyWheelType);
                                TemplateHelper.GetWheelCenter(CurrentImage, data.InnerCircleRadius, data.InnerCircleCaliperLength, out HTuple centerRow, out HTuple centerColumn);
                                //如果定中失败
                                if (centerRow.Length == 0)
                                {
                                    var deg = Math.Round(result.Radian.TupleDeg().D, 2).ToString();
                                    //结果显示
                                    DisplayResult(result.IdentifyWheelType, result.Similarity.ToString(), deg, "", "", "", "", "", "定中失败", startTime);
                                    WhetherUpdateData = true;
                                    //发送加工完成信号
                                    S7.SetBitAt(PlcHelper.PlcWriteDataBuffer, 1, 1, true);
                                    //保存数据
                                    SaveResultData(result.IdentifyWheelType, startTime, ProcessingTime, "定中失败", CurrentImage);
                                    Thread.Sleep(2000);
                                }
                                else
                                {
                                    //计算偏移
                                    var rowOffset = data.CenterRow - centerRow;
                                    var columnOffset = data.CenterColumn - centerColumn;
                                    //轮毂中心
                                    HOperatorSet.GenCrossContourXld(out HObject oldCenter, data.CenterRow, data.CenterColumn, 200, 0);
                                    HOperatorSet.GenCrossContourXld(out HObject newCenter, centerRow, centerColumn, 200, 0);
                                    //计算平均弧度
                                    HOperatorSet.TupleRad(360.0 / data.SpokeQuantity, out HTuple averageRad);
                                    HTuple radian = null;
                                    //当匹配出的弧度大于0.5时，计算最少旋转弧度(0.5rad = 28.647度)
                                    if (result.Radian > 0.5)
                                    {
                                        var rad = result.Radian / averageRad;
                                        radian = result.Radian - averageRad * ((int)rad.D + 1);
                                    }
                                    else
                                        radian = result.Radian;
                                    HOperatorSet.TupleDeg(radian, out HTuple angle);
                                    #region===============轨迹处理================
                                    var index = LocusPageViewModel.ProcessingLocusDatas.LocusName.FindIndex(t => t == result.IdentifyWheelType);
                                    if (index < 0)
                                    {
                                        //显示轨迹
                                        OldCenterCross = oldCenter;
                                        NewCenterCross = newCenter;

                                        //结果显示
                                        DisplayResult(result.IdentifyWheelType, result.Similarity.ToString(), angle.ToString(), Math.Round(centerRow.D, 2).ToString()
                                            , Math.Round(centerColumn.D, 2).ToString(), Math.Round(rowOffset.D, 2).ToString(),
                                            Math.Round(columnOffset.D, 2).ToString(), "", "无加工轨迹", startTime);
                                        WhetherUpdateData = true;
                                        //发送加工完成信号
                                        S7.SetBitAt(PlcHelper.PlcWriteDataBuffer, 1, 1, true);
                                        //保存数据
                                        SaveResultData(result.IdentifyWheelType, startTime, ProcessingTime, "无加工轨迹", CurrentImage);
                                        Thread.Sleep(2000);
                                    }
                                    else
                                    {
                                        //获取当前识别轮毂的轨迹点
                                        List<MachiningPathPosModel> datas = LocusPageViewModel.ProcessingLocusDatas.LocusPoints[index];
                                        //根据加工轨迹数据和识别数据生成新的加工数据
                                        List<MachiningPathPosModel> newDatas = new List<MachiningPathPosModel>();
                                        for (int i = 0; i < datas.Count; i++)
                                        {
                                            MachiningPathPosModel point = new MachiningPathPosModel();
                                            point.Id = datas[i].Id;
                                            point.PoseId = datas[i].PoseId;
                                            var (row, col) = TemplateHelper.RotatePointAroundNewCenter(datas[i].Row, datas[i].Column, centerRow.D, centerColumn.D, radian.D + data.AngularCompensation);
                                            point.Row = row;
                                            point.Column = col;
                                            //获取指定高度下的新齐次变换矩阵
                                            var pixelToPhysicsHomMat2D = ServiceLocator.Current.GetInstance<SettingPageViewModel>().GetPixelToPhysicsHomMat2D(datas[i].Z, TemplateHelper.PixelToPhysicsHomMat2D);
                                            HOperatorSet.AffineTransPoint2d(pixelToPhysicsHomMat2D, point.Row, point.Column, out HTuple x, out HTuple y);
                                            point.X = Math.Round(x.D, 2) + ServiceLocator.Current.GetInstance<SettingPageViewModel>().RobotAxisCompensateX;
                                            point.Y = Math.Round(y.D, 2) + ServiceLocator.Current.GetInstance<SettingPageViewModel>().RobotAxisCompensateY;
                                            point.Z = datas[i].Z + ServiceLocator.Current.GetInstance<SettingPageViewModel>().RobotAxisCompensateZ;
                                            point.EX = datas[i].EX;
                                            point.EY = datas[i].EY;
                                            //旋转模板位姿
                                            point.EZ = RobotHelper.AngleCalculationAfterRotation(datas[i].EZ, angle);
                                            var q = ViewportHelper.EulerToQuaternion(point.EX, point.EY, point.EZ);
                                            point.Q1 = q.W;
                                            point.Q2 = q.X;
                                            point.Q3 = q.Y;
                                            point.Q4 = q.Z;
                                            newDatas.Add(point);
                                        }

                                        //从新的加工数据中生成新的窗口轮廓
                                        HObject newContours = TemplateHelper.GenContours(newDatas, false);
                                        //加工轨迹偏移
                                        //if (data.LocusScale != 0)
                                        //{
                                        //    newDatas = TemplateHelper.LocusScale(newDatas, data.LocusScale, TemplateHelper.PixelToPhysicsHomMat2D);
                                        //}
                                        int spokeType1 = newDatas[newDatas.Count - 1].PoseId / 1000 / data.SpokeQuantity;
                                        string[] newOutPoseIds;
                                        newDatas = TemplateHelper.LocusScale1(newDatas, TemplateHelper.PixelToPhysicsHomMat2D, CurrentImage, spokeType1, out newOutPoseIds, data.OutPointPoseId);
                                        string newOutPoseIdStr = string.Join(",", newOutPoseIds);
                                        string filePath = "D:\\Temp\\1.csv";

                                        ConfigEdit.SaveListToCsv(newDatas, filePath,
                                             p => p.X.ToString(),
                                             p => p.Y.ToString(),
                                             p => p.Z.ToString(),
                                             p => p.Q1.ToString(),
                                             p => p.Q2.ToString(),
                                             p => p.Q3.ToString(),
                                             p => p.Q4.ToString());
                                        //轨迹窗口当前识别轮毂数据检查
                                        ServiceLocator.Current.GetInstance<LocusPageViewModel>().CurrentMachiningLocusDatas.Clear();
                                        ServiceLocator.Current.GetInstance<LocusPageViewModel>().CurrentMachiningLocusDatas = newDatas;
                                        ServiceLocator.Current.GetInstance<LocusPageViewModel>().CurrentIdentifyData = null;
                                        ServiceLocator.Current.GetInstance<LocusPageViewModel>().CurrentIdentifyData = result;
                                        ServiceLocator.Current.GetInstance<LocusPageViewModel>().CurrentMachiningImage.Dispose();
                                        ServiceLocator.Current.GetInstance<LocusPageViewModel>().CurrentMachiningImage = CurrentImage.Clone();

                                        int spokeType = newDatas[newDatas.Count - 1].PoseId / 1000 / data.SpokeQuantity;
                                        //List<MachiningPathPosModel> toRobotDatas = TemplateHelper.GenLocusOfOutPoint(newDatas, data.OutPointPoseId, spokeType);
                                        List<MachiningPathPosModel> toRobotDatas = TemplateHelper.GenLocusOfOutPoint1(newDatas, newOutPoseIdStr, spokeType);
                                        //生成新的加工轨迹newXld
                                        HObject newXld = TemplateHelper.GenContours(toRobotDatas, true);
                                        //生成点的十字
                                        HOperatorSet.GenEmptyObj(out HObject crosss);
                                        HOperatorSet.CountObj(newXld, out HTuple number);
                                        for (int i = 1; i <= number; i++)
                                        {
                                            HOperatorSet.SelectObj(newXld, out HObject selectexXld, i);
                                            HOperatorSet.GetContourXld(selectexXld, out HTuple row, out HTuple col);
                                            HOperatorSet.GenCrossContourXld(out HObject cross, row, col, 8, 0);
                                            HOperatorSet.ConcatObj(crosss, cross, out crosss);
                                        }
                                        #endregion
                                        //获取源模板图像
                                        string imagePath = $"D:\\DeburrSystem\\TemplateImages\\{result.IdentifyWheelType}";
                                        HOperatorSet.ReadImage(out HObject sourceImage, imagePath);
                                        //根据中心和角度生成仿射矩阵
                                        HOperatorSet.VectorAngleToRigid(data.CenterRow, data.CenterColumn, 0, centerRow, centerColumn, radian, out HTuple homMat2D);
                                        HOperatorSet.AffineTransImage(sourceImage, out HObject affineImage, homMat2D, "constant", "false");
                                        HObject burr = TemplateHelper.LargeBurrJudgment(affineImage, CurrentImage, newContours);
                                        HOperatorSet.CountObj(burr, out HTuple n);

                                        //显示轨迹
                                        SourceLocus = newContours;
                                        OldCenterCross = oldCenter;
                                        ProcessingLocus = newXld;
                                        NewCenterCross = newCenter;
                                        LocusCross = crosss;

                                        //如果中心偏移大于15
                                        if (rowOffset.D > 10 || columnOffset.D > 10)
                                        {
                                            //结果显示
                                            DisplayResult(result.IdentifyWheelType, result.Similarity.ToString(), angle.D.ToString("0.00"), Math.Round(centerRow.D, 2).ToString()
                                                , Math.Round(centerColumn.D, 2).ToString(), Math.Round(rowOffset.D, 2).ToString(),
                                                Math.Round(columnOffset.D, 2).ToString(), datas.Count.ToString(), "偏心过大", startTime);
                                            WhetherUpdateData = true;
                                            //发送加工完成信号
                                            S7.SetBitAt(PlcHelper.PlcWriteDataBuffer, 1, 1, true);
                                            //保存数据
                                            SaveResultData(result.IdentifyWheelType, startTime, ProcessingTime, "偏心过大", CurrentImage);
                                            Thread.Sleep(2000);
                                        }
                                        //如果存在大毛刺
                                        //else if (n > 0)
                                        //{
                                        //    DisplayResult(result.IdentifyWheelType, result.Similarity.ToString(), angle.ToString(), Math.Round(centerRow.D, 2).ToString()
                                        //        , Math.Round(centerColumn.D, 2).ToString(), Math.Round(rowOffset.D, 2).ToString(),
                                        //        Math.Round(columnOffset.D, 2).ToString(), datas.Count.ToString(), "毛刺过大", startTime);
                                        //    BurrContours = burr;
                                        //    WhetherUpdateData = true;
                                        //    //发送加工完成信号
                                        //    S7.SetBitAt(PlcHelper.PlcWriteDataBuffer, 1, 1, true);
                                        //    //保存数据
                                        //    SaveResultData(result.IdentifyWheelType, startTime, ProcessingTime, "毛刺过大", CurrentImage);
                                        //    Thread.Sleep(2000);
                                        //}
                                        //加工使能
                                        else if (!data.ProcessingEnable)
                                        {
                                            DisplayResult(result.IdentifyWheelType, result.Similarity.ToString(), angle.ToString(),
                                                Math.Round(centerRow.D, 2).ToString(), Math.Round(centerColumn.D, 2).ToString(),
                                                Math.Round(rowOffset.D, 2).ToString(), Math.Round(columnOffset.D, 2).ToString(), datas.Count.ToString(), "未加工", startTime);
                                            WhetherUpdateData = true;
                                            //发送加工完成信号
                                            S7.SetBitAt(PlcHelper.PlcWriteDataBuffer, 1, 1, true);
                                            //保存数据
                                            SaveResultData(result.IdentifyWheelType, startTime, CurrentBeat, "未加工", CurrentImage);
                                            Thread.Sleep(2000);
                                        }
                                        //都正常
                                        else
                                        {
                                            DisplayResult(result.IdentifyWheelType, result.Similarity.ToString(), angle.ToString(),
                                                Math.Round(centerRow.D, 2).ToString(), Math.Round(centerColumn.D, 2).ToString(),
                                                Math.Round(rowOffset.D, 2).ToString(), Math.Round(columnOffset.D, 2).ToString(),
                                                toRobotDatas.Count.ToString(), "处理完成", startTime);
                                            WhetherUpdateData = true;
                                            #region===============发送数据=================
                                            //发送数据
                                            var processingStartTime = DateTime.Now;
                                            MessageShow("加工轨迹发送中......");

                                            //System.Threading.Tasks.Task.Run(() =>
                                            //{
                                            //写入加工压力
                                            byte[] bytes = BitConverter.GetBytes(data.ProcessingPressure);
                                            byte[] buffer = new byte[] { bytes[3], bytes[2], bytes[1], bytes[0] };
                                            Buffer.BlockCopy(buffer, 0, PlcWriteDataBuffer, 2, buffer.Length);
                                            bool processing = false;
                                            try
                                            {
                                                Console.WriteLine("发送加工轨迹:" + toRobotDatas.Count);
                                                SendLocusDatas(toRobotDatas);
                                                MessageShow("加工轨迹发送完成！");
                                                Console.WriteLine("加工轨迹发送完成");
                                                processing = true;
                                            }
                                            catch (Exception ex)
                                            {
                                                inProcessing = false;
                                                MessageShow("加工轨迹发送失败1：" + ex);
                                                Console.WriteLine("加工轨迹发送失败1：" + ex);
                                                CurrentBeat = "??";
                                                SaveResultData(result.IdentifyWheelType, processingStartTime, CurrentBeat, "加工异常", CurrentImage);
                                            }

                                            while (processing)
                                            {
                                                if (RobotProcessingState == 0)
                                                {
                                                    var processingEndTime = DateTime.Now;
                                                    TimeSpan time = processingEndTime.Subtract(processingStartTime);
                                                    CurrentBeat = ((int)time.TotalSeconds - 3).ToString() + "s";
                                                    ProcessingTotal += 1;
                                                    MonthProcessing += 1;
                                                    DayProcessing += 1;
                                                    ToolLoss += 1;
                                                    //保存数据
                                                    SaveResultData(result.IdentifyWheelType, processingStartTime, CurrentBeat, "加工完成", CurrentImage);
                                                    processing = false;
                                                }
                                            }
                                            //});
                                            #endregion
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (inProcessing && RobotProcessingState == 1)
                    {
                        //给PLC发送加工完成信号
                        S7.SetBitAt(PlcWriteDataBuffer, 1, 1, true);
                    }
                    //在加工中并且加工位没有感应到轮毂，复位加工中标志，进入下一次加工
                    if (inProcessing && !S7.GetBitAt(PlcReadDataBuffer, 5, 0) && RobotProcessingState == 0)
                    {
                        //复位加工中标志，进行下一次加工
                        inProcessing = false;
                        //复位加工完成信号
                        S7.SetBitAt(PlcWriteDataBuffer, 1, 1, false);
                    }
                    inProcessing = false;
                }
            });
        }



        /// <summary>
        /// 读写PLC线程
        /// </summary>
        public void ReadWritePLCThread()
        {
            ReadWritePlcThreadControl = true;
            int num = 0;
            System.Threading.Tasks.Task.Run(async () =>
            {
                while (ReadWritePlcThreadControl)
                {
                    int readResult = PlcCilent.DBRead(ReadPlcDB, 0, ReadPlcDataLength, PlcReadDataBuffer);
                    int writeResult = PlcCilent.DBWrite(WritePlcDB, 0, WritePlcDataLength, PlcWriteDataBuffer);
                    if (readResult != 0 || writeResult != 0)
                    {
                        num++;
                        Thread.Sleep(1000);
                        if (num > 3)
                        {
                            ReadWritePlcThreadControl = false;
                            MessageShow("PLC连接异常！");
                            if (!ExternalConnectionThreadControl)
                                ExternalConnectionThread();
                            num = 0;
                        }
                    }
                    else
                    {

                        //允许相机拍照
                        if (S7.GetBitAt(PlcReadDataBuffer, 14, 0))
                            TakePhotos = true;
                        else
                            TakePhotos = false;
                        //给机器人发送信号
                        //bool bol1 = S7.GetBitAt(PlcReadDataBuffer, 14, 1); //灯在原点
                        //bool bol2 = S7.GetBitAt(PlcReadDataBuffer, 14, 2); //灯在工作中
                        //WriteBoolToRAPID(RobotController, "Data", "LightHome", bol1);
                        //WriteBoolToRAPID(RobotController, "Data", "LightWork", bol2);
                        //浮动磨头气压显示
                        float ss = S7.GetRealAt(PlcReadDataBuffer, 10);
                        byte[] data = new byte[] { PlcReadDataBuffer[13], PlcReadDataBuffer[12], PlcReadDataBuffer[11], PlcReadDataBuffer[10] };
                        Math.Round(BitConverter.ToSingle(data, 0), 3);
                        ProcessingPressure = Math.Round(BitConverter.ToSingle(data, 0), 3).ToString();
                        //心跳脉冲-PLC
                        S7.SetBitAt(PlcHelper.PlcWriteDataBuffer, 24, 2, true);
                        //心跳脉冲 - 机器人
                        //WriteBoolToRAPID(RobotController, "Data", "VisionHeart", true);
                        //PLC故障 - 断机器人使能
                        if (S7.GetBitAt(PlcReadDataBuffer, 26, 0))
                        {
                            StopRobot();
                        }
                        await System.Threading.Tasks.Task.Delay(100);
                    }
                }
            });

            System.Threading.Tasks.Task.Run(async () =>
            {
                while (ReadWritePlcThreadControl)
                {
                    //await Console.Out.WriteLineAsync(ReadWritePlcThreadControl.ToString());
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {

                        //电机状态
                        ServiceLocator.Current.GetInstance<EquipmentPageViewModel>().InletMotorStatus = MotorStatusSet(Convert.ToInt16(PlcReadDataBuffer[0]));
                        ServiceLocator.Current.GetInstance<EquipmentPageViewModel>().ProcessingMotorStatus = MotorStatusSet(Convert.ToInt16(PlcReadDataBuffer[1]));
                        ServiceLocator.Current.GetInstance<EquipmentPageViewModel>().ExportMotor1Status = MotorStatusSet(Convert.ToInt16(PlcReadDataBuffer[2]));
                        ServiceLocator.Current.GetInstance<EquipmentPageViewModel>().ExportMotor2Status = MotorStatusSet(Convert.ToInt16(PlcReadDataBuffer[3]));
                        //推杆气缸电磁阀
                        if (S7.GetBitAt(PlcReadDataBuffer, 4, 5))
                            ServiceLocator.Current.GetInstance<EquipmentPageViewModel>().PushrodCylinderBackground = TemplateHelper.GetLinearGradientBrush(Colors.LightGreen);
                        if (S7.GetBitAt(PlcReadDataBuffer, 4, 6))
                            ServiceLocator.Current.GetInstance<EquipmentPageViewModel>().PushrodCylinderBackground = TemplateHelper.GetLinearGradientBrush(Colors.OrangeRed);
                        //定中机构电磁阀
                        if (S7.GetBitAt(PlcReadDataBuffer, 4, 4))
                            ServiceLocator.Current.GetInstance<EquipmentPageViewModel>().FixedCenterBackground = TemplateHelper.GetLinearGradientBrush(Colors.LightGreen);
                        else
                            ServiceLocator.Current.GetInstance<EquipmentPageViewModel>().FixedCenterBackground = TemplateHelper.GetLinearGradientBrush(Colors.OrangeRed);
                        //入口光电
                        if (S7.GetBitAt(PlcReadDataBuffer, 4, 7))
                            ServiceLocator.Current.GetInstance<EquipmentPageViewModel>().InletPhotoelectricityBackground = TemplateHelper.GetLinearGradientBrush(Colors.LightGreen);
                        else
                            ServiceLocator.Current.GetInstance<EquipmentPageViewModel>().InletPhotoelectricityBackground = TemplateHelper.GetLinearGradientBrush(Colors.OrangeRed);
                        //加工位光电
                        if (S7.GetBitAt(PlcReadDataBuffer, 5, 0))
                            ServiceLocator.Current.GetInstance<EquipmentPageViewModel>().ProcessingPhotoelectricityBackground = TemplateHelper.GetLinearGradientBrush(Colors.LightGreen);
                        else
                            ServiceLocator.Current.GetInstance<EquipmentPageViewModel>().ProcessingPhotoelectricityBackground = TemplateHelper.GetLinearGradientBrush(Colors.OrangeRed);
                        //出口光电1
                        if (S7.GetBitAt(PlcReadDataBuffer, 5, 1))
                            ServiceLocator.Current.GetInstance<EquipmentPageViewModel>().ExportPhotoelectricity1Background = TemplateHelper.GetLinearGradientBrush(Colors.LightGreen);
                        else
                            ServiceLocator.Current.GetInstance<EquipmentPageViewModel>().ExportPhotoelectricity1Background = TemplateHelper.GetLinearGradientBrush(Colors.OrangeRed);
                        //出口光电2
                        if (S7.GetBitAt(PlcReadDataBuffer, 5, 2))
                            ServiceLocator.Current.GetInstance<EquipmentPageViewModel>().ExportPhotoelectricity2Background = TemplateHelper.GetLinearGradientBrush(Colors.LightGreen);
                        else
                            ServiceLocator.Current.GetInstance<EquipmentPageViewModel>().ExportPhotoelectricity2Background = TemplateHelper.GetLinearGradientBrush(Colors.OrangeRed);
                        //推杆回位磁感应
                        if (S7.GetBitAt(PlcReadDataBuffer, 6, 2))
                            ServiceLocator.Current.GetInstance<EquipmentPageViewModel>().PushrodInductionBackground = TemplateHelper.GetLinearGradientBrush(Colors.LightGreen);
                        else
                            ServiceLocator.Current.GetInstance<EquipmentPageViewModel>().PushrodInductionBackground = TemplateHelper.GetLinearGradientBrush(Colors.OrangeRed);
                        //定中机构回位磁感应
                        if (S7.GetBitAt(PlcReadDataBuffer, 6, 1))
                            ServiceLocator.Current.GetInstance<EquipmentPageViewModel>().FixedCenterInductionBackground = TemplateHelper.GetLinearGradientBrush(Colors.LightGreen);
                        else
                            ServiceLocator.Current.GetInstance<EquipmentPageViewModel>().FixedCenterInductionBackground = TemplateHelper.GetLinearGradientBrush(Colors.OrangeRed);
                        //电控柜急停按钮
                        if (S7.GetBitAt(PlcReadDataBuffer, 7, 4))
                        {
                            EmergencyStopBackground = TemplateHelper.GetLinearGradientBrush(Colors.Green);
                            EmergencyStopState = true;
                        }
                        else
                        {
                            EmergencyStopBackground = TemplateHelper.GetLinearGradientBrush(Colors.Red);
                            EmergencyStopState = false;
                            StopRobot();
                        }

                        //回零完成
                        if (S7.GetBitAt(PlcReadDataBuffer, 14, 3))
                            ServiceLocator.Current.GetInstance<EquipmentPageViewModel>().ReturnZeroCompletionBackground = TemplateHelper.GetLinearGradientBrush(Colors.LightGreen);
                        else
                            ServiceLocator.Current.GetInstance<EquipmentPageViewModel>().ReturnZeroCompletionBackground = TemplateHelper.GetLinearGradientBrush(Colors.OrangeRed);
                        //使能完成
                        if (S7.GetBitAt(PlcReadDataBuffer, 14, 4))
                        {
                            ServiceLocator.Current.GetInstance<EquipmentPageViewModel>().EnableCompletionBackground = TemplateHelper.GetLinearGradientBrush(Colors.LightGreen);
                            ServiceLocator.Current.GetInstance<EquipmentPageViewModel>().MyBtuContent = "手动使能关";
                        }
                        else
                        {
                            ServiceLocator.Current.GetInstance<EquipmentPageViewModel>().EnableCompletionBackground = TemplateHelper.GetLinearGradientBrush(Colors.OrangeRed);
                            ServiceLocator.Current.GetInstance<EquipmentPageViewModel>().MyBtuContent = "手动使能开";
                        }
                        //使能错误
                        if (S7.GetBitAt(PlcReadDataBuffer, 14, 5))
                            ServiceLocator.Current.GetInstance<EquipmentPageViewModel>().EnableErrorBackground = TemplateHelper.GetLinearGradientBrush(Colors.LightGreen);
                        else
                            ServiceLocator.Current.GetInstance<EquipmentPageViewModel>().EnableErrorBackground = TemplateHelper.GetLinearGradientBrush(Colors.OrangeRed);
                        //点动运行中
                        if (S7.GetBitAt(PlcReadDataBuffer, 14, 6))
                            ServiceLocator.Current.GetInstance<EquipmentPageViewModel>().JoggingBackground = TemplateHelper.GetLinearGradientBrush(Colors.LightGreen);
                        else
                            ServiceLocator.Current.GetInstance<EquipmentPageViewModel>().JoggingBackground = TemplateHelper.GetLinearGradientBrush(Colors.OrangeRed);
                        //故障
                        if (S7.GetBitAt(PlcReadDataBuffer, 14, 7))
                            ServiceLocator.Current.GetInstance<EquipmentPageViewModel>().FaultBackground = TemplateHelper.GetLinearGradientBrush(Colors.LightGreen);
                        else
                            ServiceLocator.Current.GetInstance<EquipmentPageViewModel>().FaultBackground = TemplateHelper.GetLinearGradientBrush(Colors.OrangeRed);
                        //复位完成
                        if (S7.GetBitAt(PlcReadDataBuffer, 14, 7))
                            ServiceLocator.Current.GetInstance<EquipmentPageViewModel>().ResetCompletedBackground = TemplateHelper.GetLinearGradientBrush(Colors.LightGreen);
                        else
                            ServiceLocator.Current.GetInstance<EquipmentPageViewModel>().ResetCompletedBackground = TemplateHelper.GetLinearGradientBrush(Colors.OrangeRed);
                        //上限位
                        if (S7.GetBitAt(PlcReadDataBuffer, 48, 0))
                            ServiceLocator.Current.GetInstance<EquipmentPageViewModel>().UpperLimitBackground = TemplateHelper.GetLinearGradientBrush(Colors.LightGreen);
                        else
                            ServiceLocator.Current.GetInstance<EquipmentPageViewModel>().UpperLimitBackground = TemplateHelper.GetLinearGradientBrush(Colors.Gray);
                        //下限位
                        if (S7.GetBitAt(PlcReadDataBuffer, 48, 1))
                            ServiceLocator.Current.GetInstance<EquipmentPageViewModel>().LowerLimitBackground = TemplateHelper.GetLinearGradientBrush(Colors.LightGreen);
                        else
                            ServiceLocator.Current.GetInstance<EquipmentPageViewModel>().LowerLimitBackground = TemplateHelper.GetLinearGradientBrush(Colors.Gray);
                        //故障代码
                        byte[] dataCode = new byte[] { PlcReadDataBuffer[17], PlcReadDataBuffer[16] };
                        ServiceLocator.Current.GetInstance<EquipmentPageViewModel>().FaultCode = BitConverter.ToChar(dataCode, 0).ToString();
                        //当前位置
                        byte[] dataLocation = new byte[] { PlcReadDataBuffer[21], PlcReadDataBuffer[20], PlcReadDataBuffer[19], PlcReadDataBuffer[18] };
                        ServiceLocator.Current.GetInstance<EquipmentPageViewModel>().CurrentLocation = Math.Round(BitConverter.ToSingle(dataLocation, 0), 3).ToString();
                        //当前速度
                        byte[] dataSpeed = new byte[] { PlcReadDataBuffer[25], PlcReadDataBuffer[24], PlcReadDataBuffer[23], PlcReadDataBuffer[22] };
                        ServiceLocator.Current.GetInstance<EquipmentPageViewModel>().CurrentSpeed = Math.Round(BitConverter.ToSingle(dataSpeed, 0), 3).ToString();
                        //零点补偿反馈
                        byte[] dataZero = new byte[] { PlcReadDataBuffer[31], PlcReadDataBuffer[30], PlcReadDataBuffer[29], PlcReadDataBuffer[28] };
                        ServiceLocator.Current.GetInstance<EquipmentPageViewModel>().ZeroOffsetBack = Math.Round(BitConverter.ToSingle(dataZero, 0), 3).ToString();
                        //点动速度反馈
                        byte[] dataJog = new byte[] { PlcReadDataBuffer[35], PlcReadDataBuffer[34], PlcReadDataBuffer[33], PlcReadDataBuffer[32] };
                        ServiceLocator.Current.GetInstance<EquipmentPageViewModel>().JogSpeedBack = Math.Round(BitConverter.ToSingle(dataJog, 0), 3).ToString();

                        //绝对运动速度
                        byte[] dataAbsolute = new byte[] { PlcReadDataBuffer[43], PlcReadDataBuffer[42], PlcReadDataBuffer[41], PlcReadDataBuffer[40] };
                        ServiceLocator.Current.GetInstance<EquipmentPageViewModel>().AbsoluteVelocitySpeedBack = Math.Round(BitConverter.ToSingle(dataAbsolute, 0), 3).ToString();
                        //当前轮毂运行高度
                        byte[] dataLight = new byte[] { PlcReadDataBuffer[47], PlcReadDataBuffer[46], PlcReadDataBuffer[45], PlcReadDataBuffer[44] };
                        ServiceLocator.Current.GetInstance<EquipmentPageViewModel>().LightLocationBack = Math.Round(BitConverter.ToSingle(dataLight, 0), 3).ToString();
                    }));
                    await System.Threading.Tasks.Task.Delay(400);
                }
            });
        }



        /// <summary>
        /// 写入机器人方法
        /// </summary>
        /// <param name="aController"></param>
        /// <param name="moduleName"></param>
        /// <param name="boolVarName">Bool变量名称</param>
        /// <param name="boolValue"></param>
        private void WriteBoolToRAPID(ABB.Robotics.Controllers.Controller aController, string moduleName, string boolVarName, bool boolValue)
        {
            try
            {

                if (aController != null && aController.Connected && aController.OperatingMode == ControllerOperatingMode.Auto)
                {
                    ABB.Robotics.Controllers.RapidDomain.Task tRob1 = aController.Rapid.GetTask("T_ROB1");
                    if (tRob1 != null)
                    {
                        RapidData rd = tRob1.GetRapidData(moduleName, boolVarName);
                        if (rd.Value is ABB.Robotics.Controllers.RapidDomain.Bool)
                        {
                            ABB.Robotics.Controllers.RapidDomain.Bool rapidBool = (ABB.Robotics.Controllers.RapidDomain.Bool)rd.Value;
                            if (rapidBool.Value != boolValue) //数据改变
                            {
                                Console.WriteLine($"数据改变-地址：{boolVarName} 值：{rapidBool.Value}-PLC值：{boolValue}");
                                rapidBool.Value = boolValue;
                                if (aController.Rapid != null)
                                {

                                    using (Mastership.Request(aController.Rapid))
                                    {
                                        rd.Value = rapidBool;
                                    }
                                }

                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageShow(ex.Message);
            }
        }

        /// <summary>
        /// 断机器人使能
        /// </summary>
        private void StopRobot()
        {
            try
            {
                if (RobotController != null && RobotController.Connected)
                {
                    if (RobotController.OperatingMode == ControllerOperatingMode.Auto)
                    {
                        //if (RobotController.Rapid.ExecutionStatus != ExecutionStatus.Stopped)
                        {
                            using (Mastership.Request(RobotController.Rapid))
                            {
                                RobotController.Rapid.Stop(StopMode.Immediate);
                            }
                            if (RobotController.State != ControllerState.MotorsOff)
                            {
                                RobotController.State = ControllerState.MotorsOff;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageShow(ex.Message);
            }
        }

        /// <summary>
        /// 电机状态设置
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string MotorStatusSet(int value)
        {
            if (value == 0) return "停止";
            else if (value == 1) return "运行";
            else if (value == 2) return "节能";
            else if (value == 3) return "故障";
            else return "";
        }

        /// <summary>
        /// 初始化控制器
        /// </summary>
        public void InitializeController()
        {
            //ABB.Robotics.Controllers.PC
            if (Scanner == null)
                Scanner = new NetworkScanner();
            Scanner.Scan();
            ControllerInfoCollection infos = Scanner.Controllers;
            if (infos.Count > 0)
            {
                RobotController = new ABB.Robotics.Controllers.Controller(infos[0]);
                RobotController.Logon(UserInfo.DefaultUser);
                if (RobotController.Connected)
                {
                    RobotController.StateChanged += Controller_StateChanged;
                    RobotController.Rapid.ExecutionStatusChanged += Rapid_ExecutionStatusChanged;
                    RobotController.OperatingModeChanged += Controller_OperatingModeChanged;
                    RobotController.ConnectionChanged += RobotController_ConnectionChanged;
                    //机器人TCP速度
                    Signal sigSpeed = RobotController.IOSystem.GetSignal("ao_speed");
                    sigSpeed.Changed += SigSpeed_Changed;
                    //机器人浮动锁紧控制信号
                    Signal sigLocal_DO1 = RobotController.IOSystem.GetSignal("Local_IO_0_DO1");

                    sigLocal_DO1.Changed += SigLocal_DO1_Changed;
                    //机器人打磨吹扫控制信号
                    Signal sigLocal_DO2 = RobotController.IOSystem.GetSignal("Local_IO_0_DO2");
                    sigLocal_DO2.Changed += SigLocal_DO2_Changed;
                    //机器人浮动磨头控制信号
                    Signal sigLocal_DO3 = RobotController.IOSystem.GetSignal("Local_IO_0_DO3");
                    sigLocal_DO3.Changed += SigLocal_DO3_Changed;
                    //机器人加工状态
                    RapidData processingState = RobotController.Rapid.GetRapidData("T_ROB1", "Data", "processingState");
                    processingState.ValueChanged += ProcessingState_ValueChanged;
                    //气源压力信号状态
                    Signal airPressState = RobotController.IOSystem.GetSignal("Local_IO_0_DI1");
                    airPressState.Changed += AirPressState_Changed;
                    //机器人手自动
                    Signal auto = RobotController.IOSystem.GetSignal("Local_IO_0_DO9");
                    auto.Changed += Auto_Changed;
                    //机器人在原点
                    Signal origin = RobotController.IOSystem.GetSignal("Local_IO_0_DO10");
                    origin.Changed += Origin_Changed;
                    //机器人在工作
                    Signal work = RobotController.IOSystem.GetSignal("Local_IO_0_DO11");
                    work.Changed += Work_Changed;
                    //机器人运行完成
                    Signal workDown = RobotController.IOSystem.GetSignal("Local_IO_0_DO12");
                    workDown.Changed += WorkDown_Changed;
                    //机器人工作干涉区 =1的时候在工作位
                    Signal workInterference = RobotController.IOSystem.GetSignal("Local_IO_0_DO13");
                    workInterference.Changed += WorkInterference_Changed;

                    InitDataStream();
                    //浮动磨头锁紧
                    Signal do1 = RobotController.IOSystem.GetSignal("Local_IO_0_DO1");
                    DigitalSignal sig = (DigitalSignal)do1;
                    if (Convert.ToBoolean(sig.Value) != true) sig.Set();

                    if (RobotController.Rapid.ExecutionStatus == ExecutionStatus.Running)
                    {
                        ExecutionState = "启动";
                        ButtonEnabled = false;
                        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            ExecutionStateBackground = TemplateHelper.GetLinearGradientBrush(Colors.LightGreen);
                        }));
                    }
                    else if (RobotController.Rapid.ExecutionStatus == ExecutionStatus.Stopped)
                    {
                        ExecutionState = "停止";
                        ButtonEnabled = true;
                        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            ExecutionStateBackground = TemplateHelper.GetLinearGradientBrush(Colors.LightPink);
                        }));
                    }
                    else
                    {
                        ExecutionState = "未知";
                        ButtonEnabled = true;
                        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            ExecutionStateBackground = TemplateHelper.GetLinearGradientBrush(Colors.Yellow);
                        }));
                    }

                    MessageShow("机器人控制器连接成功！");
                    RobotConnectionState = "连接成功";
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        RobotConnectBackground = TemplateHelper.GetLinearGradientBrush(Colors.LightGreen);
                    }));
                }
                else
                {
                    MessageShow("机器人控制器连接失败，请检查控制器连接！");
                    RobotConnectionState = "连接失败";
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        RobotConnectBackground = TemplateHelper.GetLinearGradientBrush(Colors.Red);
                    }));
                }
            }
            else
            {
                MessageShow("未扫描到网络中的控制器，请检查网络！");
                RobotConnectionState = "连接失败";
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    RobotConnectBackground = TemplateHelper.GetLinearGradientBrush(Colors.Red);
                }));
            }
        }





        /// <summary>
        /// 初始化数据流
        /// </summary>
        private void InitDataStream()
        {
            ABB.Robotics.Controllers.RapidDomain.Task tRob1 = RobotController.Rapid.GetTask("T_ROB1");
            if (tRob1 != null)
            {
                //目标数量
                TargetsNum = tRob1.GetRapidData("Data", "targetsNum");
                //轨迹点
                LocusPoints = tRob1.GetRapidData("Data", "locusPoints");
                //运行控制
                RunControl = tRob1.GetRapidData("Data", "runControl");
                //加工状态
                RobotProcessingStateValue = tRob1.GetRapidData("Data", "processingState");
            }
        }

        /// <summary>
        /// 发送加工轨迹数据
        /// </summary>
        /// <param name="datas"></param>
        public void SendLocusDatas(List<MachiningPathPosModel> datas)
        {
            //将目标点数量发送给机器人
            Num numValue = new Num();
            numValue.FillFromString2(datas.Count.ToString());
            using (Mastership.Request(RobotController.Rapid))
            {
                TargetsNum.Value = numValue;
            }
            //发送轨迹数据
            RobTarget point;
            try
            {
                for (int i = 0; i < datas.Count; i++)
                {
                    point = new RobTarget();
                    //外轴数据中，第1位为路径点编号，第4位为进出刀点标记，第5位为路径起点标记，第6位为路径终点标记
                    //point.FillFromString2("[" + "[" + datas[i].X + "," + datas[i].Y + "," + datas[i].Z + "]" + "," + "[" + datas[i].Q1 + "," + datas[i].Q2 + "," + datas[i].Q3 + "," + datas[i].Q4 + "]" + "," + "[0,0,0,0]" + "," + $"[{RobotHelper.SendNumber},0,0,{datas[i].EX},{datas[i].EY},{datas[i].EZ}]" + "]");
                    point.FillFromString2("[" + "[" + datas[i].X + "," + datas[i].Y + "," + datas[i].Z + "]" + "," + "[" + datas[i].Q1 + "," + datas[i].Q2 + "," + datas[i].Q3 + "," + datas[i].Q4 + "]" + "," + "[-1,0,-1,0]" + "," + "[9E+09,9E+09,9E+09,9E+09,9E+09,9E+09]" + "]");
                    //Console.WriteLine("点位置：" + i + " [" + "[" + datas[i].X + "," + datas[i].Y + "," + datas[i].Z + "]" + "," + "[" + datas[i].Q1 + "," + datas[i].Q2 + "," + datas[i].Q3 + "," + datas[i].Q4 + "]" + "," + "[-1,0,-1,0]" + "," + "[9E+09,9E+09,9E+09,9E+09,9E+09,9E+09]" + "]");
                    using (Mastership.Request(RobotController.Rapid))
                    {
                        LocusPoints.WriteItem(point, i);
                    }
                    //发送到NumberOfCoordinates个坐标时启动加工
                    if (i == RobotHelper.NumberOfCoordinates)
                    {
                        //发送1为启动加工
                        RunControlFlag.FillFromString2("1");
                        using (Mastership.Request(RobotController.Rapid))
                        {
                            RunControl.Value = RunControlFlag;
                        }
                    }
                    //如果发送轨迹过程中机器人停止，则停止发送轨迹
                    if (RobotController.Rapid.ExecutionStatus == ExecutionStatus.Stopped)
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("SendLocusDatas" + ex.ToString());
                throw ex;
            }
        }

        /// <summary>
        /// 停止加工
        /// </summary>
        public void ToStop()
        {
            if (RobotController != null && RobotController.Connected && RobotController.OperatingMode == ControllerOperatingMode.Auto &&
                RobotController.Rapid.ExecutionStatus == ExecutionStatus.Running)
            {
                try
                {
                    //发送-1为停止加工
                    RunControlFlag.FillFromString2("-1");
                    using (Mastership.Request(RobotController.Rapid))
                    {
                        RunControl.Value = RunControlFlag;
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            //关闭线程
            ExternalConnectionThreadControl = false;
            PlcHelper.ReadWritePlcThreadControl = false;
            HubDeburrThreadControl = false;
            //释放机器人控制器连接
            if (RobotController != null && RobotController.Connected)
            {
                try
                {
                    if (RobotController.OperatingMode == ControllerOperatingMode.Auto)
                    {
                        using (Mastership.Request(RobotController.Rapid))
                        {
                            RobotController.Rapid.Stop(StopMode.Immediate);
                        }
                    }
                }
                catch { }
                RobotController.Logoff();
                RobotController.Dispose();
                RobotController = null;
            }
            //释放PLC连接
            if (PlcHelper.PlcCilent.Connected)
            {
                PlcHelper.PlcCilent.Disconnect();
            }
            //释放相机
            if (TemplateHelper.CameraHandle != null)
            {
                HOperatorSet.CloseAllFramegrabbers();
                TemplateHelper.CameraHandle = null;
            }
        }

        #region=========机器人某个状态改变事件=========
        //连接状态改变
        private void RobotController_ConnectionChanged(object sender, ConnectionChangedEventArgs e)
        {
            if (RobotController.Connected == true)
            {
                MessageShow("机器人控制器连接成功！");
                RobotConnectionState = "连接成功";
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    RobotConnectBackground = TemplateHelper.GetLinearGradientBrush(Colors.LightGreen);
                }));
            }
            else
            {
                MessageShow("机器人控制器连接失败，请检查控制器连接！");
                RobotConnectionState = "连接失败";
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    RobotConnectBackground = TemplateHelper.GetLinearGradientBrush(Colors.Red);
                    RobotControllerState = "";
                    ControllerStateBackground = TemplateHelper.GetLinearGradientBrush(Colors.Gray);
                    OperatingMode = "";
                    OperatingModeBackground = TemplateHelper.GetLinearGradientBrush(Colors.Gray);
                    ExecutionState = "";
                    ExecutionStateBackground = TemplateHelper.GetLinearGradientBrush(Colors.Gray);
                    ProcessingState = "";
                    ProcessingStateBackground = TemplateHelper.GetLinearGradientBrush(Colors.Gray);
                    RobotSpeed = "";
                }));
            }
        }
        //速度改变
        private void SigSpeed_Changed(object sender, SignalChangedEventArgs e)
        {
            try
            {
                Signal s = (Signal)sender;
                RobotSpeed = Math.Round(s.Value * 1000).ToString();
            }
            catch { }
        }
        //控制器操作模式改变
        private void Controller_OperatingModeChanged(object sender, OperatingModeChangeEventArgs e)
        {
            if (RobotController.OperatingMode == ControllerOperatingMode.Auto)
            {
                OperatingMode = "自动";
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    OperatingModeBackground = TemplateHelper.GetLinearGradientBrush(Colors.LightGreen);
                }));
            }
            else if (RobotController.OperatingMode == ControllerOperatingMode.Init)
            {
                OperatingMode = "初始化";
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    OperatingModeBackground = TemplateHelper.GetLinearGradientBrush(Colors.Gray);
                }));
            }
            else if (RobotController.OperatingMode == ControllerOperatingMode.ManualReducedSpeed)
            {
                OperatingMode = "手动减速";
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    OperatingModeBackground = TemplateHelper.GetLinearGradientBrush(Colors.LightPink);
                }));
            }
            else if (RobotController.OperatingMode == ControllerOperatingMode.ManualFullSpeed)
            {
                OperatingMode = "手动全速";
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    OperatingModeBackground = TemplateHelper.GetLinearGradientBrush(Colors.LightBlue);
                }));
            }
            else if (RobotController.OperatingMode == ControllerOperatingMode.AutoChange)
            {
                OperatingMode = "已请求更改为自动模式";
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    OperatingModeBackground = TemplateHelper.GetLinearGradientBrush(Colors.Yellow);
                }));
            }
            else if (RobotController.OperatingMode == ControllerOperatingMode.ManualFullSpeedChange)
            {
                OperatingMode = "已请求更改为手动全速";
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    OperatingModeBackground = TemplateHelper.GetLinearGradientBrush(Colors.Yellow);
                }));
            }
            else if (RobotController.OperatingMode == ControllerOperatingMode.NotApplicable)
            {
                OperatingMode = "控制器操作模式不适用于当前控制器状态";
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    OperatingModeBackground = TemplateHelper.GetLinearGradientBrush(Colors.Yellow);
                }));
            }
        }
        //Rapid执行状态改变
        private void Rapid_ExecutionStatusChanged(object sender, ExecutionStatusChangedEventArgs e)
        {
            if (RobotController.Connected)
            {
                try
                {
                    if (RobotController.Rapid.ExecutionStatus == ExecutionStatus.Running)
                    {
                        ExecutionState = "启动";
                        ButtonEnabled = false;
                        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            ExecutionStateBackground = TemplateHelper.GetLinearGradientBrush(Colors.LightGreen);
                        }));
                    }
                    else if (RobotController.Rapid.ExecutionStatus == ExecutionStatus.Stopped)
                    {
                        if (RobotProcessingState == 2)
                        {
                            Signal do3 = RobotController.IOSystem.GetSignal("Local_IO_0_DO3");
                            DigitalSignal sig3 = (DigitalSignal)do3;
                            if (Convert.ToBoolean(sig3.Value) != false)
                                sig3.Reset();
                            Signal do2 = RobotController.IOSystem.GetSignal("Local_IO_0_DO2");
                            DigitalSignal sig2 = (DigitalSignal)do2;
                            if (Convert.ToBoolean(sig2.Value) != false)
                                sig2.Reset();
                        }
                        ExecutionState = "停止";
                        ButtonEnabled = true;
                        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            ExecutionStateBackground = TemplateHelper.GetLinearGradientBrush(Colors.LightPink);
                        }));
                    }
                    else
                    {
                        ExecutionState = "未知";
                        ButtonEnabled = true;
                        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            ExecutionStateBackground = TemplateHelper.GetLinearGradientBrush(Colors.Yellow);
                        }));
                    }
                }
                catch { }
            }
        }
        //控制器状态改变
        private void Controller_StateChanged(object sender, StateChangedEventArgs e)
        {
            if (RobotController.Connected == true)
            {
                try
                {
                    if (RobotController.State == ABB.Robotics.Controllers.ControllerState.Init)
                    {
                        RobotControllerState = "初始化";
                        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            ControllerStateBackground = TemplateHelper.GetLinearGradientBrush(Colors.Gray);
                        }));
                    }
                    else if (RobotController.State == ABB.Robotics.Controllers.ControllerState.MotorsOff)
                    {
                        RobotControllerState = "电机下电";
                        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            ControllerStateBackground = TemplateHelper.GetLinearGradientBrush(Colors.Yellow);
                        }));
                    }
                    else if (RobotController.State == ABB.Robotics.Controllers.ControllerState.MotorsOn)
                    {
                        RobotControllerState = "电机上电";
                        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            ControllerStateBackground = TemplateHelper.GetLinearGradientBrush(Colors.LightGreen);
                        }));
                    }
                    else if (RobotController.State == ABB.Robotics.Controllers.ControllerState.GuardStop)
                    {
                        RobotControllerState = "保护停止";
                        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            ControllerStateBackground = TemplateHelper.GetLinearGradientBrush(Colors.Red);
                        }));
                    }
                    else if (RobotController.State == ABB.Robotics.Controllers.ControllerState.EmergencyStop)
                    {
                        RobotControllerState = "紧急停止";
                        S7.SetBitAt(PlcHelper.PlcWriteDataBuffer, 1, 0, false);
                        StartStopButtonContent = "系统停止";
                        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            ControllerStateBackground = TemplateHelper.GetLinearGradientBrush(Colors.Red);
                        }));
                    }
                    else if (RobotController.State == ABB.Robotics.Controllers.ControllerState.EmergencyStopReset)
                    {
                        RobotControllerState = "紧急停止复位";
                        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            ControllerStateBackground = TemplateHelper.GetLinearGradientBrush(Colors.Yellow);
                        }));
                    }
                    else if (RobotController.State == ABB.Robotics.Controllers.ControllerState.SystemFailure)
                    {
                        RobotControllerState = "系统故障";
                        S7.SetBitAt(PlcHelper.PlcWriteDataBuffer, 1, 0, false);
                        StartStopButtonContent = "系统停止";
                        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            ControllerStateBackground = TemplateHelper.GetLinearGradientBrush(Colors.Red);
                        }));
                    }
                    else if (RobotController.State == ABB.Robotics.Controllers.ControllerState.Unknown)
                    {
                        RobotControllerState = "未知";
                        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            ControllerStateBackground = TemplateHelper.GetLinearGradientBrush(Colors.Gray);
                        }));
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// 浮动磨头控制电磁阀状态改变委托
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SigLocal_DO3_Changed(object sender, SignalChangedEventArgs e)
        {
            Signal s = (Signal)sender;
            if (Convert.ToBoolean(s.Value))
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    ServiceLocator.Current.GetInstance<EquipmentPageViewModel>().DO3Background = TemplateHelper.GetLinearGradientBrush(Colors.LightGreen);
                }));
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    ServiceLocator.Current.GetInstance<EquipmentPageViewModel>().DO3Background = TemplateHelper.GetLinearGradientBrush(Colors.OrangeRed);
                }));
            }
        }

        /// <summary>
        /// 打磨吹扫控制电磁阀状态改变委托
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SigLocal_DO2_Changed(object sender, SignalChangedEventArgs e)
        {
            Signal s = (Signal)sender;
            if (Convert.ToBoolean(s.Value))
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    ServiceLocator.Current.GetInstance<EquipmentPageViewModel>().DO2Background = TemplateHelper.GetLinearGradientBrush(Colors.LightGreen);
                }));
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    ServiceLocator.Current.GetInstance<EquipmentPageViewModel>().DO2Background = TemplateHelper.GetLinearGradientBrush(Colors.OrangeRed);
                }));
            }
        }

        /// <summary>
        /// 浮动锁紧控制电磁阀状态改变委托
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SigLocal_DO1_Changed(object sender, SignalChangedEventArgs e)
        {
            Signal s = (Signal)sender;
            if (Convert.ToBoolean(s.Value))
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    ServiceLocator.Current.GetInstance<EquipmentPageViewModel>().DO1Background = TemplateHelper.GetLinearGradientBrush(Colors.LightGreen);
                }));
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    ServiceLocator.Current.GetInstance<EquipmentPageViewModel>().DO1Background = TemplateHelper.GetLinearGradientBrush(Colors.OrangeRed);
                }));
            }
        }

        /// <summary>
        /// 加工状态改变委托方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProcessingState_ValueChanged(object sender, DataValueChangedEventArgs e)
        {
            RapidData result = sender as RapidData;
            RobotProcessingState = int.Parse(result.Value.ToString());
            if (RobotProcessingState == 0)
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    ProcessingState = "加工停止";
                    ProcessingStateBackground = TemplateHelper.GetLinearGradientBrush(Colors.LightYellow);
                    MessageShow("加工停止");
                }));
            }
            else if (RobotProcessingState == 1)
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    ProcessingState = "加工完成";
                    ProcessingStateBackground = TemplateHelper.GetLinearGradientBrush(Colors.LightGreen);
                    MessageShow("加工完成");
                }));
            }
            else if (RobotProcessingState == 2)
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    ProcessingState = "加工中...";
                    ProcessingStateBackground = TemplateHelper.GetLinearGradientBrush(Colors.LightBlue);
                    MessageShow("加工中...");
                }));
            }
            else if (RobotProcessingState == 3)
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    ProcessingState = "加工异常！";
                    ProcessingStateBackground = TemplateHelper.GetLinearGradientBrush(Colors.Red);
                    MessageShow("加工异常！");
                }));
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    ProcessingState = "";
                    ProcessingStateBackground = TemplateHelper.GetLinearGradientBrush(Colors.Gray);
                }));
            }
        }

        /// <summary>
        /// 气源状态改变委托方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AirPressState_Changed(object sender, SignalChangedEventArgs e)
        {
            Signal s = (Signal)sender;
            if (Convert.ToBoolean(s.Value))
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    AirPressStateBackground = TemplateHelper.GetLinearGradientBrush(Colors.Green);
                }));
                AirPressState = true;
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    AirPressStateBackground = TemplateHelper.GetLinearGradientBrush(Colors.Red);
                }));
                AirPressState = false;
            }
        }
        /// <summary>
        /// 机器人手自动切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void Auto_Changed(object sender, SignalChangedEventArgs e)
        {
            Signal s = (Signal)sender;
            if (!Convert.ToBoolean(s.Value)) //手动模式
            {
                Console.WriteLine("手动模式");
                S7.SetBitAt(PlcHelper.PlcWriteDataBuffer, 6, 2, false); //拍照完成
                S7.SetBitAt(PlcHelper.PlcWriteDataBuffer, 24, 0, false); //机器人完成工作
                S7.SetBitAt(PlcHelper.PlcWriteDataBuffer, 0, 0, false); //手动模式
                S7.SetBitAt(PlcHelper.PlcWriteDataBuffer, 1, 0, false);
                S7.SetBitAt(PlcHelper.PlcWriteDataBuffer, 0, 6, false);
                OperatingModeSwitchingButtonContent = "手动模式";
                StartStopButtonContent = "系统停止";
                ServiceLocator.Current.GetInstance<EquipmentPageViewModel>().ManualButtonEnable = true;
            }
        }
        /// <summary>
        /// 原点状态改变委托方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void Origin_Changed(object sender, SignalChangedEventArgs e)
        {
            Signal s = (Signal)sender;
            //Console.WriteLine(s.Value);
            if (Convert.ToBoolean(s.Value))
            {
                S7.SetBitAt(PlcHelper.PlcWriteDataBuffer, 6, 0, true); //机器人在原点
            }
            else
            {
                S7.SetBitAt(PlcHelper.PlcWriteDataBuffer, 6, 0, false); //
            }
        }

        /// <summary>
        /// 机器人在工作状态改变委托方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Work_Changed(object sender, SignalChangedEventArgs e)
        {
            Signal s = (Signal)sender;
            //Console.WriteLine(s.Value);
            if (Convert.ToBoolean(s.Value))
            {
                Console.WriteLine($"机器人在工作true");
                S7.SetBitAt(PlcHelper.PlcWriteDataBuffer, 6, 1, true); //机器人在工作

            }
            else
            {
                Console.WriteLine($"机器人在工作false");
                S7.SetBitAt(PlcHelper.PlcWriteDataBuffer, 6, 1, false); //
            }
        }
        /// <summary>
        /// 机器人工作完成委托方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WorkDown_Changed(object sender, SignalChangedEventArgs e)
        {
            Signal s = (Signal)sender;
            if (Convert.ToBoolean(s.Value))
            {
                S7.SetBitAt(PlcHelper.PlcWriteDataBuffer, 24, 0, true); //机器人完成工作
            }

        }
        /// <summary>
        /// 机器人工作干涉区变化委托方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WorkInterference_Changed(object sender, SignalChangedEventArgs e)
        {
            Signal s = (Signal)sender;
            if (Convert.ToBoolean(s.Value))  //在干涉区
            {
                S7.SetBitAt(PlcHelper.PlcWriteDataBuffer, 24, 3, true);
            }
            else //不在干涉区
            {
                S7.SetBitAt(PlcHelper.PlcWriteDataBuffer, 24, 3, false);
            }

        }

        #endregion

        #region=========机器人功能按钮=========
        //PP移至Main
        private void PpToMain()
        {
            if (RobotController == null || !RobotController.Connected)
            {
                UMessageBox.Show("未连接控制器！", MessageType.Default);
                MessageShow("未连接控制器！");
                return;
            }
            if (RobotController.OperatingMode != ControllerOperatingMode.Auto)
            {
                UMessageBox.Show("请将控制器切换到自动模式！", MessageType.Default);
                return;
            }
            try
            {
                var tasks = RobotController.Rapid.GetTask("T_ROB1");
                using (Mastership.Request(RobotController.Rapid))
                {
                    tasks.ResetProgramPointer();
                }
                MessageShow("PP移至Main");
            }
            catch (InvalidOperationException ex)
            {
                MessageShow("权限被其他客服端占有：" + ex.Message);
            }
            catch (Exception ex)
            {
                MessageShow(ex.Message);
            }
        }
        //电机下电
        private void MotorOff()
        {
            if (RobotController == null || !RobotController.Connected)
            {
                UMessageBox.Show("未连接控制器！", MessageType.Default);
                MessageShow("未连接控制器！");
                return;
            }
            if (RobotController.OperatingMode != ControllerOperatingMode.Auto)
            {
                UMessageBox.Show("请将控制器切换到自动模式！", MessageType.Default);
                return;
            }
            try
            {
                if (RobotController.State != ControllerState.MotorsOff)
                {
                    RobotController.State = ControllerState.MotorsOff;
                    MessageShow("电机下电");
                }
            }
            catch (Exception ex)
            {
                MessageShow(ex.Message);
            }
        }
        //电机上电
        private void MotorOn()
        {
            if (RobotController == null || !RobotController.Connected)
            {
                UMessageBox.Show("未连接控制器！", MessageType.Default);
                MessageShow("未连接控制器！");
                return;
            }
            if (RobotController.OperatingMode != ControllerOperatingMode.Auto)
            {
                UMessageBox.Show("请将控制器切换到自动模式！", MessageType.Default);
                return;
            }
            try
            {
                if (RobotController.State != ControllerState.MotorsOn)
                {
                    RobotController.State = ControllerState.MotorsOn;
                    MessageShow("电机上电");
                }
            }
            catch (Exception ex)
            {

                MessageShow(ex.Message);
            }
        }
        //机器人停止
        private void RobotStop()
        {
            if (RobotController == null || !RobotController.Connected)
            {
                UMessageBox.Show("未连接控制器！", MessageType.Default);
                MessageShow("未连接控制器！");
                return;
            }
            if (RobotController.OperatingMode != ControllerOperatingMode.Auto)
            {
                UMessageBox.Show("请将控制器切换到自动模式！", MessageType.Default);
                return;
            }
            if (RobotController.Rapid.ExecutionStatus == ExecutionStatus.Stopped)
            {
                return;
            }
            bool result = UMessageBox.Show("RAPID 停止确认", "确定停止 RAPID 执行吗？");
            if (result)
            {
                try
                {
                    using (Mastership.Request(RobotController.Rapid))
                    {
                        RobotController.Rapid.Stop(StopMode.Immediate);
                    }
                    MessageShow("RAPID 执行停止");
                }
                catch (Exception ex)
                {
                    MessageShow(ex.Message);
                }
            }
        }
        //机器人启动
        private void RobotStart()
        {
            if (RobotController == null || !RobotController.Connected)
            {
                UMessageBox.Show("未连接控制器！", MessageType.Default);
                MessageShow("未连接控制器！");
                return;
            }
            if (RobotController.OperatingMode != ControllerOperatingMode.Auto)
            {
                UMessageBox.Show("请将控制器切换到自动模式！", MessageType.Default);
                return;
            }
            if (RobotController.State != ControllerState.MotorsOn)
            {
                UMessageBox.Show("电机未上电！", MessageType.Default);
                return;
            }
            if (RobotController.Rapid.ExecutionStatus == ExecutionStatus.Running)
            {
                return;
            }
            try
            {
                using (Mastership.Request(RobotController.Rapid))
                {
                    StartResult result = RobotController.Rapid.Start();
                }
                MessageShow("机器人加工启动");
            }
            catch (Exception ex)
            {
                MessageShow(ex.Message);
            }
        }
        //控制器刷新
        private void Flushed()
        {
            if (RobotController == null || !RobotController.Connected)
                InitializeController();
        }
        #endregion

        /// <summary>
        /// 信息显示
        /// </summary>
        /// <param name="str"></param>
        public void MessageShow(string str)
        {
            App.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                MessageList.Add(new MessageModel { DateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Message = str });
                // 发送滚动请求消息
                Messenger.Default.Send(new NotificationMessage("ScrollToEnd"));
            }));
        }

        /// <summary>
        /// 显示结果
        /// </summary>
        /// <param name="wheelType">轮毂型号</param>
        /// <param name="similarity">相似度</param>
        /// <param name="rotationAngle">角度</param>
        /// <param name="centerRow">中心行坐标</param>
        /// <param name="centerColumn">中心列坐标</param>
        /// <param name="centerRowOffset">中心行偏移</param>
        /// <param name="centerColumnOffset">中心列偏移</param>
        /// <param name="startTime">处理开始时间</param>
        private void DisplayResult(string wheelType, string similarity, string rotationAngle, string centerRow, string centerColumn, string centerRowOffset, string centerColumnOffset, string totalPoints, string processingResult, DateTime startTime)
        {
            WheelType = wheelType;
            Similarity = similarity;
            RotationAngle = rotationAngle;
            ImageCenterRow = centerRow;
            ImageCenterColumn = centerColumn;
            ImageCenterRowOffset = centerRowOffset;
            ImageCenterColumnOffset = centerColumnOffset;
            TotalPoints = totalPoints;
            ProcessingResult = processingResult;
            var endTime = DateTime.Now;
            TimeSpan processingTime = endTime.Subtract(startTime);
            ProcessingTime = Math.Round(processingTime.TotalSeconds, 2).ToString() + "s";
            ProcessingResultDisplayed = Visibility.Visible;
        }

        /// <summary>
        /// 保存结果数据
        /// </summary>
        /// <param name="wheelModel">轮毂型号</param>
        /// <param name="processingTime">加工时的时间</param>
        /// <param name="consumingTime">加工消耗的时间</param>
        /// <param name="processingResult">处理结果</param>
        /// <param name="resultImage">结果图像</param>
        public void SaveResultData(string wheelModel, DateTime processingTime, string consumingTime, string processingResult, HObject resultImage)
        {
            SaveDatasState = true;
            System.Threading.Tasks.Task.Run(() =>
            {
                var pDB = new SqlAccess().ProductionDataAccess;
                int id = pDB.Queryable<ProductionDatas>().Max(it => it.Id);
                string sTime = processingTime.ToString("yyyy-MM-dd HH:mm:ss");
                DateTime time = DateTime.Parse(sTime);
                ProductionDatas data = new ProductionDatas
                {
                    Id = id,
                    WheelModel = wheelModel,
                    ProcessingResult = processingResult,
                    ConsumingTime = consumingTime,
                    ProcessingTime = time
                };
                pDB.Insertable(data).ExecuteCommand();

                var month = Convert.ToString(processingTime.Month);//获取当前的月份
                var day = Convert.ToString(processingTime.Day);//获取当前的日期
                string Month_Path = @"D:\DeburrSystem\HistoricalImages\" + month + "月";//月文件夹路径
                string Day_Path = @"D:\DeburrSystem\HistoricalImages\" + month + @"月\" + day + "日";//日文件夹路径
                if (Directory.Exists(Month_Path) == false) Directory.CreateDirectory(Month_Path);
                if (Directory.Exists(Day_Path) == false) Directory.CreateDirectory(Day_Path);
                string wheelSavePath = @"D:\DeburrSystem\HistoricalImages\" + month + @"月\" + day + @"日\" + wheelModel.Trim('_');
                if (Directory.Exists(wheelSavePath) == false) Directory.CreateDirectory(wheelSavePath);
                var diskFree = GetHardDiskFreeSpace("D");//获取D盘剩余空间
                if (diskFree < 1024)
                {
                    MessageShow("磁盘存储空间不足，请检查！");
                    return;
                }
                if (wheelModel == "NG")
                {
                    string Unknown_path = $"D:/DeburrSystem/HistoricalImages/{month}月/{day}日/NG/NG_{processingTime:HHmmss}_{processingResult}.tif";
                    HOperatorSet.WriteImage(resultImage, "tiff", 0, Unknown_path);
                }
                else
                {
                    string Historical_Path = $"D:/DeburrSystem/HistoricalImages/{month}月/{day}日/{wheelModel.Trim('_')}/{wheelModel}_{processingTime:HHmmss}_{processingResult}.tif";
                    HOperatorSet.WriteImage(resultImage, "tiff", 0, Historical_Path);
                }
                SaveDatasState = false;
            });
        }

        ///  <summary> 
        /// 获取指定驱动器的剩余空间总大小(单位为MB) 
        ///  </summary> 
        ///  <param name="HardDiskName">代表驱动器的字母(必须大写字母) </param> 
        ///  <returns> </returns> 
        private static long GetHardDiskFreeSpace(string HardDiskName)
        {
            long freeSpace = new long();
            HardDiskName = HardDiskName + ":\\";
            System.IO.DriveInfo[] drives = System.IO.DriveInfo.GetDrives();
            foreach (System.IO.DriveInfo drive in drives)
            {
                if (drive.Name == HardDiskName)
                {
                    freeSpace = drive.TotalFreeSpace / (1024 * 1024);
                }
            }
            return freeSpace;
        }
    }
}
