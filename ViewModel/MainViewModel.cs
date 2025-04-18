using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HalconDotNet;
using HubDeburrSystem.Models;
using HubDeburrSystem.Public;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using static HubDeburrSystem.Models.LocusParameterSettingModel;
using static HubDeburrSystem.Models._3DLocusParameterSettingModel;

namespace HubDeburrSystem.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// 菜单集合
        /// </summary>
        public List<MenuModel> Menus { get; set; }

        private object _viewContent;
        /// <summary>
        /// 窗口Page内容显示
        /// </summary>
        public object ViewContent
        {
            get { return _viewContent; }
            set { Set(ref _viewContent, value); }
        }

        /// <summary>
        /// 切换页面命令
        /// </summary>
        public RelayCommand<object> SwitchPageCommand { get; set; }
        public MainViewModel()
        {
            //在此处加载系统所需要的全部数据
            Task.Run(() =>
            {
                ServiceLocator.Current.GetInstance<MonitorPageViewModel>().MessageShow("系统数据加载中......");
                SystemConfigInitialize();
                ServiceLocator.Current.GetInstance<TemplatePageViewModel>().LoadedTemplateData();
                ServiceLocator.Current.GetInstance<TemplatePageViewModel>().LoadedTemplate();
                ServiceLocator.Current.GetInstance<TemplatePageViewModel>().LoadedSourceLocus();
                ServiceLocator.Current.GetInstance<LocusPageViewModel>().LoadProcessingLocus();
                ServiceLocator.Current.GetInstance<SettingPageViewModel>().LoadSettingPageData();
                ServiceLocator.Current.GetInstance<MonitorPageViewModel>().InitializeController();
                ServiceLocator.Current.GetInstance<MonitorPageViewModel>().MessageShow("系统数据加载完成！");
            });
            //没有在设计模式，在运行模式
            if (!IsInDesignMode)
            {
                #region 构建主菜单
                Menus = new List<MenuModel>
                {
                    new MenuModel
                    {
                        IsSelected = true,
                        MenuHeader = "监控",
                        MenuIcon = "\xe60a",
                        TargetView = "MonitorPage"
                    },
                    new MenuModel
                    {
                        MenuHeader = "模板",
                        MenuIcon = "\xe608",
                        TargetView = "TemplatePage"
                    },
                    new MenuModel
                    {
                        MenuHeader = "轨迹",
                        MenuIcon = "\xe753",
                        TargetView = "LocusPage"
                    },
                    new MenuModel
                    {
                        MenuHeader = "设备",
                        MenuIcon = "\xe65f",
                        TargetView = "EquipmentPage"
                    },
                    new MenuModel
                    {
                        MenuHeader = "报表",
                        MenuIcon = "\xe607",
                        TargetView = "ReportPage"
                    },
                    new MenuModel
                    {
                        MenuHeader = "设置",
                        MenuIcon = "\xe61e",
                        TargetView = "SettingsPage"
                    }
                };
                #endregion
                ShowPage(Menus[0]);
                SwitchPageCommand = new RelayCommand<object>(ShowPage);
 
            }
        }

        /// <summary>
        /// 系统配置数据初始化
        /// </summary>
        private void SystemConfigInitialize()
        {
            //模板设置参数
            ConfigEdit.ReadAppSettings("InnerCaliperLength", out string innerCaliperLength);
            InnerCaliperLength = int.Parse(innerCaliperLength);
            ConfigEdit.ReadAppSettings("InnerRadius", out string innerRadius);
            InnerRadius = int.Parse(innerRadius);
            ConfigEdit.ReadAppSettings("CalipersDevExpand", out string calipersDevExpand);
            CalipersDevExpand = int.Parse(calipersDevExpand);
            ConfigEdit.ReadAppSettings("CalipersMeaLength", out string calipersMeaLength);
            CalipersMeaLength = int.Parse(calipersMeaLength);
            ConfigEdit.ReadAppSettings("CalipersMeaWidth", out string calipersMeaWidth);
            CalipersMeaWidth = int.Parse(calipersMeaWidth);
            ConfigEdit.ReadAppSettings("CalipersAmpThreshold", out string calipersAmpThreshold);
            CalipersAmpThreshold = int.Parse(calipersAmpThreshold);
            ConfigEdit.ReadAppSettings("CalipersSmooth", out string calipersSmooth);
            CalipersSmooth = int.Parse(calipersSmooth);

            ConfigEdit.ReadAppSettings("OuterMinThreshold", out string outerMinThreshold);
            OuterMinThreshold = int.Parse(outerMinThreshold);
            ConfigEdit.ReadAppSettings("MinSimilarity", out string minSimilarity);
            MinSimilarity = double.Parse(minSimilarity);
            ConfigEdit.ReadAppSettings("ImageScale", out string imageScale);
            ImageScale = double.Parse(imageScale);
            ConfigEdit.ReadAppSettings("TemplateAngleStart", out string templateAngleStart);
            TemplateAngleStart = double.Parse(templateAngleStart);
            ConfigEdit.ReadAppSettings("TemplateAngleExtent", out string templateAngleExtent);
            TemplateAngleExtent = double.Parse(templateAngleExtent);

            //2D轨迹设置参数
            ConfigEdit.ReadAppSettings("DarkMinArea", out string darkMinArea);
            DarkMinArea = double.Parse(darkMinArea);
            ConfigEdit.ReadAppSettings("BrightMinArea", out string brightMinArea);
            BrightMinArea = double.Parse(brightMinArea);
            ConfigEdit.ReadAppSettings("SingleXldDilation", out string singleXldDilation);
            SingleXldDilation = double.Parse(singleXldDilation);
            ConfigEdit.ReadAppSettings("DarkMaxThreshold", out string darkMaxThreshold);
            DarkMaxThreshold = int.Parse(darkMaxThreshold);
            ConfigEdit.ReadAppSettings("BrightMinThreshold", out string brightMinThreshold);
            BrightMinThreshold = int.Parse(brightMinThreshold);
            ConfigEdit.ReadAppSettings("UnionDilationErosion", out string unionDilationErosion);
            UnionDilationErosion = double.Parse(unionDilationErosion);
            ConfigEdit.ReadAppSettings("MachiningLocusOffset", out string machiningLocusOffset);
            MachiningLocusOffset = int.Parse(machiningLocusOffset);
            ConfigEdit.ReadAppSettings("MaxDistance", out string maxDistance);
            MaxDistance = double.Parse(maxDistance);

            ConfigEdit.ReadAppSettings("CannyAlpha", out string cannyAlpha);
            CannyAlpha = double.Parse(cannyAlpha);
            ConfigEdit.ReadAppSettings("CannyLowThresold", out string cannyLowThresold);
            CannyLowThresold = int.Parse(cannyLowThresold);
            ConfigEdit.ReadAppSettings("CannyHighThresold", out string cannyHighThresold);
            CannyHighThresold = int.Parse(cannyHighThresold);
            ConfigEdit.ReadAppSettings("XldMinLength", out string xldMinLength);
            XldMinLength = int.Parse(xldMinLength);
            ConfigEdit.ReadAppSettings("MaskWidthHeight", out string maskWidthHeight);
            MaskWidthHeight = int.Parse(maskWidthHeight);


            ConfigEdit.ReadAppSettings("IgsPath", out string IgsPath);
            LocusParameterSettingModel.IgsPath = IgsPath;


            //从配置文件中获取基础姿态四元数
            ConfigEdit.ReadAppSettings("BaseQuaternion", out string baseQuaternion);
            var quaternion = baseQuaternion.Split(',').ToList();
            ViewportHelper.W = double.Parse(quaternion[0]);
            ViewportHelper.X = double.Parse(quaternion[1]);
            ViewportHelper.Y = double.Parse(quaternion[2]);
            ViewportHelper.Z = double.Parse(quaternion[3]);

            //四元数转欧拉角
            ViewportHelper.QuaternionToEuler(ViewportHelper.W, ViewportHelper.X, ViewportHelper.Y, ViewportHelper.Z, out double ex, out double ey, out double ez);
            ViewportHelper.EX = Math.Round(ex, 2);
            ViewportHelper.EY = Math.Round(ey, 2);
            ViewportHelper.EZ = Math.Round(ez, 2);

            //3D轨迹设置参数
            ConfigEdit.ReadAppSettings("EntryPointXAxisOffsetDistance", out string entryPointXAxisOffsetDistance);
            EntryPointXAxisOffsetDistance = double.Parse(entryPointXAxisOffsetDistance);
            ConfigEdit.ReadAppSettings("EntryPointYAxisOffsetDistance", out string entryPointYAxisOffsetDistance);
            EntryPointYAxisOffsetDistance = double.Parse(entryPointYAxisOffsetDistance);
            ConfigEdit.ReadAppSettings("ExitPointXAxisOffsetDistance", out string exitPointXAxisOffsetDistance);
            ExitPointXAxisOffsetDistance = double.Parse(exitPointXAxisOffsetDistance);
            ConfigEdit.ReadAppSettings("ExitPointYAxisOffsetDistance", out string exitPointYAxisOffsetDistance);
            ExitPointYAxisOffsetDistance = double.Parse(exitPointYAxisOffsetDistance);
            ConfigEdit.ReadAppSettings("EntryExitPointOffsetHeight", out string entryExitPointOffsetHeight);
            EntryExitPointOffsetHeight = double.Parse(entryExitPointOffsetHeight);
            ConfigEdit.ReadAppSettings("TotalTrajectoryPointsAllowed", out string totalTrajectoryPointsAllowed);
            TotalTrajectoryPointsAllowed = int.Parse(totalTrajectoryPointsAllowed);
            ConfigEdit.ReadAppSettings("IncreasePointOffsetDistance", out string increasePointOffsetDistance);
            IncreasePointOffsetDistance = double.Parse(increasePointOffsetDistance);
        }

        private void ShowPage(object obj)
        {
            var model = obj as MenuModel;
            if (model != null)
            {
                //解决重复创建
                if (ViewContent != null && ViewContent.GetType().Name == model.TargetView) return;
                Type type =  Assembly.Load("HubDeburrSystem").GetType("HubDeburrSystem.Views.Pages." +  model.TargetView);
                ViewContent = Activator.CreateInstance(type);
            }
        }
    }
}