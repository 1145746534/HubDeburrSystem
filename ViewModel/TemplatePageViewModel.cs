using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows.Forms;
using HubDeburrSystem.Public;
using HubDeburrSystem.Models;
using System.Collections.ObjectModel;
using HubDeburrSystem.Views.Dialog;
using HubDeburrSystem.DataAccess;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using CommonServiceLocator;
using System.IO;
using SqlSugar;
using HubDeburrSystem.Views.Pages;
using System.Windows.Media.Media3D;

namespace HubDeburrSystem.ViewModel
{
    public class TemplatePageViewModel : ViewModelBase
    {
        private bool _templateButtonEnabled;
        /// <summary>
        /// 模板按钮使能
        /// </summary>
        public bool TemplateButtonEnabled
        {
            get { return _templateButtonEnabled; }
            set { 
                _templateButtonEnabled = value;
                RaisePropertyChanged(nameof(TemplateButtonEnabled));
            }
        }

        private Visibility _resultShow;
        /// <summary>
        /// 模板制作窗口识别结果显示
        /// </summary>
        public Visibility ResultShow
        {
            get { return _resultShow; }
            set 
            {
                _resultShow = value;
                RaisePropertyChanged(nameof(ResultShow));
            }
        }


        #region ===============全局静态数据，系统关闭时需释放==================
        private ObservableCollection<TemplateDataModel> _templateDatas;
        /// <summary>
        /// 用于实时修改的轮型数据集
        /// </summary>
        public ObservableCollection<TemplateDataModel> TemplateDatas
        {
            get { return _templateDatas; }
            set {Set(ref _templateDatas, value); }
        }

        private ObservableCollection<TemplateDataModel> _processingTemplateDatas;
        /// <summary>
        /// 用于加工的轮型数据集
        /// </summary>
        public ObservableCollection<TemplateDataModel> ProcessingTemplateDatas
        {
            get { return _processingTemplateDatas; }
            set { Set(ref _processingTemplateDatas, value); }
        }

        /// <summary>
        /// 控制 将实时修改的模板数据集 的修改应用 到加工的模板数据集里
        /// </summary>
        public bool ImplementRevise { get; set; }

        /// <summary>
        /// 用于匹配的模板列表
        /// </summary>
        public static TemplateDataListModel TemplateList { get; set; }

        /// <summary>
        /// 源轨迹数据
        /// </summary>
        public static LocusDxfModel SourceLocusDatas { get; set; }

        #endregion


        /// <summary>
        /// 添加轮型命令
        /// </summary>
        public RelayCommand AddWheelTypeCommand { get; set; }

        /// <summary>
        /// 模板参数设置命令
        /// </summary>
        public RelayCommand TemplateParameterSettingsCommand { get; set; }

        /// <summary>
        /// 轨迹参数设置命令
        /// </summary>
        public RelayCommand LocusParameterSettingsCommand { get; set; }

        private int _selectIndex;
        /// <summary>
        /// 选中的索引，绑定到DataGrid.SelectedIndex
        /// </summary>
        public int SelectIndex
        {
            get { return _selectIndex; }
            set 
            {
                if( _selectIndex != value )
                {
                    _selectIndex = value;
                    RaisePropertyChanged(nameof(SelectIndex));
                }
            }
        }

        public TemplatePageViewModel()
        {
            TemplateDatas = new ObservableCollection<TemplateDataModel>();
            ProcessingTemplateDatas = new ObservableCollection<TemplateDataModel>();
            TemplateList = new TemplateDataListModel();
            SourceLocusDatas = new LocusDxfModel();
            AddWheelTypeCommand = new RelayCommand(ShowAddWheelType);
            TemplateParameterSettingsCommand = new RelayCommand(TemplateParameterSettings);
            LocusParameterSettingsCommand = new RelayCommand(LocusParameterSettings);
            TemplateButtonEnabled = true;
            ResultShow = Visibility.Hidden;
        }

