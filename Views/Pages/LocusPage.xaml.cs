using CommonServiceLocator;
using HalconDotNet;
using HelixToolkit.Wpf;
using HubDeburrSystem.DataAccess;
using HubDeburrSystem.Models;
using HubDeburrSystem.Public;
using HubDeburrSystem.ViewModel;
using HubDeburrSystem.Views.Dialog;
using NPOI.SS.Formula.Functions;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using static HubDeburrSystem.Models._3DLocusParameterSettingModel;

namespace HubDeburrSystem.Views.Pages
{
    /// <summary>
    /// LocusPage.xaml 的交互逻辑
    /// </summary>
    public partial class LocusPage : UserControl
    {
        #region=========数据=========
        /// <summary>
        /// 全部窗口路径点3D模型集合
        /// </summary>
        private List<PathPointModel> ModelLists { get; set; } = new List<PathPointModel>();

        /// <summary>
        /// 原点坐标系的GeometryModel3D模型集合（中心为圆）
        /// </summary>
        private List<GeometryModel3D> CoordinateSystemGM3D_C { get; set; } = new List<GeometryModel3D>();

        /// <summary>
        /// 原点坐标系的GeometryModel3D模型集合（中心为矩形）
        /// </summary>
        private List<GeometryModel3D> CoordinateSystemGM3D_R { get; set; } = new List<GeometryModel3D>();

        /// <summary>
        /// 当前窗口选中模型的ModelUIElement3D
        /// </summary>
        private ModelUIElement3D CurrentModelUIElement3D { get; set; } = null;

        /// <summary>
        /// 当前选择轮型的加工路径数据
        /// </summary>
        private List<MachiningPathPosModel> MachiningPathDatas { get; set; } = new List<MachiningPathPosModel>();

        /// <summary>
        /// 当前编辑的轮型数据
        /// </summary>
        private TemplateDataModel CurrentTemplateData {  set; get; } = new TemplateDataModel();

        /// <summary>
        /// 当前轮辐种类
        /// </summary>
        private int CurrentSpokeType { get; set; } = 0;

        /// <summary>
        /// 加工数据修改后存储列表
        /// </summary>
        public List<List<MachiningPathPosModel>> MachiningPathDataList { get; set; } = new List<List<MachiningPathPosModel>>();
        /// <summary>
        /// 当前加工数据修改的索引
        /// </summary>
        private int CurrentMachiningPathDataIndex { get; set; } = -1;
        #endregion

        #region=========当前选中模型数据=========
        /// <summary>
        /// 当前选中模型的窗口编号
        /// </summary>
        private int WinNum { get; set; } = -1;
        /// <summary>
        /// 当前选中模型在CModelUIElement3Ds中的索引
        /// </summary>
        private int PointNum { get; set; } = -1;
        /// <summary>
        /// 当前选中模型在MachiningPathDatas数据中的PoseId
        /// </summary>
        private int PoseId { get; set; } = -1;
        /// <summary>
        /// 当前选中模型的图像
        /// </summary>
        private HObject CurrentImage { get; set; } = new HObject();
        /// <summary>
        /// 当前选中模型原点在3D窗口中的索引
        /// </summary>
        private int CModel3DIndex { get; set; } = -1;
        /// <summary>
        /// 当前选中模型X轴在3D窗口中的索引
        /// </summary>
        private int XModel3DIndex { get; set; } = -1;
        /// <summary>
        /// 当前选中模型Y轴在3D窗口中的索引
        /// </summary>
        private int YModel3DIndex { get; set; } = -1;
        /// <summary>
        /// 当前选中模型Z轴在3D窗口中的索引
        /// </summary>
        private int ZModel3DIndex { get; set; } = -1;
        #endregion
        public LocusPage()
        {
            InitializeComponent();
            ReadWheelListToComboBox();
            LoadCoordinateSystemSTL();
            IncreasePointOffsetDistance_tbx.Text = IncreasePointOffsetDistance.ToString();
            this.Unloaded += LocusPage_Unloaded;
            this.Loaded += LocusPage_Loaded;
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

        #region=========事件=========
        private void LocusPage_Loaded(object sender, RoutedEventArgs e)
        {
            HWindowDisplay2DLocus.HalconWindow.SetWindowParam("graphics_stack_max_element_num", 150);
        }
        /// <summary>
        /// 页面关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LocusPage_Unloaded(object sender, RoutedEventArgs e)
        {
            ModelLists.Clear();
            ModelLists = null;
            CoordinateSystemGM3D_C.Clear();
            CoordinateSystemGM3D_C = null;
            CoordinateSystemGM3D_R.Clear();
            CoordinateSystemGM3D_R = null;
            CurrentModelUIElement3D = null;
            CurrentTemplateData = null;
            //LocusImage.Dispose();
        }
        /// <summary>
        /// 轮型选择改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WheelTypeChoose_cb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //获取当前轮型模板数据
            var sDB = new SqlAccess().SystemDataAccess;
            var wheelType = WheelTypeChoose_cb.SelectedItem.ToString();
            CurrentTemplateData = sDB.Queryable<TemplateDataModel>().Where(it => it.WheelType == wheelType).First();
            var slDB = new SqlAccess().SourceLocusDataAccess;
            var datas = slDB.Queryable<MachiningPathPosModel>().AS(wheelType).ToList();
            CurrentSpokeType = datas[datas.Count - 1].PoseId / 1000 / CurrentTemplateData.SpokeQuantity;
        }
        /// <summary>
        /// 模型鼠标左键点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Mui3D_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //选取模型前判断之前是否有选中,有则恢复上一个选中模型的材质
            if (CurrentModelUIElement3D != null)
            {
                if (PoseId % 1000 >= 901 && PoseId % 1000 <= 903)
                {
                    (ModelLists[WinNum].CModelUIElement3Ds[PointNum].Model as GeometryModel3D).Material = NormalMaterial(Brushes.LawnGreen);
                }
                else if (PoseId % 1000 >= 904 && PoseId % 1000 <= 906)
                {
                    (ModelLists[WinNum].CModelUIElement3Ds[PointNum].Model as GeometryModel3D).Material = NormalMaterial(Brushes.IndianRed);
                }
                else
                {
                    (ModelLists[WinNum].CModelUIElement3Ds[PointNum].Model as GeometryModel3D).Material = NormalMaterial(Brushes.MediumPurple);
                }
            }
            //从sender获取选中的模型UI 3D元素
            CurrentModelUIElement3D = sender as ModelUIElement3D;
            int id = 0;
            WinNum = -1;
            PointNum = -1;
            PoseId = -1;
            //在原点3D模型列表中查找当前选中原点的索引
            for (int i = 0; i < ModelLists.Count; i++)
            {
                var d = ModelLists[i].CModelUIElement3Ds.FindIndex(c => c == CurrentModelUIElement3D);
                if (d >= 0)
                {
                    WinNum = i;
                    PointNum = d;
                    id = id + PointNum + 1;
                    break;
                }
                id += ModelLists[i].CModelUIElement3Ds.Count;
            }
            if (WinNum < 0 || PointNum < 0) return;

            CModel3DIndex = HelixViewport.Children.IndexOf(ModelLists[WinNum].CModelUIElement3Ds[PointNum]);
            XModel3DIndex = HelixViewport.Children.IndexOf(ModelLists[WinNum].XModelUIElement3Ds[PointNum]);
            YModel3DIndex = HelixViewport.Children.IndexOf(ModelLists[WinNum].YModelUIElement3Ds[PointNum]);
            ZModel3DIndex = HelixViewport.Children.IndexOf(ModelLists[WinNum].ZModelUIElement3Ds[PointNum]);

