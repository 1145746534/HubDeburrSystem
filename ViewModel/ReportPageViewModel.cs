using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HubDeburrSystem.DataAccess;
using HubDeburrSystem.Models;
using HubDeburrSystem.Views.Dialog;
using MathNet.Numerics.Statistics;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Threading;

namespace HubDeburrSystem.ViewModel
{
    public class ReportPageViewModel : ViewModelBase
    {
        #region==============日期时间相关属性================
        private DateTime _startDate;
        /// <summary>
        /// 起始日期
        /// </summary>
        public DateTime StartDate
        {
            get { return _startDate; }
            set { Set(ref _startDate, value); }
        }

        private DateTime _endDate;
        /// <summary>
        /// 结束日期
        /// </summary>
        public DateTime EndDate
        {
            get { return _endDate; }
            set { Set(ref _endDate, value); }
        }

        private string _startHour;
        /// <summary>
        /// 起始小时
        /// </summary>
        public string StartHour
        {
            get { return _startHour; }
            set { Set(ref _startHour, value); }
        }

        private string _startMinute;
        /// <summary>
        /// 起始分钟
        /// </summary>
        public string StartMinute
        {
            get { return _startMinute; }
            set { Set(ref _startMinute, value); }
        }

        private string _endHour;
        /// <summary>
        /// 结束小时
        /// </summary>
        public string EndHour
        {
            get { return _endHour; }
            set { Set(ref _endHour, value); }
        }

        private string _endMinute;
        /// <summary>
        /// 结束分钟
        /// </summary>
        public string EndMinute
        {
            get { return _endMinute; }
            set { Set(ref _endMinute, value); }
        }
        #endregion
        #region==============按钮命令相关属性================
        /// <summary>
        /// 显示源轨迹命令
        /// </summary>
        public RelayCommand ShowSourceLocusDataCommand { get; set; }

        /// <summary>
        /// 显示加工轨迹命令
        /// </summary>
        public RelayCommand ShowProcessingLocusDataCommand { get; set; }
        public RelayCommand DisplayWheelDataCommand { get; set; }
        /// <summary>
        /// 数据刷新命令
        /// </summary>
        public RelayCommand DataRefreshCommand { get; set; }
        /// <summary>
        /// 数据查询命令
        /// </summary>
        public RelayCommand DataInquireCommand { get; set; }
        /// <summary>
        /// 数据统计命令
        /// </summary>
        public RelayCommand DataStatisticsCommand { get; set; }
        /// <summary>
        /// 数据导出命令
        /// </summary>
        public RelayCommand DataExportCommand { get; set; }
        /// <summary>
        /// 轮型查询命令
        /// </summary>
        public RelayCommand InquireWheelTypeCommand { get; set; }
        #endregion
        #region==============信息显示相关属性================
        private Visibility _messageVisibility;
        /// <summary>
        /// 信息框显示控制
        /// </summary>
        public Visibility MessageVisibility
        {
            get { return _messageVisibility; }
            set { Set(ref _messageVisibility, value); }
        }

        private string _displayMessage;
        /// <summary>
        /// 显示的信息
        /// </summary>
        public string DisplayMessage
        {
            get { return _displayMessage; }
            set { Set(ref _displayMessage, value); }
        }

        private Brush _messageBackground;
        /// <summary>
        /// 信息框背景颜色
        /// </summary>
        public Brush MessageBackground
        {
            get { return _messageBackground; }
            set { Set(ref _messageBackground, value); }
        }
        #endregion
        #region==============轨迹数据，轮毂数据相关属性================
        private List<string> _wheelTypeDatas;
        /// <summary>
        /// 轮形数据
        /// </summary>
        public List<string> WheelTypeDatas
        {
            get { return _wheelTypeDatas; }
            set { Set(ref _wheelTypeDatas, value); }
        }

        private string _selectedItem;
        /// <summary>
        /// 所选项目
        /// </summary>
        public string SelectedItem
        {
            get { return _selectedItem; }
            set { Set(ref _selectedItem, value); }
        }

        private bool _locusButtonEnable;
        /// <summary>
        /// 轨迹按钮使能
        /// </summary>
        public bool LocusButtonEnable
        {
            get { return _locusButtonEnable; }
            set
            {
                _locusButtonEnable = value;
                RaisePropertyChanged(nameof(LocusButtonEnable));
            }
        }
        private ObservableCollection<MachiningPathPosModel> _locusDatas;
        /// <summary>
        /// 用于显示的轨迹数据
        /// </summary>
        public ObservableCollection<MachiningPathPosModel> LocusDatas
        {
            get { return _locusDatas; }
            set { Set(ref _locusDatas, value); }
        }

