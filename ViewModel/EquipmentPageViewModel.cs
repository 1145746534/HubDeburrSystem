using ABB.Robotics.Controllers.IOSystemDomain;
using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HubDeburrSystem.Public;
using HubDeburrSystem.Views.Dialog;
using RobotStudio.Services.RobApi.RobApi1;
using Sharp7;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace HubDeburrSystem.ViewModel
{
    public class EquipmentPageViewModel : ViewModelBase
    {
        #region ===================属性字段定义====================
        private LinearGradientBrush _pushrodCylinderBackground;
        /// <summary>
        /// 推杆气缸状态背景颜色
        /// </summary>
        public LinearGradientBrush PushrodCylinderBackground
        {
            get { return _pushrodCylinderBackground; }
            set
            {
                _pushrodCylinderBackground = value;
                RaisePropertyChanged("PushrodCylinderBackground");
            }
        }

        private LinearGradientBrush _fixedCenterBackground;
        /// <summary>
        /// 定中机构状态背景颜色
        /// </summary>
        public LinearGradientBrush FixedCenterBackground
        {
            get { return _fixedCenterBackground; }
            set
            {
                _fixedCenterBackground = value;
                RaisePropertyChanged("FixedCenterBackground");
            }
        }

        private LinearGradientBrush _do1Background;
        /// <summary>
        /// 浮动锁紧DO1状态背景颜色
        /// </summary>
        public LinearGradientBrush DO1Background
        {
            get { return _do1Background; }
            set
            {
                _do1Background = value;
                RaisePropertyChanged("DO1Background");
            }
        }

        private LinearGradientBrush _do2Background;
        /// <summary>
        /// 打磨吹扫DO2状态背景颜色
        /// </summary>
        public LinearGradientBrush DO2Background
        {
            get { return _do2Background; }
            set
            {
                _do2Background = value;
                RaisePropertyChanged("DO2Background");
            }
        }

        private LinearGradientBrush _do3Background;
        /// <summary>
        /// 打磨主轴DO3状态背景颜色
        /// </summary>
        public LinearGradientBrush DO3Background
        {
            get { return _do3Background; }
            set
            {
                _do3Background = value;
                RaisePropertyChanged("DO3Background");
            }
        }

        private string _inletMotorStatus;
        /// <summary>
        /// 入口电机状态
        /// </summary>
        public string InletMotorStatus
        {
            get { return _inletMotorStatus; }
            set
            {
                Set(ref _inletMotorStatus, value);
                InletMotorBackground = MotorStatusBackgroundSet(InletMotorStatus);
            }
        }

        private LinearGradientBrush _inletMotorBackground;
        /// <summary>
        /// 入口电机背景颜色
        /// </summary>
        public LinearGradientBrush InletMotorBackground
        {
            get { return _inletMotorBackground; }
            set
            {
                _inletMotorBackground = value;
                RaisePropertyChanged("InletMotorBackground");
            }
        }

        private string _processingMotorStatus;
        /// <summary>
        /// 加工位电机状态
        /// </summary>
        public string ProcessingMotorStatus
        {
            get { return _processingMotorStatus; }
            set
            {
                Set(ref _processingMotorStatus, value);
                ProcessingMotorBackground = MotorStatusBackgroundSet(ProcessingMotorStatus);
            }
        }

        private LinearGradientBrush _processingMotorBackground;
        /// <summary>
        /// 加工位电机背景颜色
        /// </summary>
        public LinearGradientBrush ProcessingMotorBackground
        {
            get { return _processingMotorBackground; }
            set
            {
                _processingMotorBackground = value;
                RaisePropertyChanged("ProcessingMotorBackground");
            }
        }

        private string _exportMotor1Status;
        /// <summary>
        /// 出口电机1状态
        /// </summary>
        public string ExportMotor1Status
        {
            get { return _exportMotor1Status; }
            set
            {
                Set(ref _exportMotor1Status, value);
                ExportMotor1Background = MotorStatusBackgroundSet(ExportMotor1Status);
            }
        }

        private LinearGradientBrush _exportMotor1Background;
        /// <summary>
        /// 出口电机1背景颜色
        /// </summary>
        public LinearGradientBrush ExportMotor1Background
        {
            get { return _exportMotor1Background; }
            set
            {
                _exportMotor1Background = value;
                RaisePropertyChanged("ExportMotor1Background");
            }
        }

        private string _exportMotor2Status;
        /// <summary>
        /// 出口电机2状态
        /// </summary>
        public string ExportMotor2Status
        {
            get { return _exportMotor2Status; }
            set
            {
                Set(ref _exportMotor2Status, value);
                ExportMotor2Background = MotorStatusBackgroundSet(ExportMotor2Status);
            }
        }

        private LinearGradientBrush _exportMotor2Background;
        /// <summary>
        /// 出口电机2背景颜色
        /// </summary>
        public LinearGradientBrush ExportMotor2Background
        {
            get { return _exportMotor2Background; }
            set
            {
                _exportMotor2Background = value;
                RaisePropertyChanged("ExportMotor2Background");
            }
        }

        private LinearGradientBrush _inletPhotoelectricityBackground;
        /// <summary>
        /// 入口光电状态背景颜色
        /// </summary>
        public LinearGradientBrush InletPhotoelectricityBackground
        {
            get { return _inletPhotoelectricityBackground; }
            set
            {
                _inletPhotoelectricityBackground = value;
                RaisePropertyChanged("InletPhotoelectricityBackground");
            }
        }

        private LinearGradientBrush _processingPhotoelectricityBackground;
        /// <summary>
        /// 加工位光电状态背景颜色
        /// </summary>
        public LinearGradientBrush ProcessingPhotoelectricityBackground
        {
            get { return _processingPhotoelectricityBackground; }
            set
            {
                _processingPhotoelectricityBackground = value;
                RaisePropertyChanged("ProcessingPhotoelectricityBackground");
            }
        }

        private LinearGradientBrush _exportPhotoelectricity1Background;
        /// <summary>
        /// 出口光电1状态背景颜色
        /// </summary>
        public LinearGradientBrush ExportPhotoelectricity1Background
        {
            get { return _exportPhotoelectricity1Background; }
            set
            {
                _exportPhotoelectricity1Background = value;
                RaisePropertyChanged("ExportPhotoelectricity1Background");
            }
        }

        private LinearGradientBrush _exportPhotoelectricity2Background;
        /// <summary>
        /// 出口光电2状态背景颜色
        /// </summary>
        public LinearGradientBrush ExportPhotoelectricity2Background
        {
            get { return _exportPhotoelectricity2Background; }
            set
            {
                _exportPhotoelectricity2Background = value;
                RaisePropertyChanged("ExportPhotoelectricity2Background");
            }
        }

        private LinearGradientBrush _pushrodInductionBackground;
        /// <summary>
        /// 推杆回位磁感应状态背景颜色
        /// </summary>
        public LinearGradientBrush PushrodInductionBackground
        {
            get { return _pushrodInductionBackground; }
            set
            {
                _pushrodInductionBackground = value;
                RaisePropertyChanged("PushrodInductionBackground");
            }
        }

        private LinearGradientBrush _fixedCenterInductionBackground;
        /// <summary>
        /// 定中机构回位磁感应状态背景颜色
        /// </summary>
        public LinearGradientBrush FixedCenterInductionBackground
        {
            get { return _fixedCenterInductionBackground; }
            set
            {
                _fixedCenterInductionBackground = value;
                RaisePropertyChanged("FixedCenterInductionBackground");
            }
        }

        private LinearGradientBrush _returnZeroCompletionBackground;
        /// <summary>
        /// 回零完成状态背景颜色
        /// </summary>
        public LinearGradientBrush ReturnZeroCompletionBackground
        {
            get { return _returnZeroCompletionBackground; }
            set
            {
                _returnZeroCompletionBackground = value;
                RaisePropertyChanged("ReturnZeroCompletionBackground");
            }
        }

        private LinearGradientBrush _enableCompletionBackground;
        /// <summary>
        /// 使能完成状态背景颜色
        /// </summary>
        public LinearGradientBrush EnableCompletionBackground
        {
            get { return _enableCompletionBackground; }
            set
            {
                _enableCompletionBackground = value;
                RaisePropertyChanged("EnableCompletionBackground");
            }
        }

        private LinearGradientBrush _enableErrorBackground;
        /// <summary>
        /// 使能错误状态背景颜色
        /// </summary>
        public LinearGradientBrush EnableErrorBackground
        {
            get { return _enableErrorBackground; }
            set
            {
                _enableErrorBackground = value;
                RaisePropertyChanged("EnableErrorBackground");
            }
        }

        private LinearGradientBrush _joggingBackground;
        /// <summary>
        /// 点动运行中状态背景颜色
        /// </summary>
        public LinearGradientBrush JoggingBackground
        {
            get { return _joggingBackground; }
            set
            {
                _joggingBackground = value;
                RaisePropertyChanged("JoggingBackground");
            }
        }

        private LinearGradientBrush _faultBackground;
        /// <summary>
        /// 故障状态背景颜色
        /// </summary>
        public LinearGradientBrush FaultBackground
        {
            get { return _faultBackground; }
            set
            {
                _faultBackground = value;
                RaisePropertyChanged("FaultBackground");
            }
        }

        private LinearGradientBrush _resetCompletedBackground;
        /// <summary>
        /// 复位完成状态背景颜色
        /// </summary>
        public LinearGradientBrush ResetCompletedBackground
        {
            get { return _resetCompletedBackground; }
            set
            {
                _resetCompletedBackground = value;
                RaisePropertyChanged("ResetCompletedBackground");
            }
        }

        private LinearGradientBrush _upperLimitBackground;
        /// <summary>
        /// 上限位背景颜色
        /// </summary>
        public LinearGradientBrush UpperLimitBackground
        {
            get { return _upperLimitBackground; }
            set
            {
                _upperLimitBackground = value;
                RaisePropertyChanged("UpperLimitBackground");
            }
        }

        private LinearGradientBrush _lowerLimitBackground;
        /// <summary>
        /// 下限位背景颜色
        /// </summary>
        public LinearGradientBrush LowerLimitBackground
        {
            get { return _lowerLimitBackground; }
            set
            {
                _lowerLimitBackground = value;
                RaisePropertyChanged("LowerLimitBackground");
            }
        }

        private string _faultCode;
        /// <summary>
        /// 故障代码
        /// </summary>
        public string FaultCode
        {
            get { return _faultCode; }
            set { Set(ref _faultCode, value); }
        }

        private string _currentLocation;
        /// <summary>
        /// 当前位置
        /// </summary>
        public string CurrentLocation
        {
            get { return _currentLocation; }
            set { Set(ref _currentLocation, value); }
        }

        private string _currentSpeed;
        /// <summary>
        /// 当前速度
        /// </summary>
        public string CurrentSpeed
        {
            get { return _currentSpeed; }
            set { Set(ref _currentSpeed, value); }
        }

        private bool _manualButtonEnable;
        /// <summary>
        /// 手动按钮使能
        /// </summary>
        public bool ManualButtonEnable
        {
            get { return _manualButtonEnable; }
            set { Set(ref _manualButtonEnable, value); }
        }



        private string _reachLocation;
        /// <summary>
        /// 到达位置
        /// </summary>
        public string ReachLocation
        {
            get { return _reachLocation; }
            set { Set(ref _reachLocation, value); }
        }

        private string _jogSpeed;
        /// <summary>
        /// 点动速度
        /// </summary>
        public string JogSpeed
        {
            get { return _jogSpeed; }
            set { Set(ref _jogSpeed, value); }
        }

        private string _zeroOffset;
        /// <summary>
        /// 零点偏移
        /// </summary>
        public string ZeroOffset
        {
            get { return _zeroOffset; }
            set { Set(ref _zeroOffset, value); }
        }

        private string _absoluteVelocity;
        /// <summary>
        /// 绝对速度
        /// </summary>
        public string AbsoluteVelocity
        {
            get { return _absoluteVelocity; }
            set { Set(ref _absoluteVelocity, value); }
        }

        private string _myBtuContent;
        /// <summary>
        /// 使能显示
        /// </summary>
        public string MyBtuContent
        {
            get { return _myBtuContent; }
            set { Set(ref _myBtuContent, value); }
        }

        private string _lightLocationBack;
        /// <summary>
        /// 光源位置反馈
        /// </summary>
        public string LightLocationBack
        {
            get { return _lightLocationBack; }
            set { Set(ref _lightLocationBack, value); }
        }

        private string _jogSpeedBack;
        /// <summary>
        /// 点动速度反馈
        /// </summary>
        public string JogSpeedBack
        {
            get { return _jogSpeedBack; }
            set { Set(ref _jogSpeedBack, value); }
        }

        private string _zeroOffsetBack;
        /// <summary>
        /// 零点补偿反馈
        /// </summary>
        public string ZeroOffsetBack
        {
            get { return _zeroOffsetBack; }
            set { Set(ref _zeroOffsetBack, value); }
        }

        private string _absoluteVelocitySpeedBack;
        /// <summary>
        /// 绝对运动速度反馈
        /// </summary>
        public string AbsoluteVelocitySpeedBack
        {
            get { return _absoluteVelocitySpeedBack; }
            set { Set(ref _absoluteVelocitySpeedBack, value); }
        }
        #endregion

        #region ===================按钮命令定义====================
        /// <summary>
        /// 推杆气缸伸出命令
        /// </summary>
        public RelayCommand PushrodCylinderProtrudeCommand { get; set; }
        /// <summary>
        /// 推杆气缸缩回命令
        /// </summary>
        public RelayCommand PushrodCylinderRetractedCommand { get; set; }
        /// <summary>
        /// 定中夹紧命令
        /// </summary>
        public RelayCommand ClampingCommand { get; set; }
        /// <summary>
        /// 定中松开命令
        /// </summary>
        public RelayCommand LoosenCommand { get; set; }
        /// <summary>
        /// 打磨主轴开命令
        /// </summary>
        public RelayCommand DO3OpenCommand { get; set; }
        /// <summary>
        /// 打磨主轴关命令
        /// </summary>
        public RelayCommand DO3CloseCommand { get; set; }
        /// <summary>
        /// 打磨吹扫开命令
        /// </summary>
        public RelayCommand DO2OpenCommand { get; set; }
        /// <summary>
        /// 打磨吹扫关命令
        /// </summary>
        public RelayCommand DO2CloseCommand { get; set; }
        /// <summary>
        /// 浮动锁紧开命令
        /// </summary>
        public RelayCommand DO1OpenCommand { get; set; }
        /// <summary>
        /// 浮动锁紧关命令
        /// </summary>
        public RelayCommand DO1CloseCommand { get; set; }

        public RelayCommand EntranceMotorOpenCommand { get; set; }

        public RelayCommand EntranceMotorCloseCommand { get; set; }

        public RelayCommand ProcessingMotorOpenCommand { get; set; }

        public RelayCommand ProcessingMotorCloseCommand { get; set; }

        public RelayCommand OutletMotor1OpenCommand { get; set; }

        public RelayCommand OutletMotor1CloseCommand { get; set; }

        public RelayCommand OutletMotor2OpenCommand { get; set; }

        public RelayCommand OutletMotor2CloseCommand { get; set; }

        public RelayCommand<string> SetLightLocationCommand { get; set; }

        public RelayCommand<string> PLCActionCommand { get; set; }
        public RelayCommand<string> SetJogSpeedCommand { get; set; }
        public RelayCommand<string> SetZeroOffsetCommand { get; set; }
        public RelayCommand<string> SetAbsoluteVelocityCommand { get; set; }

        public RelayCommand<string> ButtonMouseDownCommand { get; set; }


        #endregion
        public EquipmentPageViewModel()
        {
            PushrodCylinderBackground = TemplateHelper.GetLinearGradientBrush(Colors.Gray);
            FixedCenterBackground = TemplateHelper.GetLinearGradientBrush(Colors.Gray);
            DO1Background = TemplateHelper.GetLinearGradientBrush(Colors.Gray);
            DO2Background = TemplateHelper.GetLinearGradientBrush(Colors.Gray);
            DO3Background = TemplateHelper.GetLinearGradientBrush(Colors.Gray);
            InletPhotoelectricityBackground = TemplateHelper.GetLinearGradientBrush(Colors.Gray);
            ProcessingPhotoelectricityBackground = TemplateHelper.GetLinearGradientBrush(Colors.Gray);
            ExportPhotoelectricity1Background = TemplateHelper.GetLinearGradientBrush(Colors.Gray);
            ExportPhotoelectricity2Background = TemplateHelper.GetLinearGradientBrush(Colors.Gray);
            PushrodInductionBackground = TemplateHelper.GetLinearGradientBrush(Colors.Gray);
            FixedCenterInductionBackground = TemplateHelper.GetLinearGradientBrush(Colors.Gray);
            InletMotorStatus = "";
            ProcessingMotorStatus = "";
            ExportMotor1Status = "";
            ExportMotor2Status = "";
            PushrodCylinderProtrudeCommand = new RelayCommand(PushrodCylinderProtrude);
            PushrodCylinderRetractedCommand = new RelayCommand(PushrodCylinderRetracted);
            ClampingCommand = new RelayCommand(Clamping);
            LoosenCommand = new RelayCommand(Loosen);
            DO3OpenCommand = new RelayCommand(DO3Open);
            DO3CloseCommand = new RelayCommand(DO3Close);
            DO2OpenCommand = new RelayCommand(DO2Open);
            DO2CloseCommand = new RelayCommand(DO2Close);
            DO1OpenCommand = new RelayCommand(DO1Open);
            DO1CloseCommand = new RelayCommand(DO1Close);
            EntranceMotorOpenCommand = new RelayCommand(EntranceMotorOpen);
            EntranceMotorCloseCommand = new RelayCommand(EntranceMotorClose);
            ProcessingMotorOpenCommand = new RelayCommand(ProcessingMotorOpen);
            ProcessingMotorCloseCommand = new RelayCommand(ProcessingMotorClose);
            OutletMotor1OpenCommand = new RelayCommand(OutletMotor1Open);
            OutletMotor1CloseCommand = new RelayCommand(OutletMotor1Close);
            OutletMotor2OpenCommand = new RelayCommand(OutletMotor2Open);
            OutletMotor2CloseCommand = new RelayCommand(OutletMotor2Close);
            SetLightLocationCommand = new RelayCommand<string>(SetLightLocation);
            SetJogSpeedCommand = new RelayCommand<string>(SetJogSpeed);
            PLCActionCommand = new RelayCommand<string>(PLCAction);
            SetZeroOffsetCommand = new RelayCommand<string>(SetZeroOffset);
            SetAbsoluteVelocityCommand = new RelayCommand<string>(SetAbsoluteVelocity);
            ButtonMouseDownCommand = new RelayCommand<string>(ButtonMouseDown);
        }


        #region ===================按钮命令委托====================
        private void PushrodCylinderProtrude()
        {
            if (PlcHelper.PlcCilent.Connected)
            {
                S7.SetBitAt(PlcHelper.PlcWriteDataBuffer, 0, 6, true);
            }
        }

        private void PushrodCylinderRetracted()
        {
            if (PlcHelper.PlcCilent.Connected)
            {
                S7.SetBitAt(PlcHelper.PlcWriteDataBuffer, 0, 6, false);
            }
        }

        private void DO1Close()
        {
            if (ServiceLocator.Current.GetInstance<MonitorPageViewModel>().RobotController == null || !ServiceLocator.Current.GetInstance<MonitorPageViewModel>().RobotController.Connected)
            {
                UMessageBox.Show("机器人控制器未连接！", Models.MessageType.Default);
                return;
            }
            try
            {
                Signal do1 = ServiceLocator.Current.GetInstance<MonitorPageViewModel>().RobotController.IOSystem.GetSignal("Local_IO_0_DO1");
                DigitalSignal sig = (DigitalSignal)do1;
                if (Convert.ToBoolean(sig.Value) != false)
                    sig.Reset();
            }
            catch (Exception ex)
            {
                UMessageBox.Show(ex.Message, Models.MessageType.Error);
            }
        }

        private void DO1Open()
        {
            if (ServiceLocator.Current.GetInstance<MonitorPageViewModel>().RobotController == null || !ServiceLocator.Current.GetInstance<MonitorPageViewModel>().RobotController.Connected)
            {
                UMessageBox.Show("机器人控制器未连接！", Models.MessageType.Default);
                return;
            }
            try
            {
                Signal do1 = ServiceLocator.Current.GetInstance<MonitorPageViewModel>().RobotController.IOSystem.GetSignal("Local_IO_0_DO1");
                DigitalSignal sig = (DigitalSignal)do1;
                if (Convert.ToBoolean(sig.Value) != true)
                    sig.Set();
            }
            catch (Exception ex)
            {
                UMessageBox.Show(ex.Message, Models.MessageType.Error);
            }
        }

        private void DO2Close()
        {
            if (ServiceLocator.Current.GetInstance<MonitorPageViewModel>().RobotController == null || !ServiceLocator.Current.GetInstance<MonitorPageViewModel>().RobotController.Connected)
            {
                UMessageBox.Show("机器人控制器未连接！", Models.MessageType.Default);
                return;
            }
            try
            {
                Signal do2 = ServiceLocator.Current.GetInstance<MonitorPageViewModel>().RobotController.IOSystem.GetSignal("Local_IO_0_DO2");
                DigitalSignal sig = (DigitalSignal)do2;
                if (Convert.ToBoolean(sig.Value) != false)
                    sig.Reset();
            }
            catch (Exception ex)
            {
                UMessageBox.Show(ex.Message, Models.MessageType.Error);
            }
        }

        private void DO2Open()
        {
            if (ServiceLocator.Current.GetInstance<MonitorPageViewModel>().RobotController == null || !ServiceLocator.Current.GetInstance<MonitorPageViewModel>().RobotController.Connected)
            {
                UMessageBox.Show("机器人控制器未连接！", Models.MessageType.Default);
                return;
            }
            try
            {
                Signal do2 = ServiceLocator.Current.GetInstance<MonitorPageViewModel>().RobotController.IOSystem.GetSignal("Local_IO_0_DO2");
                DigitalSignal sig = (DigitalSignal)do2;
                if (Convert.ToBoolean(sig.Value) != true)
                    sig.Set();
            }
            catch (Exception ex)
            {
                UMessageBox.Show(ex.Message, Models.MessageType.Error);
            }
        }

        private void DO3Open()
        {
            if (ServiceLocator.Current.GetInstance<MonitorPageViewModel>().RobotController == null || !ServiceLocator.Current.GetInstance<MonitorPageViewModel>().RobotController.Connected)
            {
                UMessageBox.Show("机器人控制器未连接！", Models.MessageType.Default);
                return;
            }
            try
            {
                Signal do3 = ServiceLocator.Current.GetInstance<MonitorPageViewModel>().RobotController.IOSystem.GetSignal("Local_IO_0_DO3");
                DigitalSignal sig = (DigitalSignal)do3;
                if (Convert.ToBoolean(sig.Value) != true)
                    sig.Set();
            }
            catch (Exception ex)
            {
                UMessageBox.Show(ex.Message, Models.MessageType.Error);
            }
        }

        private void DO3Close()
        {
            if (ServiceLocator.Current.GetInstance<MonitorPageViewModel>().RobotController == null || !ServiceLocator.Current.GetInstance<MonitorPageViewModel>().RobotController.Connected)
            {
                UMessageBox.Show("机器人控制器未连接！", Models.MessageType.Default);
                return;
            }
            try
            {
                Signal do3 = ServiceLocator.Current.GetInstance<MonitorPageViewModel>().RobotController.IOSystem.GetSignal("Local_IO_0_DO3");
                DigitalSignal sig = (DigitalSignal)do3;
                if (Convert.ToBoolean(sig.Value) != false)
                    sig.Reset();
            }
            catch (Exception ex)
            {
                UMessageBox.Show(ex.Message, Models.MessageType.Error);
            }
        }

        private void Clamping()
        {
            if (PlcHelper.PlcCilent.Connected)
            {
                S7.SetBitAt(PlcHelper.PlcWriteDataBuffer, 0, 5, true);
            }
        }
        /// <summary>
        /// 设置零点补偿
        /// </summary>
        /// <param name="obj"></param>
        private void SetZeroOffset(string obj)
        {
            SetPlcWriteInt(obj, 8);
        }
        /// <summary>
        /// 设置绝对速度
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void SetAbsoluteVelocity(string obj)
        {
            SetPlcWriteInt(obj, 20);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void ButtonMouseDown(string obj)
        {
            Console.WriteLine(obj);
        }

        public void SetLightLocation(string value)
        {
            SetPlcWriteInt(value, 16);
        }

        /// <summary>
        /// 设置点动速度
        /// </summary>
        /// <param name="obj"></param>
        private void SetJogSpeed(string obj)
        {
            SetPlcWriteInt(obj, 12);
        }

        private void SetPlcWriteInt(string obj, int startIndex)
        {
            if (PlcHelper.PlcCilent.Connected)
            {
                //byte[] bytes = BitConverter.GetBytes(0.1f);
                //byte[] buffer = new byte[] { bytes[3], bytes[2], bytes[1], bytes[0] };
                //Buffer.BlockCopy(buffer, 0, PlcWriteDataBuffer, 2, buffer.Length);

                float speed = Convert.ToSingle(obj);
                byte[] bytes = BitConverter.GetBytes(speed);
                byte[] buffer = new byte[] { bytes[3], bytes[2], bytes[1], bytes[0] };
                Buffer.BlockCopy(buffer, 0, PlcHelper.PlcWriteDataBuffer, startIndex, buffer.Length);
            }
        }

        /// <summary>
        /// PLC的动作命令-集成方法
        /// </summary>
        private void PLCAction(string value)
        {
            if (PlcHelper.PlcCilent.Connected)
            {
                switch (value)
                {
                    case "手动使能":
                        S7.SetBitAt(PlcHelper.PlcWriteDataBuffer, 6, 3, true);
                        break;
                    case "复位伺服报警":
                        S7.SetBitAt(PlcHelper.PlcWriteDataBuffer, 6, 4, true);
                        break;
                    case "轴正向点动":
                        S7.SetBitAt(PlcHelper.PlcWriteDataBuffer, 6, 5, true);
                        break;
                    case "轴负向点动":
                        S7.SetBitAt(PlcHelper.PlcWriteDataBuffer, 6, 6, true);
                        break;
                    case "轴暂停":
                        S7.SetBitAt(PlcHelper.PlcWriteDataBuffer, 6, 7, true);
                        break;
                    case "手动一键回零":
                        S7.SetBitAt(PlcHelper.PlcWriteDataBuffer, 7, 0, true);
                        break;
                }
            }
        }

        private void Loosen()
        {
            if (PlcHelper.PlcCilent.Connected)
            {
                S7.SetBitAt(PlcHelper.PlcWriteDataBuffer, 0, 5, false);
            }
        }

        private void EntranceMotorOpen()
        {
            if (PlcHelper.PlcCilent.Connected)
            {
                S7.SetBitAt(PlcHelper.PlcWriteDataBuffer, 0, 1, true);
            }
        }

        private void EntranceMotorClose()
        {
            if (PlcHelper.PlcCilent.Connected)
            {
                S7.SetBitAt(PlcHelper.PlcWriteDataBuffer, 0, 1, false);
            }
        }

        private void ProcessingMotorOpen()
        {
            S7.SetBitAt(PlcHelper.PlcWriteDataBuffer, 0, 2, true);
        }

        private void ProcessingMotorClose()
        {
            S7.SetBitAt(PlcHelper.PlcWriteDataBuffer, 0, 2, false);
        }

        private void OutletMotor1Open()
        {
            S7.SetBitAt(PlcHelper.PlcWriteDataBuffer, 0, 3, true);
        }

        private void OutletMotor1Close()
        {
            S7.SetBitAt(PlcHelper.PlcWriteDataBuffer, 0, 3, false);
        }

        private void OutletMotor2Open()
        {
            S7.SetBitAt(PlcHelper.PlcWriteDataBuffer, 0, 4, true);
        }

        private void OutletMotor2Close()
        {
            S7.SetBitAt(PlcHelper.PlcWriteDataBuffer, 0, 4, false);
        }
        #endregion 

        /// <summary>
        /// 电机状态背景颜色设置
        /// </summary>
        /// <param name="motorStatus">电机状态</param>
        /// <returns>对应状态的背景颜色</returns>
        public LinearGradientBrush MotorStatusBackgroundSet(string motorStatus)
        {
            LinearGradientBrush linearGradientBrush;
            if (motorStatus == "运行")
            {
                linearGradientBrush = TemplateHelper.GetLinearGradientBrush(Colors.LightGreen);
            }
            else if (motorStatus == "停止")
            {
                linearGradientBrush = TemplateHelper.GetLinearGradientBrush(Colors.Yellow);
            }
            else if (motorStatus == "节能")
            {
                linearGradientBrush = TemplateHelper.GetLinearGradientBrush(Colors.Blue);
            }
            else if (motorStatus == "故障")
            {
                linearGradientBrush = TemplateHelper.GetLinearGradientBrush(Colors.Red);
            }
            else
            {
                linearGradientBrush = TemplateHelper.GetLinearGradientBrush(Colors.Gray);
            }
            return linearGradientBrush;
        }
    }
}
