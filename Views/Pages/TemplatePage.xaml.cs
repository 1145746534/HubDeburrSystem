using ABB.Robotics.Controllers.RapidDomain;
using CommonServiceLocator;
using HalconDotNet;
using HubDeburrSystem.DataAccess;
using HubDeburrSystem.Models;
using HubDeburrSystem.Public;
using HubDeburrSystem.ViewModel;
using HubDeburrSystem.Views.Dialog;
using Sharp7;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using CheckBox = System.Windows.Controls.CheckBox;
using DataGrid = System.Windows.Controls.DataGrid;
using static HubDeburrSystem.Models.LocusParameterSettingModel;
using ContextMenu = System.Windows.Controls.ContextMenu;
using MenuItem = System.Windows.Controls.MenuItem;
using System.Windows.Threading;
using System.Windows.Media;
using NPOI.OpenXmlFormats.Vml;
using NPOI.OpenXmlFormats.Wordprocessing;

namespace HubDeburrSystem.Views.Pages
{
    /// <summary>
    /// TemplatePage.xaml 的交互逻辑
    /// </summary>
    public partial class TemplatePage : System.Windows.Controls.UserControl
    {
        #region==============字段定义================
        /// <summary>
        /// 用于制作模板的图片
        /// </summary>
        private HObject TemplateImage = new HObject();

        /// <summary>
        /// 原始窗口XLD
        /// </summary>
        private HObject WindowXLD = new HObject();

        /// <summary>
        /// 新窗口XLD
        /// </summary>
        private HObject NewWindowXLD = new HObject();

        /// <summary>
        /// 轨迹窗口XLD
        /// </summary>
        private HObject LocusWindowXLD = new HObject();

        /// <summary>
        /// 内圆XLD
        /// </summary>
        private HObject InnerCircleXLD = new HObject();

        /// <summary>
        /// 外圆XLD
        /// </summary>
        private HObject OuterCircleXLD = new HObject();

        /// <summary>
        /// 内圆中心Row坐标(Y)
        /// </summary>
        private HTuple CenterRow = new HTuple();

        /// <summary>
        /// 内圆中心Column坐标(X)
        /// </summary>
        private HTuple CenterColumn = new HTuple();

        /// <summary>
        /// 从原始图像中剪切出的轮毂图像
        /// </summary>
        private HObject ImageReduced = new HObject();

        /// <summary>
        /// 轮辐排序反转
        /// </summary>
        private bool SpokeSorting = false;

        /// <summary>
        /// 当前编辑的加工轨迹数据
        /// </summary>
        private List<MachiningPathPosModel> MachiningPathDatas { get; set; } = new List<MachiningPathPosModel>();
        /// <summary>
        /// 最终的加工轨迹数据
        /// </summary>
        private List<MachiningPathPosModel> EndMachiningPathDatas { get; set; } = new List<MachiningPathPosModel>();
        #endregion

        public TemplatePage()
        {
            InitializeComponent();

            if (IsImageEnhancement)
                ImageEnhancement.IsChecked = true;
            else ImageEnhancement.IsChecked = false;
            this.Unloaded += TemplatePage_Unloaded;
            this.Loaded += TemplatePage_Loaded;
            SelectWheelModel = (TemplateDataGrid.SelectedItem as TemplateDataModel).WheelType;

        }

        #region==============信息显示================
        private void MessageShowTimer_Tick(object sender, EventArgs e)
        {
            MessageBorder.Visibility = Visibility.Hidden;
            MessageShowTimer.Stop();
        }
        /// <summary>
        /// 信息显示定时器
        /// </summary>
        private DispatcherTimer MessageShowTimer;
        private void MessageShow(string message, MessageType type)
        {
            MessageTextBlock.Text = message;
            MessageBorder.Visibility = Visibility.Visible;
            if (type == MessageType.Default)
            {
                MessageBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFDDE3FB"));
            }
            else if (type == MessageType.Warning)
            {
                MessageBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF3F5C8"));
            }
            else if (type == MessageType.Success)
            {
                MessageBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFAADEB7"));
            }
            else if (type == MessageType.Error)
            {
                MessageBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF78883"));
            }
            if (MessageShowTimer != null && MessageShowTimer.IsEnabled) MessageShowTimer.Stop();
            MessageShowTimer = new DispatcherTimer();
            MessageShowTimer.Interval = new TimeSpan(0, 0, 3);
            MessageShowTimer.Tick += MessageShowTimer_Tick;
            MessageShowTimer.Start();
        }
        #endregion

        /// <summary>
        /// 结果显示
        /// </summary>
        /// <param name="wheelType">轮型</param>
        /// <param name="similarity">相似度</param>
        /// <param name="rotationAngle">旋转角度</param>
        /// <param name="centerRow">中心行坐标</param>
        /// <param name="centerColumn">中心列坐标</param>
        /// <param name="centerRowOffset">中心行坐标偏移</param>
        /// <param name="centerColumnOffset">中心列坐标偏移</param>
        /// <param name="totalPoints">总轨迹点数</param>
        /// <param name="startTime">开始时间</param>
        private void ResultDisplay(string wheelType, string similarity, HTuple rotationAngle, HTuple centerRow, HTuple centerColumn,
            HTuple centerRowOffset, HTuple centerColumnOffset, string totalPoints, DateTime startTime)
        {
            if (wheelType != null) WheelType_Label.Text = wheelType;
            else WheelType_Label.Text = "";
            if (similarity != null) Similarity_Label.Content = similarity;
            else Similarity_Label.Content = "";
            if (rotationAngle != null) RotationAngle_Label.Content = Math.Round(rotationAngle.TupleDeg().D, 2).ToString() + " °";
            else RotationAngle_Label.Content = "";
            if (centerRow != null) ImageCenterRow_Label.Content = Math.Round(centerRow.D, 2).ToString();
            else ImageCenterRow_Label.Content = "";
            if (centerColumn != null) ImageCenterColumn_Label.Content = Math.Round(centerColumn.D, 2).ToString();
            else ImageCenterColumn_Label.Content = "";
            if (centerRowOffset != null) ImageCenterRowOffset_Label.Content = Math.Round(centerRowOffset.D, 2).ToString() + " 像素";
            else ImageCenterRowOffset_Label.Content = "";
            if (centerColumnOffset != null) ImageCenterColumnOffset_Label.Content = Math.Round(centerColumnOffset.D, 2).ToString() + " 像素";
            else ImageCenterColumnOffset_Label.Content = "";
            if (totalPoints != null) TotalPoints_Label.Content = totalPoints;
            else TotalPoints_Label.Content = "";
            if (startTime != null)
            {
                var endTime = DateTime.Now;
                TimeSpan processingTime = endTime.Subtract(startTime);
                ProcessingTime_Label.Content = Math.Round(processingTime.TotalMilliseconds, 0).ToString() + "ms";
                ServiceLocator.Current.GetInstance<TemplatePageViewModel>().ResultShow = Visibility.Visible;
            }
            else ProcessingTime_Label.Content = "";
        }
        /// <summary>
        /// 根据原始轨迹数据和新的轮廓生成新的轨迹数据
        /// </summary>
        /// <param name="sourceDatas">原始轨迹数据</param>
        /// <param name="newXld">新的轮廓</param>
        /// <returns>新的轨迹数据</returns>
        private List<MachiningPathPosModel> GenMachiningPathDatas(List<MachiningPathPosModel> sourceDatas, HObject newXld)
        {
            List<MachiningPathPosModel> datas = new List<MachiningPathPosModel>();
            HOperatorSet.CountObj(newXld, out HTuple number);
            for (int i = 1; i <= number; i++)
            {
                HOperatorSet.SelectObj(newXld, out HObject selectedXld, i);
                HOperatorSet.GetContourXld(selectedXld, out HTuple rows, out HTuple cols);
                List<MachiningPathPosModel> points = sourceDatas.Where(x => x.PoseId >= i * 1000 && x.PoseId <= i * 1000 + 900).ToList();
                for (int j = 0; j < rows.Length; j++)
                {
                    MachiningPathPosModel data = new MachiningPathPosModel();
                    data.Id = points[j].Id;
                    data.PoseId = points[j].PoseId;
                    data.Row = rows[j];
                    data.Column = cols[j];
                    //获取指定高度下的新齐次变换矩阵
                    var homMat2D = ServiceLocator.Current.GetInstance<SettingPageViewModel>().GetPixelToPhysicsHomMat2D(points[j].Z, TemplateHelper.PixelToPhysicsHomMat2D);
                    HOperatorSet.AffineTransPoint2d(homMat2D, rows[j], cols[j], out HTuple qx, out HTuple qy);
                    data.X = Math.Round(qx.D, 3);
                    data.Y = Math.Round(qy.D, 3);
                    data.Z = points[j].Z;
                    data.EX = points[j].EX;
                    data.EY = points[j].EY;
                    data.EZ = points[j].EZ;
                    data.Q1 = points[j].Q1;
                    data.Q2 = points[j].Q2;
                    data.Q3 = points[j].Q3;
                    data.Q4 = points[j].Q4;
                    datas.Add(data);
                }
            }
            return datas;
        }