        private ObservableCollection<TemplateDataModel> _wheelDatas;
        /// <summary>
        /// 用于显示的轮毂数据
        /// </summary>
        public ObservableCollection<TemplateDataModel> WheelDatas
        {
            get { return _wheelDatas; }
            set { Set(ref _wheelDatas, value); }
        }

        private Visibility _locusDatasVisibility;
        /// <summary>
        /// 轨迹数据显示控制
        /// </summary>
        public Visibility LocusDatasVisibility
        {
            get { return _locusDatasVisibility; }
            set { Set(ref _locusDatasVisibility, value); }
        }
        private Visibility _wheelDatasVisibility;
        /// <summary>
        /// 轮毂数据显示控制
        /// </summary>
        public Visibility WheelDatasVisibility
        {
            get { return _wheelDatasVisibility; }
            set { Set(ref _wheelDatasVisibility, value); }
        }
        #endregion
        #region==============生产数据显示相关属性================
        private ObservableCollection<ProductionDatas> _productionDatas;
        /// <summary>
        /// 生产数据显示
        /// </summary>
        public ObservableCollection<ProductionDatas> ProductionDatas
        {
            get { return _productionDatas; }
            set { Set(ref _productionDatas, value); }
        }

        private ObservableCollection<StatisticsDataModel> _statisticsDatas;
        /// <summary>
        /// 统计数据显示
        /// </summary>
        public ObservableCollection<StatisticsDataModel> StatisticsDatas
        {
            get { return _statisticsDatas; }
            set { Set(ref _statisticsDatas, value); }
        }

        private Visibility _productionDataVisibility;
        /// <summary>
        /// 生产数据显示控制
        /// </summary>
        public Visibility ProductionDataVisibility
        {
            get { return _productionDataVisibility; }
            set { Set(ref _productionDataVisibility, value); }
        }

        private Visibility _statisticsDataVisibility;
        /// <summary>
        /// 统计数据显示控制
        /// </summary>
        public Visibility StatisticsDataVisibility
        {
            get { return _statisticsDataVisibility; }
            set { Set(ref _statisticsDataVisibility, value); }
        }

        private string _inquireWheelType;
        /// <summary>
        /// 查询轮型
        /// </summary>
        public string InquireWheelType
        {
            get { return _inquireWheelType; }
            set { Set(ref _inquireWheelType, value); }
        }

        #endregion

        public ReportPageViewModel()
        {
            LocusDatas = new ObservableCollection<MachiningPathPosModel>();
            ProductionDatas = new ObservableCollection<ProductionDatas>();
            StatisticsDatas = new ObservableCollection<StatisticsDataModel>();
            WheelTypeDatas = new List<string>();
            ShowSourceLocusDataCommand = new RelayCommand(ShowSourceLocusData);
            ShowProcessingLocusDataCommand = new RelayCommand(ShowProcessingLocus);
            DisplayWheelDataCommand = new RelayCommand(DisplayWheelData);
            DataRefreshCommand = new RelayCommand(DataRefresh);
            DataInquireCommand = new RelayCommand(DataInquire);
            DataStatisticsCommand = new RelayCommand(DataStatistics);
            DataExportCommand = new RelayCommand(DataExport);
            InquireWheelTypeCommand = new RelayCommand(InquireWheel);
            LocusButtonEnable = true;
            StartDate = DateTime.Now.Date;
            EndDate = DateTime.Now.Date;
            StartHour = DateTime.Now.Hour.ToString();
            StartMinute = "00";
            EndHour = DateTime.Now.Hour.ToString();
            EndMinute = "00";
            ProductionDataVisibility = Visibility.Hidden;
            StatisticsDataVisibility = Visibility.Hidden;
            LocusDatasVisibility = Visibility.Hidden;
            WheelDatasVisibility = Visibility.Hidden;
        }