            //获取当前点的加工路径数据
            var index = MachiningPathDatas.FindIndex(c => c.Id == id);
            if (index >= 0)
            {
                PoseId = MachiningPathDatas[index].PoseId;
                LocusPointInformationDisplay(PoseId);
                //更改选中模型的材质
                (ModelLists[WinNum].CModelUIElement3Ds[PointNum].Model as GeometryModel3D).Material = NormalMaterial(Brushes.Aquamarine);
                TemplateHelper.DisplayImage(CurrentImage, HWindowDisplay2DLocus.HalconWindow, false, LocusParameterSettingModel.IsImageEnhancement);
                var ds = TemplateHelper.GenLocusOfOutPoint(MachiningPathDatas, CurrentTemplateData.OutPointPoseId, CurrentSpokeType);
                TemplateHelper.GenAndShowLocus(HWindowDisplay2DLocus.HalconWindow, ds, PoseId);
            }
        }
        /// <summary>
        /// 3D窗口鼠标双击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HelixViewport_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SetHelixViewport();
        }
        #endregion

        #region=========数据加载=========
        /// <summary>
        /// 从数据库读取轮型列表到ComboBox
        /// </summary>
        private void ReadWheelListToComboBox()
        {
            var hDB = new SqlAccess().SourceLocusDataAccess;
            var tables = hDB.DbMaintenance.GetTableInfoList(false);
            if (tables.Count > 0)
            {
                int index = 0;
                ServiceLocator.Current.GetInstance<ReportPageViewModel>().WheelTypeDatas.Clear();
                for (int i = 0; i < tables.Count; i++)
                {
                    WheelTypeChoose_cb.Items.Add(tables[i].Name);
                    if (tables[i].Name == LocusParameterSettingModel.SelectWheelModel)
                    {
                        index = i;
                    }
                    ServiceLocator.Current.GetInstance<ReportPageViewModel>().WheelTypeDatas.Add(tables[i].Name);
                }
                ServiceLocator.Current.GetInstance<ReportPageViewModel>().SelectedItem = ServiceLocator.Current.GetInstance<ReportPageViewModel>().WheelTypeDatas[0];

                WheelTypeChoose_cb.SelectedIndex = index;
                //获取当前轮型模板数据
                var sDB = new SqlAccess().SystemDataAccess;
                var wheelType = WheelTypeChoose_cb.SelectedItem.ToString();
                CurrentTemplateData = sDB.Queryable<TemplateDataModel>().Where(it => it.WheelType == wheelType).First();
            }
        }
        /// <summary>
        /// 加载坐标系STL文件
        /// </summary>
        private void LoadCoordinateSystemSTL()
        {
            //获取指定目录中的文件名称（包括其路径）
            string[] model_files_c = System.IO.Directory.GetFiles(Environment.CurrentDirectory + @"\STL\CoordinateSystemSTL_C");
            //从文件导入模型
            ModelImporter importer_c = new ModelImporter();
            foreach (string file in model_files_c)
            {
                var mg3DG = importer_c.Load(file);
                //获取Model3DGroup中的GeometryModel3D
                var gm3D = mg3DG.Children[0] as GeometryModel3D;
                //设置对应模型材质
                if (file.IndexOf("c.stl") > -1) gm3D.Material = NormalMaterial(Brushes.MediumPurple);
                else if (file.IndexOf("x.stl") > -1) gm3D.Material = NormalMaterial(Brushes.Red);
                else if (file.IndexOf("y.stl") > -1) gm3D.Material = NormalMaterial(Brushes.Green);
                else if (file.IndexOf("z.stl") > -1) gm3D.Material = NormalMaterial(Brushes.Blue);
                CoordinateSystemGM3D_C.Add(gm3D);
            }

            //获取指定目录中的文件名称（包括其路径）
            string[] model_files_r = System.IO.Directory.GetFiles(Environment.CurrentDirectory + @"\STL\CoordinateSystemSTL_R");
            //从文件导入模型
            ModelImporter importer_r = new ModelImporter();
            foreach (string file in model_files_r)
            {
                var mg3DG = importer_r.Load(file);
                //获取Model3DGroup中的GeometryModel3D
                var gm3D = mg3DG.Children[0] as GeometryModel3D;
                //设置对应模型材质
                if (file.IndexOf("r.stl") > -1) gm3D.Material = NormalMaterial(Brushes.MediumPurple);
                else if (file.IndexOf("x.stl") > -1) gm3D.Material = NormalMaterial(Brushes.Red);
                else if (file.IndexOf("y.stl") > -1) gm3D.Material = NormalMaterial(Brushes.Green);
                else if (file.IndexOf("z.stl") > -1) gm3D.Material = NormalMaterial(Brushes.Blue);
                CoordinateSystemGM3D_R.Add(gm3D);
            }
        }
        #endregion

        #region=========功能=========
        /// <summary>
        /// 轨迹点信息显示
        /// </summary>
        private void LocusPointInformationDisplay(int poseId)
        {
            if (CurrentModelUIElement3D == null) return;
            int winNum = poseId / 1000;
            //索引显示
            CurrentModel.Text = $"窗口：{winNum}   点总数：{ModelLists[winNum - 1].CModelUIElement3Ds.Count}   点序号：{poseId}";
            //原点XYZ显示
            var z = ((CurrentModelUIElement3D.Transform as Transform3DGroup).Children[1] as TranslateTransform3D).OffsetZ;
            var y = ((CurrentModelUIElement3D.Transform as Transform3DGroup).Children[1] as TranslateTransform3D).OffsetY;
            var x = ((CurrentModelUIElement3D.Transform as Transform3DGroup).Children[1] as TranslateTransform3D).OffsetX;
            Height_tb.Text = z.ToString();
            Coordinate_tb.Text = "[ " + x + ", " + y + ", " + z + " ]";
            //旋转角度显示
            var zAngle = (((CurrentModelUIElement3D.Transform as Transform3DGroup).Children[4] as RotateTransform3D).Rotation as AxisAngleRotation3D).Angle;
            var yAngle = (((CurrentModelUIElement3D.Transform as Transform3DGroup).Children[3] as RotateTransform3D).Rotation as AxisAngleRotation3D).Angle;
            var xAngle = (((CurrentModelUIElement3D.Transform as Transform3DGroup).Children[2] as RotateTransform3D).Rotation as AxisAngleRotation3D).Angle;
            EulerAngles_tb.Text = "[ " + zAngle.ToString("f2") + ", " + yAngle.ToString("f2") + ", " + xAngle.ToString("f2") + " ]";
            //欧拉角转四元数并显示
            var q = ViewportHelper.EulerToQuaternion(xAngle, yAngle, zAngle);
            Quaternion_tb.Text = "[ " + q.W.ToString("f5") + ", " + q.X.ToString("f5") + ", " + q.Y.ToString("f5") + ", " + q.Z.ToString("f5") + " ]";
        }
        /// <summary>
        /// 普通材质设置
        /// </summary>
        /// <param name="mainColor"></param>
        /// <returns></returns>
        private static MaterialGroup NormalMaterial(Brush mainColor)
        {
            var materialGroup = new MaterialGroup();
            //发光材质
            EmissiveMaterial emissMat = new EmissiveMaterial(mainColor);
            //漫反射材质
            DiffuseMaterial diffMat = new DiffuseMaterial(mainColor);
            //镜面材质
            SpecularMaterial specMat = new SpecularMaterial(mainColor, 200);

            materialGroup.Children.Add(emissMat);
            materialGroup.Children.Add(diffMat);
            materialGroup.Children.Add(specMat);

            return materialGroup;
        }
        /// <summary>
        /// 设置3D窗口的全景显示
        /// </summary>
        private void SetHelixViewport()
        {
            HelixViewport.Camera.LookDirection = new Vector3D(-293.996, -1.739, -843.58);
            HelixViewport.Camera.UpDirection = new Vector3D(-0.944, -0.005, 0.328);
            HelixViewport.Camera.Position = new Point3D(259.460, 1.685, 882.318);
        }
        /// <summary>
        /// 加载轨迹点
        /// </summary>
        public List<PathPointModel> LoadLocus(List<MachiningPathPosModel> datas)
        {
            if (datas.Count == 0) return null;
            List<PathPointModel> pathPoints = new List<PathPointModel>();
            //通过最后一条数据的PoseId获取窗口轮廓的总数
            var a = datas[datas.Count - 1].PoseId / 1000;
            for (int i = 0; i < a; i++)
            {
                PathPointModel pathPoint = new PathPointModel();
                //通过PoseId获取第i+1个窗口的数据
                var points = datas.Where(x => x.PoseId >= (i + 1) * 1000 && x.PoseId <= (i + 1) * 1000 + 999).ToList();
                for (int j = 0; j < points.Count; j++)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        ModelUIElement3D mui3D = new ModelUIElement3D();
                        //每个窗口的PoseId 为 x0000的点显示为正方体
                        if (points[j].PoseId == (i + 1) * 1000)
                        {
                            mui3D.Model = new GeometryModel3D(CoordinateSystemGM3D_R[k].Geometry, CoordinateSystemGM3D_R[k].Material);
                        }
                        else if (points[j].PoseId == (i + 1) * 1000 + 901 && k == 0 || points[j].PoseId == (i + 1) * 1000 + 902 && k == 0 || points[j].PoseId == (i + 1) * 1000 + 903 && k == 0)
                        {
                            var ma = NormalMaterial(Brushes.LawnGreen);
                            mui3D.Model = new GeometryModel3D(CoordinateSystemGM3D_C[k].Geometry, ma);
                        }
                        else if (points[j].PoseId == (i + 1) * 1000 + 904 && k == 0 || points[j].PoseId == (i + 1) * 1000 + 905 && k == 0 || points[j].PoseId == (i + 1) * 1000 + 906 && k == 0)
                        {
                            var ma = NormalMaterial(Brushes.IndianRed);
                            mui3D.Model = new GeometryModel3D(CoordinateSystemGM3D_C[k].Geometry, ma);
                        }
                        //其他显示为圆
                        else
                        {
                            mui3D.Model = new GeometryModel3D(CoordinateSystemGM3D_C[k].Geometry, CoordinateSystemGM3D_C[k].Material);
                        }
                        //构建变换3D组
                        var tg = new Transform3DGroup();

                        //缩放
                        if (points[j].PoseId == (i + 1) * 1000 && k == 0)//起点模型的缩放
                        {
                            ScaleTransform3D st = new ScaleTransform3D(0.25, 0.25, 0.25, 0, 0, 0);
                            tg.Children.Add(st);
                        }
                        else if (points[j].PoseId > (i + 1) * 1000 + 900)//进出刀点缩放
                        {
                            ScaleTransform3D st = new ScaleTransform3D(0.6, 0.6, 0.6, 0, 0, 0);
                            tg.Children.Add(st);
                        }
                        else//其他模型的缩放
                        {
                            ScaleTransform3D st = new ScaleTransform3D(0.5, 0.5, 0.5, 0, 0, 0);
                            tg.Children.Add(st);
                        }
                        //平移
                        TranslateTransform3D tt3d = new TranslateTransform3D();
                        tt3d.OffsetX = points[j].X;
                        tt3d.OffsetY = points[j].Y;
                        tt3d.OffsetZ = points[j].Z;
                        tg.Children.Add(tt3d);

                        //旋转
                        RotateTransform3D rtz = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), points[j].EZ), new Point3D(points[j].X, points[j].Y, points[j].Z));
                        RotateTransform3D rty = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), points[j].EY), new Point3D(points[j].X, points[j].Y, points[j].Z));
                        RotateTransform3D rtx = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), points[j].EX), new Point3D(points[j].X, points[j].Y, points[j].Z));
                        tg.Children.Add(rtx);
                        tg.Children.Add(rty);
                        tg.Children.Add(rtz);

                        //设置变换
                        mui3D.Transform = tg;
                        //将模型添加到对应变量存储 0：原点模型 1：X轴模型 2：Y轴模型 3：Z轴模型
                        if (k == 0) pathPoint.CModelUIElement3Ds.Add(mui3D);
                        if (k == 1) pathPoint.XModelUIElement3Ds.Add(mui3D);
                        if (k == 2) pathPoint.YModelUIElement3Ds.Add(mui3D);
                        if (k == 3) pathPoint.ZModelUIElement3Ds.Add(mui3D);
                    }
                }
                pathPoints.Add(pathPoint);
            }
            return pathPoints;
        }
        /// <summary>
        /// 显示路径点
        /// </summary>
        /// <param name="pathPoints"></param>
        public void ShowPathPoints(List<PathPointModel> pathPoints)
        {
            if (pathPoints == null) return;
            //显示路径点
            for (int i = 0; i < pathPoints.Count; i++)
            {
                for (int j = 0; j < pathPoints[i].CModelUIElement3Ds.Count; j++)
                {
                    pathPoints[i].CModelUIElement3Ds[j].MouseLeftButtonDown += Mui3D_MouseLeftButtonDown;
                    HelixViewport.Children.Add(pathPoints[i].CModelUIElement3Ds[j]);
                }
            }
        }
        /// <summary>
        /// 显示坐标轴XYZ
        /// </summary>
        /// <param name="modelLists"></param>
        private void ShowAxisXYZ(List<PathPointModel> modelLists)
        {
            for (int i = 0; i < modelLists.Count; i++)
            {
                for (int j = 0; j < modelLists[i].XModelUIElement3Ds.Count; j++)
                {
                    HelixViewport.Children.Add(modelLists[i].XModelUIElement3Ds[j]);
                    HelixViewport.Children.Add(modelLists[i].YModelUIElement3Ds[j]);
                    HelixViewport.Children.Add(modelLists[i].ZModelUIElement3Ds[j]);
                }
            }
            if (WinNum < 0 || PointNum < 0) return;
            XModel3DIndex = HelixViewport.Children.IndexOf(ModelLists[WinNum].XModelUIElement3Ds[PointNum]);
            YModel3DIndex = HelixViewport.Children.IndexOf(ModelLists[WinNum].YModelUIElement3Ds[PointNum]);
            ZModel3DIndex = HelixViewport.Children.IndexOf(ModelLists[WinNum].ZModelUIElement3Ds[PointNum]);
        }
        /// <summary>
        /// 清除HelixViewport显示
        /// </summary>
        private void ClearHelixViewport()
        {
            //清除HelixViewport显示
            for (int i = HelixViewport.Children.Count - 1; i > 2; i--)
            {
                HelixViewport.Children.RemoveAt(i);
            }
            if (ModelLists == null) return;
            //清除生成的模型
            for (int i = 0; i < ModelLists.Count; i++)
            {
                for (int j = 0; j < ModelLists[i].CModelUIElement3Ds.Count; j++)
                {
                    ModelLists[i].CModelUIElement3Ds[j].MouseLeftButtonDown -= Mui3D_MouseLeftButtonDown;
                }
                ModelLists[i].CModelUIElement3Ds.Clear();

                ModelLists[i].XModelUIElement3Ds.Clear();
                ModelLists[i].YModelUIElement3Ds.Clear();
                ModelLists[i].ZModelUIElement3Ds.Clear();
            }
            ModelLists.Clear();
            ModelLists = null;
            CurrentModelUIElement3D = null;
            HWindowDisplay2DLocus.HalconWindow.ClearWindow();
        }
        /// <summary>
        /// 计算处于p1和p2之间距离p1指定长度的点
        /// </summary>
        /// <param name="p1">第一个点</param>
        /// <param name="p2">第二个点</param>
        /// <param name="distance">新点与第一个点的距离</param>
        /// <returns></returns>
        public MachiningPathPosModel CalculateThePointsBetweenTwoPoints(MachiningPathPosModel p1, MachiningPathPosModel p2, double distance)
        {
            MachiningPathPosModel newPose = new MachiningPathPosModel();
            //=========计算新点的X Y坐标
            //计算p1和p2之间的距离 d
            var d = Math.Sqrt((p2.X - p1.X) * (p2.X - p1.X) + (p2.Y - p1.Y) * (p2.Y - p1.Y));
            //计算p1和p2之间的距离 distance 的比例 p
            var p = distance / d;
            //计算点p的坐标
            var px = p1.X + p * (p2.X - p1.X);
            var py = p1.Y + p * (p2.Y - p1.Y);
            newPose.X = Math.Round(px, 2);
            newPose.Y = Math.Round(py, 2);
            // 计算新点的高度 
            //计算p1到p2的高度差 hd
            var hd = Math.Abs(p2.Z - p1.Z);
            //计算p1到p2之间距离 distance 的比例 hp
            var hp = hd / d;
            //计算点p的高度z
            double z = 0;
            if (p2.Z > p1.Z)
                z = p1.Z + hp * distance;
            else
                z = p1.Z - hp * distance;
            newPose.Z = Math.Round(z, 2);

            //获取指定高度下的新齐次变换矩阵
            var homMat2D = ServiceLocator.Current.GetInstance<SettingPageViewModel>().GetPixelToPhysicsHomMat2D(newPose.Z, TemplateHelper.PixelToPhysicsHomMat2D);
            HOperatorSet.HomMat2dInvert(homMat2D, out HTuple homMat2DInvert);
            HOperatorSet.AffineTransPoint2d(homMat2DInvert, newPose.X, newPose.Y, out HTuple row, out HTuple col);
            newPose.Row = Math.Round(row.D, 2);
            newPose.Column = Math.Round(col.D, 2); ;

            newPose.Id = 999;
            newPose.PoseId = p1.PoseId;
            newPose.EX = p1.EX;
            newPose.EY = p1.EY;
            newPose.EZ = p1.EZ;
            newPose.Q1 = p1.Q1;
            newPose.Q2 = p1.Q2;
            newPose.Q3 = p1.Q3;
            newPose.Q4 = p1.Q4;

            return newPose;
        }
        /// <summary>
        /// 保存当前修改的轨迹数据
        /// </summary>
        private void ReviseSave(List<MachiningPathPosModel> datas)
        {
            List<MachiningPathPosModel> ds = new List<MachiningPathPosModel>();
            for (int i = 0; i < datas.Count; i++)
            {
                MachiningPathPosModel data = new MachiningPathPosModel
                {
                    Id = datas[i].Id,
                    PoseId = datas[i].PoseId,
                    Row = datas[i].Row,
                    Column = datas[i].Column,
                    X = datas[i].X,
                    Y = datas[i].Y,
                    Z = datas[i].Z,
                    EX = datas[i].EX,
                    EY = datas[i].EY,
                    EZ = datas[i].EZ,
                    Q1 = datas[i].Q1,
                    Q2 = datas[i].Q2,
                    Q3 = datas[i].Q3,
                    Q4 = datas[i].Q4
                };
                ds.Add(data);
            }
            CurrentMachiningPathDataIndex += 1;
            MachiningPathDataList.Insert(CurrentMachiningPathDataIndex, ds);
            if (MachiningPathDataList.Count > 10)
            {
                MachiningPathDataList.RemoveAt(0);
                CurrentMachiningPathDataIndex -= 1;
            }
        }
        /// <summary>
        /// 移除3D窗口显示的坐标轴
        /// </summary>
        private void RemoveAxisDisplay()
        {
            if (HelixViewport.Children.Count > MachiningPathDatas.Count + 3)
            {
                int idx = MachiningPathDatas.Count + 3;
                int quantity = HelixViewport.Children.Count - idx;
                for (int i = 0; i < quantity; i++)
                {
                    HelixViewport.Children.RemoveAt(idx);
                }
            }
        }
        /// <summary>
        /// 设置当前选中点
        /// </summary>
        private void SetCurrentChoosePoint()
        {
            if (WinNum == -1 && PointNum == -1) return;
            if (WinNum > ModelLists.Count) return;
            else if (WinNum != -1 && PointNum == -1)
            {
                CurrentModelUIElement3D = ModelLists[WinNum].CModelUIElement3Ds[0];
                (ModelLists[WinNum].CModelUIElement3Ds[0].Model as GeometryModel3D).Material = NormalMaterial(Brushes.Aquamarine);
            }
            else
            {
                if (PointNum <= ModelLists[WinNum].CModelUIElement3Ds.Count - 1)
                {
                    CurrentModelUIElement3D = ModelLists[WinNum].CModelUIElement3Ds[PointNum];
                    (ModelLists[WinNum].CModelUIElement3Ds[PointNum].Model as GeometryModel3D).Material = NormalMaterial(Brushes.Aquamarine);
                }
                else if (PointNum > ModelLists[WinNum].CModelUIElement3Ds.Count - 1)
                {
                    PointNum = ModelLists[WinNum].CModelUIElement3Ds.Count - 1;
                    CurrentModelUIElement3D = ModelLists[WinNum].CModelUIElement3Ds[PointNum];
                    (ModelLists[WinNum].CModelUIElement3Ds[PointNum].Model as GeometryModel3D).Material = NormalMaterial(Brushes.Aquamarine);
                }
            }
        }
        /// <summary>
        /// 3D模型位置调整（XYZ）
        /// </summary>
        /// <param name="index">点在当前加工路径数据中的索引</param>
        private void PositionAdjustment(int index)
        {
            //获取指定高度下的新齐次变换矩阵
            var homMat2D = ServiceLocator.Current.GetInstance<SettingPageViewModel>().GetPixelToPhysicsHomMat2D(MachiningPathDatas[index].Z, TemplateHelper.PixelToPhysicsHomMat2D);
            HOperatorSet.AffineTransPoint2d(homMat2D, MachiningPathDatas[index].Row, MachiningPathDatas[index].Column, out HTuple qx, out HTuple qy);
            MachiningPathDatas[index].X = Math.Round(qx.D, 2);
            MachiningPathDatas[index].Y = Math.Round(qy.D, 2);
            for (int k = 0; k < 4; k++)
            {
                ModelUIElement3D mui3D = new ModelUIElement3D();
                //每个窗口的PoseId 为 x0000的点显示为正方体
                if (PoseId == (WinNum + 1) * 1000)
                {
                    mui3D.Model = new GeometryModel3D(CoordinateSystemGM3D_R[k].Geometry, CoordinateSystemGM3D_R[k].Material);
                }
                else if (PoseId == (WinNum + 1) * 1000 + 901 && k == 0 || PoseId == (WinNum + 1) * 1000 + 902 && k == 0 || PoseId == (WinNum + 1) * 1000 + 903 && k == 0)
                {
                    var ma = NormalMaterial(Brushes.LawnGreen);
                    mui3D.Model = new GeometryModel3D(CoordinateSystemGM3D_C[k].Geometry, ma);
                }
                else if (PoseId == (WinNum + 1) * 1000 + 904 && k == 0 || PoseId == (WinNum + 1) * 1000 + 905 && k == 0 || PoseId == (WinNum + 1) * 1000 + 906 && k == 0)
                {
                    var ma = NormalMaterial(Brushes.IndianRed);
                    mui3D.Model = new GeometryModel3D(CoordinateSystemGM3D_C[k].Geometry, ma);
                }
                //其他显示为圆
                else
                {
                    mui3D.Model = new GeometryModel3D(CoordinateSystemGM3D_C[k].Geometry, CoordinateSystemGM3D_C[k].Material);
                }
                //构建变换3D组
                var tg = new Transform3DGroup();

                //缩放
                if (PoseId == (WinNum + 1) * 1000 && k == 0)//起点模型的缩放
                {
                    ScaleTransform3D st = new ScaleTransform3D(0.25, 0.25, 0.25, 0, 0, 0);
                    tg.Children.Add(st);
                }
                else if (PoseId > (WinNum + 1) * 1000 + 900)//进出刀点缩放
                {
                    ScaleTransform3D st = new ScaleTransform3D(0.6, 0.6, 0.6, 0, 0, 0);
                    tg.Children.Add(st);
                }
                else//其他模型的缩放
                {
                    ScaleTransform3D st = new ScaleTransform3D(0.5, 0.5, 0.5, 0, 0, 0);
                    tg.Children.Add(st);
                }
                //平移
                TranslateTransform3D tt3d = new TranslateTransform3D();
                tt3d.OffsetX = MachiningPathDatas[index].X;
                tt3d.OffsetY = MachiningPathDatas[index].Y;
                tt3d.OffsetZ = MachiningPathDatas[index].Z;
                tg.Children.Add(tt3d);

                //旋转
                RotateTransform3D rtz = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), MachiningPathDatas[index].EZ), new Point3D(MachiningPathDatas[index].X, MachiningPathDatas[index].Y, MachiningPathDatas[index].Z));
                RotateTransform3D rty = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), MachiningPathDatas[index].EY), new Point3D(MachiningPathDatas[index].X, MachiningPathDatas[index].Y, MachiningPathDatas[index].Z));
                RotateTransform3D rtx = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), MachiningPathDatas[index].EX), new Point3D(MachiningPathDatas[index].X, MachiningPathDatas[index].Y, MachiningPathDatas[index].Z));
                tg.Children.Add(rtx);
                tg.Children.Add(rty);
                tg.Children.Add(rtz);

                //设置变换
                mui3D.Transform = tg;
                //将模型添加到对应变量存储 0：原点模型 1：X轴模型 2：Y轴模型 3：Z轴模型
                if (k == 0)
                {
                    if (CModel3DIndex > 2)
                        ModelLists[WinNum].CModelUIElement3Ds[PointNum].MouseLeftButtonDown -= Mui3D_MouseLeftButtonDown;
                    mui3D.MouseLeftButtonDown += Mui3D_MouseLeftButtonDown;
                    HelixViewport.Children[CModel3DIndex] = mui3D;
                    ModelLists[WinNum].CModelUIElement3Ds[PointNum] = mui3D;
                }
                if (k == 1)
                {
                    if (XModel3DIndex > 2 && HelixViewport.Children.Count >= XModel3DIndex)
                        HelixViewport.Children[XModel3DIndex] = mui3D;
                    ModelLists[WinNum].XModelUIElement3Ds[PointNum] = mui3D;
                }
                if (k == 2)
                {
                    if (YModel3DIndex > 2 && HelixViewport.Children.Count >= YModel3DIndex)
                        HelixViewport.Children[YModel3DIndex] = mui3D;
                    ModelLists[WinNum].YModelUIElement3Ds[PointNum] = mui3D;
                }
                if (k == 3)
                {
                    if (ZModel3DIndex > 2 && HelixViewport.Children.Count >= ZModel3DIndex)
                        HelixViewport.Children[ZModel3DIndex] = mui3D;
                    ModelLists[WinNum].ZModelUIElement3Ds[PointNum] = mui3D;
                }
            }
        }
        /// <summary>
        /// 3D模型姿态调整（EX,EY,EZ）
        /// </summary>
        /// <param name="rotationAxis">旋转轴</param>
        /// <param name="winNum">窗口编号</param>
        /// <param name="pointNum">点编号</param>
        /// <param name="machiningDataIndex">点在当前加工路径数据中的索引</param>
        /// <param name="rotationAngle">旋转的角度</param>
        private void PoseAdjustment(string rotationAxis, int winNum, int pointNum, int machiningDataIndex, double rotationAngle)
        {
            double angle_C = 0;//初始角度
            int childern = 0;//哪个轴旋转
            if (rotationAxis == "Z轴")
            {
                angle_C = (((ModelLists[winNum].CModelUIElement3Ds[pointNum].Transform as Transform3DGroup).Children[4] as RotateTransform3D).Rotation as AxisAngleRotation3D).Angle;
                childern = 4;
            }
            if (rotationAxis == "Y轴")
            {
                angle_C = (((ModelLists[winNum].CModelUIElement3Ds[pointNum].Transform as Transform3DGroup).Children[3] as RotateTransform3D).Rotation as AxisAngleRotation3D).Angle;
                childern = 3;
            }
            if (rotationAxis == "X轴")
            {
                angle_C = (((ModelLists[winNum].CModelUIElement3Ds[pointNum].Transform as Transform3DGroup).Children[2] as RotateTransform3D).Rotation as AxisAngleRotation3D).Angle;
                childern = 2;
            }
            var angle = RobotHelper.AngleCalculationAfterRotation(angle_C, rotationAngle);
            (((ModelLists[winNum].CModelUIElement3Ds[pointNum].Transform as Transform3DGroup).Children[childern] as RotateTransform3D).Rotation as AxisAngleRotation3D).Angle = angle;
            (((ModelLists[winNum].ZModelUIElement3Ds[pointNum].Transform as Transform3DGroup).Children[childern] as RotateTransform3D).Rotation as AxisAngleRotation3D).Angle = angle;
            (((ModelLists[winNum].YModelUIElement3Ds[pointNum].Transform as Transform3DGroup).Children[childern] as RotateTransform3D).Rotation as AxisAngleRotation3D).Angle = angle;
            (((ModelLists[winNum].XModelUIElement3Ds[pointNum].Transform as Transform3DGroup).Children[childern] as RotateTransform3D).Rotation as AxisAngleRotation3D).Angle = angle;

            //获取旋转角度
            var ez = (((ModelLists[winNum].CModelUIElement3Ds[pointNum].Transform as Transform3DGroup).Children[4] as RotateTransform3D).Rotation as AxisAngleRotation3D).Angle;
            var ey = (((ModelLists[winNum].CModelUIElement3Ds[pointNum].Transform as Transform3DGroup).Children[3] as RotateTransform3D).Rotation as AxisAngleRotation3D).Angle;
            var ex = (((ModelLists[winNum].CModelUIElement3Ds[pointNum].Transform as Transform3DGroup).Children[2] as RotateTransform3D).Rotation as AxisAngleRotation3D).Angle;

            MachiningPathDatas[machiningDataIndex].EX = Math.Round(ex, 2);
            MachiningPathDatas[machiningDataIndex].EY = Math.Round(ey, 2);
            MachiningPathDatas[machiningDataIndex].EZ = Math.Round(ez, 2);
            //欧拉角转四元数并显示
            var q = ViewportHelper.EulerToQuaternion(MachiningPathDatas[machiningDataIndex].EX, MachiningPathDatas[machiningDataIndex].EY, MachiningPathDatas[machiningDataIndex].EZ);
            MachiningPathDatas[machiningDataIndex].Q1 = Math.Round(q.X, 5);
            MachiningPathDatas[machiningDataIndex].Q2 = Math.Round(q.Y, 5);
            MachiningPathDatas[machiningDataIndex].Q3 = Math.Round(q.Z, 5);
            MachiningPathDatas[machiningDataIndex].Q4 = Math.Round(q.W, 5);
        }
        #endregion

        #region=========按钮=========
        /// <summary>
        /// 加载源轨迹按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadSourceLocus_Click(object sender, RoutedEventArgs e)
        {
            ServiceLocator.Current.GetInstance<LocusPageViewModel>().LocusButtonEnable = true;
            if (WheelTypeChoose_cb.Text == "")
            {
                MessageShow("请选择轮型！", MessageType.Default);
                return;
            }
            var slDB = new SqlAccess().SourceLocusDataAccess;
            //获取原始轨迹数据
            MachiningPathDatas.Clear();
            MachiningPathDatas = slDB.Queryable<MachiningPathPosModel>().AS(WheelTypeChoose_cb.Text).ToList();
            //===================构建3D模型数据======================
            ClearHelixViewport();
            ModelLists = LoadLocus(MachiningPathDatas);
            ShowPathPoints(ModelLists);
            SetHelixViewport();
            var templateImagePath = @"D:/DeburrSystem/TemplateImages/" + WheelTypeChoose_cb.Text + ".tif";
            if (File.Exists(templateImagePath))
            {
                CurrentImage.Dispose();
                HOperatorSet.ReadImage(out HObject locusImage, templateImagePath);
                CurrentImage = locusImage;
                TemplateHelper.DisplayImage(CurrentImage, HWindowDisplay2DLocus.HalconWindow, false, LocusParameterSettingModel.IsImageEnhancement);
                //生成并显示轨迹
                var ds = TemplateHelper.GenLocusOfOutPoint(MachiningPathDatas, CurrentTemplateData.OutPointPoseId, CurrentSpokeType);
                TemplateHelper.GenAndShowLocus(HWindowDisplay2DLocus.HalconWindow, ds, -1);
            }
        }
        /// <summary>
        /// 加载加工轨迹按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadProcessingLocus_Click(object sender, RoutedEventArgs e)
        {
            ServiceLocator.Current.GetInstance<LocusPageViewModel>().LocusButtonEnable = true;
            if (WheelTypeChoose_cb.Text == "")
            {
                MessageShow("请选择轮型！", MessageType.Default);
                return;
            }
            var plDB = new SqlAccess().ProcessingLocusDataAccess;
            //判断表是否存在
            var r = plDB.DbMaintenance.IsAnyTable(WheelTypeChoose_cb.Text, false);
            if (!r)
            {
                MessageShow("轮型" + WheelTypeChoose_cb.Text + "的加工轨迹不存在，请创建加工轨迹！", MessageType.Default);
                return;
            }
            MachiningPathDataList.Clear();
            ClearHelixViewport();
            //获取加工轨迹数据
            MachiningPathDatas.Clear();
            MachiningPathDatas = plDB.Queryable<MachiningPathPosModel>().AS(WheelTypeChoose_cb.Text).ToList();
            ModelLists = LoadLocus(MachiningPathDatas);
            ShowPathPoints(ModelLists);
            SetHelixViewport();
            var templateImagePath = @"D:/DeburrSystem/TemplateImages/" + WheelTypeChoose_cb.Text + ".tif";
            if (File.Exists(templateImagePath))
            {
                CurrentImage.Dispose();
                HOperatorSet.ReadImage(out HObject locusImage, templateImagePath);
                CurrentImage = locusImage;
                var sDB = new SqlAccess().SystemDataAccess;
                CurrentTemplateData = sDB.Queryable<TemplateDataModel>().Where(it => it.WheelType == WheelTypeChoose_cb.Text).First();
                TemplateHelper.DisplayImage(CurrentImage, HWindowDisplay2DLocus.HalconWindow, false, LocusParameterSettingModel.IsImageEnhancement);
                var ds = TemplateHelper.GenLocusOfOutPoint(MachiningPathDatas, CurrentTemplateData.OutPointPoseId, CurrentSpokeType);
                //生成并显示轨迹
                TemplateHelper.GenAndShowLocus(HWindowDisplay2DLocus.HalconWindow, ds, -1);
            }
            CurrentMachiningPathDataIndex = -1;
            ReviseSave(MachiningPathDatas);
        }
        /// <summary>
        /// 调整轨迹位姿按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AdjustmentPose_Click(object sender, RoutedEventArgs e)
        {
            if (MachiningPathDatas.Count < 1)
            {
                MessageShow("请先加载轨迹！", MessageType.Default);
                return;
            }
            //构建每个种类的单个窗口数据
            List<MachiningPathPosModel> pathPosDatas = new List<MachiningPathPosModel>();
            for (int i = 0; i < CurrentSpokeType; i++)
            {
                var datas = MachiningPathDatas.Where(x => x.PoseId >= (i + 1) * 1000 && x.PoseId <= (i + 1) * 1000 + 999).ToList();
                for (int j = 0; j < datas.Count; j++)
                {
                    pathPosDatas.Add(datas[j]);
                }
            }
            MachiningPathDatas.Clear();
            MachiningPathDatas = pathPosDatas;
            ClearHelixViewport();
            ModelLists = LoadLocus(MachiningPathDatas);
            ShowPathPoints(ModelLists);
            SetCurrentChoosePoint();
            TemplateHelper.DisplayImage(CurrentImage, HWindowDisplay2DLocus.HalconWindow, false, LocusParameterSettingModel.IsImageEnhancement);
            var ds = TemplateHelper.GenLocusOfOutPoint(MachiningPathDatas, CurrentTemplateData.OutPointPoseId, CurrentSpokeType);
            TemplateHelper.GenAndShowLocus(HWindowDisplay2DLocus.HalconWindow, ds, -1);
            ReviseSave(MachiningPathDatas);
        }
        /// <summary>
        /// 生成全部位姿按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GenAllPose_Click(object sender, RoutedEventArgs e)
        {
            if (WheelTypeChoose_cb.Text == "")
            {
                MessageShow("请选择轮型！", MessageType.Default);
                return;
            }
            if (MachiningPathDatas.Count == 0)
            {
                MessageShow("无路径数据！", MessageType.Default);
                return;
            }
            //窗口数量
            var num = MachiningPathDatas[MachiningPathDatas.Count - 1].PoseId / 1000;
            if (num == CurrentTemplateData.SpokeQuantity * CurrentSpokeType)
            {
                MessageShow("已生全部位姿，请勿重复生成！", MessageType.Default);
                return;
            }
            int id = 1;
            List<MachiningPathPosModel> datas = new List<MachiningPathPosModel>();
            for (int i = 0; i < CurrentTemplateData.SpokeQuantity; i++)
            {               
                //单轮辐旋转的角度
                var angle = 360.0 / CurrentTemplateData.SpokeQuantity * i;
                //角度转弧度
                HOperatorSet.TupleRad(angle, out HTuple rad);
                var wins = MachiningPathDatas[MachiningPathDatas.Count - 1].PoseId / 1000;
                for (int j = 0; j < wins; j++)
                {
                    int startPoseId = (j + i * wins + 1) * 1000;
                    var ds = MachiningPathDatas.Where(x => x.PoseId >= (j + 1) * 1000 && x.PoseId <= (j + 1) * 1000 + 999).ToList();
                    for (int k = 0; k < ds.Count; k++)
                    {
                        MachiningPathPosModel m = new MachiningPathPosModel();
                        m.Id = id;
                        m.PoseId = ds[k].PoseId % 1000 + startPoseId;
                        TemplateHelper.GetThePointAfterRotation(ds[k].Row, ds[k].Column, CurrentTemplateData.CenterRow, CurrentTemplateData.CenterColumn, rad, out HTuple r, out HTuple c);
                        m.Row = Math.Round(r.D, 2);
                        m.Column = Math.Round(c.D, 2);
                        //获取指定高度下的新齐次变换矩阵
                        var homMat2D = ServiceLocator.Current.GetInstance<SettingPageViewModel>().GetPixelToPhysicsHomMat2D(ds[k].Z, TemplateHelper.PixelToPhysicsHomMat2D);
                        HOperatorSet.AffineTransPoint2d(homMat2D, m.Row, m.Column, out HTuple qx, out HTuple qy);
                        m.X = Math.Round(qx.D, 2);
                        m.Y = Math.Round(qy.D, 2);
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
            ClearHelixViewport();
            MachiningPathDatas.Clear();
            MachiningPathDatas = datas;
            ModelLists = LoadLocus(MachiningPathDatas);
            ShowPathPoints(ModelLists);
            SetHelixViewport();
            TemplateHelper.DisplayImage(CurrentImage, HWindowDisplay2DLocus.HalconWindow, false, LocusParameterSettingModel.IsImageEnhancement);
            var d = TemplateHelper.GenLocusOfOutPoint(MachiningPathDatas, CurrentTemplateData.OutPointPoseId, CurrentSpokeType);
            TemplateHelper.GenAndShowLocus(HWindowDisplay2DLocus.HalconWindow, d, -1);
            MachiningPathDataList.Clear();
            CurrentMachiningPathDataIndex = -1;
        }
        /// <summary>
        /// 加载姿态按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadPose_Click(object sender, RoutedEventArgs e)
        {
            if (MachiningPathDatas.Count < 1)
            {
                MessageShow("无轨迹数据，请先加载迹数据！", MessageType.Default);
                return;
            }
            if (HelixViewport.Children.Count > MachiningPathDatas.Count * 4)
            {
                MessageShow("轨迹姿态已加载，请勿重复加载！", MessageType.Default);
                return;
            }
            ShowAxisXYZ(ModelLists);
        }
        /// <summary>
        /// 姿态单个调整按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SingleAdjust_Click(object sender, RoutedEventArgs e)
        {
            if (MachiningPathDatas == null || MachiningPathDatas.Count == 0)
            {
                MessageShow("请先加载轨迹！", MessageType.Default);
                return;
            }
            if (MachiningPathDatas[MachiningPathDatas.Count - 1].PoseId / 1000 > CurrentSpokeType)
            {
                MessageShow("请点击《调整轨迹位姿》按钮加载单个窗口轨迹！", MessageType.Default);
                return;
            }
            if (CurrentModelUIElement3D == null)
            {
                MessageShow("未选中轨迹点！", MessageType.Default);
                return;
            }
            if (HelixViewport.Children.Count < MachiningPathDatas.Count * 4)
            {
                MessageShow("请先加载轨迹坐标系！", MessageType.Default);
                return;
            }
            double value = 0;
            try
            {
                value = double.Parse(Rotate_tb.Text);
            }
            catch (Exception ex)
            {
                MessageShow("角度输入错误：" + ex.Message, MessageType.Error);
                return;
            }
            SingleAdjust_btn.IsEnabled = false;
            BatchAdjustment_btn.IsEnabled = false;
            var index = MachiningPathDatas.FindIndex(t => t.PoseId == PoseId);
            //调整选中点
            PoseAdjustment(RotationChoose_cb.Text, WinNum, PointNum, index, value);
            //如果选中点是起点
            if(PoseId % 1000 == 0)
            {
                var n =MachiningPathDatas.Where(x => x.PoseId >= PoseId /1000 * 1000 + 901 && x.PoseId <= PoseId / 1000 * 1000 + 903).Count();
                if(n > 0)
                {
                    var index3 = MachiningPathDatas.FindIndex(t => t.PoseId == PoseId / 1000 * 1000 + 903);
                    //调整903
                    PoseAdjustment(RotationChoose_cb.Text, WinNum, PointNum - 1, index3, value);
                    //调整902
                    PoseAdjustment(RotationChoose_cb.Text, WinNum, PointNum - 2, index3 - 1, value);
                }
            }
            //如果选中点是出刀参照点
            string[] outPointPoseIds = CurrentTemplateData.OutPointPoseId.Split(',');
            if (PoseId % 1000 == int.Parse(outPointPoseIds[WinNum]))
            {
                var outPoints = MachiningPathDatas.Where(x => x.PoseId >= PoseId / 1000 * 1000 + 904 && x.PoseId <= PoseId / 1000 * 1000 + 906).Count();
                if (outPoints > 0)
                {
                    var inPoints = MachiningPathDatas.Where(x => x.PoseId >= PoseId / 1000 * 1000 + 901 && x.PoseId <= PoseId / 1000 * 1000 + 903).Count();
                    var datasCount = MachiningPathDatas.Where(x => x.PoseId >= PoseId / 1000 * 1000 && x.PoseId <= PoseId / 1000 * 1000 + 900).Count();
                    var index4 = MachiningPathDatas.FindIndex(t => t.PoseId == PoseId / 1000 * 1000 + 904);
                    //调整904
                    PoseAdjustment(RotationChoose_cb.Text, WinNum, datasCount + inPoints, index4, value);
                    //调整905
                    PoseAdjustment(RotationChoose_cb.Text, WinNum, datasCount + inPoints + 1, index4 + 1, value);
                }
            }
            LocusPointInformationDisplay(PoseId);
            ReviseSave(MachiningPathDatas);
            SingleAdjust_btn.IsEnabled = true;
            BatchAdjustment_btn.IsEnabled = true;
        }
        /// <summary>
        /// 姿态批量调整按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BatchAdjustment_Click(object sender, RoutedEventArgs e)
        {
            double rotateValue;
            int startPoseValue, endPoseValue;
            //输入条件判断
            {
                if (MachiningPathDatas == null || MachiningPathDatas.Count == 0)
                {
                    MessageShow("请先加载轨迹！", MessageType.Default);
                    return;
                }
                if (MachiningPathDatas[MachiningPathDatas.Count - 1].PoseId / 1000 > CurrentSpokeType)
                {
                    MessageShow("请点击《调整轨迹位姿》按钮加载单个窗口轨迹！", MessageType.Default);
                    return;
                }
                try
                {
                    rotateValue = double.Parse(Rotate_tb.Text);
                    startPoseValue = int.Parse(startPose_tb.Text);
                    endPoseValue = int.Parse(endPose_tb.Text);
                }
                catch (Exception ex)
                {
                    MessageShow("调整参数输入错误：" + ex.Message + "。请检查输入的起点值和终点值以及调整的角度值！", MessageType.Error);
                    return;
                }
                if (startPoseValue == endPoseValue)
                {
                    MessageShow("起点与终点不能相等！", MessageType.Default);
                    return;
                }
                if (HelixViewport.Children.Count < MachiningPathDatas.Count * 4)
                {
                    MessageShow("请先点击《显示轨迹姿态》按钮加载轨迹姿态！", MessageType.Default);
                    return;
                }
                int startPoseIndex = MachiningPathDatas.FindIndex(x => x.PoseId == startPoseValue);
                if (startPoseIndex < 0)
                {
                    MessageShow("批量调整的起点输入错误，请重新输入！", MessageType.Default);
                    return;
                }
                int endPoseIndex = MachiningPathDatas.FindIndex(x => x.PoseId == endPoseValue);
                if (endPoseIndex < 0)
                {
                    MessageShow("批量调整的终点输入错误，请重新输入！", MessageType.Default);
                    return;
                }
                if (startPoseValue % 1000 > 900 || endPoseValue % 1000 > 900)
                {
                    MessageShow("批量调整的起点或终点不能包含进出刀点，请重新输入！", MessageType.Default);
                    return;
                }
            }

            int startPoseId = startPoseValue / 1000 * 1000;
            //=============需要获取调整区间的点的索引
            var datas = MachiningPathDatas.Where(x => x.PoseId >= startPoseId && x.PoseId <= startPoseId + 999).ToList();
            int index = -1;
            int count = -1;
            WinNum = startPoseValue / 1000 - 1;
            //如果起始点小于结束点
            if (startPoseValue < endPoseValue)
            {
                index = datas.FindIndex(t => t.PoseId == startPoseValue);
                count = endPoseValue - startPoseValue;
                if (count > MachiningPathDatas.Count)
                {
                    MessageShow("批量调整的终点输入错误，请重新输入！", MessageType.Default);
                    return;
                }
                SingleAdjust_btn.IsEnabled = false;
                BatchAdjustment_btn.IsEnabled = false;
                //调整区间
                var mIndex = MachiningPathDatas.FindIndex(p => p.PoseId == startPoseValue);
                for (int i = index; i <= index + count; i++)
                {
                    PoseAdjustment(RotationChoose_cb.Text, WinNum, i, mIndex, rotateValue);
                    mIndex++;
                }
                //如果调整区间包含起点
                if(startPoseValue % 1000 == 0)
                {
                    var inPoints = MachiningPathDatas.Where(x => x.PoseId >= startPoseValue / 1000 * 1000 + 901 && x.PoseId <= startPoseValue / 1000 * 1000 + 903).Count();
                    if (inPoints > 0)
                    {
                        var index3 = MachiningPathDatas.FindIndex(t => t.PoseId == startPoseValue / 1000 * 1000 + 903);
                        //调整903
                        PoseAdjustment(RotationChoose_cb.Text, WinNum, 2, index3, rotateValue);
                        //调整902
                        PoseAdjustment(RotationChoose_cb.Text, WinNum, 1, index3 - 1, rotateValue);
                    }
                }
                //如果调整的区间包含出刀参照点
                var outPointPoseIds = CurrentTemplateData.OutPointPoseId.Split(',');
                if (startPoseValue % 1000 <= int.Parse(outPointPoseIds[WinNum]) && endPoseValue % 1000 >= int.Parse(outPointPoseIds[WinNum]))
                {
                    var outPoints = MachiningPathDatas.Where(x => x.PoseId >= startPoseValue / 1000 * 1000 + 904 && x.PoseId <= startPoseValue / 1000 * 1000 + 906).Count();
                    if (outPoints > 0)
                    {
                        var inPoints = MachiningPathDatas.Where(x => x.PoseId >= startPoseValue / 1000 * 1000 + 901 && x.PoseId <= startPoseValue / 1000 * 1000 + 903).Count();
                        var datasCount = MachiningPathDatas.Where(x => x.PoseId >= startPoseValue / 1000 * 1000 && x.PoseId <= startPoseValue / 1000 * 1000 + 900).Count();
                        var index4 = MachiningPathDatas.FindIndex(t => t.PoseId == startPoseValue / 1000 * 1000 + 904);
                        //调整904
                        PoseAdjustment(RotationChoose_cb.Text, WinNum, datasCount + inPoints, index4, rotateValue);
                        //调整905
                        PoseAdjustment(RotationChoose_cb.Text, WinNum, datasCount + inPoints + 1, index4 + 1, rotateValue);
                    }
                }
            }
            //如果起始点大于结束点
            else
            {
                //是否存在进刀点
                var inPoints = MachiningPathDatas.Where(x => x.PoseId >= startPoseValue / 1000 * 1000 + 901 && x.PoseId <= startPoseValue / 1000 * 1000 + 903).Count();
                //调整从startPoseValue到最后一个点之间的点
                var startIndex = datas.FindIndex(t => t.PoseId == startPoseValue);
                var mIndex = MachiningPathDatas.FindIndex(p => p.PoseId == startPoseValue);
                for (int i = startIndex; i < datas.Count - 3; i++)
                {
                    PoseAdjustment(RotationChoose_cb.Text, WinNum, i, mIndex, rotateValue);
                    mIndex++;
                }

                //调整从0到endPoseValue之间的点
                int i1 = 0;
                //如果存在进刀点
                if (inPoints > 0) i1 = 3;
                var endIndex = endPoseValue % 1000 + i1;
                mIndex = MachiningPathDatas.FindIndex(p => p.PoseId == endPoseValue / 1000 * 1000);
                for (int i = i1; i <= endIndex; i++)
                {
                    PoseAdjustment(RotationChoose_cb.Text, WinNum, i, mIndex, rotateValue);
                    mIndex++;
                }

                //调整进刀点902、903
                if (inPoints > 0)
                {
                    var index3 = MachiningPathDatas.FindIndex(t => t.PoseId == startPoseValue / 1000 * 1000 + 903);
                    //调整903
                    PoseAdjustment(RotationChoose_cb.Text, WinNum, 2, index3, rotateValue);
                    //调整902
                    PoseAdjustment(RotationChoose_cb.Text, WinNum, 1, index3 - 1, rotateValue);
                }

                var countData = MachiningPathDatas.Where(x => x.PoseId >= startPoseId && x.PoseId <= startPoseId + 900).Count();
                //判断出刀参照点是否在调整的区间之内
                var outPointPoseIds = CurrentTemplateData.OutPointPoseId.Split(',');
                if (int.Parse(outPointPoseIds[WinNum]) >= 0 && int.Parse(outPointPoseIds[WinNum]) <= endPoseValue % 1000 ||
                    startPoseValue % 1000 <= int.Parse(outPointPoseIds[WinNum]) && countData >= int.Parse(outPointPoseIds[WinNum]))
                {
                    var outPoints = MachiningPathDatas.Where(x => x.PoseId >= startPoseValue / 1000 * 1000 + 904 && x.PoseId <= startPoseValue / 1000 * 1000 + 906).Count();
                    if (outPoints > 0)
                    {
                        var inPoint = MachiningPathDatas.Where(x => x.PoseId >= startPoseValue / 1000 * 1000 + 901 && x.PoseId <= startPoseValue / 1000 * 1000 + 903).Count();
                        var datasCount = MachiningPathDatas.Where(x => x.PoseId >= startPoseValue / 1000 * 1000 && x.PoseId <= startPoseValue / 1000 * 1000 + 900).Count();
                        var index4 = MachiningPathDatas.FindIndex(t => t.PoseId == startPoseValue / 1000 * 1000 + 904);
                        //调整904
                        PoseAdjustment(RotationChoose_cb.Text, WinNum, datasCount + inPoint, index4, rotateValue);
                        //调整905
                        PoseAdjustment(RotationChoose_cb.Text, WinNum, datasCount + inPoint + 1, index4 + 1, rotateValue);
                    }
                }
            }

            //选取模型前判断之前是否有选中,有则恢复上一个选中模型的材质
            if (CurrentModelUIElement3D != null)
            {
                if (PoseId % 1000 >= 901 && PoseId % 1000 <= 903)
                {
                    (ModelLists[WinNum].CModelUIElement3Ds[PointNum].Model as GeometryModel3D).Material = NormalMaterial(Brushes.LawnGreen);
                }
                else if (PoseId % 1000 >= 904 && PoseId % 1000 <= 906)
                {
                    (ModelLists[WinNum].CModelUIElement3Ds[PointNum].Model as GeometryModel3D).Material = NormalMaterial(Brushes.IndianRed);
                }
                else
                {
                    (ModelLists[WinNum].CModelUIElement3Ds[PointNum].Model as GeometryModel3D).Material = NormalMaterial(Brushes.MediumPurple);
                }
            }
            #region=======设置PoseId、PointNum========
            //查找是否存在进出刀点
            var ds = MachiningPathDatas.Where(x => x.PoseId > startPoseId + 900 && x.PoseId <= startPoseId + 999).ToList();
            if (PoseId == -1)
            {
                PoseId = startPoseValue;
                if (ds.Count > 0) PointNum = startPoseValue % 1000 + 3;
                else PointNum = startPoseValue % 1000;
            }
            //如果起始点大于结束点
            else if (startPoseValue > endPoseValue)
            {
                if (PoseId >= endPoseValue + 1 && PoseId <= startPoseValue - 1)
                {
                    PoseId = startPoseValue;
                    if (ds.Count > 0) PointNum = startPoseValue % 1000 + 3;
                    else PointNum = startPoseValue % 1000;
                }
            }
            //如果起始点小于结束点
            else
            {
                if (PoseId <= startPoseValue - 1 || PoseId >= endPoseValue + 1)
                {
                    PoseId = startPoseValue;
                    if (ds.Count > 0) PointNum = startPoseValue % 1000 + 3;
                    else PointNum = startPoseValue % 1000;
                }
            }
            #endregion
            SetCurrentChoosePoint();
            LocusPointInformationDisplay(PoseId);
            ReviseSave(MachiningPathDatas);
            SingleAdjust_btn.IsEnabled = true;
            BatchAdjustment_btn.IsEnabled = true;
        }
        /// <summary>
        /// 高度单个调整按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HeightSingleAdjust_Click(object sender, RoutedEventArgs e)
        {
            if (MachiningPathDatas == null || MachiningPathDatas.Count == 0)
            {
                MessageShow("请先加载轨迹！", MessageType.Default);
                return;
            }
            if (MachiningPathDatas[MachiningPathDatas.Count - 1].PoseId / 1000 > CurrentSpokeType)
            {
                MessageShow("请点击《调整轨迹位姿》按钮加载单个窗口轨迹！", MessageType.Default);
                return;
            }
            if (CurrentModelUIElement3D == null)
            {
                MessageShow("未选中轨迹点！", MessageType.Default);
                return;
            }
            double value = 0;
            try
            {
                value = double.Parse(Height_tb.Text);
            }
            catch (Exception ex)
            {
                MessageShow("高度输入错误：" + ex.Message, MessageType.Error);
                return;
            }
            if (value < 0) return;
            //修改数据
            var index = MachiningPathDatas.FindIndex(t => t.PoseId == PoseId);
            if (MachiningPathDatas[index].Z == value) return;
            ServiceLocator.Current.GetInstance<LocusPageViewModel>().LocusButtonEnable = false;
            //调整选中模型高度
            MachiningPathDatas[index].Z = value;
            PositionAdjustment(index);
            SetCurrentChoosePoint();
            LocusPointInformationDisplay(PoseId);
            TemplateHelper.DisplayImage(CurrentImage, HWindowDisplay2DLocus.HalconWindow, false, LocusParameterSettingModel.IsImageEnhancement);
            var ds = TemplateHelper.GenLocusOfOutPoint(MachiningPathDatas, CurrentTemplateData.OutPointPoseId, CurrentSpokeType);
            TemplateHelper.GenAndShowLocus(HWindowDisplay2DLocus.HalconWindow, ds, MachiningPathDatas[index].PoseId);
            RemoveAxisDisplay();
            ReviseSave(MachiningPathDatas);
            ServiceLocator.Current.GetInstance<LocusPageViewModel>().LocusButtonEnable = true;
        }
        /// <summary>
        /// 高度批量调整按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HeightBatchAdjustment_Click(object sender, RoutedEventArgs e)
        {
            if (MachiningPathDatas == null || MachiningPathDatas.Count == 0)
            {
                MessageShow("请先加载轨迹！", MessageType.Default);
                return;
            }
            if (MachiningPathDatas[MachiningPathDatas.Count - 1].PoseId / 1000 > CurrentSpokeType)
            {
                MessageShow("请点击《调整轨迹位姿》按钮加载单个窗口轨迹！", MessageType.Default);
                return;
            }
            double heightValue;
            int startPoseValue, endPoseValue;
            try
            {
                heightValue = double.Parse(Height_tb.Text);
                startPoseValue = int.Parse(startPose_tb.Text);
                endPoseValue = int.Parse(endPose_tb.Text);
            }
            catch (Exception ex)
            {
                MessageShow("调整参数输入错误：" + ex.Message + "。请检查输入的起点值和终点值以及调整的高度值！", MessageType.Error);
                return;
            }
            if (heightValue < 0) return;
            int startPoseIndex = MachiningPathDatas.FindIndex(x => x.PoseId == startPoseValue);
            if (startPoseIndex < 0)
            {
                MessageShow("批量调整的起点输入错误，请重新输入！", MessageType.Default);
                return;
            }
            int endPoseIndex = MachiningPathDatas.FindIndex(x => x.PoseId == endPoseValue);
            if (endPoseIndex < 0)
            {
                MessageShow("批量调整的终点输入错误，请重新输入！", MessageType.Default);
                return;
            }
            if (startPoseValue % 1000 > 900 || endPoseValue % 1000 > 900)
            {
                MessageShow("批量调整的起点或终点不能包含进出刀点，请重新输入！", MessageType.Default);
                return;
            }
            if (startPoseValue < 1000 || endPoseValue < 1000)
            {
                MessageShow("批量调整的起点或终点输入错误，请重新输入！", MessageType.Default);
                return;
            }
            ServiceLocator.Current.GetInstance<LocusPageViewModel>().LocusButtonEnable = false;
            //获取窗口索引
            var winNum = startPoseValue / 1000;
            //=============需要获取调整区间的点的索引
            var datas = MachiningPathDatas.Where(x => x.PoseId >= winNum * 1000 && x.PoseId <= winNum * 1000 + 999).ToList();
            int index = -1;
            int count = -1;
            var homMat2D = ServiceLocator.Current.GetInstance<SettingPageViewModel>().GetPixelToPhysicsHomMat2D(heightValue, TemplateHelper.PixelToPhysicsHomMat2D);
            //如果起始点小于结束点
            if (startPoseValue < endPoseValue)
            {
                index = datas.FindIndex(t => t.PoseId == startPoseValue);
                count = endPoseValue - startPoseValue;
                if (count > MachiningPathDatas.Count)
                {
                    MessageShow("批量调整的终点输入错误，请重新输入！", MessageType.Default);
                    return;
                }
                var mIndex = MachiningPathDatas.FindIndex(p => p.PoseId == startPoseValue);
                for (int i = index; i <= index + count; i++)
                {
                    MachiningPathDatas[mIndex].Z = heightValue;
                    HOperatorSet.AffineTransPoint2d(homMat2D, MachiningPathDatas[mIndex].Row, MachiningPathDatas[mIndex].Column, out HTuple qx, out HTuple qy);
                    MachiningPathDatas[mIndex].X = Math.Round(qx.D, 2);
                    MachiningPathDatas[mIndex].Y = Math.Round(qy.D, 2);
                    mIndex++;
                }
            }
            else
            {
                var ii = 0;
                var iii = datas.Count;
                //获取结束点的索引
                index = datas.FindIndex(t => t.PoseId == endPoseValue);
                var mIndex = MachiningPathDatas.FindIndex(p => p.PoseId == endPoseValue / 1000 * 1000);
                //判断是否包含进出刀点，如果第一条数据的PosId大于等于winNum * 1000 + 901，则包含
                if (datas[0].PoseId >= winNum * 1000 + 901)
                {
                    //ii = 2 是因为前两个轨迹点为进刀点
                    ii = 3;
                    //datas.Count -2是因为最后两个点为出刀点
                    iii = datas.Count - 3;
                }
                //调整从索引ii开始到index结束的轨迹点
                for (int i = ii; i <= index; i++)
                {
                    MachiningPathDatas[mIndex].Z = heightValue;
                    HOperatorSet.AffineTransPoint2d(homMat2D, MachiningPathDatas[mIndex].Row, MachiningPathDatas[mIndex].Column, out HTuple qx, out HTuple qy);
                    MachiningPathDatas[mIndex].X = Math.Round(qx.D, 2);
                    MachiningPathDatas[mIndex].Y = Math.Round(qy.D, 2);
                    mIndex++;
                }
                //调整从index开始到iii的结束点，
                index = datas.FindIndex(t => t.PoseId == startPoseValue);
                mIndex = MachiningPathDatas.FindIndex(p => p.PoseId == startPoseValue);
                for (int i = index; i < iii; i++)
                {
                    MachiningPathDatas[mIndex].Z = heightValue;
                    HOperatorSet.AffineTransPoint2d(homMat2D, MachiningPathDatas[mIndex].Row, MachiningPathDatas[mIndex].Column, out HTuple qx, out HTuple qy);
                    MachiningPathDatas[mIndex].X = Math.Round(qx.D, 2);
                    MachiningPathDatas[mIndex].Y = Math.Round(qy.D, 2);
                    mIndex++;
                }
            }
            //选取模型前判断之前是否有选中,有则恢复上一个选中模型的材质
            if (CurrentModelUIElement3D != null)
            {
                if (PoseId % 1000 >= 901 && PoseId % 1000 <= 903)
                {
                    (ModelLists[WinNum].CModelUIElement3Ds[PointNum].Model as GeometryModel3D).Material = NormalMaterial(Brushes.LawnGreen);
                }
                else if (PoseId % 1000 >= 904 && PoseId % 1000 <= 906)
                {
                    (ModelLists[WinNum].CModelUIElement3Ds[PointNum].Model as GeometryModel3D).Material = NormalMaterial(Brushes.IndianRed);
                }
                else
                {
                    (ModelLists[WinNum].CModelUIElement3Ds[PointNum].Model as GeometryModel3D).Material = NormalMaterial(Brushes.MediumPurple);
                }
            }
            //查找是否存在进出刀点
            var ds = MachiningPathDatas.Where(x => x.PoseId > winNum * 1000 + 900 && x.PoseId <= winNum * 1000 + 999).ToList();
            if (PoseId == -1)
            {
                PoseId = startPoseValue;
                if (ds.Count > 0) PointNum = startPoseValue % 1000 + 3;
                else PointNum = startPoseValue % 1000;
            }
            //如果起始点大于结束点
            else if (startPoseValue > endPoseValue)
            {
                if (PoseId >= endPoseValue + 1 && PoseId <= startPoseValue - 1)
                {
                    PoseId = startPoseValue;
                    if (ds.Count > 0) PointNum = startPoseValue % 1000 + 3;
                    else PointNum = startPoseValue % 1000;
                }
            }
            //如果起始点小于结束点
            else
            {
                if (PoseId <= startPoseValue - 1 || PoseId >= endPoseValue + 1)
                {
                    PoseId = startPoseValue;
                    if (ds.Count > 0) PointNum = startPoseValue % 1000 + 3;
                    else PointNum = startPoseValue % 1000;
                }
            }
            CurrentModelUIElement3D = ModelLists[winNum - 1].CModelUIElement3Ds[PointNum];
            LocusPointInformationDisplay(PoseId);
            ClearHelixViewport();
            ModelLists = LoadLocus(MachiningPathDatas);
            ShowPathPoints(ModelLists);

            //更改选中模型的材质
            (ModelLists[winNum - 1].CModelUIElement3Ds[PointNum].Model as GeometryModel3D).Material = NormalMaterial(Brushes.Aquamarine);
            ShowAxisXYZ(ModelLists);
            TemplateHelper.DisplayImage(CurrentImage, HWindowDisplay2DLocus.HalconWindow, false, LocusParameterSettingModel.IsImageEnhancement);
            var mds = TemplateHelper.GenLocusOfOutPoint(MachiningPathDatas, CurrentTemplateData.OutPointPoseId, CurrentSpokeType);
            TemplateHelper.GenAndShowLocus(HWindowDisplay2DLocus.HalconWindow, mds, PoseId);
            RemoveAxisDisplay();
            ReviseSave(MachiningPathDatas);
            ServiceLocator.Current.GetInstance<LocusPageViewModel>().LocusButtonEnable = true;
        }
        /// <summary>
        /// 高度整体偏移按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AllHeightOffset_Click(object sender, RoutedEventArgs e)
        {
            if (MachiningPathDatas == null || MachiningPathDatas.Count == 0)
            {
                MessageShow("请先加载轨迹！", MessageType.Default);
                return;
            }
            if (MachiningPathDatas[MachiningPathDatas.Count - 1].PoseId / 1000 > CurrentSpokeType)
            {
                MessageShow("请点击《调整轨迹位姿》按钮加载单个窗口轨迹！", MessageType.Default);
                return;
            }
            if (CurrentModelUIElement3D == null)
            {
                MessageShow("未选中轨迹点！", MessageType.Default);
                return;
            }
            double value = 0;
            try
            {
                value = double.Parse(Height_tb.Text);
            }
            catch (Exception ex)
            {
                MessageShow("高度输入错误：" + ex.Message, MessageType.Error);
                return;
            }
            if (value == 0) return;
            var datas = MachiningPathDatas.Where(x => x.PoseId >= (WinNum + 1) * 1000 && x.PoseId <= (WinNum + 1) * 1000 + 999).ToList();
            int index = MachiningPathDatas.FindIndex(x => x.PoseId == datas[0].PoseId);
            for (int i = 0; i < datas.Count; i++)
            {
                MachiningPathDatas[index].Z += value;
                var homMat2D = ServiceLocator.Current.GetInstance<SettingPageViewModel>().GetPixelToPhysicsHomMat2D(MachiningPathDatas[index].Z, TemplateHelper.PixelToPhysicsHomMat2D);
                HOperatorSet.HomMat2dInvert(homMat2D, out HTuple homMat2DInvert);
                HOperatorSet.AffineTransPoint2d(homMat2DInvert, MachiningPathDatas[index].X, MachiningPathDatas[index].Y, out HTuple row, out HTuple col);
                MachiningPathDatas[index].Row = Math.Round(row.D, 2);
                MachiningPathDatas[index].Column = Math.Round(col.D, 2);
                index++;
            }
            index = MachiningPathDatas.FindIndex(x => x.PoseId == PoseId);
            Coordinate_tb.Text = "[ " + MachiningPathDatas[index].X + ", " + MachiningPathDatas[index].Y + ", " + MachiningPathDatas[index].Z + " ]";
            ClearHelixViewport();
            ModelLists = LoadLocus(MachiningPathDatas);
            ShowPathPoints(ModelLists);
            SetCurrentChoosePoint();
            ShowAxisXYZ(ModelLists);
            TemplateHelper.DisplayImage(CurrentImage, HWindowDisplay2DLocus.HalconWindow, false, LocusParameterSettingModel.IsImageEnhancement);
            var ds = TemplateHelper.GenLocusOfOutPoint(MachiningPathDatas, CurrentTemplateData.OutPointPoseId, CurrentSpokeType);
            TemplateHelper.GenAndShowLocus(HWindowDisplay2DLocus.HalconWindow, ds, PoseId);
            RemoveAxisDisplay();
            ReviseSave(MachiningPathDatas);
        }
        /// <summary>
        /// 姿态单个复位按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SinglePoseReset_Click(object sender, RoutedEventArgs e)
        {
            if (MachiningPathDatas == null || MachiningPathDatas.Count == 0)
            {
                MessageShow("请先加载轨迹！", MessageType.Default);
                return;
            }
            if (MachiningPathDatas[MachiningPathDatas.Count - 1].PoseId / 1000 > CurrentSpokeType)
            {
                MessageShow("请点击《调整轨迹位姿》按钮加载单个窗口轨迹！", MessageType.Default);
                return;
            }
            if (CurrentModelUIElement3D == null)
            {
                MessageShow("未选中轨迹点！", MessageType.Default);
                return;
            }
            //修改数据
            var index = MachiningPathDatas.FindIndex(t => t.PoseId == PoseId);
            if (MachiningPathDatas[index].Q1 == ViewportHelper.W && MachiningPathDatas[index].Q2 == ViewportHelper.X &&
                MachiningPathDatas[index].Q3 == ViewportHelper.Y && MachiningPathDatas[index].Q4 == ViewportHelper.Z)
            {
                return;
            }
            MachiningPathDatas[index].EX = ViewportHelper.EX;
            MachiningPathDatas[index].EY = ViewportHelper.EY;
            MachiningPathDatas[index].EZ = ViewportHelper.EZ;
            MachiningPathDatas[index].Q1 = ViewportHelper.W;
            MachiningPathDatas[index].Q2 = ViewportHelper.X;
            MachiningPathDatas[index].Q3 = ViewportHelper.Y;
            MachiningPathDatas[index].Q4 = ViewportHelper.Z;
            EulerAngles_tb.Text = "[ " + ViewportHelper.EZ + ", " + ViewportHelper.EY + ", " + ViewportHelper.EX + " ]";
            Quaternion_tb.Text = "[ " + ViewportHelper.W + ", " + ViewportHelper.X + ", " + ViewportHelper.Y + ", " + ViewportHelper.Z + " ]";
            //显示
            ClearHelixViewport();
            ModelLists = LoadLocus(MachiningPathDatas);
            ShowPathPoints(ModelLists);
            ShowAxisXYZ(ModelLists);
            CurrentModelUIElement3D = ModelLists[WinNum].CModelUIElement3Ds[PointNum];
            //更改选中模型的材质
            (ModelLists[WinNum].CModelUIElement3Ds[PointNum].Model as GeometryModel3D).Material = NormalMaterial(Brushes.Aquamarine);
            TemplateHelper.DisplayImage(CurrentImage, HWindowDisplay2DLocus.HalconWindow, false, LocusParameterSettingModel.IsImageEnhancement);
            var ds = TemplateHelper.GenLocusOfOutPoint(MachiningPathDatas, CurrentTemplateData.OutPointPoseId, CurrentSpokeType);
            TemplateHelper.GenAndShowLocus(HWindowDisplay2DLocus.HalconWindow, ds, PoseId);
            ReviseSave(MachiningPathDatas);
        }
        /// <summary>
        /// 姿态批量复位按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BatchPoseReset_Click(object sender, RoutedEventArgs e)
        {
            if (MachiningPathDatas == null || MachiningPathDatas.Count == 0)
            {
                MessageShow("请先加载轨迹！", MessageType.Default);
                return;
            }
            if (MachiningPathDatas[MachiningPathDatas.Count - 1].PoseId / 1000 > CurrentSpokeType)
            {
                MessageShow("请点击《调整轨迹位姿》按钮加载单个窗口轨迹！", MessageType.Default);
                return;
            }
            int startPoseValue, endPoseValue;
            try
            {
                startPoseValue = int.Parse(startPose_tb.Text);
                endPoseValue = int.Parse(endPose_tb.Text);
            }
            catch (Exception ex)
            {
                MessageShow("输入错误：" + ex.Message + "。请检查输入的起点值和终点值！", MessageType.Error);
                return;
            }
            int startPoseIndex = MachiningPathDatas.FindIndex(x => x.PoseId == startPoseValue);
            if (startPoseIndex < 0)
            {
                MessageShow("批量调整的起点输入错误，请重新输入！", MessageType.Default);
                return;
            }
            int endPoseIndex = MachiningPathDatas.FindIndex(x => x.PoseId == endPoseValue);
            if (endPoseIndex < 0)
            {
                MessageShow("批量调整的终点输入错误，请重新输入！", MessageType.Default);
                return;
            }
            if (startPoseValue % 1000 > 900 || endPoseValue % 1000 > 900)
            {
                MessageShow("批量调整的起点或终点不能包含进出刀点，请重新输入！", MessageType.Default);
                return;
            }
            int winNum = startPoseValue / 1000;
            WinNum = winNum - 1;
            //如果起始点小于结束点
            if (startPoseValue < endPoseValue)
            {
                int count = endPoseValue - startPoseValue;
                if (count > MachiningPathDatas.Count)
                {
                    MessageShow("批量调整的终点输入错误，请重新输入！", MessageType.Default);
                    return;
                }
                int mIndex = MachiningPathDatas.FindIndex(p => p.PoseId == startPoseValue);
                int c = mIndex + count;
                for (int i = mIndex; i <= c; i++)
                {
                    MachiningPathDatas[i].EX = ViewportHelper.EX;
                    MachiningPathDatas[i].EY = ViewportHelper.EY;
                    MachiningPathDatas[i].EZ = ViewportHelper.EZ;
                    MachiningPathDatas[i].Q1 = ViewportHelper.W;
                    MachiningPathDatas[i].Q2 = ViewportHelper.X;
                    MachiningPathDatas[i].Q3 = ViewportHelper.Y;
                    MachiningPathDatas[i].Q4 = ViewportHelper.Z;
                }
            }
            else
            {
                //获取结束点的索引
                int endIndex = MachiningPathDatas.FindIndex(t => t.PoseId == endPoseValue);
                //起点索引
                int startIndex = MachiningPathDatas.FindIndex(p => p.PoseId == endPoseValue / 1000 * 1000);
                //调整：从轨迹的起点到轨迹的结束点
                for (int i = startIndex; i <= endIndex; i++)
                {
                    MachiningPathDatas[i].EX = ViewportHelper.EX;
                    MachiningPathDatas[i].EY = ViewportHelper.EY;
                    MachiningPathDatas[i].Q1 = ViewportHelper.W;
                    MachiningPathDatas[i].Q2 = ViewportHelper.X;
                    MachiningPathDatas[i].Q3 = ViewportHelper.Y;
                    MachiningPathDatas[i].Q4 = ViewportHelper.Z;
                }
                //调整：输入的起点到轨迹的终点
                //输入的起点索引
                int startIndex1 = MachiningPathDatas.FindIndex(t => t.PoseId == startPoseValue);
                var datas = MachiningPathDatas.Where(t => t.PoseId >= startPoseValue / 1000 * 1000 && t.PoseId <= endPoseValue / 1000 * 1000 + 900).ToList();
                //轨迹的终点索引
                int endIndex1 = MachiningPathDatas.FindIndex(p => p.PoseId == datas[datas.Count - 1].PoseId);
                for (int i = startIndex1; i <= endIndex1; i++)
                {
                    MachiningPathDatas[i].EX = ViewportHelper.EX;
                    MachiningPathDatas[i].EY = ViewportHelper.EY;
                    MachiningPathDatas[i].Q1 = ViewportHelper.W;
                    MachiningPathDatas[i].Q2 = ViewportHelper.X;
                    MachiningPathDatas[i].Q3 = ViewportHelper.Y;
                    MachiningPathDatas[i].Q4 = ViewportHelper.Z;
                }
            }
            if (CurrentModelUIElement3D != null)
            {
                if (PoseId % 1000 >= 901 && PoseId % 1000 <= 903)
                {
                    (ModelLists[WinNum].CModelUIElement3Ds[PointNum].Model as GeometryModel3D).Material = NormalMaterial(Brushes.LawnGreen);
                }
                else if (PoseId % 1000 >= 904 && PoseId % 1000 <= 906)
                {
                    (ModelLists[WinNum].CModelUIElement3Ds[PointNum].Model as GeometryModel3D).Material = NormalMaterial(Brushes.IndianRed);
                }
                else
                {
                    (ModelLists[WinNum].CModelUIElement3Ds[PointNum].Model as GeometryModel3D).Material = NormalMaterial(Brushes.MediumPurple);
                }
            }
            //查找是否存在进出刀点
            var ds = MachiningPathDatas.Where(x => x.PoseId > winNum * 1000 + 900 && x.PoseId <= winNum * 1000 + 999).ToList();
            if (PoseId == -1)
            {
                PoseId = startPoseValue;
                if (ds.Count > 0) PointNum = startPoseValue % 1000 + 3;
                else PointNum = startPoseValue % 1000;
            }
            //如果起始点大于结束点
            else if (startPoseValue > endPoseValue)
            {
                if (PoseId >= endPoseValue + 1 && PoseId <= startPoseValue - 1)
                {
                    PoseId = startPoseValue;
                    if (ds.Count > 0) PointNum = startPoseValue % 1000 + 3;
                    else PointNum = startPoseValue % 1000;
                }
            }
            //如果起始点小于结束点
            else
            {
                if (PoseId <= startPoseValue - 1 || PoseId >= endPoseValue + 1)
                {
                    PoseId = startPoseValue;
                    if (ds.Count > 0) PointNum = startPoseValue % 1000 + 3;
                    else PointNum = startPoseValue % 1000;
                }
            }
            CurrentModelUIElement3D = ModelLists[winNum - 1].CModelUIElement3Ds[PointNum];
            //更改选中模型的材质
            (ModelLists[winNum - 1].CModelUIElement3Ds[PointNum].Model as GeometryModel3D).Material = NormalMaterial(Brushes.Aquamarine);
            LocusPointInformationDisplay(PoseId);
            ClearHelixViewport();
            ModelLists = LoadLocus(MachiningPathDatas);
            ShowPathPoints(ModelLists);
            ShowAxisXYZ(ModelLists);
            TemplateHelper.DisplayImage(CurrentImage, HWindowDisplay2DLocus.HalconWindow, false, LocusParameterSettingModel.IsImageEnhancement);
            var mds = TemplateHelper.GenLocusOfOutPoint(MachiningPathDatas, CurrentTemplateData.OutPointPoseId, CurrentSpokeType);
            TemplateHelper.GenAndShowLocus(HWindowDisplay2DLocus.HalconWindow, mds, PoseId);
            ReviseSave(MachiningPathDatas);
        }
        /// <summary>
        /// 保存加工轨迹按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveProcessingLocus_Click(object sender, RoutedEventArgs e)
        {
            if (WheelTypeChoose_cb.Text == "")
            {
                MessageShow("请选择轮型！", MessageType.Default);
                return;
            }
            if (MachiningPathDatas.Count < 1)
            {
                MessageShow("无轨迹数据，请先加载轨迹！", MessageType.Default);
                return;
            }
            string[] outPointPoseIds = CurrentTemplateData.OutPointPoseId.Split(',');
            int ops = 0;
            for (int i = 0; i < outPointPoseIds.Length; i++)
            {
                int op = int.Parse(outPointPoseIds[i]);
                if (op != 0)
                {
                    ops += CurrentTemplateData.SpokeQuantity * (op + 1);
                }
            }
            if (MachiningPathDatas.Count + ops > TotalTrajectoryPointsAllowed)
            {
                MessageShow($"加工轨迹点数不能大于{TotalTrajectoryPointsAllowed}，请优化轨迹！", MessageType.Default);
                return;
            }
            bool result = UMessageBox.Show("轮型选择确认", "您选择的轮型是：" + CurrentTemplateData.WheelType + " ，请确定轮型选择正确后，再点击确认！");
            if (result)
            {
                //从最后一条数据判断包含多少个窗口
                int count = MachiningPathDatas[MachiningPathDatas.Count - 1].PoseId / 1000;
                //当前轮型模板数据
                var data = ServiceLocator.Current.GetInstance<TemplatePageViewModel>().TemplateDatas.Where(t => t.WheelType == CurrentTemplateData.WheelType).ToList();
                if (data[0].SpokeQuantity > count)
                {
                    MessageShow("未生成所有窗口轨迹，请检查！", MessageType.Error);
                    return;
                }
                var plDB = new SqlAccess().ProcessingLocusDataAccess;
                //判断表是否存在
                var r1 = plDB.DbMaintenance.IsAnyTable(CurrentTemplateData.WheelType, false);
                //如果数据存在,则清空表
                if (r1) plDB.DbMaintenance.TruncateTable(CurrentTemplateData.WheelType);
                //如果不存在,则根据轮型创建表
                else plDB.CodeFirst.As<MachiningPathPosModel>(CurrentTemplateData.WheelType).InitTables<MachiningPathPosModel>();
                //根据表名插入数据
                plDB.Insertable(MachiningPathDatas).AS(CurrentTemplateData.WheelType).ExecuteCommand();
                ServiceLocator.Current.GetInstance<LocusPageViewModel>().LoadProcessingLocus();

                //判断当前加工轨迹是否生成全部进出刀点
                int ioCount = MachiningPathDatas.Where(x => x.PoseId % 1000 > 900).Count();
                if(ioCount != count * 6)
                {
                    //修改加工使能
                    if (CurrentTemplateData.ProcessingEnable) 
                        CurrentTemplateData.ProcessingEnable = false;
                }
                //修改数据库
                var sDB = new SqlAccess().SystemDataAccess;
                sDB.Updateable(CurrentTemplateData).ExecuteCommand();
                //更新保存在内存中的实时修改显示的轮型数据
                ServiceLocator.Current.GetInstance<TemplatePageViewModel>().UpdateWheelDatas(CurrentTemplateData.WheelType);
                //更新加工用的轮型数据
                ServiceLocator.Current.GetInstance<TemplatePageViewModel>().UpdateProcessingWheelDatas();
                try
                {
                    List<MachiningPathPosModel> datas = new List<MachiningPathPosModel>();
                    //从轨迹数据中生成原始窗口轮廓
                    for (int i = 0; i < MachiningPathDatas.Count; i++)
                    {
                        if (MachiningPathDatas[i].PoseId % 1000 <= 900) datas.Add(MachiningPathDatas[i]);
                    }
                    HObject processingContours = TemplateHelper.GenContours(datas, false);
                    HOperatorSet.WriteContourXldDxf(processingContours, "d:/Deburr/DXF文件/" + data[0].WheelType + ".dxf");
                }
                catch { }
                MessageShow("加工轨迹保存成功，轮型是：" + WheelTypeChoose_cb.Text, MessageType.Success);
            }
        }
        /// <summary>
        /// 加工路径反转按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MachiningPathReversal_Click(object sender, RoutedEventArgs e)
        {
            if (MachiningPathDatas == null || MachiningPathDatas.Count == 0)
            {
                MessageShow("请先加载轨迹！", MessageType.Default);
                return;
            }
            if (MachiningPathDatas[MachiningPathDatas.Count - 1].PoseId / 1000 > CurrentSpokeType)
            {
                MessageShow("请点击《调整轨迹位姿》按钮加载单个窗口轨迹！", MessageType.Default);
                return;
            }
            if (CurrentModelUIElement3D == null)
            {
                MessageShow("未选中轨迹点！", MessageType.Default);
                return;
            }
            var datas = MachiningPathDatas.Where(x => x.PoseId > (WinNum + 1) * 1000 + 900 && x.PoseId <= (WinNum + 1) * 1000 + 999).ToList();
            if (datas.Count > 0)
            {
                MessageShow("请先删除进出刀点后再反转加工路径！", MessageType.Default);
                return;
            }
            List<MachiningPathPosModel> winDatas = MachiningPathDatas.Where(x1 => x1.PoseId >= (WinNum + 1) * 1000 && x1.PoseId <= (WinNum + 1) * 1000 + 900).ToList();
            int startIndex = MachiningPathDatas.FindIndex(x => x.Id == winDatas[0].Id);
            int startPoseId = winDatas[0].PoseId;
            int endIndex = winDatas.FindIndex(x => x.Id == winDatas[winDatas.Count - 1].Id);
            for (int j = 0; j < winDatas.Count; j++)
            {
                MachiningPathDatas[startIndex] = winDatas[endIndex];
                MachiningPathDatas[startIndex].Id = startIndex + 1;
                MachiningPathDatas[startIndex].PoseId = startPoseId;
                startPoseId++;
                startIndex++;
                endIndex--;
            }
            ClearHelixViewport();
            ModelLists = LoadLocus(MachiningPathDatas);
            ShowPathPoints(ModelLists);
            CurrentModelUIElement3D = ModelLists[WinNum].CModelUIElement3Ds[PointNum];
            //更改选中模型的材质
            (ModelLists[WinNum].CModelUIElement3Ds[PointNum].Model as GeometryModel3D).Material = NormalMaterial(Brushes.Aquamarine);
            //LocusPointInformationDisplay(WinNum, (WinNum + 1) * 1000);
            TemplateHelper.DisplayImage(CurrentImage, HWindowDisplay2DLocus.HalconWindow, false, LocusParameterSettingModel.IsImageEnhancement);
            var ds = TemplateHelper.GenLocusOfOutPoint(MachiningPathDatas, CurrentTemplateData.OutPointPoseId, CurrentSpokeType);
            TemplateHelper.GenAndShowLocus(HWindowDisplay2DLocus.HalconWindow, ds, PoseId);
        }
        /// <summary>
        /// 设置起点按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetStartPoint_Click(object sender, RoutedEventArgs e)
        {
            if (MachiningPathDatas == null || MachiningPathDatas.Count == 0)
            {
                MessageShow("请先加载轨迹！", MessageType.Default);
                return;
            }
            if (MachiningPathDatas[MachiningPathDatas.Count - 1].PoseId / 1000 > CurrentSpokeType)
            {
                MessageShow("请点击《调整轨迹位姿》按钮加载单个窗口轨迹！", MessageType.Default);
                return;
            }
            if (CurrentModelUIElement3D == null)
            {
                MessageShow("未选中轨迹点！", MessageType.Default);
                return;
            }
            var p4 = MachiningPathDatas.Where(x1 => x1.PoseId >= PoseId / 1000 * 1000 + 901 && x1.PoseId <= PoseId / 1000 * 1000 + 906).ToList();
            if (p4.Count > 0)
            {
                MessageShow("请先删除进出刀点再设置起点！", MessageType.Default);
                return;
            }
            //通过最后一条数据的PoseId获取窗口轮廓的总数
            int winIndex = -1;
            var contourTotal = MachiningPathDatas[MachiningPathDatas.Count - 1].PoseId / 1000;
            //计算当前类型的窗口索引
            if (contourTotal == CurrentTemplateData.SpokeQuantity * CurrentSpokeType) winIndex = WinNum % CurrentSpokeType;
            else winIndex = WinNum % CurrentSpokeType;

            for (int i = winIndex; i < contourTotal; i += CurrentSpokeType)
            {
                var newDatas = new List<MachiningPathPosModel>();
                //构建新的排序
                //通过PoseId获取第i+1个窗口编号0-900的轨迹数据
                var points = MachiningPathDatas.Where(x => x.PoseId >= (i + 1) * 1000 && x.PoseId <= (i + 1) * 1000 + 900).ToList();
                var newId = points[0].Id;
                var newPoseId = points[0].PoseId;
                //选中的新起点到最后一个点的排序
                for (int k = PointNum; k < points.Count; k++)
                {
                    var newData = points[k];
                    //构建新的Id和PoseId
                    newData.Id = newId;
                    newData.PoseId = newPoseId;
                    newDatas.Add(newData);
                    newId++;
                    newPoseId++;
                }
                //旧起点到新起点的前一个点的排序
                for (int j = 0; j < points.Count - (points.Count - PointNum); j++)
                {
                    var newData = points[j];
                    newData.Id = newId;
                    newData.PoseId = newPoseId;
                    newDatas.Add(newData);
                    newId++;
                    newPoseId++;
                }
                //更新加工路径中的数据
                for (int f = 0; f < newDatas.Count; f++)
                {
                    MachiningPathDatas[newDatas[f].Id - 1] = newDatas[f];
                }
            }
            ClearHelixViewport();
            ModelLists = LoadLocus(MachiningPathDatas);
            ShowPathPoints(ModelLists);
            PointNum = 0;
            SetCurrentChoosePoint();
            var index = MachiningPathDatas.FindIndex(x => x.PoseId == (WinNum + 1) * 1000);
            LocusPointInformationDisplay(MachiningPathDatas[index].PoseId);
            TemplateHelper.DisplayImage(CurrentImage, HWindowDisplay2DLocus.HalconWindow, false, LocusParameterSettingModel.IsImageEnhancement);
            var ds = TemplateHelper.GenLocusOfOutPoint(MachiningPathDatas, CurrentTemplateData.OutPointPoseId, CurrentSpokeType);
            TemplateHelper.GenAndShowLocus(HWindowDisplay2DLocus.HalconWindow, ds, PoseId);
        }
        /// <summary>
        /// 生成进刀点按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GenEntryPoint_Click(object sender, RoutedEventArgs e)
        {
            #region=============条件判断============
            if (MachiningPathDatas == null || MachiningPathDatas.Count == 0)
            {
                MessageShow("请先加载轨迹！", MessageType.Default);
                return;
            }
            if (MachiningPathDatas[MachiningPathDatas.Count - 1].PoseId / 1000 > CurrentSpokeType)
            {
                MessageShow("请点击《调整轨迹位姿》按钮加载单个窗口轨迹！", MessageType.Default);
                return;
            }
            if (CurrentModelUIElement3D == null)
            {
                MessageShow("未选中轨迹点！", MessageType.Default);
                return;
            }
            int windowNumber = PoseId / 1000;
            //判断是否已生成进刀点：通过PoseId获取901 - 903之间的轨迹数据，此段数据为进刀点数据
            var p = MachiningPathDatas.Where(x => x.PoseId >= windowNumber * 1000 + 901 && x.PoseId <= windowNumber * 1000 + 903).ToList();
            if (p.Count > 0)
            {
                MessageShow("进刀点已生成，请勿重复生成！", MessageType.Default);
                return;
            }
            #endregion
            #region=============生成点============
            //通过PoseId获取当前选中窗口的轨迹数据
            var points = MachiningPathDatas.Where(x1 => x1.PoseId >= windowNumber * 1000 && x1.PoseId <= windowNumber * 1000 + 900).ToList();
            //1.=============生成第3号进刀点p903，由起点偏移生成
            MachiningPathPosModel p903 = new MachiningPathPosModel();
            p903.Id = 9999;
            p903.PoseId = PoseId / 1000 * 1000 + 903;
            p903.Row = points[0].Row + EntryPointXAxisOffsetDistance;
            p903.Column = points[0].Column + EntryPointYAxisOffsetDistance;
            p903.Z = points[0].Z;
            //获取指定高度下的新齐次变换矩阵
            var homMat2D3 = ServiceLocator.Current.GetInstance<SettingPageViewModel>().GetPixelToPhysicsHomMat2D(p903.Z, TemplateHelper.PixelToPhysicsHomMat2D);
            HOperatorSet.AffineTransPoint2d(homMat2D3, p903.Row, p903.Column, out HTuple qx3, out HTuple qy3);
            p903.X = Math.Round(qx3.D, 2);
            p903.Y = Math.Round(qy3.D, 2);
            p903.EX = points[0].EX;
            p903.EY = points[0].EY;
            p903.EZ = points[0].EZ;
            p903.Q1 = points[0].Q1;
            p903.Q2 = points[0].Q2;
            p903.Q3 = points[0].Q3;
            p903.Q4 = points[0].Q4;
            //2.=============生成第2号进刀点，由第3号进刀点偏移生成
            MachiningPathPosModel p902 = new MachiningPathPosModel();
            p902.Id = 9999;
            p902.PoseId = PoseId / 1000 * 1000 + 902;
            p902.Row = p903.Row + EntryPointXAxisOffsetDistance;
            p902.Column = p903.Column + EntryPointYAxisOffsetDistance;
            p902.Z = p903.Z;
            var homMat2D2 = ServiceLocator.Current.GetInstance<SettingPageViewModel>().GetPixelToPhysicsHomMat2D(p902.Z, TemplateHelper.PixelToPhysicsHomMat2D);
            HOperatorSet.AffineTransPoint2d(homMat2D2, p902.Row, p902.Column, out HTuple qx2, out HTuple qy2);
            p902.X = Math.Round(qx2.D, 2);
            p902.Y = Math.Round(qy2.D, 2);
            p902.EX = p903.EX;
            p902.EY = p903.EY;
            p902.EZ = p903.EZ;
            p902.Q1 = p903.Q1;
            p902.Q2 = p903.Q2;
            p902.Q3 = p903.Q3;
            p902.Q4 = p903.Q4;
            //3.=============生成第1号进刀点，由第2号进刀点偏移高度生成
            MachiningPathPosModel p901 = new MachiningPathPosModel();
            p901.Id = 9999;
            p901.PoseId = PoseId / 1000 * 1000 + 901;
            p901.Row = p902.Row;
            p901.Column = p902.Column;
            p901.Z = p902.Z + EntryExitPointOffsetHeight;
            var homMat2D1 = ServiceLocator.Current.GetInstance<SettingPageViewModel>().GetPixelToPhysicsHomMat2D(p901.Z, TemplateHelper.PixelToPhysicsHomMat2D);
            HOperatorSet.AffineTransPoint2d(homMat2D1, p901.Row, p901.Column, out HTuple qx1, out HTuple qy1);
            p901.X = Math.Round(qx1.D, 2);
            p901.Y = Math.Round(qy1.D, 2);
            p901.EX = Math.Round(ViewportHelper.EX, 2);
            p901.EY = Math.Round(ViewportHelper.EY, 2);
            p901.EZ = p902.EZ;
            var q = ViewportHelper.EulerToQuaternion(p901.EX, p901.EY, p901.EZ);
            p901.Q1 = q.W;
            p901.Q2 = q.X;
            p901.Q3 = q.Y;
            p901.Q4 = q.Z;
            #endregion
            #region=============修改数据============
            //获取进刀点的插入位置
            var index1 = MachiningPathDatas.FindIndex(x => x.Id == points[0].Id);
            //插入进刀点
            MachiningPathDatas.Insert(index1, p901);
            MachiningPathDatas.Insert(index1 + 1, p902);
            MachiningPathDatas.Insert(index1 + 2, p903);
            //修改id
            for (int i = 0; i < MachiningPathDatas.Count; i++)
            {
                MachiningPathDatas[i].Id = i + 1;
            }
            #endregion
            #region=============显示============
            ClearHelixViewport();
            ModelLists = LoadLocus(MachiningPathDatas);
            SetCurrentChoosePoint();
            ShowPathPoints(ModelLists);
            TemplateHelper.DisplayImage(CurrentImage, HWindowDisplay2DLocus.HalconWindow, false, LocusParameterSettingModel.IsImageEnhancement);
            var ds = TemplateHelper.GenLocusOfOutPoint(MachiningPathDatas, CurrentTemplateData.OutPointPoseId, CurrentSpokeType);
            TemplateHelper.GenAndShowLocus(HWindowDisplay2DLocus.HalconWindow, ds, PoseId);
            #endregion
        }
        /// <summary>
        /// 生成出刀点按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GenExitPoint_Click(object sender, RoutedEventArgs e)
        {
            #region=============条件判断============
            if (MachiningPathDatas == null || MachiningPathDatas.Count == 0)
            {
                MessageShow("请先加载轨迹！", MessageType.Default);
                return;
            }
            if (MachiningPathDatas[MachiningPathDatas.Count - 1].PoseId / 1000 > CurrentSpokeType)
            {
                MessageShow("请点击《调整轨迹位姿》按钮加载单个窗口轨迹！", MessageType.Default);
                return;
            }
            if (CurrentModelUIElement3D == null)
            {
                MessageShow("未选中轨迹点！", MessageType.Default);
                return;
            }
            int windowNumber = PoseId / 1000;
            //判断出刀点是否已生成：通过PoseId获取904 - 906之间的轨迹数据，此段数据为进刀点数据
            var p = MachiningPathDatas.Where(x => x.PoseId >= windowNumber * 1000 + 904 && x.PoseId <= windowNumber * 1000 + 906).ToList();
            if (p.Count > 0)
            {
                MessageShow("出刀点已生成，请勿重复生成！", MessageType.Default);
                return;
            }
            #endregion
            #region=============生成点============
            //通过PoseId获取当前选中窗口的轨迹数据
            var points = MachiningPathDatas.Where(x1 => x1.PoseId >= windowNumber * 1000 && x1.PoseId <= windowNumber * 1000 + 900).ToList();
            //生成出刀点的参照点的索引
            int index = -1;
            if (CircleChoose_cb.Text == "1圈")
            {
                index = points.FindIndex(x => x.PoseId == points[points.Count - 1].PoseId);
            }
            else if (CircleChoose_cb.Text == "1.5圈")
            {
                if (PoseId % 1000 >= 901 && PoseId % 1000 <= 903)
                {
                    MessageShow("不能选择进刀点为出刀点的参照点，请重新选择！", MessageType.Default);
                    return;
                }
                if (PoseId % 1000 == 0)
                {
                    MessageShow("不能选择起点为出刀点的参照点，请重新选择！", MessageType.Default);
                    return;
                }
                if (PoseId % 1000 == points[points.Count - 1].PoseId % 1000)
                {
                    MessageShow("不能选择终点为出刀点的参照点，请重新选择！", MessageType.Default);
                    return;
                }
                index = points.FindIndex(x => x.PoseId == PoseId);
            }
            //1.=============生成第1号出刀点904，由选中的点偏移生成
            MachiningPathPosModel p904 = new MachiningPathPosModel();
            p904.Id = 9999;
            p904.PoseId = windowNumber * 1000 + 904;
            p904.Row = points[index].Row + ExitPointXAxisOffsetDistance;
            p904.Column = points[index].Column + ExitPointYAxisOffsetDistance;
            p904.Z = points[index].Z;
            var homMat2D4 = ServiceLocator.Current.GetInstance<SettingPageViewModel>().GetPixelToPhysicsHomMat2D(p904.Z, TemplateHelper.PixelToPhysicsHomMat2D);
            HOperatorSet.AffineTransPoint2d(homMat2D4, p904.Row, p904.Column, out HTuple qx4, out HTuple qy4);
            p904.X = Math.Round(qx4.D, 2);
            p904.Y = Math.Round(qy4.D, 2);
            p904.EX = points[index].EX;
            p904.EY = points[index].EY;
            p904.EZ = points[index].EZ;
            p904.Q1 = points[index].Q1;
            p904.Q2 = points[index].Q2;
            p904.Q3 = points[index].Q3;
            p904.Q4 = points[index].Q4;
            //2.=============生成第2号出刀点p905，由第1号出刀点904偏移生成
            MachiningPathPosModel p905 = new MachiningPathPosModel();
            p905.Id = 9999;
            p905.PoseId = PoseId / 1000 * 1000 + 905;
            p905.Row = p904.Row + ExitPointXAxisOffsetDistance;
            p905.Column = p904.Column + ExitPointYAxisOffsetDistance;
            p905.Z = p904.Z;
            var homMat2D5 = ServiceLocator.Current.GetInstance<SettingPageViewModel>().GetPixelToPhysicsHomMat2D(p905.Z, TemplateHelper.PixelToPhysicsHomMat2D);
            HOperatorSet.AffineTransPoint2d(homMat2D5, p905.Row, p905.Column, out HTuple qx5, out HTuple qy5);
            p905.X = Math.Round(qx5.D, 2);
            p905.Y = Math.Round(qy5.D, 2);
            p905.EX = p904.EX;
            p905.EY = p904.EY;
            p905.EZ = p904.EZ;
            p905.Q1 = p904.Q1;
            p905.Q2 = p904.Q2;
            p905.Q3 = p904.Q3;
            p905.Q4 = p904.Q4;
            //3.=============生成第3号出刀点p906，由第2号出刀点p905偏移高度生成
            MachiningPathPosModel p906 = new MachiningPathPosModel();
            p906.Id = 9999;
            p906.PoseId = PoseId / 1000 * 1000 + 906;
            p906.Row = p905.Row;
            p906.Column = p905.Column;
            p906.Z = p905.Z + EntryExitPointOffsetHeight;
            var homMat2D6 = ServiceLocator.Current.GetInstance<SettingPageViewModel>().GetPixelToPhysicsHomMat2D(p906.Z, TemplateHelper.PixelToPhysicsHomMat2D);
            HOperatorSet.AffineTransPoint2d(homMat2D5, p906.Row, p906.Column, out HTuple qx6, out HTuple qy6);
            p906.X = Math.Round(qx6.D, 2);
            p906.Y = Math.Round(qy6.D, 2);
            p906.EX = Math.Round(ViewportHelper.EX, 2);
            p906.EY = Math.Round(ViewportHelper.EY, 2);
            p906.EZ = p905.EZ;
            var q = ViewportHelper.EulerToQuaternion(p906.EX, p906.EY, p906.EZ);
            p906.Q1 = q.W;
            p906.Q2 = q.X;
            p906.Q3 = q.Y;
            p906.Q4 = q.Z;
            #endregion
            #region=============修改数据============
            //获取出刀点的插入位置
            int index1 = MachiningPathDatas.FindIndex(x => x.Id == points[points.Count - 1].Id);
            //插入出刀点
            MachiningPathDatas.Insert(index1 + 1, p904);
            MachiningPathDatas.Insert(index1 + 2, p905);
            MachiningPathDatas.Insert(index1 + 3, p906);
            //修改id
            for (int i = 0; i < MachiningPathDatas.Count; i++)
            {
                MachiningPathDatas[i].Id = i + 1;
            }
            //修改当前窗口的出刀点
            string[] outPointPoseIds = CurrentTemplateData.OutPointPoseId.Split(',');
            outPointPoseIds[windowNumber - 1] = (points[index].PoseId % 1000).ToString();
            string str = string.Join(",", outPointPoseIds);
            CurrentTemplateData.OutPointPoseId = str;
            #endregion
            #region=============显示============
            ClearHelixViewport();
            ModelLists = LoadLocus(MachiningPathDatas);
            SetCurrentChoosePoint();
            ShowPathPoints(ModelLists);
            TemplateHelper.DisplayImage(CurrentImage, HWindowDisplay2DLocus.HalconWindow, false, LocusParameterSettingModel.IsImageEnhancement);
            var ds = TemplateHelper.GenLocusOfOutPoint(MachiningPathDatas, CurrentTemplateData.OutPointPoseId, CurrentSpokeType);
            TemplateHelper.GenAndShowLocus(HWindowDisplay2DLocus.HalconWindow, ds, PoseId);
            #endregion
        }
        /// <summary>
        /// 删除进刀点按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DelEntryPoint_Click(object sender, RoutedEventArgs e)
        {
            if (MachiningPathDatas == null || MachiningPathDatas.Count == 0)
            {
                MessageShow("请先加载轨迹！", MessageType.Default);
                return;
            }
            if (MachiningPathDatas[MachiningPathDatas.Count - 1].PoseId / 1000 > CurrentSpokeType)
            {
                MessageShow("请点击《调整轨迹位姿》按钮加载单个窗口轨迹！", MessageType.Default);
                return;
            }
            if (CurrentModelUIElement3D == null)
            {
                MessageShow("未选中轨迹点！", MessageType.Default);
                return;
            }
            if (WheelTypeChoose_cb.SelectedItem == null)
            {
                MessageShow("轮型选择为空，请选择轮型！", MessageType.Default);
                return;
            }
            //判断是否已生成进刀点：通过PoseId获取窗口的编号为901 - 903之间的轨迹数据，此段数据为进刀点数据
            var p = MachiningPathDatas.Where(x => x.PoseId >= PoseId / 1000 * 1000 + 901 && x.PoseId <= 1000 * 1000 + 903).ToList();
            if (p.Count == 0) return;
            List<MachiningPathPosModel> m = new List<MachiningPathPosModel>();
            int startPoseId = PoseId / 1000 * 1000;
            for (int i = 0; i < MachiningPathDatas.Count; i++)
            {
                if (MachiningPathDatas[i].PoseId >= startPoseId + 901 && MachiningPathDatas[i].PoseId <= startPoseId + 903) { }
                else m.Add(MachiningPathDatas[i]);
            }
            for (int i = 0; i < m.Count; i++)
            {
                m[i].Id = i + 1;
            }
            MachiningPathDatas.Clear();
            MachiningPathDatas = m;
            ClearHelixViewport();
            ModelLists = LoadLocus(MachiningPathDatas);
            ShowPathPoints(ModelLists);
            SetCurrentChoosePoint();
            TemplateHelper.DisplayImage(CurrentImage, HWindowDisplay2DLocus.HalconWindow, false, LocusParameterSettingModel.IsImageEnhancement);
            var ds = TemplateHelper.GenLocusOfOutPoint(MachiningPathDatas, CurrentTemplateData.OutPointPoseId, CurrentSpokeType);
            TemplateHelper.GenAndShowLocus(HWindowDisplay2DLocus.HalconWindow, ds, PoseId);
        }
        /// <summary>
        /// 删除出刀点按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DelExitPoint_Click(object sender, RoutedEventArgs e)
        {
            if (MachiningPathDatas == null || MachiningPathDatas.Count == 0)
            {
                MessageShow("请先加载轨迹！", MessageType.Default);
                return;
            }
            if (MachiningPathDatas[MachiningPathDatas.Count - 1].PoseId / 1000 > CurrentSpokeType)
            {
                MessageShow("请点击《调整轨迹位姿》按钮加载单个窗口轨迹！", MessageType.Default);
                return;
            }
            if (CurrentModelUIElement3D == null)
            {
                MessageShow("未选中轨迹点！", MessageType.Default);
                return;
            }
            if (WheelTypeChoose_cb.SelectedItem == null)
            {
                MessageShow("轮型选择为空，请选择轮型！", MessageType.Default);
                return;
            }
            //判断是否已生成出刀点：通过PoseId获取窗口的编号为904 - 906之间的轨迹数据，此段数据为出刀点数据
            var p = MachiningPathDatas.Where(x => x.PoseId >= PoseId / 1000 * 1000 + 904 && x.PoseId <= 1000 * 1000 + 9036).ToList();
            if (p.Count == 0) return;
            List<MachiningPathPosModel> m = new List<MachiningPathPosModel>();
            int startPoseId = PoseId / 1000 * 1000;
            for (int i = 0; i < MachiningPathDatas.Count; i++)
            {
                if (MachiningPathDatas[i].PoseId >= startPoseId + 904 && MachiningPathDatas[i].PoseId <= startPoseId + 906) { }
                else m.Add(MachiningPathDatas[i]);
            }
            for (int i = 0; i < m.Count; i++)
            {
                m[i].Id = i + 1;
            }

            int windowNumber = PoseId / 1000;
            //修改当前窗口的出刀点
            var outPointPoseIds = CurrentTemplateData.OutPointPoseId.Split(',');
            outPointPoseIds[windowNumber - 1] = "0";
            string str = string.Join(",", outPointPoseIds);
            CurrentTemplateData.OutPointPoseId = str;

            MachiningPathDatas.Clear();
            MachiningPathDatas = m;
            ClearHelixViewport();
            ModelLists = LoadLocus(MachiningPathDatas);
            ShowPathPoints(ModelLists);
            SetCurrentChoosePoint();
            TemplateHelper.DisplayImage(CurrentImage, HWindowDisplay2DLocus.HalconWindow, false, LocusParameterSettingModel.IsImageEnhancement);
            var ds = TemplateHelper.GenLocusOfOutPoint(MachiningPathDatas, CurrentTemplateData.OutPointPoseId, CurrentSpokeType);
            TemplateHelper.GenAndShowLocus(HWindowDisplay2DLocus.HalconWindow, ds, PoseId);
        }
        /// <summary>
        /// 删除进出刀点按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteEntryExitPoint_Click(object sender, RoutedEventArgs e)
        {
            if (WheelTypeChoose_cb.SelectedItem == null)
            {
                MessageShow("轮型选择为空，请选择轮型！", MessageType.Default);
                return;
            }
            if (MachiningPathDatas == null || MachiningPathDatas.Count == 0)
            {
                MessageShow("请先加载轨迹！", MessageType.Default);
                return;
            }
            //判断是否已生成进出刀点：通过PoseId获取第1个窗口的编号为900 - 999之间的轨迹数据，此段数据为进出刀点数据
            var p = MachiningPathDatas.Where(x1 => x1.PoseId % 1000 > 900).ToList();
            if (p.Count == 0) return;

            List<MachiningPathPosModel> m = new List<MachiningPathPosModel>();
            for (int i = 0; i < MachiningPathDatas.Count; i++)
            {
                if (MachiningPathDatas[i].PoseId % 1000 <= 900)
                {
                    m.Add(MachiningPathDatas[i]);
                }
            }
            for (int i = 0; i < m.Count; i++)
            {
                m[i].Id = i + 1;
            }

            int windowNumber = PoseId / 1000;
            //修改出刀点
            CurrentTemplateData.OutPointPoseId = "0,0,0,0,0,0,0,0,0,0";

            MachiningPathDatas.Clear();
            MachiningPathDatas = m;
            ClearHelixViewport();
            ModelLists = LoadLocus(MachiningPathDatas);
            ShowPathPoints(ModelLists);
            SetCurrentChoosePoint();
            TemplateHelper.DisplayImage(CurrentImage, HWindowDisplay2DLocus.HalconWindow, false, LocusParameterSettingModel.IsImageEnhancement);
            var ds = TemplateHelper.GenLocusOfOutPoint(MachiningPathDatas, CurrentTemplateData.OutPointPoseId, CurrentSpokeType);
            TemplateHelper.GenAndShowLocus(HWindowDisplay2DLocus.HalconWindow, ds, PoseId);
        }
        /// <summary>
        /// 删除轨迹点按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteChoosed_Click(object sender, RoutedEventArgs e)
        {
            if (MachiningPathDatas == null || MachiningPathDatas.Count == 0)
            {
                MessageShow("请先加载轨迹！", MessageType.Default);
                return;
            }
            if (MachiningPathDatas[MachiningPathDatas.Count - 1].PoseId / 1000 > CurrentSpokeType)
            {
                MessageShow("请点击《调整轨迹位姿》按钮加载单个窗口轨迹！", MessageType.Default);
                return;
            }
            if (CurrentModelUIElement3D == null)
            {
                MessageShow("未选中轨迹点！", MessageType.Default);
                return;
            }
            if (PoseId % 1000 > 900)
            {
                MessageShow("请使用【删除进出刀点】按钮删除！", MessageType.Default);
                return;
            }
            List<MachiningPathPosModel> inPoints = MachiningPathDatas.Where(x => x.PoseId >= (WinNum + 1) * 1000 + 901 && x.PoseId <= (WinNum + 1) * 1000 + 903).ToList();
            List<MachiningPathPosModel> outPoints = MachiningPathDatas.Where(x => x.PoseId >= (WinNum + 1) * 1000 + 904 && x.PoseId <= (WinNum + 1) * 1000 + 906).ToList();
            string[] outPointPoseIds = CurrentTemplateData.OutPointPoseId.Split(',');
            if (PoseId % 1000 == int.Parse(outPointPoseIds[WinNum]) && outPoints.Count > 0)
            {
                MessageShow("删除出刀参照点前请先删除出刀点！", MessageType.Default);
                return;
            }
            if (PoseId % 1000 == 0 && inPoints.Count > 0)
            {
                MessageShow("删除起点前请先删除进刀点！", MessageType.Default);
                return;
            }
            List<MachiningPathPosModel> ps = MachiningPathDatas.Where(x => x.PoseId >= (WinNum + 1) * 1000 && x.PoseId <= (WinNum + 1) * 1000 + 900).ToList();

            int winIndex = -1;
            //通过最后一条数据的PoseId获取窗口轮廓的总数
            int contourTotal = MachiningPathDatas[MachiningPathDatas.Count - 1].PoseId / 1000;
            //计算当前类型的窗口索引
            if (contourTotal == CurrentTemplateData.SpokeQuantity * CurrentSpokeType)
                winIndex = WinNum % CurrentSpokeType;
            else 
                winIndex = WinNum % CurrentSpokeType;
            //判断当前窗口的轨迹点是否少于20
            if (ModelLists[winIndex].CModelUIElement3Ds.Count == 20)
            {
                MessageShow("轨迹点不能少于20个！", MessageType.Default);
                return;
            }
            DeleteLocusPoint.IsEnabled = false;

            //从加载的模型窗口中移除选中的所有对应类型窗口对应位置的模型
            for (int i = winIndex; i < contourTotal; i += CurrentSpokeType)
            {
                //移除显示的3D模型
                int indexC = HelixViewport.Children.IndexOf(ModelLists[i].CModelUIElement3Ds[PointNum]);
                if (indexC > 2) HelixViewport.Children.RemoveAt(indexC);
                int indexX = HelixViewport.Children.IndexOf(ModelLists[i].XModelUIElement3Ds[PointNum]);
                if (indexX > 2) HelixViewport.Children.RemoveAt(indexX);
                int indexY = HelixViewport.Children.IndexOf(ModelLists[i].YModelUIElement3Ds[PointNum]);
                if (indexY > 2) HelixViewport.Children.RemoveAt(indexY);
                int indexZ = HelixViewport.Children.IndexOf(ModelLists[i].ZModelUIElement3Ds[PointNum]);
                if (indexZ > 2) HelixViewport.Children.RemoveAt(indexZ);
                //删除当前模型的事件
                ModelLists[i].CModelUIElement3Ds[PointNum].MouseLeftButtonDown -= Mui3D_MouseLeftButtonDown;
                //移除当前选中点的模型
                ModelLists[i].CModelUIElement3Ds.RemoveAt(PointNum);
                //如果移除的是起始点，修改起始点模型
                if (PointNum == 0)
                {
                    (ModelLists[i].CModelUIElement3Ds[PointNum].Model as GeometryModel3D).Geometry = CoordinateSystemGM3D_R[0].Geometry;
                    ((ModelLists[i].CModelUIElement3Ds[PointNum].Transform as Transform3DGroup).Children[0] as ScaleTransform3D).ScaleX = 0.25;
                    ((ModelLists[i].CModelUIElement3Ds[PointNum].Transform as Transform3DGroup).Children[0] as ScaleTransform3D).ScaleY = 0.25;
                    ((ModelLists[i].CModelUIElement3Ds[PointNum].Transform as Transform3DGroup).Children[0] as ScaleTransform3D).ScaleZ = 0.25;
                }
                ModelLists[i].XModelUIElement3Ds.RemoveAt(PointNum);
                ModelLists[i].YModelUIElement3Ds.RemoveAt(PointNum);
                ModelLists[i].ZModelUIElement3Ds.RemoveAt(PointNum);
                //根据PoseId找到当前点在MachiningPathDatas中的索引
                int index = MachiningPathDatas.FindIndex(x => x.PoseId == PoseId);
                //删除最终加工路径中的数据
                MachiningPathDatas.RemoveAt(index);
            }
            //=======================重构MachiningPathDatas数据中的Id,PoseId============================
            //第一步：修改PoseId
            for (int i = winIndex; i < contourTotal; i += CurrentSpokeType)
            {
                //获取删除点窗口的数据
                List<MachiningPathPosModel> points = MachiningPathDatas.Where(x => x.PoseId >= (i + 1) * 1000 && x.PoseId < (i + 1) * 1000 + 900).ToList();
                //根据PoseId找到当前点在MachiningPathDatas中的索引
                int poseId = points[0].PoseId;
                int index = MachiningPathDatas.FindIndex(x => x.PoseId == poseId);
                //如果删除的是起点
                if (poseId % 1000 != 0)
                {
                    poseId = poseId / 1000 * 1000;
                }
                for (int j = 0; j < points.Count; j++)
                {
                    MachiningPathDatas[index].PoseId = poseId + j;
                    index++;
                }
            }
            //第二步：修改Id
            for (int i = 0; i < MachiningPathDatas.Count; i++)
            {
                MachiningPathDatas[i].Id = i + 1;
            }

            //修改当前窗口的出刀点
            if (PoseId % 1000 <= int.Parse(outPointPoseIds[WinNum]))
            {
                outPointPoseIds[WinNum] = (int.Parse(outPointPoseIds[WinNum]) - 1).ToString();
                string str = string.Join(",", outPointPoseIds);
                CurrentTemplateData.OutPointPoseId = str;
            }
            //更改选中模型
            if (PointNum == ModelLists[WinNum].CModelUIElement3Ds.Count)
            {
                PointNum--;
                PoseId--;
            }
            SetCurrentChoosePoint();

            CModel3DIndex = HelixViewport.Children.IndexOf(ModelLists[WinNum].CModelUIElement3Ds[PointNum]);
            XModel3DIndex = HelixViewport.Children.IndexOf(ModelLists[WinNum].XModelUIElement3Ds[PointNum]);
            YModel3DIndex = HelixViewport.Children.IndexOf(ModelLists[WinNum].YModelUIElement3Ds[PointNum]);
            ZModel3DIndex = HelixViewport.Children.IndexOf(ModelLists[WinNum].ZModelUIElement3Ds[PointNum]);

            LocusPointInformationDisplay(PoseId);
            TemplateHelper.DisplayImage(CurrentImage, HWindowDisplay2DLocus.HalconWindow, false, LocusParameterSettingModel.IsImageEnhancement);

            List<MachiningPathPosModel> ds = TemplateHelper.GenLocusOfOutPoint(MachiningPathDatas, CurrentTemplateData.OutPointPoseId, CurrentSpokeType);
            TemplateHelper.GenAndShowLocus(HWindowDisplay2DLocus.HalconWindow, ds, PoseId);
            ReviseSave(MachiningPathDatas);
            DeleteLocusPoint.IsEnabled = true;
        }
        /// <summary>
        /// 删除源轨迹按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteSourceLocus_Click(object sender, RoutedEventArgs e)
        {
            if (WheelTypeChoose_cb.SelectedItem == null)
            {
                MessageShow("轮型选择为空，请选择轮型！", MessageType.Default);
                return;
            }
            bool result = UMessageBox.Show("删除源轨迹确认", "您选择的轮型是：" + WheelTypeChoose_cb.SelectedItem + " ，请确定轮型选择正确后，再点击确认！");
            if (result)
            {
                var wheelType = WheelTypeChoose_cb.SelectedItem.ToString();
                //根据轮型删除对应轮型的源轨迹数据
                var slDB = new SqlAccess().SourceLocusDataAccess;
                //判断对应轮型的源轨迹数据表是否存在
                var r = slDB.DbMaintenance.IsAnyTable(wheelType, false);
                if (r)
                {
                    var rr = slDB.DbMaintenance.DropTable(wheelType);
                    if (rr) MessageShow("轮型" + wheelType + "的源轨迹数据删除成功！", MessageType.Success);
                    //====================删除内存中的源轨迹数据=======================
                    var index = TemplatePageViewModel.SourceLocusDatas.LocusName.FindIndex(x => x == wheelType);
                    if (index >= 0)
                    {
                        TemplatePageViewModel.SourceLocusDatas.LocusName.RemoveAt(index);
                        TemplatePageViewModel.SourceLocusDatas.LocusPoints.RemoveAt(index);
                    }
                }
            }
        }
        /// <summary>
        /// 删除加工轨迹按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteProcessingLocus_Click(object sender, RoutedEventArgs e)
        {
            if (WheelTypeChoose_cb.SelectedItem == null)
            {
                MessageShow("轮型选择为空，请选择轮型！", MessageType.Default);
                return;
            }
            bool result = UMessageBox.Show("删除加工轨迹确认", "您选择的轮型是：" + WheelTypeChoose_cb.SelectedItem + " ，请确定轮型选择正确后，再点击确认！");
            if (result)
            {
                var wheelType = WheelTypeChoose_cb.SelectedItem.ToString();
                var plDB = new SqlAccess().ProcessingLocusDataAccess;
                //判断对应轮型的加工轨迹数据表是否存在
                var r = plDB.DbMaintenance.IsAnyTable(wheelType, false);
                if (r)
                {
                    var rr = plDB.DbMaintenance.DropTable(wheelType);
                    if (rr) MessageShow("轮型" + wheelType + "的加工轨迹数据删除成功！", MessageType.Success);
                    ServiceLocator.Current.GetInstance<LocusPageViewModel>().DeleteProcessingLocus(wheelType);
                }
            }
        }
        /// <summary>
        /// 当前加工显示按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CurrentMachiningDisplayed_Click(object sender, RoutedEventArgs e)
        {
            var identifyData = ServiceLocator.Current.GetInstance<LocusPageViewModel>().CurrentIdentifyData;
            if (identifyData.IdentifyWheelType == null) return;
            var image = ServiceLocator.Current.GetInstance<LocusPageViewModel>().CurrentMachiningImage;
            if (!image.IsInitialized()) return;
            else 
            {
                CurrentImage.Dispose();
                CurrentImage = image.Clone();
            }
            var datas = ServiceLocator.Current.GetInstance<LocusPageViewModel>().CurrentMachiningLocusDatas;
            if (datas.Count == 0) return;
            else MachiningPathDatas = datas;
            ServiceLocator.Current.GetInstance<LocusPageViewModel>().LocusButtonEnable = false;
            ClearHelixViewport();
            HWindowDisplay2DLocus.HalconWindow.ClearWindow();
            ModelLists = LoadLocus(MachiningPathDatas);
            ShowPathPoints(ModelLists);
            ShowAxisXYZ(ModelLists);
            SetHelixViewport();
            TemplateHelper.DisplayImage(CurrentImage, HWindowDisplay2DLocus.HalconWindow, false, LocusParameterSettingModel.IsImageEnhancement);
            //生成并显示轨迹
            SqlSugarClient sDB = new SqlAccess().SystemDataAccess;
            CurrentTemplateData = sDB.Queryable<TemplateDataModel>().Where(it => it.WheelType == identifyData.IdentifyWheelType).First();
            int spokeType = MachiningPathDatas[MachiningPathDatas.Count - 1].PoseId / 1000 / CurrentTemplateData.SpokeQuantity;
            List<MachiningPathPosModel> ds = TemplateHelper.GenLocusOfOutPoint(MachiningPathDatas, CurrentTemplateData.OutPointPoseId, spokeType);
            TemplateHelper.GenAndShowLocus(HWindowDisplay2DLocus.HalconWindow, ds, -1);
        }
        /// <summary>
        /// 左移按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentModelUIElement3D == null) return;
            double value = 0;
            try
            {
                value = double.Parse(XYOffsetValue_tb.Text);
            }
            catch (Exception ex)
            {
                MessageShow("XY偏移值输入错误：" + ex.Message, MessageType.Error);
                return;
            }
            ServiceLocator.Current.GetInstance<LocusPageViewModel>().LocusButtonEnable = false;
            //修改数据
            var index = MachiningPathDatas.FindIndex(t => t.PoseId == PoseId);
            MachiningPathDatas[index].Column = MachiningPathDatas[index].Column - value;
            PositionAdjustment(index);
            SetCurrentChoosePoint();
            LocusPointInformationDisplay(PoseId);
            TemplateHelper.DisplayImage(CurrentImage, HWindowDisplay2DLocus.HalconWindow, false, LocusParameterSettingModel.IsImageEnhancement);
            var ds = TemplateHelper.GenLocusOfOutPoint(MachiningPathDatas, CurrentTemplateData.OutPointPoseId, CurrentSpokeType);
            TemplateHelper.GenAndShowLocus(HWindowDisplay2DLocus.HalconWindow, ds, MachiningPathDatas[index].PoseId);
            ServiceLocator.Current.GetInstance<LocusPageViewModel>().LocusButtonEnable = true;
        }
        /// <summary>
        /// 上移按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AboveButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentModelUIElement3D == null) return;
            double value = 0;
            try
            {
                value = double.Parse(XYOffsetValue_tb.Text);
            }
            catch (Exception ex)
            {
                MessageShow("XY偏移值输入错误：" + ex.Message, MessageType.Error);
                return;
            }
            ServiceLocator.Current.GetInstance<LocusPageViewModel>().LocusButtonEnable = false;
            //修改数据
            var index = MachiningPathDatas.FindIndex(t => t.PoseId == PoseId);
            MachiningPathDatas[index].Row = Math.Round(MachiningPathDatas[index].Row - value, 2);
            PositionAdjustment(index);
            SetCurrentChoosePoint();
            LocusPointInformationDisplay(PoseId);
            TemplateHelper.DisplayImage(CurrentImage, HWindowDisplay2DLocus.HalconWindow, false, LocusParameterSettingModel.IsImageEnhancement);
            var ds = TemplateHelper.GenLocusOfOutPoint(MachiningPathDatas, CurrentTemplateData.OutPointPoseId, CurrentSpokeType);
            TemplateHelper.GenAndShowLocus(HWindowDisplay2DLocus.HalconWindow, ds, MachiningPathDatas[index].PoseId);
            ServiceLocator.Current.GetInstance<LocusPageViewModel>().LocusButtonEnable = true;
        }
        /// <summary>
        /// 右移按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RightButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentModelUIElement3D == null) return;
            double value = 0;
            try
            {
                value = double.Parse(XYOffsetValue_tb.Text);
            }
            catch (Exception ex)
            {
                MessageShow("XY偏移值输入错误：" + ex.Message, MessageType.Error);
                return;
            }
            ServiceLocator.Current.GetInstance<LocusPageViewModel>().LocusButtonEnable = false;
            //修改数据
            var index = MachiningPathDatas.FindIndex(t => t.PoseId == PoseId);
            MachiningPathDatas[index].Column = MachiningPathDatas[index].Column + value;
            PositionAdjustment(index);
            SetCurrentChoosePoint();
            LocusPointInformationDisplay(PoseId);
            TemplateHelper.DisplayImage(CurrentImage, HWindowDisplay2DLocus.HalconWindow, false, LocusParameterSettingModel.IsImageEnhancement);
            var ds = TemplateHelper.GenLocusOfOutPoint(MachiningPathDatas, CurrentTemplateData.OutPointPoseId, CurrentSpokeType);
            TemplateHelper.GenAndShowLocus(HWindowDisplay2DLocus.HalconWindow, ds, MachiningPathDatas[index].PoseId);
            ServiceLocator.Current.GetInstance<LocusPageViewModel>().LocusButtonEnable = true;
        }
        /// <summary>
        /// 下移按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UnderButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentModelUIElement3D == null) return;
            double value = 0;
            try
            {
                value = double.Parse(XYOffsetValue_tb.Text);
            }
            catch (Exception ex)
            {
                MessageShow("XY偏移值输入错误：" + ex.Message, MessageType.Error);
                return;
            }
            ServiceLocator.Current.GetInstance<LocusPageViewModel>().LocusButtonEnable = false;
            //修改数据
            var index = MachiningPathDatas.FindIndex(t => t.PoseId == PoseId);
            MachiningPathDatas[index].Row = MachiningPathDatas[index].Row + value;
            PositionAdjustment(index);
            SetCurrentChoosePoint();
            LocusPointInformationDisplay(PoseId);
            TemplateHelper.DisplayImage(CurrentImage, HWindowDisplay2DLocus.HalconWindow, false, LocusParameterSettingModel.IsImageEnhancement);
            var ds = TemplateHelper.GenLocusOfOutPoint(MachiningPathDatas, CurrentTemplateData.OutPointPoseId, CurrentSpokeType);
            TemplateHelper.GenAndShowLocus(HWindowDisplay2DLocus.HalconWindow, ds, MachiningPathDatas[index].PoseId);
            ServiceLocator.Current.GetInstance<LocusPageViewModel>().LocusButtonEnable = true;
        }
        /// <summary>
        /// 增加轨迹点按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IncreaseLocusPoint_Click(object sender, RoutedEventArgs e)
        {
            if (MachiningPathDatas == null || MachiningPathDatas.Count == 0)
            {
                MessageShow("请先加载轨迹！", MessageType.Default);
                return;
            }
            if (MachiningPathDatas[MachiningPathDatas.Count - 1].PoseId / 1000 > CurrentSpokeType)
            {
                MessageShow("请点击《调整轨迹位姿》按钮加载单个窗口轨迹！", MessageType.Default);
                return;
            }
            if (CurrentModelUIElement3D == null)
            {
                MessageShow("未选中轨迹点！", MessageType.Default);
                return;
            }
            try
            {
                var value = double.Parse(IncreasePointOffsetDistance_tbx.Text);
                if (value >= -100 && value <= 100)
                {
                    IncreasePointOffsetDistance = value;
                    ConfigEdit.SetAppSettings("IncreasePointOffsetDistance", IncreasePointOffsetDistance_tbx.Text);
                }
                else
                {
                    MessageShow("增加点于选中点偏移距离必须大于等于-100或小于等于100，请重新输入！", MessageType.Error);
                    return;
                }
            }
            catch
            {
                MessageShow("增加点于选中点偏移距离值输入错误，请重新输入", MessageType.Error);
                return;
            }
            var datas = MachiningPathDatas.Where(x => x.PoseId >= PoseId / 1000 * 1000 && x.PoseId <= PoseId / 1000 * 1000 + 900).ToList();
            if (PoseId == datas[datas.Count - 1].PoseId || PoseId > PoseId / 1000 * 1000 + 900)
            {
                MessageShow("终点和进出刀点后不能生成点！", MessageType.Default);
                return;
            }
            //通过最后一条数据的PoseId获取窗口轮廓的总数
            int winIndex = -1;
            var contourTotal = MachiningPathDatas[MachiningPathDatas.Count - 1].PoseId / 1000;
            //计算当前类型的窗口索引
            if (contourTotal == CurrentTemplateData.SpokeQuantity * CurrentSpokeType) winIndex = WinNum % CurrentSpokeType;
            else winIndex = WinNum % CurrentSpokeType;
            for (int i = winIndex; i < contourTotal; i += CurrentSpokeType)
            {
                //通过PoseId获取第i+1个窗口的编号为0 - 900之间的轨迹数据
                var points = MachiningPathDatas.Where(x1 => x1.PoseId >= (i + 1) * 1000 && x1.PoseId <= (i + 1) * 1000 + 900).ToList();
                //获取当前选中点在points数据中的索引
                int index = points.FindIndex(x => x.PoseId == PoseId % 1000 + (i + 1) * 1000);
                //通过index点和第二个点计算终点
                var p = CalculateThePointsBetweenTwoPoints(points[index], points[index + 1], IncreasePointOffsetDistance);
                //获取当前选中点在MachiningPathDatas数据中的索引
                int index1 = MachiningPathDatas.FindIndex(x => x.PoseId == PoseId % 1000 + (i + 1) * 1000);
                //插入数据
                MachiningPathDatas.Insert(index1 + 1, p);
                //修改PoseId 
                var ps = MachiningPathDatas.Where(x1 => x1.PoseId >= (i + 1) * 1000 && x1.PoseId <= (i + 1) * 1000 + 900).ToList();
                int index2 = MachiningPathDatas.FindIndex(x => x.PoseId == ps[0].PoseId);
                int poseId = ps[0].PoseId;
                for (int j = 0; j < ps.Count; j++)
                {
                    MachiningPathDatas[index2].PoseId = poseId;
                    index2++;
                    poseId++;
                }
            }
            for (int i = 0; i < MachiningPathDatas.Count; i++)
            {
                MachiningPathDatas[i].Id = i + 1;
            }
            var outPointPoseIds = CurrentTemplateData.OutPointPoseId.Split(',');
            //修改当前窗口的出刀点
            if (PoseId % 1000 <= int.Parse(outPointPoseIds[WinNum]))
            {
                outPointPoseIds[WinNum] = (int.Parse(outPointPoseIds[WinNum]) + 1).ToString();
                string str = string.Join(",", outPointPoseIds);
                CurrentTemplateData.OutPointPoseId = str;
            }
            ClearHelixViewport();
            ModelLists = LoadLocus(MachiningPathDatas);
            ShowPathPoints(ModelLists);
            PointNum += 1;
            PoseId += 1;
            CModel3DIndex = HelixViewport.Children.IndexOf(ModelLists[WinNum].CModelUIElement3Ds[PointNum]);
            XModel3DIndex = HelixViewport.Children.IndexOf(ModelLists[WinNum].XModelUIElement3Ds[PointNum]);
            YModel3DIndex = HelixViewport.Children.IndexOf(ModelLists[WinNum].YModelUIElement3Ds[PointNum]);
            ZModel3DIndex = HelixViewport.Children.IndexOf(ModelLists[WinNum].ZModelUIElement3Ds[PointNum]);
            SetCurrentChoosePoint();
            LocusPointInformationDisplay(PoseId);
            TemplateHelper.DisplayImage(CurrentImage, HWindowDisplay2DLocus.HalconWindow, false, LocusParameterSettingModel.IsImageEnhancement);
            var ds = TemplateHelper.GenLocusOfOutPoint(MachiningPathDatas, CurrentTemplateData.OutPointPoseId, CurrentSpokeType);
            TemplateHelper.GenAndShowLocus(HWindowDisplay2DLocus.HalconWindow, ds, PoseId);
            ReviseSave(MachiningPathDatas);
        }
        /// <summary>
        /// 上一步按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Previous_Cilck(object sender, RoutedEventArgs e)
        {
            if (CurrentMachiningPathDataIndex == 0) return;
            if (CurrentMachiningPathDataIndex < 0) return;
            if (CurrentMachiningPathDataIndex >= 1) CurrentMachiningPathDataIndex -= 1;
            Previous_btn.IsEnabled = false;
            Next_btn.IsEnabled = false;
            ClearHelixViewport();
            MachiningPathDatas = MachiningPathDataList[CurrentMachiningPathDataIndex];
            ModelLists = LoadLocus(MachiningPathDatas);
            ShowPathPoints(ModelLists);
            TemplateHelper.DisplayImage(CurrentImage, HWindowDisplay2DLocus.HalconWindow, false, LocusParameterSettingModel.IsImageEnhancement);
            var ds = TemplateHelper.GenLocusOfOutPoint(MachiningPathDatas, CurrentTemplateData.OutPointPoseId, CurrentSpokeType);
            TemplateHelper.GenAndShowLocus(HWindowDisplay2DLocus.HalconWindow, ds, PoseId);
            Previous_btn.IsEnabled = true;
            Next_btn.IsEnabled = true;
        }
        /// <summary>
        /// 下一步按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Next_Cilck(object sender, RoutedEventArgs e)
        {
            if (CurrentMachiningPathDataIndex == MachiningPathDataList.Count - 1) return;
            if (CurrentMachiningPathDataIndex < 0) return;
            if (CurrentMachiningPathDataIndex < MachiningPathDataList.Count - 1) CurrentMachiningPathDataIndex += 1;
            Previous_btn.IsEnabled = false;
            Next_btn.IsEnabled = false;
            ClearHelixViewport();
            MachiningPathDatas = MachiningPathDataList[CurrentMachiningPathDataIndex];
            ModelLists = LoadLocus(MachiningPathDatas);
            ShowPathPoints(ModelLists);
            var ds = TemplateHelper.GenLocusOfOutPoint(MachiningPathDatas, CurrentTemplateData.OutPointPoseId, CurrentSpokeType);
            TemplateHelper.GenAndShowLocus(HWindowDisplay2DLocus.HalconWindow, ds, PoseId);
            Previous_btn.IsEnabled = true;
            Next_btn.IsEnabled = true;
        }
        #endregion

        private void Load3DModel_Click(object sender, RoutedEventArgs e)
        {
            //ModelImporter model = new ModelImporter();
            //var mg3DG = model.Load(@"D:\Deburr\STL\07017c13.stl");
            ////获取Model3DGroup中的GeometryModel3D
            //var gm3D = mg3DG.Children[0] as GeometryModel3D;
            //gm3D.Material = NormalMaterial(Brushes.Gray);
            //ModelUIElement3D mui3D = new ModelUIElement3D();
            //mui3D.Model = gm3D;
            //HelixViewport.Children.Add(mui3D);
        }
        #region=========TextBox输入限制=========
        private void IncreasePointOffsetDistance_tbx_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.]"); // 只允许数字和小数点
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

        private void startPose_tb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]"); // 只允许数字
            if (regex.IsMatch(e.Text))
            {
                e.Handled = true;
            }
        }

        private void endPose_tb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]"); // 只允许数字
            if (regex.IsMatch(e.Text))
            {
                e.Handled = true;
            }
        }

        private void Height_tb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.-]"); // 只允许数字和小数点
            if (regex.IsMatch(e.Text))
            {
                e.Handled = true;
            }
        }

        private void Rotate_tb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.-]"); // 只允许数字和小数点
            if (regex.IsMatch(e.Text))
            {
                e.Handled = true;
            }
        }
        #endregion
    }
}
