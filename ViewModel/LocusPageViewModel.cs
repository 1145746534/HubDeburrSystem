using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HalconDotNet;
using HelixToolkit.Wpf;
using HubDeburrSystem.DataAccess;
using HubDeburrSystem.Models;
using HubDeburrSystem.Public;
using HubDeburrSystem.Views.Pages;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace HubDeburrSystem.ViewModel
{
    public class LocusPageViewModel : ViewModelBase
    {
        /// <summary>
        /// 加工轨迹数据
        /// </summary>
        public static LocusDxfModel ProcessingLocusDatas { get; set; } = new LocusDxfModel();

        /// <summary>
        /// 参数设置命令
        /// </summary>
        public RelayCommand ParameterSettingsCommand { get; set; }

        #region===================识别结果显示相关属性字段定义====================
        private HObject _currentImage;
        /// <summary>
        /// 当前图像
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

        private string _currentColor;
        /// <summary>
        /// 当前窗口显示颜色
        /// </summary>
        public string CurrentColor
        {
            get { return _currentColor; }
            set { Set(ref _currentColor, value); }
        }
        #endregion

        private bool _locusButtonEnable;
        /// <summary>
        /// 轨迹点调整按钮使能
        /// </summary>
        public bool LocusButtonEnable
        {
            get { return _locusButtonEnable; }
            set { Set(ref _locusButtonEnable, value); }
        }

        /// <summary>
        /// 当前加工轮毂的识别数据
        /// </summary>
        public IdentifyDataModel CurrentIdentifyData { set; get; } = new IdentifyDataModel();
        /// <summary>
        /// 当前加工轮毂轨迹数据
        /// </summary>
        public List<MachiningPathPosModel> CurrentMachiningLocusDatas { get; set; } = new List<MachiningPathPosModel>();
        /// <summary>
        /// 当前加工轮毂的图像
        /// </summary>
        public HObject CurrentMachiningImage { get; set; } = new HObject();

        public LocusPageViewModel()
        {
            ParameterSettingsCommand = new RelayCommand(ParameterSettings);
            LocusButtonEnable = true;
        }

        private void ParameterSettings()
        {
            DialogManager.ExecuteAndResult<object>("3DLocusParameterSettingsDialog", null);
        }

        /// <summary>
        /// 加载加工轨迹数据
        /// </summary>
        public void LoadProcessingLocus()
        {
            if (!MonitorPageViewModel.ProcessLocusDataUpdateControl)
                MonitorPageViewModel.ProcessLocusDataUpdateControl = true;
            bool load = true;
            Task.Run(() =>
            {
                while (load)
                {
                    if (MonitorPageViewModel.WhetherUpdateData)
                    {
                        ProcessingLocusDatas.LocusName.Clear();
                        ProcessingLocusDatas.LocusPoints.Clear();
                        ProcessingLocusDatas = null;
                        ProcessingLocusDatas = new LocusDxfModel();
                        var plDB = new SqlAccess().ProcessingLocusDataAccess;
                        var tables = plDB.DbMaintenance.GetTableInfoList(false);
                        for (int j = 0; j < tables.Count; j++)
                        {
                            ProcessingLocusDatas.LocusName.Add(tables[j].Name);
                            var datas = plDB.Queryable<MachiningPathPosModel>().AS(tables[j].Name).ToList();
                            ProcessingLocusDatas.LocusPoints.Add(datas);
                        }
                        MonitorPageViewModel.ProcessLocusDataUpdateControl = false;
                        load = false;
                    }
                }
            });
        }

        /// <summary>
        /// 根据轮型删除保存在内存中的加工轨迹
        /// </summary>
        /// <param name="locusName"></param>
        public void DeleteProcessingLocus(string locusName)
        {
            if (!MonitorPageViewModel.DeleteProcessLoucsControl)
                MonitorPageViewModel.DeleteProcessLoucsControl = true;
            bool delete = true;
            Task.Run(() =>
            {
                while (delete)
                {
                    if(MonitorPageViewModel.WhetherUpdateData)
                    {
                        var index = ProcessingLocusDatas.LocusName.FindIndex(x => x == locusName);
                        if(index >= 0)
                        {
                            ProcessingLocusDatas.LocusName.RemoveAt(index);
                            ProcessingLocusDatas.LocusPoints.RemoveAt(index);
                        }
                        MonitorPageViewModel.DeleteProcessLoucsControl = false;
                        delete = false;
                    }
                }
            });
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            ProcessingLocusDatas.LocusName?.Clear();
            ProcessingLocusDatas.LocusPoints?.Clear();
            ProcessingLocusDatas = null;
            CurrentMachiningLocusDatas?.Clear();
            CurrentMachiningLocusDatas = null;
            CurrentIdentifyData = null;
        }
    }
}