        /// <summary>
        /// 打开轨迹参数设置窗口
        /// </summary>
        private void LocusParameterSettings()
        {
            DialogManager.ExecuteAndResult<object>("2DLocusParameterSettingsDialog", null);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            TemplateDatas?.Clear();
            TemplateDatas = null;
            ProcessingTemplateDatas?.Clear();
            ProcessingTemplateDatas = null;
            for (int i = 0; i < TemplateList.ActiveTemplateList.Count; i++)
            {
                HOperatorSet.ClearNccModel(TemplateList.ActiveTemplateList[i]);
            }
            for (int i = 0; i < TemplateList.NotActiveTemplateList.Count; i++)
            {
                HOperatorSet.ClearNccModel(TemplateList.NotActiveTemplateList[i]);
            }
            TemplateList.ActiveWheelTypeList?.Clear();
            TemplateList.ActiveWheelTypeList = null;
            TemplateList.NotActiveWheelTypeList?.Clear();
            TemplateList.NotActiveWheelTypeList = null;
            TemplateList.ActiveTemplateList?.Clear();
            TemplateList.ActiveTemplateList = null;
            TemplateList.NotActiveTemplateList?.Clear();
            TemplateList.NotActiveTemplateList = null;
            TemplateList = null;

            SourceLocusDatas.LocusName?.Clear();
            SourceLocusDatas.LocusPoints?.Clear();
            SourceLocusDatas = null;
        }

        /// <summary>
        /// 打开模板参数设置窗口
        /// </summary>
        private void TemplateParameterSettings()
        {
            DialogManager.ExecuteAndResult<object>("TemplateParameterSettingsDialog", null);
        }

        /// <summary>
        /// 打开添加轮型窗口
        /// </summary>
        private void ShowAddWheelType()
        {
            //可以在此处添加权限
            DialogManager.ExecuteAndResult<object>("DialoAddWheelTypeWindow", null);
            //if ()
            //{

            //}
        }

        /// <summary>
        /// 加载模板表格数据
        /// </summary>
        public void LoadedTemplateData()
        {
            var sDB = new SqlAccess().SystemDataAccess;
            var data = sDB.Queryable<TemplateDataModel>().ToList();
            TemplateDatas.Clear();
            ProcessingTemplateDatas.Clear();
            for (int i = 0; i < data.Count; i++)
            {
                TemplateDatas.Add(data[i]);
                ProcessingTemplateDatas.Add(data[i]);
            }
        }