        #region==============信息显示================
        private void MessageShowTimer_Tick(object sender, EventArgs e)
        {
            MessageVisibility = Visibility.Hidden;
            MessageShowTimer.Stop();
        }
        /// <summary>
        /// 信息显示定时器
        /// </summary>
        private DispatcherTimer MessageShowTimer;
        /// <summary>
        /// 信息显示
        /// </summary>
        /// <param name="message"></param>
        /// <param name="type"></param>
        private void MessageShow(string message, MessageType type)
        {
            DisplayMessage = message;
            MessageVisibility = Visibility.Visible;
            if (type == MessageType.Default)
            {
                MessageBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFDDE3FB"));
            }
            else if (type == MessageType.Warning)
            {
                MessageBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF3F5C8"));
            }
            else if (type == MessageType.Success)
            {
                MessageBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFAADEB7"));
            }
            else if (type == MessageType.Error)
            {
                MessageBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF78883"));
            }
            if(MessageShowTimer != null && MessageShowTimer.IsEnabled) MessageShowTimer.Stop();
            MessageShowTimer = new DispatcherTimer();
            MessageShowTimer.Interval = new TimeSpan(0, 0, 3);
            MessageShowTimer.Tick += MessageShowTimer_Tick;
            MessageShowTimer.Start();
        }
        #endregion
        #region==============生产数据操作================
        /// <summary>
        /// 数据刷新
        /// </summary>
        private void DataRefresh()
        {
            var pDB = new SqlAccess().ProductionDataAccess;
            int maxId = pDB.Queryable<ProductionDatas>().Max(x => x.Id);
            StatisticsDatas?.Clear();
            List<ProductionDatas> productionList = new List<ProductionDatas>();
            if (maxId > 100)
            {
                //查询两个序号区间的数据
                productionList = pDB.Queryable<ProductionDatas>().Where(it => SqlFunc.Between(it.Id, maxId - 100, maxId)).OrderBy((sc) => sc.Id, OrderByType.Desc).ToList();
            }
            else
            {
                productionList = pDB.Queryable<ProductionDatas>().Where(it => SqlFunc.Between(it.Id, 0, maxId)).OrderBy((sc) => sc.Id, OrderByType.Desc).ToList();
            }
            ProductionDatas?.Clear();
            ProductionDatas = new ObservableCollection<ProductionDatas>(productionList);
            StatisticsDataVisibility = Visibility.Hidden;
            ProductionDataVisibility = Visibility.Visible;
        }
        /// <summary>
        /// 数据查询
        /// </summary>
        private void DataInquire()
        {
            bool r = JudgmentInputsDateTime(StartDate, StartHour, StartMinute, EndDate, EndHour, EndMinute, out GetDateTimeModel result);
            if (r)
            {
                var pDB = new SqlAccess().ProductionDataAccess;
                StatisticsDatas?.Clear();
                var productionList = pDB.Queryable<ProductionDatas>().Where(it => SqlFunc.Between(it.ProcessingTime, result.StartDateTime, result.EndDateTime)).OrderBy((sc) => sc.Id, OrderByType.Desc).ToList();
                ProductionDatas?.Clear();
                ProductionDatas = new ObservableCollection<ProductionDatas>(productionList);
                StatisticsDataVisibility = Visibility.Hidden;
                ProductionDataVisibility = Visibility.Visible;
            }
            else MessageShow(result.Result, MessageType.Warning);
        }
        /// <summary>
        /// 数据统计
        /// </summary>
        private void DataStatistics()
        {
            bool r = JudgmentInputsDateTime(StartDate, StartHour, StartMinute, EndDate, EndHour, EndMinute, out GetDateTimeModel result);
            if (r)
            {
                var pDB = new SqlAccess().ProductionDataAccess;
                var productionList = pDB.Queryable<ProductionDatas>().Where(it => SqlFunc.Between(it.ProcessingTime, result.StartDateTime, result.EndDateTime)).OrderBy((sc) => sc.Id, OrderByType.Desc).ToList();
                List<StatisticsDataModel> statisticsDatas = new List<StatisticsDataModel>();
                for (int i = 0; i < productionList.Count; i++)
                {
                    int index = statisticsDatas.FindIndex(x => x.WheelModel == productionList[i].WheelModel.Trim('_'));
                    if (index < 0)
                    {
                        StatisticsDataModel statisticsDataModel = new StatisticsDataModel();
                        statisticsDataModel.Id = statisticsDatas.Count + 1;
                        statisticsDataModel.WheelModel = productionList[i].WheelModel.Trim('_');
                        if (productionList[i].ProcessingResult == "加工完成")
                        {
                            statisticsDataModel.ProcessingComplete += 1;
                            statisticsDataModel.MachiningAnomalies = 0;
                        }
                        else
                        {
                            statisticsDataModel.ProcessingComplete = 0;
                            statisticsDataModel.MachiningAnomalies += 1;
                        }
                        statisticsDataModel.Total += 1;
                        statisticsDatas.Add(statisticsDataModel);
                    }
                    else
                    {
                        if (productionList[i].ProcessingResult == "加工完成")
                        {
                            statisticsDatas[index].ProcessingComplete += 1;
                        }
                        else
                        {
                            statisticsDatas[index].MachiningAnomalies += 1;
                        }
                        statisticsDatas[index].Total += 1;
                    }
                }
                //按轮型排序
                statisticsDatas.Sort((p1, p2) =>//排序
                {
                    if (p1.WheelModel != p2.WheelModel)
                    {
                        return p1.WheelModel.CompareTo(p2.WheelModel);
                    }
                    else return 0;
                });
                //调整序号
                for (int i = 0; i < statisticsDatas.Count; i++)//整理序号
                {
                    statisticsDatas[i].Id = i + 1;
                }
                //计算总数
                StatisticsDataModel statisticsData = new StatisticsDataModel();
                statisticsData.Id = statisticsDatas.Count + 1;
                statisticsData.WheelModel = "总计";
                for (int i = 0; i < statisticsDatas.Count; i++)
                {
                    statisticsData.ProcessingComplete = statisticsData.ProcessingComplete + statisticsDatas[i].ProcessingComplete;
                    statisticsData.MachiningAnomalies = statisticsData.MachiningAnomalies + statisticsDatas[i].MachiningAnomalies;
                    statisticsData.Total = statisticsData.Total + statisticsDatas[i].Total;
                }
                statisticsDatas.Add(statisticsData);
                ProductionDatas?.Clear();
                StatisticsDatas?.Clear();
                StatisticsDatas = new ObservableCollection<StatisticsDataModel>(statisticsDatas);
                ProductionDataVisibility = Visibility.Hidden;
                StatisticsDataVisibility = Visibility.Visible;
            }
            else MessageShow(result.Result, MessageType.Warning);
        }
        /// <summary>
        /// 数据导出
        /// </summary>
        private void DataExport()
        {
            if(ProductionDatas.Count == 0 && StatisticsDatas.Count == 0)
            {
                MessageShow("无导出的数据，请检查！", MessageType.Warning);
                return;
            }
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Title = "请选择要导出的位置",
                Filter = "Excel文件(*.xls,*.xlsx)|*.xls;*.xlsx"
            };
            if (saveFileDialog.ShowDialog() != DialogResult.OK) return;
            if (saveFileDialog.FileName != "")
            {
                var FileSavePath = saveFileDialog.FileName.ToString();
                DataTable datas = new DataTable();
                if (ProductionDatas.Count > 0) datas = ExcelDataAccess.ListToDataTable(ProductionDatas);
                else if (StatisticsDatas.Count > 0) datas = ExcelDataAccess.ListToDataTable(StatisticsDatas);
                var result = ExcelDataAccess.DataTableToExcel(datas, FileSavePath, out string exportResult);
                if(result) MessageShow(exportResult, MessageType.Success);
                else MessageShow(exportResult, MessageType.Error);
            }
        }
        /// <summary>
        /// 轮型查询
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void InquireWheel()
        {
            bool r = JudgmentInputsDateTime(StartDate, StartHour, StartMinute, EndDate, EndHour, EndMinute, out GetDateTimeModel result);
            if (r)
            {
                if(InquireWheelType == null)
                {
                    MessageShow("查询轮型为空，请检查！", MessageType.Warning);
                    return;
                }
                var pDB = new SqlAccess().ProductionDataAccess;
                var productionList = pDB.Queryable<ProductionDatas>().Where(it => SqlFunc.Between(it.ProcessingTime, result.StartDateTime, result.EndDateTime)).OrderBy((sc) => sc.Id, OrderByType.Desc).ToList();
                var inquireDatas = productionList.FindAll(x => x.WheelModel.Trim('_') == InquireWheelType).ToList();
                StatisticsDatas?.Clear();
                ProductionDatas?.Clear();
                ProductionDatas = new ObservableCollection<ProductionDatas>(inquireDatas);
                StatisticsDataVisibility = Visibility.Hidden;
                ProductionDataVisibility = Visibility.Visible;
            }
            else MessageShow(result.Result, MessageType.Warning);
        }