        #region==============识别模板制作================
        /// <summary>
        /// 采集图像
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AcquireImages_btn_Click(object sender, RoutedEventArgs e)
        {
            ServiceLocator.Current.GetInstance<TemplatePageViewModel>().ResultShow = Visibility.Collapsed;
            if (TemplateHelper.CameraHandle == null)
            {
                //UMessageBox.Show("相机连接异常，请检查！", MessageType.Error);
                MessageShow("相机连接异常，请检查！", MessageType.Error);
                return;
            }
            try
            {
                TemplateImage.Dispose();
                HOperatorSet.GrabImage(out HObject image, TemplateHelper.CameraHandle);
                HOperatorSet.Rgb1ToGray(image, out TemplateImage);
                TemplateHelper.DisplayImage(TemplateImage, TemplateHalconWindow.HalconWindow, true, IsImageEnhancement);
                var month = Convert.ToString(DateTime.Now.Month);//获取当前的月份
                var day = Convert.ToString(DateTime.Now.Day);//获取当前的日期
                string monthPath = $"D:\\DeburrSystem\\GrabImages\\{month}月";
                string dayPath = $"D:\\DeburrSystem\\GrabImages\\{month}月\\{day}日";
                string name = DateTime.Now.ToString("HHmmss");
                string imagePath = @"D:/DeburrSystem/GrabImages/" + month + "月/" + day + "日/" + name + ".tif";
                if (Directory.Exists(monthPath) == false) Directory.CreateDirectory(monthPath);
                if (Directory.Exists(dayPath) == false) Directory.CreateDirectory(dayPath);
                HOperatorSet.WriteImage(TemplateImage, "tiff", 0, imagePath);
            }
            catch (Exception ex)
            {
                //UMessageBox.Show("采集异常：" + ex.Message, MessageType.Error);
                MessageShow("采集异常：" + ex.Message, MessageType.Error);
            }
        }
        /// <summary>
        /// 读取图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReadImage_btn_Click(object sender, RoutedEventArgs e)
        {
            ServiceLocator.Current.GetInstance<TemplatePageViewModel>().ResultShow = Visibility.Collapsed;
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "请选择制作模板的图片",
                Filter = "TIF文件|*.tif|JPEG文件|*.jpg|BMP文件|*.bmp|PNG文件|*.png|所有文件(*.*)|*.*",
                InitialDirectory = $"D:\\DeburrSystem\\GrabImages\\{DateTime.Now.Month}月",
                FilterIndex = 0,
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var fileName = openFileDialog.FileName;
                    TemplateImage.Dispose();
                    HOperatorSet.ReadImage(out HObject image, fileName);
                    HOperatorSet.Rgb1ToGray(image, out TemplateImage);
                    TemplateHelper.DisplayImage(TemplateImage, TemplateHalconWindow.HalconWindow, true, IsImageEnhancement);
                    //图片不拉伸显示

                    image.Dispose();
                }
                catch (Exception ex) { MessageShow(ex.Message, MessageType.Error); }
            }
        }

        /// <summary>
        /// 清除显示按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearShow_btn_Click(object sender, RoutedEventArgs e)
        {
            TemplateHalconWindow.HalconWindow.ClearWindow();
            TemplateImage.Dispose();
            WindowXLD.Dispose();
            InnerCircleXLD.Dispose();
            OuterCircleXLD.Dispose();
            NewWindowXLD.Dispose();
            LocusWindowXLD.Dispose();
            if (CenterRow != null && CenterColumn != null)
            {
                CenterRow.UnpinTuple();
                CenterColumn.UnpinTuple();
            }
            ServiceLocator.Current.GetInstance<TemplatePageViewModel>().ResultShow = Visibility.Collapsed;
        }

        /// <summary>
        /// 定位内外圆
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PositioningCircle_btn_Click(object sender, RoutedEventArgs e)
        {
            if (!TemplateImage.IsInitialized())
            {
                //UMessageBox.Show("无图像信息，请先读取或采集图像！", MessageType.Default);
                MessageShow("无图像信息，请先读取或采集图像！", MessageType.Default);
                return;
            }
            //============ 获取内圆 ============
            //HOperatorSet.GenCircle(out HObject circle, 1780, 2080, 300);
            //HOperatorSet.ReduceDomain(TemplateImage, circle, out HObject innerImage);
            //HOperatorSet.Threshold(innerImage, out HObject region, 0, InnerMaxThreshold);
            //HOperatorSet.Connection(region, out HObject connectedRegions);
            ////第一次筛选，根据圆度
            //HOperatorSet.SelectShape(connectedRegions, out HObject oneSelectedRegions, "roundness", "and", 0.95, 1);
            ////第二次筛选内圆，根据最大面积
            //HOperatorSet.SelectShapeStd(oneSelectedRegions, out HObject twoSelectedRegions, "max_area", 70);
            //HOperatorSet.AreaCenter(twoSelectedRegions, out _, out CenterRow, out CenterColumn);
            //HOperatorSet.InnerCircle(twoSelectedRegions, out _, out _, out HTuple innerCircleRadius);
            //if (CenterRow.Length <= 0)
            //{
            //    MessageShow("获取中心失败，请调整参数后再试！", MessageType.Default);
            //    return;
            //}
            //HOperatorSet.GenCircleContourXld(out InnerCircleXLD, CenterRow, CenterColumn, innerCircleRadius, 0, 6.28318, "positive", 1);



            //===========获取外圆============
            //HOperatorSet.Threshold(TemplateImage, out HObject region1, OuterMinThreshold, 255);
            //HOperatorSet.Connection(region1, out HObject connectedRegions1);
            //HOperatorSet.FillUp(connectedRegions1, out HObject regionFillUp1);
            //HOperatorSet.SelectShape(regionFillUp1, out HObject selectedRegions, "area", "and", 5000000, 99999999);
            //HOperatorSet.InnerCircle(selectedRegions, out HTuple row, out HTuple column, out HTuple radius);

            TemplateHelper.GetOuterCircle(TemplateImage, OuterMinThreshold, out HTuple row, out HTuple column, out HTuple outRadius);
            if (row.Length <= 0)
            {
                MessageShow("获取外圆失败，请调整参数后再试！", MessageType.Default);
                return;
            }

            try
            {

                //获取内圆 通过找圆心的方式
                HObject ho_Contours = TemplateHelper.GenInnerCircle(TemplateImage, row, column, InnerRadius, InnerCaliperLength, out CenterRow, out CenterColumn, out HTuple innerCircleRadius);

                HOperatorSet.GenCircleContourXld(out InnerCircleXLD, CenterRow, CenterColumn, innerCircleRadius, 0, 6.28318, "positive", 1);

                //生成外圆轮廓
                HOperatorSet.GenCircleContourXld(out OuterCircleXLD, row, column, outRadius - 10, 0, 6.28318, "positive", 1);
                //生成外圆区域
                HOperatorSet.GenRegionContourXld(OuterCircleXLD, out HObject r, "filled");

                //剪切制作模板的区域
                HOperatorSet.ReduceDomain(TemplateImage, r, out ImageReduced);
                //显示
                HOperatorSet.GenCrossContourXld(out HObject cross, CenterRow, CenterColumn, 200, 0);
                TemplateHelper.TemplateWindowDisplay(TemplateHalconWindow.HalconWindow, TemplateImage, null, InnerCircleXLD, OuterCircleXLD, null, cross, null, IsImageEnhancement);
                TemplateHalconWindow.HalconWindow.DispObj(ho_Contours);
            }
            catch (Exception ex)
            {
                MessageShow("内圆定位失败"+ex.Message, MessageType.Error);
            }
        }

        /// <summary>
        /// 显示模板NCC
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowTemplate_btn_Click(object sender, RoutedEventArgs e)
        {
            if (TemplateDataGrid.SelectedItem == null)
            {
                MessageShow("无轮型信息，请在轮毂数据窗口选择轮型!", MessageType.Default);
                return;
            }
            var wheelType = (TemplateDataGrid.SelectedItem as TemplateDataModel).WheelType;
            string imagePath = @"D:\DeburrSystem\TemplateImages\" + wheelType + ".tif";
            if (!File.Exists(imagePath))
            {
                MessageShow("此轮型无模板数据！", MessageType.Default);
                return;
            }
            string xldPath = @"d:\Deburr\DXF文件\" + wheelType + ".dxf";
            if (!File.Exists(xldPath))
            {
                MessageShow("此轮型无轮廓数据！", MessageType.Default);
                return;
            }
            ServiceLocator.Current.GetInstance<TemplatePageViewModel>().ResultShow = Visibility.Collapsed;

            HOperatorSet.ReadImage(out HObject image, imagePath);
            TemplateImage.Dispose();
            TemplateImage = image.Clone();
            image.Dispose();

            HOperatorSet.ReadContourXldDxf(out HObject contours, xldPath, new HTuple(), new HTuple(), out HTuple dxfStatus);

            HOperatorSet.CloseContoursXld(contours, out HObject closedContours);

            TemplateHelper.DisplayImage(TemplateImage, TemplateHalconWindow.HalconWindow, true, IsImageEnhancement);
            TemplateHalconWindow.HalconWindow.SetColor("magenta");
            TemplateHalconWindow.HalconWindow.DispObj(closedContours);
        }

        /// <summary>
        /// 删除模板按钮NCC
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DelTmplate_btn_Click(object sender, RoutedEventArgs e)
        {
            if (!(ServiceLocator.Current.GetInstance<TemplatePageViewModel>().TemplateDatas.Count > 0))
            {
                MessageShow("无模板数据，请先录入模板!", MessageType.Default);
                return;
            }
            if (TemplateDataGrid.SelectedItem == null)
            {
                MessageShow("无模板信息，请先选择模板!", MessageType.Default);
                return;
            }
            var wheelType = (TemplateDataGrid.SelectedItem as TemplateDataModel).WheelType;
            bool result = UMessageBox.Show("删除确认", "确定删除模板：" + wheelType + " 吗？");
            if (result)
            {
                ServiceLocator.Current.GetInstance<TemplatePageViewModel>().TemplateButtonEnabled = false;
                //===============根据轮型删除保存在硬盘中的数据并修改模板数据显示窗口===============
                var sDB = new SqlAccess().SystemDataAccess;
                //获取删除轮型的数据
                var data = sDB.Queryable<TemplateDataModel>().Where(it => it.WheelType == wheelType).ToList();

                //根据轮型删除硬盘中的数据
                sDB.Deleteable<TemplateDataModel>().Where(it => it.WheelType == wheelType).ExecuteCommand();
                //获取数据并按轮型排序
                var datas = sDB.Queryable<TemplateDataModel>().OrderBy(it => it.WheelType).ToList();
                //整理序号
                for (int i = 0; i < datas.Count; i++)
                {
                    datas[i].Index = i + 1;
                }
                //清空表
                sDB.DbMaintenance.TruncateTable<TemplateDataModel>();
                //写入表
                sDB.Insertable(datas).ExecuteCommand();
                //更新实时修改模板数据集
                ServiceLocator.Current.GetInstance<TemplatePageViewModel>().TemplateDatas.Clear();
                for (int i = 0; i < datas.Count; i++)
                {
                    ServiceLocator.Current.GetInstance<TemplatePageViewModel>().TemplateDatas.Add(datas[i]);
                }
                //修改模板数据显示窗口选中项
                if (datas.Count > 1 && data[0].Index > 1 && datas.Count != data[0].Index - 1)
                {
                    ServiceLocator.Current.GetInstance<TemplatePageViewModel>().SelectIndex = data[0].Index - 1;
                }
                else if (data[0].Index == 1)
                {
                    ServiceLocator.Current.GetInstance<TemplatePageViewModel>().SelectIndex = 0;
                }
                else
                {
                    ServiceLocator.Current.GetInstance<TemplatePageViewModel>().SelectIndex = data[0].Index - 2;
                }
                //===============删除保存在硬盘中的对应轮型的源轨迹数据
                var slDB = new SqlAccess().SourceLocusDataAccess;
                //判断对应轮型的源轨迹数据表是否存在
                var r = slDB.DbMaintenance.IsAnyTable(wheelType, false);
                //删除表
                if (r) slDB.DbMaintenance.DropTable(wheelType);

                //===============删除保存在硬盘中的加工轨迹数据表
                var plDB = new SqlAccess().ProcessingLocusDataAccess;
                var r1 = plDB.DbMaintenance.IsAnyTable(wheelType, false);
                if (r1) plDB.DbMaintenance.DropTable(wheelType);

                //======================删除保存在硬盘中的模板文件=======================
                var templateImagePath = @"D:/DeburrSystem/TemplateImages/" + wheelType + ".tif";
                string activeTemplatePath = @"D:/DeburrSystem/ActiveTemplates/" + wheelType + ".ncm";
                string notActiveTemplatePath = @"D:/DeburrSystem/NotActiveTemplates/" + wheelType + ".ncm";
                if (File.Exists(templateImagePath)) File.Delete(templateImagePath);
                if (File.Exists(activeTemplatePath)) File.Delete(activeTemplatePath);
                if (File.Exists(notActiveTemplatePath)) File.Delete(notActiveTemplatePath);

                //====================删除内存中的源轨迹数据=======================
                var index = TemplatePageViewModel.SourceLocusDatas.LocusName.FindIndex(x => x == wheelType);
                if (index >= 0)
                {
                    TemplatePageViewModel.SourceLocusDatas.LocusName.RemoveAt(index);
                    TemplatePageViewModel.SourceLocusDatas.LocusPoints.RemoveAt(index);
                }

                //====================删除内存中的加工轨迹数据=======================
                ServiceLocator.Current.GetInstance<LocusPageViewModel>().DeleteProcessingLocus(wheelType);

                //=====================删除内存中用于匹配的模板=========================
                ServiceLocator.Current.GetInstance<TemplatePageViewModel>().DeleteProcessingTemplate(wheelType);

                //=====================移除报表窗口轮形数据中对应的轮形=========================
                var index4 = ServiceLocator.Current.GetInstance<ReportPageViewModel>().WheelTypeDatas.FindIndex(x => x == wheelType);
                if (index4 >= 0)
                {
                    ServiceLocator.Current.GetInstance<ReportPageViewModel>().WheelTypeDatas.RemoveAt(index4);
                }

                MessageShow("删除成功，删除的模板是：" + wheelType, MessageType.Success);
            }
        }

        /// <summary>
        /// 识别测试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IdentificationTest_btn_Click(object sender, RoutedEventArgs e)
        {
            if (TemplatePageViewModel.TemplateList.ActiveTemplateList.Count <= 0 && TemplatePageViewModel.TemplateList.NotActiveTemplateList.Count <= 0)
            {
                MessageShow("无模板数据，请先录入模板！", MessageType.Warning);
                return;
            }
            if (!TemplateImage.IsInitialized())
            {
                MessageShow("无图像数据，请先采集图像或读取图片！", MessageType.Warning);
                return;
            }
            ServiceLocator.Current.GetInstance<TemplatePageViewModel>().ResultShow = Visibility.Collapsed;
            try
            {
                DateTime startTime = DateTime.Now;
                #region=========================轮型识别====================
                HOperatorSet.ZoomImageFactor(TemplateImage, out HObject imageZoomed, ImageScale, ImageScale, "constant");
                //轮型识别
                IdentifyDataModel result = TemplateHelper.IdentifyAlgorithm(imageZoomed, TemplatePageViewModel.TemplateList, TemplateAngleStart, TemplateAngleExtent, MinSimilarity);
                if (result == null)
                {
                    ResultDisplay("NG", null, null, null, null, null, null, null, startTime);
                    TemplateHalconWindow.HalconWindow.ClearWindow();
                    TemplateHalconWindow.HalconWindow.DispObj(TemplateImage);
                    return;
                }
                #endregion
                #region=========================获取轮毂中心====================
                //从加工模板数据中获取轮型数据
                TemplateDataModel data = ServiceLocator.Current.GetInstance<TemplatePageViewModel>().ProcessingTemplateDatas.First(x => x.WheelType == result.IdentifyWheelType);
                //获取轮毂中心
                TemplateHelper.GetWheelCenter(TemplateImage, data.InnerCircleRadius, data.InnerCircleCaliperLength, out HTuple centerRow, out HTuple centerColumn);
                if (centerRow.Length == 0)
                {
                    MessageShow("查找轮毂中心失败，请调整轮毂中心阈值参数后再试！", MessageType.Default);
                    return;
                }
                #endregion

                //计算偏移
                var rowOffset = data.CenterRow - centerRow;
                var columnOffset = data.CenterColumn - centerColumn;
                //模板中心
                HOperatorSet.GenCrossContourXld(out HObject oldCenterCross, data.CenterRow, data.CenterColumn, 200, 0);
                //当前轮毂中心
                HOperatorSet.GenCrossContourXld(out HObject newCenterCross, centerRow, centerColumn, 200, 0);
                //计算平均弧度
                HOperatorSet.TupleRad(360.0 / data.SpokeQuantity, out HTuple averageRad);
                //当匹配出的弧度大于0.5时，计算最少旋转弧度
                HTuple radian = null;
                if (result.Radian > 0.5)
                {
                    var rad = result.Radian / averageRad;
                    radian = result.Radian - averageRad * ((int)rad.D + 1);
                }
                else radian = result.Radian;
                HOperatorSet.TupleDeg(radian, out HTuple angle);
                //查找当前识别轮型是否存在加工轨迹数据
                int index = LocusPageViewModel.ProcessingLocusDatas.LocusName.FindIndex(x => x == result.IdentifyWheelType);
                //如果加工轨迹不存在
                if (index < 0)
                {
                    ResultDisplay(result.IdentifyWheelType, result.Similarity.ToString(), radian, centerRow, centerColumn, rowOffset, columnOffset, "", startTime);
                    MessageShow("加工轨迹不存在！", MessageType.Warning);
                    return;
                }
                #region=========================轨迹处理=============================
                //获取当前识别轮毂的轨迹数据
                List<MachiningPathPosModel> datas = LocusPageViewModel.ProcessingLocusDatas.LocusPoints[index];
                //根据加工轨迹数据和识别数据生成新的加工数据
                List<MachiningPathPosModel> newDatas = new List<MachiningPathPosModel>();
                for (int i = 0; i < datas.Count; i++)
                {
                    MachiningPathPosModel point = new MachiningPathPosModel();
                    point.Id = datas[i].Id;
                    point.PoseId = datas[i].PoseId;
                    //旋转 - 加工轨迹点饶当前轮毂中心点逆时针旋转
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
                //MachiningLocusOffset = 0;
                if (MachiningLocusOffset != 0)
                {
                    //newDatas = TemplateHelper.LocusScale(newDatas, MachiningLocusOffset, TemplateHelper.PixelToPhysicsHomMat2D);
                }

                int spokeType1 = newDatas[newDatas.Count - 1].PoseId / 1000 / data.SpokeQuantity;
                string[] newOutPoseIds;
                newDatas = TemplateHelper.LocusScale1(newDatas, TemplateHelper.PixelToPhysicsHomMat2D, TemplateImage,spokeType1,out newOutPoseIds, data.OutPointPoseId);
                string newOutPoseIdStr = string.Join(",", newOutPoseIds);
                //判断是否存在进出刀点
                List<MachiningPathPosModel> ioPoints = newDatas.Where(x => x.PoseId > 1900 && x.PoseId <= 1999).ToList();
                List<MachiningPathPosModel> toRobotDatas = new List<MachiningPathPosModel>();
                HOperatorSet.GenEmptyObj(out HObject endXld);
                if (ioPoints.Count >= 6)
                {
                    int spokeType = newDatas[newDatas.Count - 1].PoseId / 1000 / data.SpokeQuantity;
                    //toRobotDatas = TemplateHelper.GenLocusOfOutPoint(newDatas, data.OutPointPoseId, spokeType);
                    toRobotDatas = TemplateHelper.GenLocusOfOutPoint1(newDatas, newOutPoseIdStr, spokeType);
                    endXld = TemplateHelper.GenContours(toRobotDatas, true);
                }
                else
                {
                    toRobotDatas = newDatas;
                    endXld = TemplateHelper.GenContours(toRobotDatas, false);
                    ResultDisplay(result.IdentifyWheelType, result.Similarity.ToString(), radian, centerRow, centerColumn, rowOffset, columnOffset, datas.Count.ToString(), startTime);
                    TemplateHelper.TemplateWindowDisplay(TemplateHalconWindow.HalconWindow, TemplateImage, null, null, newContours, endXld, oldCenterCross, newCenterCross, IsImageEnhancement);
                    MessageShow("当前轮型无进出刀点，请检查！", MessageType.Warning);
                    return;
                }
                #endregion

                //轨迹窗口当前识别轮毂数据检查
                ServiceLocator.Current.GetInstance<LocusPageViewModel>().CurrentMachiningLocusDatas.Clear();
                ServiceLocator.Current.GetInstance<LocusPageViewModel>().CurrentMachiningLocusDatas = newDatas;
                ServiceLocator.Current.GetInstance<LocusPageViewModel>().CurrentIdentifyData = null;
                ServiceLocator.Current.GetInstance<LocusPageViewModel>().CurrentIdentifyData = result;
                ServiceLocator.Current.GetInstance<LocusPageViewModel>().CurrentMachiningImage.Dispose();
                ServiceLocator.Current.GetInstance<LocusPageViewModel>().CurrentMachiningImage = TemplateImage.Clone();

                #region============判断中心偏移大于=========
                if (rowOffset.D > 20 || columnOffset.D > 20)
                {
                    ResultDisplay(result.IdentifyWheelType, result.Similarity.ToString(), radian, centerRow, centerColumn, rowOffset, columnOffset, datas.Count.ToString(), startTime);
                    TemplateHelper.TemplateWindowDisplay(TemplateHalconWindow.HalconWindow, TemplateImage, null, null, newContours, endXld, oldCenterCross, newCenterCross, IsImageEnhancement);
                    MessageShow("中心偏移过大，请检查！", MessageType.Warning);
                    return;
                }
                #endregion
                #region============判断是否存在大毛刺===========
                //获取源模板图像
                string imagePath = $"D:\\DeburrSystem\\TemplateImages\\{result.IdentifyWheelType}";
                HOperatorSet.ReadImage(out HObject sourceImage, imagePath);
                //根据中心和角度生成仿射矩阵
                HOperatorSet.VectorAngleToRigid(data.CenterRow, data.CenterColumn, 0, centerRow, centerColumn, radian, out HTuple homMat2D);
                HOperatorSet.AffineTransImage(sourceImage, out HObject affineImage, homMat2D, "constant", "false");
                HObject burr = TemplateHelper.LargeBurrJudgment(affineImage, TemplateImage, newContours);
                HOperatorSet.CountObj(burr, out HTuple number);
                if (number > 0)
                {
                    ResultDisplay(result.IdentifyWheelType, result.Similarity.ToString(), radian, centerRow, centerColumn, rowOffset, columnOffset, datas.Count.ToString(), startTime);
                    TemplateHelper.TemplateWindowDisplay(TemplateHalconWindow.HalconWindow, TemplateImage, null, null, newContours, endXld, oldCenterCross, newCenterCross, IsImageEnhancement);
                    TemplateHalconWindow.HalconWindow.SetColor("red");
                    TemplateHalconWindow.HalconWindow.DispObj(burr);
                    MessageShow("毛刺过大！", MessageType.Warning);
                    return;
                }
                #endregion
                #region=========================结果显示=============================
                ResultDisplay(result.IdentifyWheelType, result.Similarity.ToString(), radian, centerRow, centerColumn, rowOffset, columnOffset, toRobotDatas.Count.ToString(), startTime);

                //TemplateHelper.TemplateWindowDisplay(TemplateHalconWindow.HalconWindow, TemplateImage, null, null, newContours, endXld, oldCenterCross, newCenterCross, IsImageEnhancement);
                TemplateHelper.TemplateWindowDisplay(TemplateHalconWindow.HalconWindow, TemplateImage, null, null, newContours, endXld, oldCenterCross, newCenterCross, IsImageEnhancement);

                #endregion

                if (PlcHelper.PlcCilent.Connected && !S7.GetBitAt(PlcHelper.PlcReadDataBuffer, 6, 0))
                {
                    MessageShow("定中机构未夹紧！", MessageType.Warning);
                    return;
                }
                if (IsProcessing.IsChecked == false || ServiceLocator.Current.GetInstance<MonitorPageViewModel>().StartStopButtonContent == "系统启动") return;
                if (ServiceLocator.Current.GetInstance<MonitorPageViewModel>().RobotController != null &&
                    ServiceLocator.Current.GetInstance<MonitorPageViewModel>().RobotController.Connected &&
                    ServiceLocator.Current.GetInstance<MonitorPageViewModel>().RobotController.Rapid.ExecutionStatus == ExecutionStatus.Running &&
                    ServiceLocator.Current.GetInstance<MonitorPageViewModel>().RobotProcessingState == 0)
                {
                    var processingStartTime = DateTime.Now;
                    ServiceLocator.Current.GetInstance<MonitorPageViewModel>().MessageShow("加工轨迹发送中......");
                    //写入加工压力
                    byte[] bytes = BitConverter.GetBytes(data.ProcessingPressure);
                    byte[] buffer = new byte[] { bytes[3], bytes[2], bytes[1], bytes[0] };
                    Buffer.BlockCopy(buffer, 0, PlcHelper.PlcWriteDataBuffer, 2, buffer.Length);
                    System.Threading.Tasks.Task.Run(() =>
                    {
                        bool processing = false;
                        try
                        {
                            ServiceLocator.Current.GetInstance<MonitorPageViewModel>().SendLocusDatas(toRobotDatas);
                            ServiceLocator.Current.GetInstance<MonitorPageViewModel>().MessageShow("加工轨迹发送完成！");
                            processing = true;
                        }
                        catch (Exception ex)
                        {
                            ServiceLocator.Current.GetInstance<MonitorPageViewModel>().MessageShow("加工轨迹发送失败2：" + ex);
                            ServiceLocator.Current.GetInstance<MonitorPageViewModel>().CurrentBeat = "??";
                        }
                        while (processing)
                        {
                            if (ServiceLocator.Current.GetInstance<MonitorPageViewModel>().RobotProcessingState == 0)
                            {
                                var processingEndTime = DateTime.Now;
                                TimeSpan time = processingEndTime.Subtract(processingStartTime);
                                ServiceLocator.Current.GetInstance<MonitorPageViewModel>().CurrentBeat = ((int)time.TotalSeconds - 3).ToString() + "s";
                                ServiceLocator.Current.GetInstance<MonitorPageViewModel>().ProcessingTotal += 1;
                                ServiceLocator.Current.GetInstance<MonitorPageViewModel>().MonthProcessing += 1;
                                ServiceLocator.Current.GetInstance<MonitorPageViewModel>().DayProcessing += 1;
                                ServiceLocator.Current.GetInstance<MonitorPageViewModel>().ToolLoss += 1;
                                if (ServiceLocator.Current.GetInstance<SettingPageViewModel>().RobotAxisCompensateZ < 5)
                                {
                                    ServiceLocator.Current.GetInstance<MonitorPageViewModel>().SaveResultData(result.IdentifyWheelType, processingStartTime, ((int)time.TotalSeconds - 3).ToString(), "加工完成", TemplateImage);
                                }
                                processing = false;
                            }
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                MessageShow(ex.Message, MessageType.Error);
            }
        }

        #endregion

        #region==============轨迹模板制作================
        /// <summary>
        /// 窗口投影按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowProjection_btn_Click(object sender, RoutedEventArgs e)
        {
            if (!TemplateImage.IsInitialized())
            {
                MessageShow("无轮毂图片数据，请先采集或读取制作模板的图片！", MessageType.Default);
                return;
            }
            if (TemplateDataGrid.SelectedItem == null)
            {
                MessageShow("无轮型信息，请在轮毂数据窗口选择轮型!", MessageType.Default);
                return;
            }
            if (!InnerCircleXLD.IsInitialized())
            {
                MessageShow("无内外圆数据，请先【定位轮毂】！", MessageType.Default);
                return;
            }
            var wheelType = (TemplateDataGrid.SelectedItem as TemplateDataModel).WheelType;
            HOperatorSet.GenEmptyObj(out HObject contours);
            List<MachiningPathPosModel> datas = new List<MachiningPathPosModel>();
            if (ProjectionChoose.Text == "IGS投影")
            {
                //获取D:\Deburr\Igs文件夹下所有文件
                var filedir = Directory.GetFiles(IgsPath);
                if (filedir.Length == 0)
                {
                    MessageShow("无IGS文件！", MessageType.Default);
                    return;
                }
                //根据轮型筛选
                List<int> indexs = new List<int>();
                for (int i = 0; i < filedir.Length; i++)
                {
                    var b = filedir[i].Contains(wheelType); //不应该是包含
                    if (b) indexs.Add(i);
                }
                if (indexs.Count == 0)
                {
                    MessageShow("当前选中轮型无Igs数据，请将制作好的Igs数据文件放至以下目录：" + IgsPath, MessageType.Default);
                    return;
                }
                int id = 1;
                for (int i = 0; i < indexs.Count; i++)
                {
                    string[] strings = File.ReadAllLines(filedir[indexs[i]]);
                    if (strings.Length > 0)
                    {
                        HTuple rows = new HTuple();
                        HTuple cols = new HTuple();
                        int poseId = (i + 1) * 1000;
                        for (int j = 0; j < strings.Length; j++)
                        {
                            var str = strings[j].Split(',');
                            if (str[0] == "116")
                            {
                                MachiningPathPosModel data = new MachiningPathPosModel();
                                data.Id = id;
                                data.PoseId = poseId;
                                data.X = Math.Round(double.Parse(str[1]), 3);
                                data.Y = Math.Round(double.Parse(str[2]), 3);
                                data.Z = Math.Round(double.Parse(str[3]), 3);
                                data.EX = Math.Round(ViewportHelper.EX, 2);
                                data.EY = Math.Round(ViewportHelper.EY, 2);
                                data.EZ = Math.Round(ViewportHelper.EZ, 2);
                                data.Q1 = Math.Round(ViewportHelper.W, 5);
                                data.Q2 = Math.Round(ViewportHelper.X, 5);
                                data.Q3 = Math.Round(ViewportHelper.Y, 5);
                                data.Q4 = Math.Round(ViewportHelper.Z, 5);
                                //获取指定高度下的新齐次变换矩阵
                                var homMat2D = ServiceLocator.Current.GetInstance<SettingPageViewModel>().GetPixelToPhysicsHomMat2D(data.Z, TemplateHelper.PixelToPhysicsHomMat2D);
                                //反转齐次变换矩阵
                                HOperatorSet.HomMat2dInvert(homMat2D, out HTuple homMat2DInvert);
                                HOperatorSet.AffineTransPoint2d(homMat2DInvert, data.X, data.Y, out HTuple qRow, out HTuple qCol);
                                data.Row = Math.Round(qRow.D, 3);
                                data.Column = Math.Round(qCol.D, 3);
                                datas.Add(data);
                                id++;
                                poseId++;

                                HOperatorSet.TupleConcat(rows, qRow, out rows);
                                HOperatorSet.TupleConcat(cols, qCol, out cols);
                            }
                        }
                        HOperatorSet.GenContourPolygonXld(out HObject contour, rows, cols);
                        HOperatorSet.ConcatObj(contours, contour, out contours);
                    }
                }

            }
            else if (ProjectionChoose.Text == "源轨迹投影")
            {
                var index = TemplatePageViewModel.SourceLocusDatas.LocusName.FindIndex(x => x == wheelType);
                if (index < 0)
                {
                    MessageShow("当前选中轮型无源轨迹数据！", MessageType.Default);
                    return;
                }
                var locusDatas = TemplatePageViewModel.SourceLocusDatas.LocusPoints[index];
                var data = ServiceLocator.Current.GetInstance<TemplatePageViewModel>().TemplateDatas.Where(x => x.WheelType == (TemplateDataGrid.SelectedItem as TemplateDataModel).WheelType).ToList();
                int numberOfWindows = locusDatas[locusDatas.Count - 1].PoseId / 1000 / data[0].SpokeQuantity; //窗口数 /1000 / 轮辐数量
                for (int i = 1; i <= numberOfWindows; i++)
                {
                    var points = locusDatas.Where(x => x.PoseId >= i * 1000 && x.PoseId <= i * 1000 + 900).ToList();
                    HTuple rows = new HTuple();
                    HTuple cols = new HTuple();
                    for (int j = 0; j < points.Count; j++)
                    {
                        HOperatorSet.TupleConcat(rows, points[j].Row, out rows);
                        HOperatorSet.TupleConcat(cols, points[j].Column, out cols);
                        datas.Add(points[j]);
                    }
                    HOperatorSet.GenContourPolygonXld(out HObject contour, rows, cols);
                    HOperatorSet.ConcatObj(contours, contour, out contours);
                }
            }
            else
            {
                var index = LocusPageViewModel.ProcessingLocusDatas.LocusName.FindIndex(x => x == wheelType);
                if (index < 0)
                {
                    MessageShow("当前选中轮型无加工轨迹数据！", MessageType.Default);
                    return;
                }
                var locusDatas = LocusPageViewModel.ProcessingLocusDatas.LocusPoints[index];
                var data = ServiceLocator.Current.GetInstance<TemplatePageViewModel>().TemplateDatas.Where(x => x.WheelType == (TemplateDataGrid.SelectedItem as TemplateDataModel).WheelType).ToList();
                int numberOfWindows = locusDatas[locusDatas.Count - 1].PoseId / 1000 / data[0].SpokeQuantity;
                for (int i = 1; i <= numberOfWindows; i++)
                {
                    var points = locusDatas.Where(x => x.PoseId >= i * 1000 && x.PoseId <= i * 1000 + 900).ToList();
                    HTuple rows = new HTuple();
                    HTuple cols = new HTuple();
                    for (int j = 0; j < points.Count; j++)
                    {
                        HOperatorSet.TupleConcat(rows, points[j].Row, out rows);
                        HOperatorSet.TupleConcat(cols, points[j].Column, out cols);
                        datas.Add(points[j]);
                    }
                    HOperatorSet.GenContourPolygonXld(out HObject contour, rows, cols);
                    HOperatorSet.ConcatObj(contours, contour, out contours);
                }
            }

            WindowXLD.Dispose();
            WindowXLD = contours;
            MachiningPathDatas.Clear();
            MachiningPathDatas = datas;
            HOperatorSet.GenCrossContourXld(out HObject oldCenterCross, CenterRow, CenterColumn, 200, 0);
            TemplateHelper.TemplateWindowDisplay(TemplateHalconWindow.HalconWindow, TemplateImage, null, InnerCircleXLD, null, WindowXLD, oldCenterCross, null, IsImageEnhancement);
            NewWindowXLD.Dispose();
        }
        /// <summary>
        /// 窗口微调按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowFineTune_btn_Click(object sender, RoutedEventArgs e)
        {
            if (!WindowXLD.IsInitialized())
            {
                MessageShow("请先执行窗口投影，并调整好窗口位置后再试！", MessageType.Default);
                return;
            }
            if (FineTuneChoose_cb.Text != "固定算法")
            {
                //===================找出窗口轮廓==========================
                HOperatorSet.GenEmptyObj(out HObject thresholdXld);
                HOperatorSet.CountObj(WindowXLD, out HTuple number);
                for (int i = 0; i < number; i++)
                {
                    //从图像中剪切出轮廓对应的部分
                    HOperatorSet.SelectObj(WindowXLD, out HObject objectSelected, i + 1);
                    HObject xld = null;
                    if (FineTuneChoose_cb.Text == "阈值算法")
                        xld = TemplateHelper.GetWindowContours(TemplateImage, objectSelected, DarkMaxThreshold, BrightMinThreshold, SingleXldDilation);
                    else
                    {
                        xld = TemplateHelper.GetWindowContours(TemplateImage, objectSelected, CannyAlpha, CannyLowThresold, CannyHighThresold, XldMinLength, SingleXldDilation, MaskWidthHeight);
                    }
                    if (xld == null)
                    {
                        MessageShow($"窗口{i + 1}获取轮廓失败，请调整参数后再试！", MessageType.Default);
                        return;
                    }
                    HOperatorSet.ConcatObj(thresholdXld, xld, out thresholdXld);

                }
                NewWindowXLD = TemplateHelper.ContourProjection(WindowXLD, thresholdXld, 10000, out List<HTuple> rowList, out List<HTuple> colList);
                HOperatorSet.GenCrossContourXld(out HObject oldCenterCross, CenterRow, CenterColumn, 200, 0);
                TemplateHelper.TemplateWindowDisplay(TemplateHalconWindow.HalconWindow, TemplateImage, WindowXLD, InnerCircleXLD, thresholdXld, NewWindowXLD, oldCenterCross, null, IsImageEnhancement);
            }
            else
            {
                NewWindowXLD = WindowXLD;
                HOperatorSet.GenCrossContourXld(out HObject oldCenterCross, CenterRow, CenterColumn, 200, 0);
                TemplateHelper.TemplateWindowDisplay(TemplateHalconWindow.HalconWindow, TemplateImage, null, InnerCircleXLD, null, NewWindowXLD, oldCenterCross, null, IsImageEnhancement);
            }
            var datas = GenMachiningPathDatas(MachiningPathDatas, NewWindowXLD);
            MachiningPathDatas.Clear();
            MachiningPathDatas = datas;
        }
        /// <summary>
        /// 生成源轨迹按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GenSourceLocus_Click(object sender, RoutedEventArgs e)
        {
            if (!NewWindowXLD.IsInitialized())
            {
                MessageShow("请先执行窗口微调后再试！", MessageType.Default);
                return;
            }
            if (TemplateDataGrid.SelectedItem == null)
            {
                MessageShow("无轮型信息，请在轮毂数据窗口选择轮型!", MessageType.Default);
                return;
            }
            if (CenterRow == null || CenterColumn == null)
            {
                MessageShow("无内外圆数据，请先执行定位内外圆！", MessageType.Default);
                return;
            }
            #region=========================根据单个轮辐种类数据生成全部轮廓点==========================
            //获取轮辐数量
            int spokeQuantity = (TemplateDataGrid.SelectedItem as TemplateDataModel).SpokeQuantity;
            HTuple rad;
            int id = 1;
            //生成全部轮廓数据
            List<MachiningPathPosModel> datas = new List<MachiningPathPosModel>();
            for (int i = 0; i < spokeQuantity; i++)
            {
                //单轮辐旋转的角度
                var angle = 360.0 / spokeQuantity * i;
                //角度转弧度
                HOperatorSet.TupleRad(angle, out HTuple hTuple);
                if (SpokeSorting == false) rad = -hTuple;
                else rad = hTuple;

                var wins = MachiningPathDatas[MachiningPathDatas.Count - 1].PoseId / 1000;
                for (int j = 0; j < wins; j++)
                {
                    int startPoseId = (j + i * wins + 1) * 1000;
                    var ds = MachiningPathDatas.Where(x => x.PoseId >= (j + 1) * 1000 && x.PoseId <= (j + 1) * 1000 + 900).ToList();
                    for (int k = 0; k < ds.Count; k++)
                    {
                        MachiningPathPosModel m = new MachiningPathPosModel();
                        m.Id = id;
                        m.PoseId = ds[k].PoseId % 1000 + startPoseId;
                        TemplateHelper.GetThePointAfterRotation(ds[k].Row, ds[k].Column, CenterRow, CenterColumn, rad, out HTuple r, out HTuple c);
                        m.Row = Math.Round(r.D, 3);
                        m.Column = Math.Round(c.D, 3);
                        //获取指定高度下的新齐次变换矩阵
                        var homMat2D = ServiceLocator.Current.GetInstance<SettingPageViewModel>().GetPixelToPhysicsHomMat2D(ds[k].Z, TemplateHelper.PixelToPhysicsHomMat2D);
                        HOperatorSet.AffineTransPoint2d(homMat2D, m.Row, m.Column, out HTuple qx, out HTuple qy);
                        m.X = Math.Round(qx.D, 3);
                        m.Y = Math.Round(qy.D, 3);
                        m.Z = ds[k].Z;
                        m.EX = ds[k].EX;
                        m.EY = ds[k].EY;
                        m.EZ = RobotHelper.AngleCalculationAfterRotation(ds[k].EZ, angle);
                        m.Q1 = ds[k].Q1;
                        m.Q2 = ds[k].Q2;
                        m.Q3 = ds[k].Q3;
                        m.Q4 = ds[k].Q4;
                        datas.Add(m);
                        id++;
                    }
                }
            }
            #endregion
            //生成窗口轮廓
            HObject contours = TemplateHelper.GenContours(datas, false);
            if (FineTuneChoose_cb.Text != "固定算法")
            {
                //存储新的窗口区域轮廓
                HOperatorSet.GenEmptyObj(out HObject thresholdContours);
                HOperatorSet.CountObj(contours, out HTuple number);
                for (int i = 0; i < number; i++)
                {
                    HOperatorSet.SelectObj(contours, out HObject objectSelected, i + 1);
                    HObject xld = null;
                    if (FineTuneChoose_cb.Text == "阈值算法")
                        xld = TemplateHelper.GetWindowContours(TemplateImage, objectSelected, DarkMaxThreshold, BrightMinThreshold, SingleXldDilation);
                    else
                        xld = TemplateHelper.GetWindowContours(TemplateImage, objectSelected, CannyAlpha, CannyLowThresold, CannyHighThresold, XldMinLength, SingleXldDilation, MaskWidthHeight);
                    HOperatorSet.ConcatObj(thresholdContours, xld, out thresholdContours);
                }
                //投影
                HObject projectionContours = TemplateHelper.ContourProjection(contours, thresholdContours, MaxDistance, out List<HTuple> rowList, out List<HTuple> colList);

                LocusWindowXLD.Dispose();
                LocusWindowXLD = projectionContours;

                var ds = GenMachiningPathDatas(datas, LocusWindowXLD);
                EndMachiningPathDatas.Clear();
                EndMachiningPathDatas = ds;
                //显示
                HOperatorSet.GenCrossContourXld(out HObject oldCenterCross, CenterRow, CenterColumn, 200, 0);
                TemplateHelper.TemplateWindowDisplay(TemplateHalconWindow.HalconWindow, TemplateImage, null, InnerCircleXLD, thresholdContours, LocusWindowXLD, oldCenterCross, null, IsImageEnhancement);
            }
            else
            {
                LocusWindowXLD.Dispose();
                LocusWindowXLD = contours;
                EndMachiningPathDatas.Clear();
                EndMachiningPathDatas = datas;
                HOperatorSet.GenCrossContourXld(out HObject oldCenterCross, CenterRow, CenterColumn, 200, 0);
                TemplateHelper.TemplateWindowDisplay(TemplateHalconWindow.HalconWindow, TemplateImage, null, InnerCircleXLD, null, LocusWindowXLD, oldCenterCross, null, IsImageEnhancement);
            }
            //轮辐排序反转
            SpokeSorting = !SpokeSorting;
        }
        /// <summary>
        /// 保存源轨迹按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveSourceLocus_btn_Click(object sender, RoutedEventArgs e)
        {
            if (!ImageReduced.IsInitialized())
            {
                MessageShow("无制作模板的图像数据，请执行定位内外圆！", MessageType.Default);
                return;
            }
            if (EndMachiningPathDatas.Count == 0)
            {
                MessageShow("无轨迹数据，请先执行生成轨迹！", MessageType.Default);
                return;
            }
            if (TemplateDataGrid.SelectedItem == null)
            {
                MessageShow("无轮型信息，请在轮毂数据窗口选择轮型!", MessageType.Default);
                return;
            }
            var wheelType = (TemplateDataGrid.SelectedItem as TemplateDataModel).WheelType;
            string activeTemplatePath = @"D:/DeburrSystem/ActiveTemplates/" + wheelType + ".ncm";
            string notActiveTemplatePath = @"D:/DeburrSystem/NotActiveTemplates/" + wheelType + ".ncm";
            var templateImagePath = @"D:/DeburrSystem/TemplateImages/" + wheelType + ".tif";
            bool result = UMessageBox.Show("轮型选择确认", "您选择的轮型是：" + wheelType + " ，请确定轮型选择正确后，再点击确认！");
            if (result)
            {
                ServiceLocator.Current.GetInstance<TemplatePageViewModel>().TemplateButtonEnabled = false;

                SelectWheelModel = wheelType;
                System.Threading.Tasks.Task.Run(new Action(() =>
                {
                    var hDB = new SqlAccess().SourceLocusDataAccess;
                    //判断当前轮型是否存在
                    var index = TemplatePageViewModel.SourceLocusDatas.LocusName.FindIndex(x => x == wheelType);
                    //如果存在
                    if (index >= 0)
                    {
                        //则清空表
                        hDB.DbMaintenance.TruncateTable(wheelType);
                        //根据表名修改数据库数据
                        hDB.Insertable(EndMachiningPathDatas).AS(wheelType).ExecuteCommand();
                        //修改内存中的数据
                        TemplatePageViewModel.SourceLocusDatas.LocusPoints[index] = EndMachiningPathDatas;
                    }
                    //如果不存在
                    else
                    {
                        //则根据轮型创建表
                        hDB.CodeFirst.As<MachiningPathPosModel>(wheelType).InitTables<MachiningPathPosModel>();
                        //根据表名插入数据
                        hDB.Insertable(EndMachiningPathDatas).AS(wheelType).ExecuteCommand();
                        //将新数据添加到内存中
                        TemplatePageViewModel.SourceLocusDatas.LocusName.Add(wheelType);
                        TemplatePageViewModel.SourceLocusDatas.LocusPoints.Add(EndMachiningPathDatas);
                        ServiceLocator.Current.GetInstance<ReportPageViewModel>().WheelTypeDatas.Add(wheelType);
                        ServiceLocator.Current.GetInstance<ReportPageViewModel>().SelectedItem = wheelType;
                    }

                    //创建NCC模板
                    //根据窗口轨迹轮廓获取制作模板的图像
                    HOperatorSet.GenRegionContourXld(LocusWindowXLD, out HObject winRegion, "filled");
                    HOperatorSet.Difference(ImageReduced, winRegion, out HObject regionDifference);
                    HOperatorSet.GenCircle(out HObject circle, CenterRow, CenterColumn, 600);
                    HOperatorSet.Difference(regionDifference, circle, out HObject templateRegion);
                    HOperatorSet.ReduceDomain(ImageReduced, templateRegion, out HObject templateImage);
                    HOperatorSet.ZoomImageFactor(templateImage, out HObject nccTemplateImage, ImageScale, ImageScale, "constant");
                    HOperatorSet.CreateNccModel(nccTemplateImage, "auto", TemplateAngleStart, TemplateAngleExtent, "auto", "use_polarity", out HTuple modelID);
                    var activeIndex = TemplatePageViewModel.TemplateList.ActiveWheelTypeList.FindIndex(x => x == wheelType);
                    var notActiveIndex = TemplatePageViewModel.TemplateList.ActiveWheelTypeList.FindIndex(x => x == wheelType);
                    if (activeIndex >= 0 || (activeIndex < 0 && notActiveIndex < 0)) HOperatorSet.WriteNccModel(modelID, activeTemplatePath);
                    else HOperatorSet.WriteNccModel(modelID, notActiveTemplatePath);
                    //HOperatorSet.WriteImage(nccTemplateImage, "tiff", 0, templateImagePath);
                    HOperatorSet.WriteImage(TemplateImage, "tiff", 0, templateImagePath);
                    HOperatorSet.ClearNccModel(modelID);

                    //保存轮廓
                    try
                    {
                        HObject processingContours = TemplateHelper.GenContours(EndMachiningPathDatas, false);
                        HOperatorSet.WriteContourXldDxf(processingContours, "d:/Deburr/DXF文件/" + wheelType + ".dxf");
                    }
                    catch { }

                    //修改数据库
                    var sDB = new SqlAccess().SystemDataAccess;
                    List<TemplateDataModel> data = sDB.Queryable<TemplateDataModel>().Where(it => it.WheelType == wheelType).ToList();
                    data[0].DarkMaxThreshold = DarkMaxThreshold;
                    data[0].LightMinThreshold = BrightMinThreshold;
                    data[0].CenterRow = Math.Round(CenterRow.D, 2);
                    data[0].CenterColumn = Math.Round(CenterColumn.D, 2);
                    data[0].DarkMaxThreshold = DarkMaxThreshold;
                    data[0].InnerCircleCaliperLength = InnerCaliperLength;
                    data[0].InnerCircleRadius = InnerRadius;
                    sDB.Updateable(data).ExecuteCommand();
                    ServiceLocator.Current.GetInstance<TemplatePageViewModel>().TemplateButtonEnabled = true;
                    System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        //更新模板数据显示表格
                        ServiceLocator.Current.GetInstance<TemplatePageViewModel>().UpdateWheelDatas(wheelType);
                        ServiceLocator.Current.GetInstance<TemplatePageViewModel>().UpdateProcessingWheelDatas();
                        ServiceLocator.Current.GetInstance<TemplatePageViewModel>().UpdateProcessingTemplateDatas(wheelType);
                        MessageShow("模板和轨迹保存成功，轮毂型号是：" + wheelType, MessageType.Success);
                    }));
                }));
            }
        }
        /// <summary>
        /// 显示源轨迹
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowSourceLocus_Click(object sender, RoutedEventArgs e)
        {
            ServiceLocator.Current.GetInstance<TemplatePageViewModel>().ResultShow = Visibility.Collapsed;
            if (TemplateDataGrid.SelectedItem == null)
            {
                MessageShow("无轮型信息，请在轮毂数据窗口选择轮型!", MessageType.Default);
                return;
            }
            var wheelType = (TemplateDataGrid.SelectedItem as TemplateDataModel).WheelType;
            var templateImagePath = @"D:/DeburrSystem/TemplateImages/" + wheelType + ".tif";
            if (!File.Exists(templateImagePath))
            {
                MessageShow("当前选中轮型无模板图像，请先录入识别模板！", MessageType.Default);
                return;
            }
            var index = TemplatePageViewModel.SourceLocusDatas.LocusName.FindIndex(x => x == wheelType);
            if (index < 0)
            {
                MessageShow("当前选中轮型无源轨迹数据，请先录入轨迹数据！", MessageType.Default);
                return;
            }
            //读取模板图像
            HOperatorSet.ReadImage(out TemplateImage, templateImagePath);
            TemplateHelper.DisplayImage(TemplateImage, TemplateHalconWindow.HalconWindow, true, IsImageEnhancement);
            var locusDatas = TemplatePageViewModel.SourceLocusDatas.LocusPoints[index];
            //生成并显示轨迹
            TemplateHelper.GenAndShowLocus(TemplateHalconWindow.HalconWindow, locusDatas, -1);
        }
        /// <summary>
        /// 显示加工轨迹按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowProcessingLocus_Click(object sender, RoutedEventArgs e)
        {
            ServiceLocator.Current.GetInstance<TemplatePageViewModel>().ResultShow = Visibility.Collapsed;
            if (TemplateDataGrid.SelectedItem == null)
            {
                MessageShow("无轮型信息，请在轮毂数据窗口选择轮型!", MessageType.Default);
                return;
            }
            var wheelType = (TemplateDataGrid.SelectedItem as TemplateDataModel).WheelType;
            var templateImagePath = @"D:/DeburrSystem/TemplateImages/" + wheelType + ".tif";
            if (!File.Exists(templateImagePath))
            {
                MessageShow("当前选中轮型无模板图像，请先录入识别模板！", MessageType.Default);
                return;
            }
            //获取轨迹数据索引
            int index = LocusPageViewModel.ProcessingLocusDatas.LocusName.FindIndex(x => x == wheelType);
            if (index < 0)
            {
                MessageShow("当前选中轮型无加工轨迹数据，请先制作加工轨迹！", MessageType.Default);
                return;
            }
            //读取模板图像
            HOperatorSet.ReadImage(out TemplateImage, templateImagePath);
            TemplateHelper.DisplayImage(TemplateImage, TemplateHalconWindow.HalconWindow, true, IsImageEnhancement);
            //获取轨迹点
            List<MachiningPathPosModel> locusDatas = LocusPageViewModel.ProcessingLocusDatas.LocusPoints[index];
            //生成并显示轨迹
            int spokeType = locusDatas[locusDatas.Count - 1].PoseId / 1000 / (TemplateDataGrid.SelectedItem as TemplateDataModel).SpokeQuantity;
            List<MachiningPathPosModel> ds = TemplateHelper.GenLocusOfOutPoint(locusDatas, (TemplateDataGrid.SelectedItem as TemplateDataModel).OutPointPoseId, spokeType);
            TemplateHelper.GenAndShowLocus(TemplateHalconWindow.HalconWindow, ds, -1, TemplateImage);
        }
        /// <summary>
        /// 轮廓左移按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            if (!WindowXLD.IsInitialized())
            {
                MessageShow("无窗口轮廓数据，请先执行窗口投影！", MessageType.Default);
                return;
            }
            if (XYOffsetValue_tbx.Text == "")
            {
                MessageShow("平移数据为空，请先输入数据！", MessageType.Default);
                return;
            }
            double value;
            try
            {
                value = double.Parse(XYOffsetValue_tbx.Text);
            }
            catch
            {
                MessageShow("平移数据错误，请重新输入！", MessageType.Error);
                return;
            }
            HObject xld = TemplateHelper.ContourScaleRotateOrTranslate(WindowXLD, "Translate", WindowChoose_cb.SelectedIndex, 0, -value, CenterRow, CenterColumn);
            var datas = GenMachiningPathDatas(MachiningPathDatas, xld);
            MachiningPathDatas.Clear();
            MachiningPathDatas = datas;
            HOperatorSet.GenCrossContourXld(out HObject oldCenterCross, CenterRow, CenterColumn, 200, 0);
            TemplateHelper.TemplateWindowDisplay(TemplateHalconWindow.HalconWindow, TemplateImage, null, InnerCircleXLD, null, xld, oldCenterCross, null, IsImageEnhancement);
            WindowXLD.Dispose();
            WindowXLD = xld;
        }
        /// <summary>
        /// 轮廓上移按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AboveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!WindowXLD.IsInitialized())
            {
                MessageShow("无窗口轮廓数据，请先执行窗口投影！", MessageType.Default);
                return;
            }
            if (XYOffsetValue_tbx.Text == "")
            {
                MessageShow("平移数据为空，请先输入数据！", MessageType.Default);
                return;
            }
            double value;
            try
            {
                value = double.Parse(XYOffsetValue_tbx.Text);
            }
            catch
            {
                MessageShow("平移数据错误，请重新输入！", MessageType.Error);
                return;
            }
            HObject xld = TemplateHelper.ContourScaleRotateOrTranslate(WindowXLD, "Translate", WindowChoose_cb.SelectedIndex, -value, 0, CenterRow, CenterColumn);
            var datas = GenMachiningPathDatas(MachiningPathDatas, xld);
            MachiningPathDatas.Clear();
            MachiningPathDatas = datas;
            HOperatorSet.GenCrossContourXld(out HObject oldCenterCross, CenterRow, CenterColumn, 200, 0);
            TemplateHelper.TemplateWindowDisplay(TemplateHalconWindow.HalconWindow, TemplateImage, null, InnerCircleXLD, null, xld, oldCenterCross, null, IsImageEnhancement);
            WindowXLD.Dispose();
            WindowXLD = xld;
        }
        /// <summary>
        /// 轮廓右移按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RightButton_Click(object sender, RoutedEventArgs e)
        {
            if (!WindowXLD.IsInitialized())
            {
                MessageShow("无窗口轮廓数据，请先执行窗口投影！", MessageType.Default);
                return;
            }
            if (XYOffsetValue_tbx.Text == "")
            {
                MessageShow("平移数据为空，请先输入数据！", MessageType.Default);
                return;
            }
            double value;
            try
            {
                value = double.Parse(XYOffsetValue_tbx.Text);
            }
            catch
            {
                MessageShow("平移数据错误，请重新输入！", MessageType.Error);
                return;
            }
            HObject xld = TemplateHelper.ContourScaleRotateOrTranslate(WindowXLD, "Translate", WindowChoose_cb.SelectedIndex, 0, value, CenterRow, CenterColumn);
            var datas = GenMachiningPathDatas(MachiningPathDatas, xld);
            MachiningPathDatas.Clear();
            MachiningPathDatas = datas;
            HOperatorSet.GenCrossContourXld(out HObject oldCenterCross, CenterRow, CenterColumn, 200, 0);
            TemplateHelper.TemplateWindowDisplay(TemplateHalconWindow.HalconWindow, TemplateImage, null, InnerCircleXLD, null, xld, oldCenterCross, null, IsImageEnhancement);
            WindowXLD.Dispose();
            WindowXLD = xld;
        }
        /// <summary>
        /// 轮廓下移按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UnderButton_Click(object sender, RoutedEventArgs e)
        {
            if (!WindowXLD.IsInitialized())
            {
                MessageShow("无窗口轮廓数据，请先执行窗口投影！", MessageType.Default);
                return;
            }
            if (XYOffsetValue_tbx.Text == "")
            {
                MessageShow("平移数据为空，请先输入数据！", MessageType.Default);
                return;
            }
            double value;
            try
            {
                value = double.Parse(XYOffsetValue_tbx.Text);
            }
            catch
            {
                MessageShow("平移数据错误，请重新输入！", MessageType.Error);
                return;
            }
            HObject xld = TemplateHelper.ContourScaleRotateOrTranslate(WindowXLD, "Translate", WindowChoose_cb.SelectedIndex, value, 0, CenterRow, CenterColumn);
            List<MachiningPathPosModel> datas = GenMachiningPathDatas(MachiningPathDatas, xld);
            MachiningPathDatas.Clear();
            MachiningPathDatas = datas;
            HOperatorSet.GenCrossContourXld(out HObject oldCenterCross, CenterRow, CenterColumn, 200, 0);
            TemplateHelper.TemplateWindowDisplay(TemplateHalconWindow.HalconWindow, TemplateImage, null, InnerCircleXLD, null, xld, oldCenterCross, null, IsImageEnhancement);
            WindowXLD.Dispose();
            WindowXLD = xld;
        }
        /// <summary>
        /// 轮廓放大按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScaleAddButton_Click(object sender, RoutedEventArgs e)
        {
            if (!WindowXLD.IsInitialized())
            {
                MessageShow("无窗口轮廓数据，请先执行窗口投影！", MessageType.Default);
                return;
            }
            if (ScaleValue_tbx.Text == "")
            {
                MessageShow("窗口缩放数据为空，请先输入数据！", MessageType.Default);
                return;
            }
            HOperatorSet.CountObj(WindowXLD, out HTuple number);
            if (WindowChoose_cb.SelectedIndex > number)
            {
                MessageShow("窗口选择错误，请重新选择！", MessageType.Error);
                return;
            }
            double value;
            try
            {
                value = double.Parse(ScaleValue_tbx.Text);
            }
            catch
            {
                MessageShow("窗口缩放数据错误，请重新输入！", MessageType.Error);
                return;
            }
            HObject xld = TemplateHelper.ContourScaleRotateOrTranslate(WindowXLD, "Scale", WindowChoose_cb.SelectedIndex, value, 0, CenterRow, CenterColumn);
            List<MachiningPathPosModel> datas = GenMachiningPathDatas(MachiningPathDatas, xld);
            MachiningPathDatas.Clear();
            MachiningPathDatas = datas;
            HOperatorSet.GenCrossContourXld(out HObject oldCenterCross, CenterRow, CenterColumn, 200, 0);
            TemplateHelper.TemplateWindowDisplay(TemplateHalconWindow.HalconWindow, TemplateImage, null, InnerCircleXLD, null, xld, oldCenterCross, null, IsImageEnhancement);
            WindowXLD.Dispose();
            WindowXLD = xld;
        }
        /// <summary>
        /// 轮廓缩小按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScaleDecButton_Click(object sender, RoutedEventArgs e)
        {
            if (!WindowXLD.IsInitialized())
            {
                MessageShow("无窗口轮廓数据，请先执行窗口投影！", MessageType.Default);
                return;
            }
            if (ScaleValue_tbx.Text == "")
            {
                MessageShow("窗口缩放数据为空，请先输入数据！", MessageType.Default);
                return;
            }
            HOperatorSet.CountObj(WindowXLD, out HTuple number);
            if (WindowChoose_cb.SelectedIndex > number)
            {
                MessageShow("窗口选择错误，请重新选择！", MessageType.Error);
                return;
            }
            double value;
            try
            {
                value = double.Parse(ScaleValue_tbx.Text);
            }
            catch
            {
                MessageShow("窗口缩放数据错误，请重新输入！", MessageType.Error);
                return;
            }
            HObject xld = TemplateHelper.ContourScaleRotateOrTranslate(WindowXLD, "Scale", WindowChoose_cb.SelectedIndex, -value, 0, CenterRow, CenterColumn);
            var datas = GenMachiningPathDatas(MachiningPathDatas, xld);
            MachiningPathDatas.Clear();
            MachiningPathDatas = datas;
            HOperatorSet.GenCrossContourXld(out HObject oldCenterCross, CenterRow, CenterColumn, 200, 0);
            TemplateHelper.TemplateWindowDisplay(TemplateHalconWindow.HalconWindow, TemplateImage, null, InnerCircleXLD, null, xld, oldCenterCross, null, IsImageEnhancement);
            WindowXLD.Dispose();
            WindowXLD = xld;
        }
        /// <summary>
        /// 轮廓逆时针旋转按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LeftRotateButton_Click(object sender, RoutedEventArgs e)
        {
            if (!WindowXLD.IsInitialized())
            {
                MessageShow("无窗口轮廓数据，请先执行窗口投影！", MessageType.Default);
                return;
            }
            if (RotateValue_tbx.Text == "")
            {
                MessageShow("窗口缩放数据为空，请先输入数据！", MessageType.Default);
                return;
            }
            HOperatorSet.CountObj(WindowXLD, out HTuple number);
            if (WindowChoose_cb.SelectedIndex > number)
            {
                MessageShow("窗口选择错误，请重新选择！", MessageType.Error);
                return;
            }
            double value;
            try
            {
                value = double.Parse(RotateValue_tbx.Text);
            }
            catch
            {
                MessageShow("窗口缩放数据错误，请重新输入！", MessageType.Error);
                return;
            }
            HObject xld = TemplateHelper.ContourScaleRotateOrTranslate(WindowXLD, "Rotate", WindowChoose_cb.SelectedIndex, value, 0, CenterRow, CenterColumn);
            var datas = GenMachiningPathDatas(MachiningPathDatas, xld);
            MachiningPathDatas.Clear();
            MachiningPathDatas = datas;
            HOperatorSet.GenCrossContourXld(out HObject oldCenterCross, CenterRow, CenterColumn, 200, 0);
            TemplateHelper.TemplateWindowDisplay(TemplateHalconWindow.HalconWindow, TemplateImage, null, InnerCircleXLD, null, xld, oldCenterCross, null, IsImageEnhancement);
            WindowXLD.Dispose();
            WindowXLD = xld;
        }
        /// <summary>
        /// 轮廓顺时针旋转按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RightRotateButton_Click(object sender, RoutedEventArgs e)
        {
            if (!WindowXLD.IsInitialized())
            {
                MessageShow("无窗口轮廓数据，请先执行窗口投影！", MessageType.Default);
                return;
            }
            if (RotateValue_tbx.Text == "")
            {
                MessageShow("窗口缩放数据为空，请先输入数据！", MessageType.Default);
                return;

            }
            HOperatorSet.CountObj(WindowXLD, out HTuple number);
            if (WindowChoose_cb.SelectedIndex > number)
            {
                MessageShow("窗口选择错误，请重新选择！", MessageType.Error);
                return;
            }
            double value;
            try
            {
                value = double.Parse(RotateValue_tbx.Text);
            }
            catch
            {
                MessageShow("窗口缩放数据错误，请重新输入！", MessageType.Error);
                return;
            }
            HObject xld = TemplateHelper.ContourScaleRotateOrTranslate(WindowXLD, "Rotate", WindowChoose_cb.SelectedIndex, -value, 0, CenterRow, CenterColumn);
            var datas = GenMachiningPathDatas(MachiningPathDatas, xld);
            MachiningPathDatas.Clear();
            MachiningPathDatas = datas;
            HOperatorSet.GenCrossContourXld(out HObject oldCenterCross, CenterRow, CenterColumn, 200, 0);
            TemplateHelper.TemplateWindowDisplay(TemplateHalconWindow.HalconWindow, TemplateImage, null, InnerCircleXLD, null, xld, oldCenterCross, null, IsImageEnhancement);
            WindowXLD.Dispose();
            WindowXLD = xld;
        }
        #endregion

        #region==============页面事件================
        private void TemplatePage_Loaded(object sender, RoutedEventArgs e)
        {
            TemplateHalconWindow.HalconWindow.SetWindowParam("graphics_stack_max_element_num", 150);
        }
        /// <summary>
        /// 当前页面退出时发生的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TemplatePage_Unloaded(object sender, RoutedEventArgs e)
        {
            //TemplateImage.Dispose();
            WindowXLD.Dispose();
            LocusWindowXLD.Dispose();
            InnerCircleXLD.Dispose();
            OuterCircleXLD.Dispose();
            NewWindowXLD.Dispose();
            ImageReduced.Dispose();
            ServiceLocator.Current.GetInstance<TemplatePageViewModel>().ResultShow = Visibility.Collapsed;
        }

        /// <summary>
        /// TemplateHalconWindow窗口鼠标按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TemplateHalconWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (TemplateImage.IsInitialized())
            {
                HOperatorSet.GetImageSize(TemplateImage, out HTuple width, out HTuple height);
                try
                {
                    TemplateHalconWindow.HalconWindow.GetMposition(out int row, out int column, out int button);

                    if (row >= 0 && row <= height - 1 && column >= 0 && column <= width - 1)
                    {
                        HOperatorSet.GetGrayval(TemplateImage, row, column, out HTuple gray);
                        WindowInformation_tbx.Text = "行:" + row.ToString() + " 列:" + column.ToString() + " 灰度值:" + gray;
                        ServiceLocator.Current.GetInstance<SettingPageViewModel>().RowCoordinate = row;
                        ServiceLocator.Current.GetInstance<SettingPageViewModel>().ColCoordinate = column;
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// 模板数据窗口鼠标左键按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TemplateDataGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)sender;
            //以下是修改TemplateDataGrid中的启用列
            if (dataGrid != null && dataGrid.Items.Count > 0 && dataGrid.CurrentItem != null)
            {
                //获取选中的行索引
                int rowIndex = dataGrid.Items.IndexOf(dataGrid.CurrentItem);
                //获取选中的列索引
                int columnIndex = dataGrid.CurrentCell.Column.DisplayIndex;
                //获取当前选择的轮型数据
                var datas = (TemplateDataModel)dataGrid.CurrentCell.Item;
                //如果点击的是启用列
                if (rowIndex >= 0 && columnIndex == 4)
                {
                    var index = LocusPageViewModel.ProcessingLocusDatas.LocusName.FindIndex(x => x == datas.WheelType);
                    if (index < 0)
                    {
                        MessageShow($"轮型{datas.WheelType}无加工轨迹数据，请先制作加工轨迹！", MessageType.Error);
                        return;
                    }
                    //获取当前选择轮毂的进出刀点数量
                    int ioCount = LocusPageViewModel.ProcessingLocusDatas.LocusPoints[index].Where(x => x.PoseId % 1000 > 900).Count();
                    int dataCount = LocusPageViewModel.ProcessingLocusDatas.LocusPoints[index].Count();
                    int winCount = LocusPageViewModel.ProcessingLocusDatas.LocusPoints[index][dataCount - 1].PoseId / 1000;
                    if (ioCount != winCount * 6)
                    {
                        MessageShow($"轮型{datas.WheelType}进出刀点数据异常，请检查！", MessageType.Error);
                        return;
                    }
                    //通过行列索引获取CheckBox的值
                    bool c = (bool)(TemplateDataGrid.Columns[columnIndex].GetCellContent(dataGrid.Items[rowIndex]) as CheckBox).IsChecked;
                    //修改用于实时修改的模板数据集
                    ServiceLocator.Current.GetInstance<TemplatePageViewModel>().TemplateDatas[rowIndex].ProcessingEnable = !c;
                    //修改数据库
                    var sDB = new SqlAccess().SystemDataAccess;
                    var data = sDB.Queryable<TemplateDataModel>().Where(it => it.Index == rowIndex + 1).ToList();
                    data[0].ProcessingEnable = !c;
                    sDB.Updateable(data).ExecuteCommand();
                    //将修改应用到加工的模板数据集
                    ServiceLocator.Current.GetInstance<TemplatePageViewModel>().ImplementRevise = true;
                }
            }
        }

        /// <summary>
        /// 此处主要是将TemplateDataGrid的滚动条滚动到当前选择项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TemplateDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)sender;
            //获取当前选择的轮型数据
            if (dataGrid != null && dataGrid.Items.Count > 0)
            {
                //将TemplateDataGrid的滚动条滚动到选中项上
                if (TemplateDataGrid.SelectedItem != null)
                {
                    TemplateDataGrid.ScrollIntoView(TemplateDataGrid.SelectedItem);
                    try
                    {
                        var datas = (TemplateDataModel)dataGrid.CurrentCell.Item;
                        SelectWheelModel = datas.WheelType;
                    }
                    catch { }
                }
            }
        }

        private void TemplateDataGrid_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)sender;
            if (dataGrid != null && dataGrid.Items.Count > 0 && dataGrid.CurrentItem != null)
            {
                SelectWheelModel = (dataGrid.CurrentItem as TemplateDataModel).WheelType;
            }
            ContextMenu contextMenu = new ContextMenu();
            MenuItem menuItem = new MenuItem();
            menuItem.Header = "修改";
            menuItem.FontSize = 14;
            menuItem.Click += MenuItem_Click;
            contextMenu.Items.Add(menuItem);
            contextMenu.IsOpen = true;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            DialogManager.ExecuteAndResult<object>("TemplateDataEditDialog", null);
        }

        private void ImageEnhancement_Checked(object sender, RoutedEventArgs e)
        {
            IsImageEnhancement = true;
        }

        private void ImageEnhancement_Unchecked(object sender, RoutedEventArgs e)
        {
            IsImageEnhancement = false;
        }
        #endregion

        #region==============TextBox限制输入=============
        private void XTranslat_tbx_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.-]"); // 只允许数字和小数点
            if (regex.IsMatch(e.Text))
            {
                e.Handled = true;
            }
        }

        private void YTranslat_tbx_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.-]"); // 只允许数字和小数点
            if (regex.IsMatch(e.Text))
            {
                e.Handled = true;
            }
        }

        private void Scale_tbx_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.-]"); // 只允许数字和小数点
            if (regex.IsMatch(e.Text))
            {
                e.Handled = true;
            }
        }

        private void WindowRotate_tbx_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.-]"); // 只允许数字、小数点和-
            if (regex.IsMatch(e.Text))
            {
                e.Handled = true;
            }
        }

        private void XYOffsetValue_tb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.]"); // 只允许数字和小数点
            if (regex.IsMatch(e.Text))
            {
                e.Handled = true;
            }
        }

        private void ScaleValue_tb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.]"); // 只允许数字和小数点
            if (regex.IsMatch(e.Text))
            {
                e.Handled = true;
            }
        }

        private void RotateValue_tb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.]"); // 只允许数字和小数点
            if (regex.IsMatch(e.Text))
            {
                e.Handled = true;
            }
        }

        #endregion
    }
}