        /// <summary>
        /// 加载模板
        /// </summary>
        public void LoadedTemplate()
        {
            for (int i = 0; i < TemplateList.ActiveTemplateList.Count; i++)
            {
                HOperatorSet.ClearNccModel(TemplateList.ActiveTemplateList[i]);
            }
            for (int i = 0; i < TemplateList.NotActiveTemplateList.Count; i++)
            {
                HOperatorSet.ClearNccModel(TemplateList.NotActiveTemplateList[i]);
            }
            TemplateList.ActiveTemplateList.Clear();
            TemplateList.NotActiveTemplateList.Clear();
            TemplateList.ActiveWheelTypeList.Clear();
            TemplateList.NotActiveWheelTypeList.Clear();
            try
            {
                HTuple readActiveTemplates = null;
                HTuple readNotActiveTemplates = null;
                string path1 = @"D:\DeburrSystem\ActiveTemplates";
                string path2 = @"D:\DeburrSystem\NotActiveTemplates";
                string[] files1 = Directory.GetFiles(path1);//获取指定路径下所有文件
                string[] files2 = Directory.GetFiles(path2);
                string newPath1;
                string newPath2;
                if (files1.Length > 0)//如果活跃模板存放文件夹不为空
                {
                    for (int a = 0; a < files1.Length; a++)
                    {
                        var wheelModel = files1[a].Substring(32, files1[a].Length - 32);//截取路径后的字符串
                        wheelModel = wheelModel.Trim('.', 'n', 'c', 'm');//修剪掉其中的.ncm
                        newPath1 = files1[a].Replace(@"\", "/");//字符串替换
                        HOperatorSet.ReadNccModel(newPath1, out readActiveTemplates);//读NCC模板
                        TemplateList.ActiveWheelTypeList.Add(wheelModel);
                        TemplateList.ActiveTemplateList.Add(readActiveTemplates);
                    }
                }
                if (files2.Length > 0)//如果不活跃模板存放文件夹不为空
                {
                    for (int a = 0; a < files2.Length; a++)
                    {
                        var wheelMode2 = files2[a].Substring(35, files2[a].Length - 35);
                        wheelMode2 = wheelMode2.Trim('.', 'n', 'c', 'm');
                        newPath2 = files2[a].Replace(@"\", "/");
                        HOperatorSet.ReadNccModel(newPath2, out readNotActiveTemplates);
                        TemplateList.NotActiveWheelTypeList.Add(wheelMode2);
                        TemplateList.NotActiveTemplateList.Add(readNotActiveTemplates);
                    }
                }
            }
            catch (Exception ex)
            {
                ServiceLocator.Current.GetInstance<MonitorPageViewModel>().MessageShow("模板文件加载失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 加载源轨迹数据
        /// </summary>
        public void LoadedSourceLocus()
        {
            SourceLocusDatas.LocusName.Clear();
            SourceLocusDatas.LocusPoints.Clear();
            try
            {
                var slDB = new SqlAccess().SourceLocusDataAccess;
                var tables = slDB.DbMaintenance.GetTableInfoList(false);
                for (int j = 0; j < tables.Count; j++)
                {
                    SourceLocusDatas.LocusName.Add(tables[j].Name);
                    var data = slDB.Queryable<MachiningPathPosModel>().AS(tables[j].Name).ToList();
                    SourceLocusDatas.LocusPoints.Add(data);
                    ServiceLocator.Current.GetInstance<ReportPageViewModel>().WheelTypeDatas.Add(tables[j].Name);
                }
                if (ServiceLocator.Current.GetInstance<ReportPageViewModel>().WheelTypeDatas.Count > 0)
                {
                    ServiceLocator.Current.GetInstance<ReportPageViewModel>().SelectedItem = ServiceLocator.Current.GetInstance<ReportPageViewModel>().WheelTypeDatas[0];
                }
            }
            catch (Exception ex)
            {
                ServiceLocator.Current.GetInstance<MonitorPageViewModel>().MessageShow("加载源轨迹数据失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 更新保存在内存中的实时修改显示的轮型数据
        /// </summary>
        /// <param name="wheelType">轮毂型号</param>
        /// <exception cref="Exception"></exception>
        public void UpdateWheelDatas(string wheelType)
        {
            try
            {
                var sDB = new SqlAccess().SystemDataAccess;
                var datas = sDB.Queryable<TemplateDataModel>().OrderBy(it => it.WheelType).ToList();
                //整理序号
                for (int i = 0; i < datas.Count; i++)
                {
                    datas[i].Index = i + 1;
                }
                var index = datas.FindIndex(it => it.WheelType == wheelType);
                //清空表
                sDB.DbMaintenance.TruncateTable<TemplateDataModel>();
                //写入表
                sDB.Insertable(datas).ExecuteCommand();
                //更新实时修改模板数据集
                TemplateDatas.Clear();
                for (int i = 0; i < datas.Count; i++)
                {
                    TemplateDatas.Add(datas[i]);
                }
                //修改TemplateDataGrid选中项
                SelectIndex = index;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 更新加工用的模板数据
        /// </summary>
        /// <param name="wheelType"></param>
        public void UpdateProcessingTemplateDatas(string wheelType)
        {
            if(!MonitorPageViewModel.TemplateDataUpdateControl)
                MonitorPageViewModel.TemplateDataUpdateControl = true;
            Task.Run(() =>
            {
                bool load = true;
                while(load)
                {
                    if (MonitorPageViewModel.WhetherUpdateData)
                    {
                        string activeTemplatePath = @"D:/DeburrSystem/ActiveTemplates/" + wheelType + ".ncm";
                        string notActiveTemplatePath = @"D:/DeburrSystem/NotActiveTemplates/" + wheelType + ".ncm";

                        var activeIndex = TemplateList.ActiveWheelTypeList.FindIndex(x => x == wheelType);
                        var notActiveIndex = TemplateList.NotActiveWheelTypeList.FindIndex(x => x == wheelType);
                        //如果当前更新轮型在活跃轮型列表中 或 不在活跃轮型列表也不在不活跃轮型列表
                        if (activeIndex >= 0 || (activeIndex < 0 && notActiveIndex < 0))
                        {
                            HOperatorSet.ReadNccModel(activeTemplatePath, out HTuple modelID);
                            if (activeIndex < 0)
                            {
                                TemplateList.ActiveWheelTypeList.Add(wheelType);
                                TemplateList.ActiveTemplateList.Add(modelID);
                            }
                            else TemplateList.ActiveTemplateList[activeIndex] = modelID;
                        }
                        else
                        {
                            HOperatorSet.ReadNccModel(notActiveTemplatePath, out HTuple modelID);
                            TemplateList.NotActiveTemplateList[notActiveIndex] = modelID;
                        }
                        load = false;
                        MonitorPageViewModel.TemplateDataUpdateControl = false;
                        TemplateButtonEnabled = true;
                    }
                }
            });
        }

        /// <summary>
        /// 更新加工用轮型数据
        /// </summary>
        /// <param name="wheelType"></param>
        public void UpdateProcessingWheelDatas()
        {
            if (!MonitorPageViewModel.TemplateDataUpdateControl)
                MonitorPageViewModel.TemplateDataUpdateControl = true;
            Task.Run(() =>
            {
                bool load = true;
                while (load)
                {
                    if (MonitorPageViewModel.WhetherUpdateData)
                    {
                        ProcessingTemplateDatas.Clear();
                        for (int i = 0; i < TemplateDatas.Count; i++)
                        {
                            ProcessingTemplateDatas.Add(TemplateDatas[i]);
                        }
                        load = false;
                        MonitorPageViewModel.TemplateDataUpdateControl = false;
                        TemplateButtonEnabled = true;
                    }
                }
            });
        }

        /// <summary>
        /// 删除指定轮型的加工用模板数据
        /// </summary>
        /// <param name="wheelType">轮型</param>
        public void DeleteProcessingTemplate(string wheelType)
        {
            if(!MonitorPageViewModel.DeleteTemplateUpdateControl)
                MonitorPageViewModel.DeleteTemplateUpdateControl = true;
            Task.Run(() =>
            {
                bool delete = true;
                while (delete)
                {
                    if (MonitorPageViewModel.WhetherUpdateData)
                    {
                        ProcessingTemplateDatas.Clear();
                        for (int i = 0; i < TemplateDatas.Count; i++)
                        {
                            ProcessingTemplateDatas.Add(TemplateDatas[i]);
                        }
                        var activeIndex = TemplateList.ActiveWheelTypeList.FindIndex(x => x == wheelType);
                        var notActiveIndex = TemplateList.NotActiveWheelTypeList.FindIndex(x => x == wheelType);
                        if(activeIndex >= 0)
                        {
                            TemplateList.ActiveWheelTypeList.RemoveAt(activeIndex);
                            TemplateList.ActiveTemplateList.RemoveAt(activeIndex);
                        }
                        if(notActiveIndex >= 0)
                        {
                            TemplateList.NotActiveWheelTypeList.RemoveAt(notActiveIndex);
                            TemplateList.NotActiveTemplateList.RemoveAt(notActiveIndex);
                        }
                        delete =false;
                        MonitorPageViewModel.DeleteTemplateUpdateControl = false;
                        TemplateButtonEnabled = true;
                    }
                }
            });
        }
    }
}