        /// <summary>
        /// 检查输入的日期时间
        /// </summary>
        /// <param name="startDate">起始日期</param>
        /// <param name="startHour">起始小时</param>
        /// <param name="startMinute">起始分钟</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="endHour">结束小时</param>
        /// <param name="endMinute">结束分钟</param>
        /// <param name="result">检查结果</param>
        /// <returns>True为无错误</returns>
        private bool JudgmentInputsDateTime(DateTime startDate, string startHour, string startMinute, DateTime endDate, string endHour, string endMinute, out GetDateTimeModel result)
        {
            GetDateTimeModel dateTime = new GetDateTimeModel();
            if (!int.TryParse(startHour, out int sh) || sh < 0 || sh > 24)
            {
                dateTime.Result = "起始小时输入错误，请检查！";
                result = dateTime;
                return false;
            }
            if (!int.TryParse(startMinute, out int sm) || sm < 0 || sm > 59)
            {
                dateTime.Result = "起始分钟输入错误，请检查！";
                result = dateTime;
                return false;
            }
            if (!int.TryParse(endHour, out int eh) || eh < 0 || eh > 24)
            {
                dateTime.Result = "结束小时输入错误，请检查！";
                result = dateTime;
                return false;
            }
            if (!int.TryParse(endMinute, out int em) || em < 0 || em > 59)
            {
                dateTime.Result = "结束分钟输入错误，请检查！";
                result = dateTime;
                return false;
            }
            GenDateTime(startDate, startHour, startMinute, out DateTime startDateTime);
            GenDateTime(endDate, endHour, endMinute, out DateTime endDateTime);
            if (startDateTime >= endDateTime)
            {
                dateTime.Result = "起始日期大于或等于结束日期，请检查！";
                result = dateTime;
                return false;
            }
            dateTime.StartDateTime = startDateTime;
            dateTime.EndDateTime = endDateTime;
            dateTime.Result = "OK";
            result = dateTime;
            return true;
        }

        /// <summary>
        /// 生成日期时间
        /// </summary>
        /// <param name="date">日期</param>
        /// <param name="hour">小时</param>
        /// <param name="minute">分钟</param>
        /// <param name="dateTime">生成的日期时间</param>
        /// <returns>True为生成成功</returns>
        private bool GenDateTime(DateTime date, string hour, string minute, out DateTime dateTime)
        {
            string strDate = date.ToString().Replace("0:00:00", "");
            string strDateTime = strDate + hour + ":" + minute + ":00";
            return DateTime.TryParse(strDateTime, out dateTime);
        }
        #endregion
        #region==============轮毂数据操作================
        /// <summary>
        /// 显示加工轨迹
        /// </summary>
        private void ShowProcessingLocus()
        {
            if (SelectedItem == null)
            {
                MessageShow("轮型为空，请选择轮型！", MessageType.Default);
                return;
            }
            var index = LocusPageViewModel.ProcessingLocusDatas.LocusName.FindIndex(x => x == SelectedItem);
            if (index < 0)
            {
                MessageShow($"轮型{SelectedItem}的加工数据不存在！", MessageType.Default);
                return;
            }
            LocusButtonEnable = false;
            LocusDatas?.Clear();
            LocusDatas = new ObservableCollection<MachiningPathPosModel>(LocusPageViewModel.ProcessingLocusDatas.LocusPoints[index]);
            WheelDatasVisibility = Visibility.Hidden;
            LocusDatasVisibility = Visibility.Visible;
            LocusButtonEnable = true;
        }

        /// <summary>
        /// 显示源轨迹
        /// </summary>
        private void ShowSourceLocusData()
        {
            if (SelectedItem == null)
            {
                MessageShow("轮型为空，请选择轮型！", MessageType.Default);
                return;
            }
            LocusButtonEnable = false;
            LocusDatas?.Clear();
            var index = TemplatePageViewModel.SourceLocusDatas.LocusName.FindIndex(x => x == SelectedItem);
            LocusDatas = new ObservableCollection<MachiningPathPosModel>(TemplatePageViewModel.SourceLocusDatas.LocusPoints[index]);
            WheelDatasVisibility = Visibility.Hidden;
            LocusDatasVisibility = Visibility.Visible;
            LocusButtonEnable = true;
        }

        /// <summary>
        /// 显示轮毂数据
        /// </summary>
        private void DisplayWheelData()
        {
            LocusButtonEnable = false;
            WheelDatas?.Clear();
            var sDB = new SqlAccess().SystemDataAccess;
            var data = sDB.Queryable<TemplateDataModel>().ToList();
            WheelDatas = new ObservableCollection<TemplateDataModel>(data);
            LocusDatasVisibility = Visibility.Hidden;
            WheelDatasVisibility = Visibility.Visible;
            LocusButtonEnable = true;
        }
        #endregion
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            LocusDatas?.Clear();
            LocusDatas = null;
            WheelDatas?.Clear();
            WheelDatas = null;
            ProductionDatas?.Clear();
            ProductionDatas = null;
            StatisticsDatas?.Clear();
            StatisticsDatas = null;
        }
    }
}
